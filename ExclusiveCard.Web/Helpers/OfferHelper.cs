using CsvHelper;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using serve = ExclusiveCard.WebAdmin.App.ServiceHelper;

namespace ExclusiveCard.WebAdmin.Helpers
{
    public class OfferHelper : CommonHelper
    {
        ////Get Imported Files and migrate
        //public static async Task GetImportedFilesAndMigrate(ILogger logger)
        //{
        //    try
        //    {
        //        int status = (int)Import.New;//statuses?.FirstOrDefault(x => x.IsActive && x.Type == StatusType.Import && x.Name == Data.Constants.Status.New)?.Id;

        //        //Step 1 - Get New import offer files 
        //        List<DTOs.StagingModels.OfferImportFile> offerImportFiles = await serve.Instance.OfferImportFileService.GetAllAsync(status);
        //        if (offerImportFiles.Any())
        //        {
        //            foreach (DTOs.StagingModels.OfferImportFile importFile in offerImportFiles)
        //            {
        //                DTOs.StagingModels.OfferImportFile offerImportFile = new DTOs.StagingModels.OfferImportFile
        //                {
        //                    Id = importFile.Id,
        //                    AffiliateFileId = importFile.AffiliateFileId,
        //                    DateImported = importFile.DateImported,
        //                    FilePath = importFile.FilePath,
        //                    ErrorFilePath = importFile.ErrorFilePath,
        //                    ImportStatus = importFile.ImportStatus,
        //                    Imported = importFile.Imported,
        //                    TotalRecords = importFile.TotalRecords,
        //                    CountryCode = importFile.CountryCode,
        //                    Duplicates = importFile.Duplicates,
        //                    Updates = importFile.Updates,
        //                    AffiliateFile = new DTOs.AffiliateFile
        //                    {
        //                        Id = importFile.AffiliateFile.Id,
        //                        AffiliateId = importFile.AffiliateFile.AffiliateId,
        //                        Description = importFile.AffiliateFile.Description,
        //                        FileName = importFile.AffiliateFile.FileName,
        //                        StagingTable = importFile.AffiliateFile.StagingTable,
        //                        AffiliateFileMappingId = importFile.AffiliateFile.AffiliateFileMappingId
        //                    }
        //                };

        //                //Step : 2 - Mapping csvFile To Awin
        //                await GetFileAndMapToOfferAwin(offerImportFile, logger);

        //                //Update migrate status to OfferImportFile
        //                await serve.Instance.OfferImportFileService.Update(offerImportFile);

        //                //Step : 3 - Mapping Awin to Staging Table
        //                await GetAwinAndImportToStaging(offerImportFile, logger);
        //                //Update staging process errorFile path to OfferImportFile
        //                await serve.Instance.OfferImportFileService.Update(offerImportFile);

        //                //Step : 4 - Transfer data from stagingtable to exclusive table
        //                await GetStagingAndMapToExclusive(offerImportFile, logger);
        //                //Update complete status to offerImportFile
        //                // await offerImportFileService.Update(offerImportFile);
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("No new records found to migrate offers");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //Get Affiliate file and offer Awin Import
        //public static async Task GetFileAndMapToOfferAwin(DTOs.StagingModels.OfferImportFile offerImportFile, ILogger logger)
        //{
        //    try
        //    {
        //        int errorRecord = 0; //successRecord = 0
        //        string errorfileName = Path.GetFileNameWithoutExtension(offerImportFile.FilePath) + "ImportError" + Path.GetExtension(offerImportFile.FilePath);
        //        string copyFileName = Path.GetFileNameWithoutExtension(offerImportFile.FilePath) + DateTime.UtcNow.ToString("dd MM yyyy hhmmss") + Path.GetExtension(offerImportFile.FilePath);

        //        List<DTOs.StagingModels.OfferImportAwin> offerImportAwins = new List<DTOs.StagingModels.OfferImportAwin>();


        //        List<ErrorImportOfferAwinViewModel> errorFile = new List<ErrorImportOfferAwinViewModel>();

        //        IList<ImportOfferAwinViewModel> record = new List<ImportOfferAwinViewModel>();

        //        //Stream reader will read .csv file in current folder
        //        using (StreamReader streamReader = new StreamReader(offerImportFile.FilePath))
        //        {
        //            //Csv reader reads the stream
        //            using (var csvread = new CsvReader(streamReader))
        //            {
        //                // Remove whitespace
        //                csvread.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", string.Empty);
        //                csvread.Configuration.MissingFieldFound = null;
        //                csvread.Configuration.IgnoreBlankLines = false;
        //                csvread.Configuration.BadDataFound = context =>
        //                {
        //                };
        //                csvread.Configuration.ReadingExceptionOccurred = (ex) =>
        //                {
        //                    logger.Error(ex.ToString());
        //                    return true;
        //                };
        //                //csvread will fetch all record in one go to the IEnumerable object record
        //                record = csvread.GetRecords<ImportOfferAwinViewModel>().ToList();
        //            }
        //        }


        //        //if(offerImportFile.FileType.StagingTable )

        //        //Each record to save importOfferAwin DB
        //        //foreach (var rec in record)
        //        await Task.WhenAll(record.Select(async rec =>
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            DTOs.StagingModels.OfferImportAwin offerImportAwin =
        //                new DTOs.StagingModels.OfferImportAwin
        //                {
        //                    OfferImportFileId = offerImportFile.Id,
        //                    PromotionId = rec.PromotionID,
        //                    Advertiser = rec.Advertiser,
        //                    AdvertiserId = rec.AdvertiserID,
        //                    Type = rec.Type,
        //                    Code = rec.Code,
        //                    Description = rec.Description,
        //                    Categories = rec.Categories,
        //                    Regions = rec.Regions,
        //                    Terms = rec.Terms,
        //                    DeeplinkTracking = rec.DeeplinkTracking,
        //                    Deeplink = rec.Deeplink,
        //                    CommissionGroups = rec.CommissionGroups,
        //                    Commission = rec.Commission,
        //                    Exclusive = rec.Exclusive,
        //                    Title = rec.Title
        //                };
        //            try
        //            {
        //                string[] formats =
        //                {
        //                    "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "M/dd/yyyy hh:mm", "MM/dd/yyyy hh:mm",
        //                    "MM-dd-yyyy", "M/d/yyyy h:mm", "dd-MMM-yyyy HH:mm", "dd-MM-yyyy HH:mm"
        //                };
        //                string cultureInfoCountry = string.Empty;
        //                if (!string.IsNullOrEmpty(offerImportFile.CountryCode))
        //                {
        //                    cultureInfoCountry = GetCultureInfoCountryList().SingleOrDefault(x => x.Text == offerImportFile.CountryCode)?.Value;
        //                    if (string.IsNullOrEmpty(cultureInfoCountry))
        //                    {
        //                        cultureInfoCountry = "en-GB"; //set default GB as countryCode culture
        //                    }
        //                }
        //                else
        //                {
        //                    cultureInfoCountry = "en-GB"; //set default GB as countryCode culture
        //                }
        //                if (!string.IsNullOrEmpty(rec.Starts.ToString()))
        //                {
        //                    DateTime start;
        //                    if (DateTime.TryParseExact(rec.Starts.ToString(), formats, new CultureInfo(cultureInfoCountry),
        //                        DateTimeStyles.None, out start))
        //                    {
        //                        offerImportAwin.Starts = start;
        //                    }
        //                    else
        //                    {
        //                        sb.Append("Invalid Start date format. Required format in dd/MM/yyyy HH:mm:ss");
        //                    }
        //                }
        //                else
        //                {
        //                    sb.Append("Start Date cannot be empty");
        //                }
        //                if (!string.IsNullOrEmpty(rec.Ends.ToString()))
        //                {
        //                    DateTime endsDate;
        //                    if (DateTime.TryParseExact(rec.Ends.ToString(), formats, new CultureInfo(cultureInfoCountry),
        //                        DateTimeStyles.None, out endsDate))
        //                    {
        //                        offerImportAwin.Ends = endsDate;
        //                    }
        //                    else
        //                    {
        //                        if (sb.Length > 0)
        //                        {
        //                            sb.Append(", Invalid End date format. Required format in dd/MM/yyyy HH:mm:ss");
        //                        }
        //                        else
        //                        {
        //                            sb.Append("Invalid End date format. Required format in dd/MM/yyyy HH:mm:ss");
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (sb.Length > 0)
        //                    {
        //                        sb.Append(", End Date cannot be empty");
        //                    }
        //                    else
        //                    {
        //                        sb.Append("End Date cannot be empty");
        //                    }
        //                }
        //                //DateTime dateAdded;
        //                if (!string.IsNullOrEmpty(rec.DateAdded.ToString()))
        //                {
        //                    DateTime dateAdded;
        //                    if (DateTime.TryParseExact(rec.DateAdded.ToString(), formats, new CultureInfo(cultureInfoCountry),
        //                        DateTimeStyles.None, out dateAdded))
        //                    {
        //                        offerImportAwin.DateAdded = dateAdded;
        //                    }
        //                    else
        //                    {
        //                        if (sb.Length > 0)
        //                        {
        //                            sb.Append(", Invalid DateAdded format. Required format in dd/MM/yyyy HH:mm:ss");
        //                        }
        //                        else
        //                        {
        //                            sb.Append("Invalid DateAdded format. Required format in dd/MM/yyyy HH:mm:ss");
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (sb.Length > 0)
        //                    {
        //                        sb.Append(", DateAdded cannot be empty");
        //                    }
        //                    else
        //                    {
        //                        sb.Append("DateAdded Date cannot be empty");
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(sb.ToString()))
        //                {
        //                    throw new Exception(sb.ToString());
        //                }
        //                //await offerImportAwinService.Add(offerImportAwin);
        //                //successRecord++;
        //                offerImportAwins.Add(offerImportAwin);
        //            }
        //            catch (Exception ex)
        //            {
        //                ErrorImportOfferAwinViewModel importAwinError = new ErrorImportOfferAwinViewModel
        //                {
        //                    PromotionID = rec.PromotionID,
        //                    Advertiser = rec.Advertiser,
        //                    AdvertiserID = rec.AdvertiserID,
        //                    Type = rec.Type,
        //                    Code = rec.Code,
        //                    Description = rec.Description,
        //                    Starts = rec.Starts,
        //                    Ends = rec.Ends,
        //                    Categories = rec.Categories,
        //                    Regions = rec.Regions,
        //                    Terms = rec.Terms,
        //                    DeeplinkTracking = rec.DeeplinkTracking,
        //                    Deeplink = rec.Deeplink,
        //                    CommissionGroups = rec.CommissionGroups,
        //                    Commission = rec.Commission,
        //                    Exclusive = rec.Exclusive,
        //                    DateAdded = rec.DateAdded,
        //                    Title = rec.Title,
        //                    ErrorMessage = ex.Message
        //                };
        //                errorFile.Add(importAwinError);
        //                errorRecord++;
        //            }
        //            await Task.CompletedTask;
        //        }));
        //        offerImportFile.TotalRecords = record.Count;

        //        try
        //        {
        //            var importAwinFailed = new List<DTOs.StagingModels.OfferImportAwin>();
        //            if (offerImportAwins.Count > 0)
        //            {
        //                importAwinFailed = await App.ServiceHelper.Instance.OfferImportAwinService.AddToAwinAsync(offerImportAwins);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error(ex);
        //        }

        //        //Any error file while data uploaded
        //        if (errorFile.Count > 0)
        //        {
        //            string path = Path.Combine(
        //                Directory.GetCurrentDirectory(), "Affiliate/" + errorfileName);
        //            using (var writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read)))
        //            using (var csvWriter = new CsvWriter(writer))
        //            {
        //                //csvWriter.Configuration.Delimiter = ";";
        //                csvWriter.Configuration.HasHeaderRecord = true;
        //                csvWriter.Configuration.AutoMap<ErrorImportOfferAwinViewModel>();

        //                csvWriter.WriteHeader<ErrorImportOfferAwinViewModel>();
        //                csvWriter.NextRecord();
        //                csvWriter.WriteRecords(errorFile);

        //                writer.Flush();
        //            }
        //            offerImportFile.ErrorFilePath = path;
        //            //offerImportFile.Error = errorRecord;
        //        }

        //        offerImportFile.ImportStatus = (int)Import.Processing; //status?.FirstOrDefault(x => x.IsActive && x.Type == StatusType.Import && x.Name == Data.Constants.Status.Processing)?.Id;
        //        //offerImportFile.Imported = successRecord;
        //        offerImportFile.Failed = errorRecord;

        //        //Copy File to Archive Folder
        //        string copyPath = Path.Combine(
        //               Directory.GetCurrentDirectory(), "Archive/");

        //        if (!Directory.Exists(copyPath))
        //        {
        //            Directory.CreateDirectory(copyPath);
        //        }

        //        copyPath = Path.Combine(copyPath, copyFileName);
        //        File.Move(offerImportFile.FilePath, copyPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        ////Get AwinRecord and Mapping then import staging table
        //public static async Task GetAwinAndImportToStaging(DTOs.StagingModels.OfferImportFile offerImportFile, ILogger logger)
        //{
        //    await Task.Yield();
        //    try
        //    {
        //        int stagedRecord = 0, errorRecord = 0;
        //        List<ErrorImportOfferAwinViewModel> errorStagingFile = new List<ErrorImportOfferAwinViewModel>();
        //        //List<string> offerCountrycodes = new List<string>();
        //        //List<int> offerCategoryids = new List<int>();

        //        //Get All Staging.OfferImportAwin with fileId
        //        List<DTOs.StagingModels.OfferImportAwin> offerImportAwins = await serve.Instance.OfferImportAwinService.GetAll(offerImportFile.Id);
        //        if (offerImportAwins.Any())
        //        {
        //            DTOs.AffiliateMapping affiliateMapping = new DTOs.AffiliateMapping();
        //            DTOs.AffiliateFile affiliateFile = await serve.Instance.AffiliateFileService.Get(offerImportFile.AffiliateFileId);

        //            //Get AffiliateFieldMapping record
        //            List<DTOs.AffiliateFieldMapping> affiliateFieldMappings = await serve.Instance.AffiliateFieldMappingService.GetAll(affiliateFile.AffiliateFileMappingId);
        //            List<Type> entityTypes = serve.Instance.AffiliateMappingService.GetEntityTypes();

        //            //Get all affiliateMapping record
        //            List<DTOs.AffiliateMapping> affiliateMappingsAllList = await serve.Instance.AffiliateMappingService.GetAll();

        //            //For each record of offerImportAwin transfer to staging table
        //            //foreach (DTOs.StagingModels.OfferImportAwin offerImportAwin in offerImportAwins)
        //            List<DTOs.StagingModels.Offer> stagingOffers = new List<DTOs.StagingModels.Offer>();
        //            //List<int> awinOffers = new List<int>();
        //            DateTime dtStart = DateTime.UtcNow;
        //            await Task.WhenAll(offerImportAwins.Select(async offerImportAwin =>
        //            {
        //                try
        //                {
        //                    //Get All AffiliateMapping Values
        //                    List<DTOs.StagingModels.OfferCategory> offerCategories =
        //                        new List<DTOs.StagingModels.OfferCategory>();
        //                    List<DTOs.OfferCountry> offerCountries =
        //                        new List<DTOs.OfferCountry>();
        //                    DTOs.StagingModels.Offer stagingOffer = new DTOs.StagingModels.Offer();
        //                    StringBuilder sb = new StringBuilder();
        //                    var props = offerImportAwin.GetType().GetProperties().Where(x => x.Name != "Id").ToList();

        //                    Object affiliateValueNewObj = new Object();

        //                    //Mapping exculsive value with affiliateField Mapping 
        //                    //foreach (PropertyInfo propertyInfo in props)
        //                    await Task.WhenAll(props.Select(async propertyInfo =>
        //                    {
        //                        if (affiliateFieldMappings.Any(x => x.AffiliateFieldName == propertyInfo.Name))
        //                        {
        //                            var mapping = affiliateFieldMappings.First(x =>
        //                                x.AffiliateFieldName == propertyInfo.Name);
        //                            var affiliateValueObj = GetPropertyValue(offerImportAwin, propertyInfo.Name);
        //                            if (affiliateValueObj == null) return;
        //                            AffiliateMatchTypes matchTypeId =
        //                                (AffiliateMatchTypes)mapping.AffiliateMatchTypeId;
        //                            if (!mapping.IsList)
        //                            {
        //                                //Single field mapping in affiliateFieldMapping process 
        //                                if (mapping.AffiliateMappingRuleId.HasValue)
        //                                {
        //                                    //DTOs.AffiliateMapping responseaffiliateMapping = await affiliateMappingService.GetAffiliateMapping(matchTypeId,(int)mapping.AffiliateMappingRuleId, affiliateValueObj);
        //                                    if (mapping.AffiliateFieldName == "Advertiser" && affiliateValueObj.ToString().Contains("(Global)"))
        //                                    {
        //                                        affiliateValueNewObj = affiliateValueObj.ToString()
        //                                              .Remove(affiliateValueObj.ToString().IndexOf(" (Global)", StringComparison.CurrentCultureIgnoreCase));
        //                                        DTOs.AffiliateMapping responseaffiliateMapping =
        //                                            GetAffiliateMappingFilter(matchTypeId,
        //                                                (int)mapping.AffiliateMappingRuleId, affiliateValueNewObj,
        //                                                affiliateMappingsAllList);
        //                                        if (responseaffiliateMapping != null)
        //                                        {
        //                                            SetPropertyValue(stagingOffer, mapping.ExclusiveFieldName,
        //                                                Convert.ToInt32(responseaffiliateMapping.ExclusiveValue));
        //                                        }
        //                                        else
        //                                        {
        //                                            responseaffiliateMapping = GetAffiliateMappingFilter(matchTypeId,
        //                                                (int)mapping.AffiliateMappingRuleId, affiliateValueObj,
        //                                                affiliateMappingsAllList);
        //                                            if (responseaffiliateMapping != null)
        //                                            {
        //                                                SetPropertyValue(stagingOffer, mapping.ExclusiveFieldName,
        //                                                    Convert.ToInt32(responseaffiliateMapping.ExclusiveValue));
        //                                            }
        //                                            else
        //                                            {
        //                                                sb.Append(mapping.AffiliateFieldName + " value not found in Exclusive");
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        DTOs.AffiliateMapping responseaffiliateMapping =
        //                                            GetAffiliateMappingFilter(matchTypeId,
        //                                                (int)mapping.AffiliateMappingRuleId, affiliateValueObj,
        //                                                affiliateMappingsAllList);
        //                                        if (responseaffiliateMapping != null)
        //                                        {
        //                                            SetPropertyValue(stagingOffer, mapping.ExclusiveFieldName,
        //                                                Convert.ToInt32(responseaffiliateMapping.ExclusiveValue));
        //                                        }
        //                                        else
        //                                        {
        //                                            sb.Append(mapping.AffiliateFieldName + " value not found in Exclusive");
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    SetPropertyValue(stagingOffer, mapping.ExclusiveFieldName,
        //                                        affiliateValueObj);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //checking merchant name with (Global) specified then consider all country codes
        //                                List<object> lst = new List<object>();
        //                                var merchantAffiliateValueObj = GetPropertyValue(offerImportAwin, "Advertiser");
        //                                if (mapping.AffiliateFieldName == "Regions" &&
        //                                    merchantAffiliateValueObj.ToString().Contains("(Global)"))
        //                                {
        //                                    List<DTOs.AffiliateMapping> affiliateMappingList = affiliateMappingsAllList
        //                                        .Where(x => x.AffiliateMappingRuleId ==
        //                                                    (int)mapping.AffiliateMappingRuleId).ToList();

        //                                    if (affiliateMappingList?.Count > 0)
        //                                    {
        //                                        foreach (var affiliateMappingvalue in affiliateMappingList)
        //                                        {
        //                                            lst.Add(new Data.StagingModels.OfferCountry()
        //                                            {
        //                                                CountryCode = affiliateMappingvalue?.ExclusiveValue,
        //                                                IsActive = true
        //                                            });
        //                                            DTOs.OfferCountry offerCountry =
        //                                                new DTOs.OfferCountry
        //                                                {
        //                                                    CountryCode = affiliateMappingvalue?.ExclusiveValue,
        //                                                    IsActive = true
        //                                                };
        //                                            offerCountries.Add(offerCountry);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //List split values with delimiter and process
        //                                    string[] affiliateValues = affiliateValueObj.ToString()
        //                                        .Split(mapping.Delimiter, StringSplitOptions.RemoveEmptyEntries);
        //                                    if (affiliateValues.Length > 0)
        //                                    {
        //                                        affiliateValues = affiliateValues.Distinct().ToArray();
        //                                    }
        //                                    Type entityType = entityTypes.FirstOrDefault(et =>
        //                                        MatchTableAndSchema(et, mapping.ExclusiveTable));

        //                                    foreach (var affiliateValue in affiliateValues)
        //                                    {
        //                                        var localValue = affiliateValue.Trim();
        //                                        if (mapping.AffiliateFieldName == "Regions") //&&country.ContainsKey(affiliateValue)
        //                                        {
        //                                            //localValue = country[affiliateValue];
        //                                            //DTOs.AffiliateMapping responseaffiliateMapping = await affiliateMappingService.GetAffiliateMapping(matchTypeId,
        //                                            //                                    (int)mapping.AffiliateMappingRuleId, affiliateValue);
        //                                            DTOs.AffiliateMapping responseaffiliateMapping =
        //                                                GetAffiliateMappingFilter(matchTypeId,
        //                                                    (int)mapping.AffiliateMappingRuleId, affiliateValue.Trim(),
        //                                                    affiliateMappingsAllList);
        //                                            if (responseaffiliateMapping != null)
        //                                            {
        //                                                localValue = responseaffiliateMapping.ExclusiveValue;
        //                                            }
        //                                            else
        //                                            {
        //                                                localValue = string.Empty;
        //                                            }
        //                                            if (!string.IsNullOrEmpty(localValue) && entityType != null)
        //                                            {
        //                                                // var instance = Activator.CreateInstance(entityType);
        //                                                // SetPropertyValue(instance, propertyInfo.Name, localValue);
        //                                                lst.Add(new Data.StagingModels.OfferCountry()
        //                                                {
        //                                                    CountryCode = localValue,
        //                                                    IsActive = true
        //                                                });
        //                                                DTOs.OfferCountry offerCountry =
        //                                                    new DTOs.OfferCountry
        //                                                    {
        //                                                        CountryCode = localValue,
        //                                                        IsActive = true
        //                                                    };
        //                                                offerCountries.Add(offerCountry);
        //                                            }
        //                                        }
        //                                        else if (mapping.AffiliateFieldName == "Categories")
        //                                        {
        //                                            //List<DTOs.AffiliateMapping> responseAffiliateMappings = await affiliateMappingService.GetAffiliateMappingList(matchTypeId,
        //                                            //    (int)mapping.AffiliateMappingRuleId, localValue);
        //                                            List<DTOs.AffiliateMapping> responseAffiliateMappings =
        //                                                GetAffiliateMappingFilterList(matchTypeId,
        //                                                    (int)mapping.AffiliateMappingRuleId, affiliateValue.Trim(),
        //                                                    affiliateMappingsAllList);
        //                                            if (responseAffiliateMappings != null)
        //                                            {
        //                                                foreach (DTOs.AffiliateMapping affiliateMappingvalue in
        //                                                    responseAffiliateMappings)
        //                                                {
        //                                                    lst.Add(new Data.StagingModels.OfferCategory()
        //                                                    {
        //                                                        CategoryId =
        //                                                            Convert.ToInt32(affiliateMappingvalue.ExclusiveValue)
        //                                                    });
        //                                                    DTOs.StagingModels.OfferCategory offerCategory =
        //                                                        new DTOs.StagingModels.OfferCategory
        //                                                        {
        //                                                            CategoryId =
        //                                                                Convert.ToInt32(
        //                                                                    affiliateMappingvalue.ExclusiveValue)
        //                                                        };
        //                                                    offerCategories.Add(offerCategory);
        //                                                }
        //                                            }
        //                                        }
        //                                        else
        //                                        {
        //                                            //DTOs.AffiliateMapping responseaffiliateMapping = await affiliateMappingService.GetAffiliateMapping(matchTypeId,
        //                                            //    (int)mapping.AffiliateMappingRuleId, localValue);
        //                                            DTOs.AffiliateMapping responseaffiliateMapping =
        //                                                GetAffiliateMappingFilter(matchTypeId,
        //                                                    (int)mapping.AffiliateMappingRuleId, affiliateValueObj,
        //                                                    affiliateMappingsAllList);
        //                                            if (responseaffiliateMapping != null)
        //                                            {
        //                                                var instance = Activator.CreateInstance(entityType);
        //                                                SetPropertyValue(instance, propertyInfo.Name, localValue);
        //                                                lst.Add(instance);
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                if (lst.Count > 0 && !string.IsNullOrEmpty(mapping.ExclusiveFieldName))
        //                                {
        //                                    if (mapping.AffiliateFieldName == "Categories")
        //                                    {
        //                                        offerCategories = offerCategories.GroupBy(x => x.CategoryId).Select(x => x.First()).ToList();
        //                                        SetPropertyValue(stagingOffer, mapping.ExclusiveFieldName,
        //                                            offerCategories);
        //                                    }
        //                                    else if (mapping.AffiliateFieldName == "Regions")
        //                                    {
        //                                        SetPropertyValue(stagingOffer, mapping.ExclusiveFieldName,
        //                                            offerCountries);
        //                                    }
        //                                    else
        //                                    {
        //                                        SetPropertyValue(stagingOffer, mapping.ExclusiveFieldName, lst);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var value = GetPropertyValue(offerImportAwin, propertyInfo.Name);
        //                            SetPropertyValue(stagingOffer, propertyInfo.Name, value);
        //                        }
        //                        await Task.CompletedTask;
        //                    }));

        //                    var break2 = DateTime.UtcNow.Subtract(dtStart).Milliseconds;
        //                    //error handling
        //                    if (!string.IsNullOrEmpty(sb.ToString()))
        //                    {
        //                        throw new Exception(sb.ToString());
        //                    }
        //                    if (stagingOffer.OfferCategories?.Count > 0)
        //                    {
        //                        stagingOffer.StatusId = (int) OfferStatus.Active;//status?.FirstOrDefault(x => x.IsActive && x.Type == StatusType.Offer && x.Name == Data.Constants.Status.Active)?.Id;
        //                    }
        //                    else
        //                    {
        //                        stagingOffer.StatusId = (int) OfferStatus.NeedsReview;//status?.FirstOrDefault(x => x.IsActive && x.Type == StatusType.Offer && x.Name == Data.Constants.Status.NeedsReview)?.Id;
        //                    }
        //                    stagingOffer.SearchRanking = 5;
        //                    stagingOffer.AffiliateId = affiliateFile.AffiliateId;
        //                    //save stagingOffer with offerCountry and offerCategory
        //                    //await stagingOfferService.Add(stagingOffer);
        //                    stagedRecord++;
        //                    //await offerImportAwinService.Delete(offerImportAwin.Id);
        //                    stagingOffers.Add(stagingOffer);
        //                    //awinOffers.Add(offerImportAwin.Id);
        //                }
        //                catch (Exception ex)
        //                {
        //                    ErrorImportOfferAwinViewModel errorFile = new ErrorImportOfferAwinViewModel
        //                    {
        //                        PromotionID = offerImportAwin.PromotionId,
        //                        Advertiser = offerImportAwin.Advertiser,
        //                        AdvertiserID = offerImportAwin.AdvertiserId,
        //                        Type = offerImportAwin.Type,
        //                        Code = offerImportAwin.Code,
        //                        Description = offerImportAwin.Description,
        //                        Starts = offerImportAwin.Starts.ToString("dd/MM/yyyy HH:mm:ss"),
        //                        Ends = offerImportAwin.Ends.ToString("dd/MM/yyyy HH:mm:ss"),
        //                        Categories = offerImportAwin.Categories,
        //                        Regions = offerImportAwin.Regions,
        //                        Terms = offerImportAwin.Terms,
        //                        DeeplinkTracking = offerImportAwin.DeeplinkTracking,
        //                        Deeplink = offerImportAwin.Deeplink,
        //                        CommissionGroups = offerImportAwin.CommissionGroups,
        //                        Commission = offerImportAwin.Commission,
        //                        Exclusive = offerImportAwin.Exclusive,
        //                        DateAdded = offerImportAwin.DateAdded.ToString("dd/MM/yyyy HH:mm:ss"),
        //                        Title = offerImportAwin.Title,
        //                        ErrorMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message
        //                    };
        //                    errorStagingFile.Add(errorFile);
        //                    errorRecord++;
        //                    //awinOffers.Add(offerImportAwin.Id);
        //                }
        //                await Task.CompletedTask;
        //            }));
        //            try
        //            {
        //                var offersFailed = new List<DTOs.StagingModels.Offer>();
        //                if (stagingOffers.Count > 0)
        //                {
        //                    offersFailed = await App.ServiceHelper.Instance.StagingOfferService.AddToStagingAsync(stagingOffers);
        //                }
        //                if (offerImportAwins.Count > 0)
        //                {
        //                    List<DTOs.StagingModels.OfferImportAwin> reqAwinsToBeDeleted =
        //                        App.ServiceHelper.Instance.Mapper
        //                            .Map<List<DTOs.StagingModels.OfferImportAwin>>(
        //                                offerImportAwins);
        //                    await App.ServiceHelper.Instance.OfferImportAwinService.Delete(reqAwinsToBeDeleted);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error(ex);
        //            }
        //            offerImportFile.Staged = stagedRecord;
        //            offerImportFile.Failed = offerImportFile.Failed + errorRecord;
        //            if (errorStagingFile.Count > 0)
        //            {
        //                if (!string.IsNullOrEmpty(offerImportFile.ErrorFilePath) && File.Exists(offerImportFile.ErrorFilePath))
        //                {
        //                    using (var sr = new StreamReader(offerImportFile.ErrorFilePath))
        //                    {
        //                        using (var sw = new StreamWriter(offerImportFile.ErrorFilePath))
        //                        {
        //                            var csvreader = new CsvReader(sr);
        //                            var csvWriter = new CsvWriter(sw);

        //                            //CSVReader will now read the whole file into an enumerable
        //                            IList<ErrorImportOfferAwinViewModel> errorData = csvreader.GetRecords<ErrorImportOfferAwinViewModel>().ToList();

        //                            //Write the entire contents of the CSV file into another
        //                            csvWriter.WriteRecords(errorData);
        //                            csvWriter.WriteRecords(errorStagingFile);
        //                            csvWriter.Flush();
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    string stagingerrorfileName = Path.GetFileNameWithoutExtension(offerImportFile.FilePath) + "StagingError" + Path.GetExtension(offerImportFile.FilePath);
        //                    string path = Path.Combine(
        //                Directory.GetCurrentDirectory(), "Affiliate/" + stagingerrorfileName);
        //                    using (var writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read)))
        //                    using (var csvWriter = new CsvWriter(writer))
        //                    {
        //                        //csvWriter.Configuration.Delimiter = ";";
        //                        csvWriter.Configuration.HasHeaderRecord = true;
        //                        csvWriter.Configuration.AutoMap<ErrorImportOfferAwinViewModel>();

        //                        csvWriter.WriteHeader<ErrorImportOfferAwinViewModel>();
        //                        csvWriter.NextRecord();
        //                        csvWriter.WriteRecords(errorStagingFile);

        //                        writer.Flush();
        //                    }
        //                    offerImportFile.ErrorFilePath = path;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //Staging record transfer to Exclusive table
        //public static async Task GetStagingAndMapToExclusive(DTOs.StagingModels.OfferImportFile offerImportFile, ILogger logger)
        //{
        //    int successRecord = 0, errorRecord = 0;
        //    try
        //    {
        //        int records = 100;
        //        int.TryParse(serve.Instance.Settings.Value.AWINRecordsToProcess, out records);

        //        await serve.Instance.OfferService.ExecuteSPMappingSTOfferToOffer(offerImportFile
        //            .AffiliateFile.AffiliateId, offerImportFile.Id, records);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static async Task UpdateOfferListBatchJob(DateTime date, IOfferService offerService)
        {
            try
            {
                //Hard coding List Names as per requirement
                var DailyDeals = await offerService.GetOfferListByName("Daily Deals");
                var EndingSoon = await offerService.GetOfferListByName("Ending Soon");

                //clear all the existing items in daily deals and ending soon list
                await offerService.DeleteOfferListItemsById(DailyDeals.Id);
                await offerService.DeleteOfferListItemsById(EndingSoon.Id);

                //get all the active Offers
                var listofOffers = await offerService.GetAll();

                //get all the existing offer where valid from is {date} add them to daily deals List
                var dailyDealsOffers = listofOffers.Where(x => x.ValidFrom != null && x.ValidFrom.Value.Date == date.Date && x.ValidTo != null && x.ValidTo.Value.Date > date.Date).ToList();


                //getoffer valid from is one day more than {date} add them to Ending soon list
                var endingSoonOffers = listofOffers.Where(x => x.ValidTo != null && x.ValidTo.Value.Date <= date.AddHours(24)).ToList();


                if (dailyDealsOffers.Count > 0)
                {
                    await AddOfferListItems(DailyDeals.Id, dailyDealsOffers, offerService);
                }
                if (endingSoonOffers.Count > 0)
                {
                    await AddOfferListItems(EndingSoon.Id, endingSoonOffers, offerService);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #region Private Methods

        private static async Task AddOfferListItems(int listId, List<DTOs.Offer> Offers, IOfferService offerService)
        {
            List<DTOs.OfferListItem> CombinedListToAdd = new List<DTOs.OfferListItem>();
            foreach (var dailydeal in Offers)
            {
                var offerContries = dailydeal.OfferCountries.ToList();

                if (offerContries.Count > 1)
                {
                    var displsyOrder = 1;
                    foreach (var country in offerContries)
                    {

                        var existngRecordsForOfferList = await offerService.GetByListIdAndCountry(listId, country.CountryCode);
                        if (existngRecordsForOfferList != null && existngRecordsForOfferList.Count > 0)
                        {
                            displsyOrder = existngRecordsForOfferList.Max(x => x.DisplayOrder) + 1;
                        }
                        DTOs.OfferListItem dailydealitem = new DTOs.OfferListItem();
                        dailydealitem.OfferListId = listId;
                        dailydealitem.OfferId = dailydeal.Id;
                        dailydealitem.CountryCode = country.CountryCode;
                        dailydealitem.DisplayFrom = dailydeal.ValidFrom;
                        dailydealitem.DisplayTo = dailydeal.ValidTo;
                        dailydealitem.DisplayOrder = Convert.ToInt16(displsyOrder);
                        await offerService.AddOfferListItem(dailydealitem);
                    }
                }
                else
                {
                    var displsyOrder = 1;
                    var existngRecordsForOfferList = await offerService.GetByListIdAndCountry(listId, dailydeal.OfferCountries.ToList()[0].CountryCode);
                    if (existngRecordsForOfferList != null && existngRecordsForOfferList.Count > 0)
                    {
                        displsyOrder = existngRecordsForOfferList.Max(x => x.DisplayOrder) + 1;
                    }
                    DTOs.OfferListItem dailydealitem = new DTOs.OfferListItem();
                    dailydealitem.OfferListId = listId;
                    dailydealitem.OfferId = dailydeal.Id;
                    dailydealitem.CountryCode = dailydeal.OfferCountries.ToList()[0].CountryCode;
                    dailydealitem.DisplayFrom = dailydeal.ValidFrom;
                    dailydealitem.DisplayTo = dailydeal.ValidTo;
                    dailydealitem.DisplayOrder = Convert.ToInt16(displsyOrder);
                    await offerService.AddOfferListItem(dailydealitem);
                }
            }


        }

        private static DTOs.AffiliateMapping GetAffiliateMappingFilter(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj, List<DTOs.AffiliateMapping> affiliateMappingsAll)
        {
            DTOs.AffiliateMapping returnaffiliateMapping = null;
            switch (matchTypeId)
            {
                case AffiliateMatchTypes.Equals:
                    returnaffiliateMapping = affiliateMappingsAll.FirstOrDefault(x => x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.Trim() == affiliateValueObj.ToString().Trim());
                    break;
                case AffiliateMatchTypes.LikeB:
                    returnaffiliateMapping = affiliateMappingsAll.FirstOrDefault(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && affiliateValueObj.ToString().Contains(x.AffilateValue)); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.LikeA:
                    returnaffiliateMapping = affiliateMappingsAll.FirstOrDefault(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.Contains(affiliateValueObj.ToString())); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.StartsWith:
                    returnaffiliateMapping = affiliateMappingsAll.FirstOrDefault(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.StartsWith(affiliateValueObj.ToString())); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.EndsWith:
                    returnaffiliateMapping = affiliateMappingsAll.FirstOrDefault(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.EndsWith(affiliateValueObj.ToString())); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.Custom:

                    break;
            }
            return returnaffiliateMapping;
        }

        private static List<DTOs.AffiliateMapping> GetAffiliateMappingFilterList(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj, List<DTOs.AffiliateMapping> affiliateMappingsAll)
        {
            List<DTOs.AffiliateMapping> affiliateMappings = null;
            switch (matchTypeId)
            {
                case AffiliateMatchTypes.Equals:
                    affiliateMappings = affiliateMappingsAll.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.Trim() == affiliateValueObj.ToString().Trim()).ToList(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.LikeB:
                    affiliateMappings = affiliateMappingsAll.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && affiliateValueObj.ToString().Contains(x.AffilateValue)).ToList(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.LikeA:
                    affiliateMappings = affiliateMappingsAll.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.Contains(affiliateValueObj.ToString())).ToList(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.StartsWith:
                    affiliateMappings = affiliateMappingsAll.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.StartsWith(affiliateValueObj.ToString())).ToList(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.EndsWith:
                    affiliateMappings = affiliateMappingsAll.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.EndsWith(affiliateValueObj.ToString())).ToList(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.Custom:

                    break;
            }
            return affiliateMappings;
        }

        #endregion

    }
}

