using ExclusiveCard.Enums;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using ST = ExclusiveCard.Services.Models.DTOs.StagingModels;
using DB = ExclusiveCard.Data.Models;
using DBST = ExclusiveCard.Data.StagingModels;
using ExclusiveCard.Data.Repositories;
using AutoMapper;

using System;
using System.Collections.Generic;
using System.Reflection;

using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using NLog;

namespace ExclusiveCard.Managers
{
    /// <summary>
    /// The OfferImportManager has but one purpose in its existance.
    /// It is here to load affiliate offer files that have been uploaded to the admin website
    /// and put the offers into the Exclusive.Offers table. 
    /// 
    /// NOTE1:  This is a generic class. DO NOT POLUTE WITH HARD CODED REFERENCES TO SPECIFIC AFFILIATES.
    /// 
    /// NOTE2:
    /// This manager is not designed to manage and update Offers.  Please use [and write if not already done]
    /// a nice new shiny  OfferManager for that.  You don't want to load this whole import shenanigans every time the 
    /// website or mobile app displays an offer.
    /// </summary>
    public class OfferImportManager : IOfferImportManager
    {
        #region Private fields and constructor

        //TODO:  Setup a factory for different affililate imports (ok, I broke NOTE 1 but left a todo to fix this - IB).
        IAffiliateManager<ST.AwinCSVFile, ST.AwinCSVFileError, ST.OfferImportAwin> _affiliateManager;

        IRepository<DBST.OfferImportFile> _offerFilesRepo;
        IRepository<DB.Offer> _offerRepo;
        IRepository<DBST.OfferImportError> _offerImportErrorRepo;
        IRepository<DB.Merchant> _merchantRepo;

        readonly ILogger _logger;

        IMapper _mapper;
        int _batchSize;

       
        const int BATCH_SIZE = 1;

        public OfferImportManager(IRepository<DB.AffiliateFile> fileRepo, IRepository<DB.AffiliateFieldMapping> fieldMappingRepo,
                                  IRepository<DBST.OfferImportFile> offerFilesRepo, IRepository<DBST.OfferImportAwin> awinRepo,
                                  IRepository<DB.Offer> offerRepo, IRepository<DBST.OfferImportError> offerImportErrorRepo,
                                  IRepository<DB.Merchant> merchantRepo, IMapper mapper)
        {
            //TODO:  Setup a factory for different affililate imports
            _affiliateManager = new AffiliateManagerAwin(fileRepo, fieldMappingRepo,  awinRepo, mapper);

            _offerFilesRepo = offerFilesRepo;
            _offerRepo = offerRepo;
            _offerImportErrorRepo = offerImportErrorRepo;
            _merchantRepo = merchantRepo;

            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();

            //TODO: Move this to property on appSettings file
            _batchSize = BATCH_SIZE;
        }

        #endregion

        #region UploadToStaging

        #region Public Methods


        /// <summary>
        /// Returns a list of files where the status is provided in the param list
        /// (always no tracking)
        /// </summary>
        /// <param name="fileStatuses"></param>
        /// <returns>A Dto object listing the files requested.</returns>
        public List<ST.OfferImportFile> GetImportFiles(params Import[] fileStatuses)
        {
            List<ST.OfferImportFile> files = null;
            List<DBST.OfferImportFile> dbFiles = null;

            dbFiles = _offerFilesRepo.Include(i => i.AffiliateFile).Where(x => Enum.IsDefined(typeof(Import), x.ImportStatus) && fileStatuses.Contains((Import) x.ImportStatus)).AsNoTracking().ToList();

            if (dbFiles != null)
                files = _mapper.Map<List<ST.OfferImportFile>>(dbFiles);

            return files;
        }

        /// <summary>
        /// Returns a list of records from the Staging.OfferImportFile table.
        /// In theory, these are a list of files that should have been uploaded by 
        /// Exclusive Admin staff using the Offers/Import screen in the admin tool
        /// </summary>
        /// <param name="fileStatus">A specific file status to look for</param>
        /// <param name="noTrack">Whether to take a quick snapshot of the data (True) or a full db entity (false). Default value is false. Can't remember why this option was added. Not really needed</param>
        /// <returns>A Dto object listing the files requested.</returns>
        public List<ST.OfferImportFile> GetImportFiles(Import fileStatus, bool noTrack = false)
        {
            List<ST.OfferImportFile> files = null;
            List<DBST.OfferImportFile> dbFiles = null;

            int status = (int)fileStatus;

            if (noTrack)
                dbFiles = _offerFilesRepo.Include(i => i.AffiliateFile).Where(x => x.ImportStatus == status).AsNoTracking().ToList();
            else
                dbFiles = _offerFilesRepo.Include(i => i.AffiliateFile).Where(x => x.ImportStatus == status).ToList();
            
            if (dbFiles != null)
                files = _mapper.Map<List<ST.OfferImportFile>>(dbFiles);

            return files;
        }

        public ST.OfferImportFile GetLatestImportFile(int affiliateId, int fileTypeId, bool noTrack = false)
        {
            ST.OfferImportFile file = null;
            DBST.OfferImportFile dbFile = null;

            if (noTrack)
                dbFile = _offerFilesRepo.Include(x => x.AffiliateFile)
                                        .Where( x => x.AffiliateFile.AffiliateId == affiliateId &&
                                                     x.AffiliateFile.Id == fileTypeId &&
                                                     x.AffiliateFileId == fileTypeId).AsNoTracking().LastOrDefault();
            else
                dbFile = _offerFilesRepo.Include(x => x.AffiliateFile)
                                        .Where(x => x.AffiliateFile.AffiliateId == affiliateId &&
                                                    x.AffiliateFile.Id == fileTypeId &&
                                                    x.AffiliateFileId == fileTypeId).LastOrDefault();

            if (dbFile != null)
                file = _mapper.Map<ST.OfferImportFile>(dbFile);

            return file;
        }

        /// <summary>
        /// STEP 1
        /// Loads an affiliate CSV file into the staging data table, as the 1st step in the Offer import process
        /// Each affiliate has its own bespoke format of data, and its own staging table in this format.
        /// If any rows are invalid, they are written out to an error file name [filename]_Error.csv, in the same dir as original file.
        /// An invalid row does not stop the rest of the file from being loaded.
        /// </summary>
        /// <param name="file">A DTO detailing the file to import. This will include the filename, 
        /// and will be updated with a count of the records loaded to staging.</param>
        /// <returns>Just a plain vanilla Task.</returns>
        public async Task UploadFileToStagingAsync(ST.OfferImportFile file)
        {
            try
            {
                UpdateOfferImportFileRecord(file, Import.Processing);

                // Read from CSV file into memory
                var records = _affiliateManager.ReadFileFromCSV(file.FilePath);
                file.TotalRecords = records.Count;

                // TODO:  When adding more Affiliates, the UploadFileToStagingAsync method will need revisiting to work
                //        on data types for any affiliate


                // Map csv data to Affiliate schema
                var cultureInfoCountry = GetCultureInfoCountryList().GetValueOrDefault(file.CountryCode);
                var stagingData = await _affiliateManager.MapToSchemaAsync(records, file, cultureInfoCountry);

                // Save the data to the staging table
                file.Staged = _affiliateManager.SaveStagingData(stagingData.Item1);

                // Write the error file out
                var errorList = stagingData.Item2;
                file.Failed = _affiliateManager.SaveErrorFile(errorList, file.ErrorFilePath);

                // Move file to archive
                MoveFileToArchive(file.FilePath);

                UpdateOfferImportFileRecord(file, Import.Uploaded);
            }
            catch(Exception ex)
            {

                ST.OfferImportError importError = new ST.OfferImportError()
                {
                    OfferImportFileId = file.Id,
                    ErrorMessage = $"Offer Import to Staging failed. {ex}",
                };
                CreateStagingRecordError(importError);

                throw new Exception("Upload of file to Staging tables failed.", ex);
            }
            
        }

        #endregion

        #region Private Methods

        private Dictionary<string, string> GetCultureInfoCountryList()
        {
            var countryCultureInfo = new Dictionary<string, string>()
            {
                { "CZ", "cs-CZ"},
                { "GB", "en-GB"},
                { "PL", "pl-PL"},
                { "SC", "en-SC"},
                { "SK", "sk-SK"}
            };

            return countryCultureInfo;
        }

        private void MoveFileToArchive(string filePath)
        {
            string copyFileName = Path.GetFileNameWithoutExtension(filePath) + DateTime.UtcNow.ToString("dd MM yyyy hhmmss") + Path.GetExtension(filePath);
            //Copy File to Archive Folder
            string copyPath = Path.Combine(Directory.GetCurrentDirectory(), "Archive\\");

            if (!Directory.Exists(copyPath))
            {
                Directory.CreateDirectory(copyPath);
            }

            copyPath = Path.Combine(copyPath, copyFileName);
            System.IO.File.Move(filePath, copyPath);
        }

        #endregion

        #endregion

        #region MigrateFromStaging

        #region public methods

        /// <summary>
        /// STEP 2
        /// Reads the data from the staging table for the specified file, 
        /// maps it from the Affiliate's own bespoke format to the standard Exclusive format 
        /// then adds it to the Exclusive.Offers table.
        /// </summary>
        /// <param name="file">A DTO listing the file that is ready to be migrated (having already been uploaded)</param>
        /// <returns></returns>
        public async Task MigrateOffersFromStagingAsync(ST.OfferImportFile file)
        {

            int recCounter = 1;
            try
            {
                UpdateOfferImportFileRecord(file, Import.Processing);

                DB.Offer dbOffer = new DB.Offer();
                DB.OfferCountry dbCountry = new DB.OfferCountry();
                DB.OfferCategory dbCategory = new DB.OfferCategory();

                // Load up the field mappings including the data mappings(merchant, category etc)
                var dtoFieldMappings = _affiliateManager.GetFieldMappings(file.AffiliateFile.AffiliateId, file.AffiliateFileId);
                var simpleMappings = dtoFieldMappings.Where(x => x.AffiliateMappingRuleId == null && x.ExclusiveTable == "Staging.Offer" && x.AffiliateMatchTypeId == (int)AffiliateMatchTypes.Equals).ToList();
                var otherMappings = dtoFieldMappings.Where(x => x.AffiliateMappingRuleId != null || x.ExclusiveTable != "Staging.Offer" || x.AffiliateMatchTypeId != (int)AffiliateMatchTypes.Equals).ToList();

                //Get the staging data
                var dtoStagedData = _affiliateManager.GetStagingData(file.Id);
                if (dtoStagedData != null && dtoStagedData.Any())
                {
                    // Use reflection to get a collection of the properties of both the source dto data and the destination DBO.Offer
                    var dbOfferProperties = dbOffer.GetType().GetProperties().Where(x => x.Name.ToLower() != "id").ToList();
                    var dtoRecordProperties = dtoStagedData.First().GetType().GetProperties().Where(x => x.Name.ToLower() != "id").ToList();
                    var dbCountryProperty = dbCountry.GetType().GetProperties().Where(x => x.Name.ToLower() == "CountryCode".ToLower()).FirstOrDefault();
                    var dbCategoryProperty = dbCategory.GetType().GetProperties().Where(x => x.Name.ToLower() == "CategoryId".ToLower()).FirstOrDefault();


                    // for each staged record,  create new offer entity then map the fields from staged record, using the FieldMappings loaded above
                    foreach (var dtoRecord in dtoStagedData)
                    {
                        try
                        {
                            ST.OfferImportRecord thisRecord = new ST.OfferImportRecord() { FileID = file.Id, RecordID = dtoRecord.Id };

                            // Do the simple mappings first - straightforward copy of value from staged data to the equivalent property on Offer
                            dbOffer = MapSimpleProperties(simpleMappings, dbOfferProperties, dtoRecordProperties, dtoRecord);

                            // Then Map the fields with extra mapping rules
                            MapOtherProperties(dbOffer, otherMappings, dbOfferProperties, dtoRecordProperties, dtoRecord, dbCountryProperty, dbCategoryProperty, thisRecord);

                            // Dedupe check of the offer to ensure it doesn't already exist

                            //TODO:  Decide what offer status the newly imported offers should be. Going with Active for now
                            dbOffer.StatusId = (int)OfferStatus.Active;

                            //Add the affiliate Id...
                            dbOffer.AffiliateId = file.AffiliateFile.AffiliateId;

                            //Make sure dbOffer looks valid before we action it and check if it already exists
                            RecordStatus recordStatus = ValidateOffer(dbOffer, dbOfferProperties);

                            switch(recordStatus)
                            {
                                case RecordStatus.Invalid:
                                    file.Failed++;
                                    break;
                                case RecordStatus.Duplicate:
                                    file.Duplicates++;
                                    break;
                                case RecordStatus.Update:
                                    file.Updates++;
                                    break;
                                default:
                                    // must be new, save the offer directly to the Exclusive Offers table
                                    _offerRepo.Create(dbOffer);
                                    file.Imported++;
                                    break;
                            }

                            //TODO: Decide when to purge the staging table record (if at all)

                            if (recordStatus == RecordStatus.Update || recordStatus == RecordStatus.New)
                            {
                                //// Only save to db in batches, not every offer record.
                                //if (recCounter >= _batchSize)
                                //{
                                    _offerRepo.SaveChanges();
                                //    recCounter = 1;
                                //}
                            }

                        }
                        catch (Exception ex)
                        {
                            ST.OfferImportError importError = new ST.OfferImportError()
                            {
                                OfferImportFileId = file.Id,
                                OfferImportRecordId = dtoRecord.Id,
                                ErrorMessage = $"Migrate offer record from staging failed. {ex}",
                            };
                            CreateStagingRecordError(importError);

                            file.Failed++;

                        }
                        finally
                        {
                            try
                            {
                                if (recCounter % 50 == 0)
                                {
                                    //Update the import file record with current progress
                                    UpdateOfferImportFileRecord(file, Import.Processing);
                                }
                            }
                            catch(Exception ex)
                            {
                                _logger.Error(ex, "Unable to save OfferImportFile record update");
                            }
                            recCounter++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,$"{file?.FilePath}");
            }
            finally
            {
                // Make sure any remaining records get saved
                if (recCounter > 0)
                    _offerRepo.SaveChanges();

                // Include any offer mapping errors in the error file
                var importErrors = GetImportErrorRecords(file);
                AppendToErrorFile(file, importErrors);

                UpdateOfferImportFileRecord(file, Import.Processed);
                await Task.CompletedTask;
            }
        }

        public ST.OfferImportFile AddImportFile(ST.OfferImportFile offerImportFile)
        {
            var dbOfferFile = _mapper.Map<DBST.OfferImportFile>(offerImportFile);
            _offerFilesRepo.Create(dbOfferFile);
            _offerFilesRepo.SaveChanges();
            ST.OfferImportFile result = _mapper.Map<ST.OfferImportFile>(dbOfferFile);
            return result;
        }

        public ST.OfferImportFile GetLastImportedFile(int affiliateId, int fileTypeId, int status)
        {
            var dbOfferFile = _offerFilesRepo.FilterNoTrack(f =>
                                        f.AffiliateFile.AffiliateId == affiliateId &&
                                        f.AffiliateFile.Id == fileTypeId &&
                                        f.AffiliateFileId == fileTypeId &&
                                        f.ImportStatus != status);
            ST.OfferImportFile result = _mapper.Map<ST.OfferImportFile>(dbOfferFile);
            return result;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Fuzzy matching validation to check if all required fields are populated
        /// </summary>
        /// <param name="dbOffer">offer record candidate</param>
        /// <param name="dbOfferProperties">db model properties to be checked</param>
        /// <returns>true if the <paramref name="dbOffer"/> has all it's required fields populated</returns>
        RecordStatus ValidateOffer(DB.Offer dbOffer, List<PropertyInfo> dbOfferProperties)
        {
            RecordStatus result = RecordStatus.New;

            //Check offer is valid
            foreach (var prop in dbOfferProperties)
                if (prop.PropertyType == typeof(int))
                {
                    //Get current property value
                    int propertyValue = (int)prop.GetValue(dbOffer);
                    //Get matching reference table property if one exists
                    var referenceProperty = dbOfferProperties.Where(p => p.Name != prop.Name && prop.Name.Contains(p.Name)).FirstOrDefault();
                    if (referenceProperty != null)
                    {
                        var referenceRecord = referenceProperty.GetValue(dbOffer);
                        //Make sure either the property is populated or the reference table record is provided
                        if (referenceRecord == null && propertyValue < 1)
                        {
                            result = RecordStatus.Invalid;
                            break;
                        }
                    }
                }
            if (result != RecordStatus.Invalid)
            {
                //Make absolutely sure the merchant Id exists (# issue in testing - AffiliateMappingRule found but no corresponding merchant record)
                var merchantTest = _merchantRepo.GetById(dbOffer.MerchantId);
                if(merchantTest == null)
                {
                    throw new Exception($"AffiliateMappingRule found for merchant id that doesn't exist: {dbOffer.MerchantId}");
                }
            }

            if (result != RecordStatus.Invalid)
            {
                result = CheckExistingOffer(dbOffer);
            }

            return result;

        }

        /// <summary>
        /// Checks to see if this is a new offer or a duplicate of an existing offer 
        /// if it is a duplicate, also checks if the original needs to be updated
        /// </summary>
        /// <param name="dbOffer"></param>
        /// <returns>RecordStatus enum</returns>
        RecordStatus CheckExistingOffer(DB.Offer dbOffer)
        {
            RecordStatus result = RecordStatus.New;

            //This should only use the unique key fields
            var existingOffer = _offerRepo.FilterNoTrack(o => o.AffiliateReference == dbOffer.AffiliateReference &&
                                                              o.AffiliateId == dbOffer.AffiliateId).FirstOrDefault();

            if (existingOffer != null)
            {
                result = RecordStatus.Duplicate;

                if (existingOffer.ValidFrom != dbOffer.ValidFrom || 
                    existingOffer.ValidTo != dbOffer.ValidTo ||
                    existingOffer.ShortDescription != dbOffer.ShortDescription ||
                    existingOffer.LongDescription != dbOffer.LongDescription ||
                    existingOffer.Terms != dbOffer.Terms ||
                    existingOffer.LinkUrl != dbOffer.LinkUrl ||
                    existingOffer.OfferCode != dbOffer.OfferCode
                    )
                {
                    result = RecordStatus.Update;
                    //map to existing offer and update
                    existingOffer = _offerRepo.GetById(existingOffer.Id);

                    existingOffer.ValidFrom = dbOffer.ValidFrom;
                    existingOffer.ValidTo = dbOffer.ValidTo;
                    existingOffer.ShortDescription = dbOffer.ShortDescription;
                    existingOffer.LongDescription = dbOffer.LongDescription;
                    existingOffer.Terms = dbOffer.Terms;
                    existingOffer.LinkUrl = dbOffer.LinkUrl;
                    existingOffer.OfferCode = dbOffer.OfferCode;

                    //The calling method doesn't get the latest version unless we update here
                    _offerRepo.Update(existingOffer);

                    dbOffer = existingOffer;
                }
            }

            return result;
        }


        private DB.Offer MapSimpleProperties(List<DTOs.AffiliateFieldMapping> simpleMappings, List<PropertyInfo> dbOfferProperties, List<PropertyInfo> dtoRecordProperties, ST.OfferImportAwin dtoRecord)
        {
            DB.Offer dbOffer = new DB.Offer();
            foreach (var simpleMap in simpleMappings)
            {
                CopyProperty(dbOfferProperties, dtoRecordProperties, dtoRecord, dbOffer, simpleMap);
            }

            return dbOffer;
        }

        private void MapOtherProperties(DB.Offer dbOffer, List<DTOs.AffiliateFieldMapping> otherMappings, List<PropertyInfo> dbOfferProperties, List<PropertyInfo> dtoRecordProperties, ST.OfferImportAwin dtoRecord, PropertyInfo dbCountryProperty, PropertyInfo dbCategoryProperty, ST.OfferImportRecord thisRecord)
        {
            foreach (var fieldMapping in otherMappings)
            {
                // If the field mapping is a list, then we need to split the delimited list in the affiliatevalue field up into individual items
                if (fieldMapping.IsList)
                {
                    char delimiter = char.Parse(fieldMapping.Delimiter);
                    var dtoRecordProperty = dtoRecordProperties.Where(x => x.Name.ToLower() == fieldMapping.AffiliateFieldName.ToLower().Trim()).FirstOrDefault();
                    string rawList = dtoRecordProperty.GetValue(dtoRecord).ToString();
                    var items = rawList.Split(delimiter);

                    foreach (var item in items)
                    {
                        // Because we already have read the item value from the record, we can pass it in as a targetValue, to save looking it up again.
                        MapProperty(thisRecord, dbOffer, dbOfferProperties, dtoRecordProperties, dtoRecord, fieldMapping, dbCountryProperty, dbCategoryProperty, item);
                    }

                }

                // If not a list, map the single value
                else
                {
                    MapProperty(thisRecord, dbOffer, dbOfferProperties, dtoRecordProperties, dtoRecord, fieldMapping, dbCountryProperty, dbCategoryProperty);
                }
            }
        }

        private void MapProperty(ST.OfferImportRecord thisRecord, DB.Offer dbOffer, List<PropertyInfo> dbOfferProperties, List<PropertyInfo> dtoRecordProperties,
                                 ST.OfferImportAwin dtoRecord, DTOs.AffiliateFieldMapping fieldMapping, PropertyInfo dbCountryProperty, 
                                 PropertyInfo dbCategoryProperty, object targetValue = null)
        {
            // Check the affiliatetransformId value in the mapping to decide how we do our mappings

            // Straigtforward copy of a value
            if (fieldMapping.AffiliateTransformId == (int)AffiliateTransform.Copy)
            {
                CopyProperty(dbOfferProperties, dtoRecordProperties, dtoRecord, dbOffer, fieldMapping, targetValue);
            }

            // Do a lookup
            else if (fieldMapping.AffiliateTransformId == (int)AffiliateTransform.Lookup)
            {
                LookupProperty(thisRecord, dbOfferProperties, dtoRecordProperties, dtoRecord, dbOffer, fieldMapping, targetValue);
            }

            // Any custom function is hard coded here.
            else
            {    
                if (fieldMapping.AffiliateMappingRuleId == 2)
                {

                    // Countries
                    var dbCountry = new DB.OfferCountry();

                    if (targetValue != null && targetValue.ToString().ToLower().Trim() == "All Regions".ToLower())
                        targetValue = "United Kingdom";

                    if (LookupProperty(thisRecord, dbOfferProperties, dtoRecordProperties, dtoRecord, dbOffer, fieldMapping, targetValue, dbCountryProperty, dbCountry))
                    {
                        if (dbOffer.OfferCountries == null)
                            dbOffer.OfferCountries = new List<DB.OfferCountry>();
                        //Check for duplicates
                        if (!dbOffer.OfferCountries.Where(c => c.CountryCode == dbCountry.CountryCode).Any())
                            dbOffer.OfferCountries.Add(dbCountry);
                    }

                }
                else if (fieldMapping.AffiliateMappingRuleId == 4)
                {
                    // Category
                    var dbCategory = new DB.OfferCategory();

                    if (LookupProperty(thisRecord, dbOfferProperties, dtoRecordProperties, dtoRecord, dbOffer, fieldMapping, targetValue, dbCategoryProperty, dbCategory))
                    {
                        if (dbOffer.OfferCategories == null)
                            dbOffer.OfferCategories = new List<DB.OfferCategory>();
                        //Check for duplicates
                        if (!dbOffer.OfferCategories.Where(c => c.CategoryId == dbCategory.CategoryId).Any())
                            dbOffer.OfferCategories.Add(dbCategory);
                    }
                }
                else
                    throw new Exception("Custom Affiliate Mapping rule Id not known - RuleID = " + fieldMapping.AffiliateMappingRuleId.ToString());
            }
        }

        /// <summary>
        /// Copies the value from a column in the affiliate file to the relevant field in the Offer record
        /// The column name and offer field are detailed in the field mappings record
        /// </summary>
        /// <param name="dbOfferProperties">List of properties on our Offer Data entity</param>
        /// <param name="dtoRecordProperties">List of properties on our affiliate data</param>
        /// <param name="dtoRecord">the data in the affiliate record</param>
        /// <param name="dbOffer">the offer data entity we will save to the Db</param>
        /// <param name="fieldMapping">the mappings for the current field we are mapping </param>
        /// <param name="targetValue">the value of the affilate data. This is optional - we already have retrieved it for lists but not otherwise</param>
        /// <param name="targetProperty">the property we want to update. This is optional, set when using custom mappings from OfferCountry or Offer Catagory table</param>
        /// <param name="targetObject">the object we want to update. Optional, set on custom mappings for OfferCountry or OfferCategory entity, will use Offer entity if left null</param>
        private void CopyProperty(List<PropertyInfo> dbOfferProperties, List<PropertyInfo> dtoRecordProperties, ST.OfferImportAwin dtoRecord, DB.Offer dbOffer, DTOs.AffiliateFieldMapping fieldMapping, object targetValue = null, PropertyInfo targetProperty = null, Object targetObject = null)
        {
            if (targetProperty == null)
                targetProperty = dbOfferProperties.Where(x => x.Name.ToLower() == fieldMapping.ExclusiveFieldName.ToLower().Trim()).FirstOrDefault();
            
            var dtoRecordProperty = dtoRecordProperties.Where(x => x.Name.ToLower() == fieldMapping.AffiliateFieldName.ToLower().Trim()).FirstOrDefault();

            if (targetProperty != null && dtoRecordProperty != null)
            {
                if (targetObject == null)
                    targetObject = dbOffer;

                if (targetValue == null)
                    targetValue = dtoRecordProperty.GetValue(dtoRecord);

                targetProperty.SetValue(targetObject, targetValue);

            }
            else
                throw new Exception($"Copy Property not found for {fieldMapping.ExclusiveFieldName}, {fieldMapping.AffiliateFieldName}");
        }

        /// <summary>
        /// Looks up the value from a column in the affiliate file in the list of mappings in the 
        /// fieldMappings.AffiliateMappingRule.AffiliateMappings collection and 
        /// then writes it to the relevant field in the Offer record
        /// The column name and offer field are detailed in the field mappings record
        /// </summary>
        /// <param name="dbOfferProperties">List of properties on our Offer Data entity</param>
        /// <param name="dtoRecordProperties">List of properties on our affiliate data</param>
        /// <param name="dtoRecord">the data in the affiliate record</param>
        /// <param name="dbOffer">the offer data entity we will save to the Db</param>
        /// <param name="fieldMapping">the mappings for the current field we are mapping </param>
        /// <param name="targetValue">the value of the affilate data. This is optional - we already have retrieved it for lists but not otherwise</param>
        /// <param name="targetProperty">the property we want to update. This is optional, set when using custom mappings from OfferCountry or Offer Catagory table</param>
        /// <param name="targetObject">the object we want to update. Optional, set on custom mappings for OfferCountry or OfferCategory entity, will use Offer entity if left null</param>
        private bool LookupProperty(ST.OfferImportRecord thisRecord, List<PropertyInfo> dbOfferProperties, List<PropertyInfo> dtoRecordProperties, ST.OfferImportAwin dtoRecord, DB.Offer dbOffer, DTOs.AffiliateFieldMapping fieldMapping, object targetValue = null, PropertyInfo targetProperty = null, Object targetObject = null)
        {
            bool result = false;

            if (targetProperty == null)
                targetProperty = dbOfferProperties.Where(x => x.Name.ToLower() == fieldMapping.ExclusiveFieldName.ToLower().Trim()).FirstOrDefault();

            var dtoRecordProperty = dtoRecordProperties.Where(x => x.Name.ToLower() == fieldMapping.AffiliateFieldName.ToLower().Trim()).FirstOrDefault();

            if (targetProperty != null && dtoRecordProperty != null)
            {
                if (targetValue == null)
                    targetValue = dtoRecordProperty.GetValue(dtoRecord);
                var lookupKey = targetValue;
                var lookupResult = fieldMapping.AffiliateMappingRule.AffiliateMappings.Where(x => x.AffilateValue.ToLower().Trim() == lookupKey.ToString().ToLower().Trim()).FirstOrDefault();
                if (lookupResult != null)
                {
                    if (targetObject == null)
                        targetObject = dbOffer;

                    object typedValue = lookupResult.ExclusiveValue;
                    switch (targetProperty.PropertyType.Name)
                    {
                        case "Int":
                        case "Int32":
                        case "Int64":
                            typedValue = int.Parse(lookupResult.ExclusiveValue);
                            break;

                        case "DateTime":
                            typedValue = DateTime.Parse(lookupResult.ExclusiveValue);
                            break;

                        default:
                            break;
                    }
                    targetProperty.SetValue(targetObject, typedValue);

                    result = true;
                }
                else
                {
                        ST.OfferImportError importError = new ST.OfferImportError()
                        {
                            OfferImportFileId= thisRecord.FileID,
                            OfferImportRecordId=thisRecord.RecordID,
                            AffiliateId = dbOffer.AffiliateId.GetValueOrDefault(),
                            AffiliateMappingRuleId = fieldMapping.AffiliateMappingRuleId.GetValueOrDefault(),
                            AffiliateValue = lookupKey.ToString(),
                            ErrorMessage = $"No AffiliateMappingRule exists for {fieldMapping.ExclusiveFieldName}, {fieldMapping.AffiliateFieldName}, {lookupKey}",
                        };
                        CreateStagingRecordError(importError);
                }
            }
            else
                throw new Exception("Lookup Property not found");

            return result;
        }

        #endregion

        #endregion

        #region Shared Private Methods

        /// <summary>
        /// Locates all records in the staging OfferImportError table associated with <paramref name="dtoFile"/>
        /// </summary>
        /// <param name="dtoFile">Import file details to look for</param>
        /// <returns>list of error records related to the file</returns>
        private List<ST.OfferImportError> GetImportErrorRecords(ST.OfferImportFile dtoFile)
        {
            var dbErrorRecords = _offerImportErrorRepo.FilterNoTrack(e => e.OfferImportFileId == dtoFile.Id);
            var results = _mapper.Map<List<ST.OfferImportError>>(dbErrorRecords);
            return results;
        }

        private void CreateStagingRecordError(ST.OfferImportError importError)
        {
            //_logger.Error($"{importError.ErrorMessage}");
            //return;
            try
            {
                importError.ErrorDateTime = DateTime.UtcNow; //Make sure date is populated
                var dbOfferError = _mapper.Map<DBST.OfferImportError>(importError);
                _offerImportErrorRepo.Create(dbOfferError);

                _offerImportErrorRepo.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to save record to OfferImportError");
            }
        }

        private void AppendToErrorFile(ST.OfferImportFile dtoFile, List<ST.OfferImportError> dtoImportErrors)
        {
            _affiliateManager.AppendToErrorFile(dtoImportErrors, dtoFile.ErrorFilePath);
        }
        #endregion


        public void UpdateOfferImportFileRecord(ST.OfferImportFile dtoFile, Import fileStatusId)
        {
            var dbFile = _offerFilesRepo.GetById(dtoFile.Id);

            if (dbFile != null)
            {
                _mapper.Map(dtoFile, dbFile);
                dbFile.ImportStatus = (int)fileStatusId;
                _offerFilesRepo.Update(dbFile);
                _offerFilesRepo.SaveChanges();
            }
            else
            {
                throw new Exception("Unable to update the Staging.OfferImportFile record for Id " + dtoFile.Id.ToString());
            }
        }

    }
}
