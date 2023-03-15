using CsvHelper;
using ExclusiveCard.Data.Repositories;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO = ExclusiveCard.Services.Models.DTOs;

using ST = ExclusiveCard.Services.Models.DTOs.StagingModels;
using DB = ExclusiveCard.Data.Models;
using DBST = ExclusiveCard.Data.StagingModels;
using AutoMapper;

namespace ExclusiveCard.Managers
{
    public class AffiliateManagerAwin : AffiliateManager<ST.AwinCSVFile, ST.AwinCSVFileError, ST.OfferImportAwin>
    {

        #region Private fields and Constructor

        IRepository<DBST.OfferImportAwin> _awinRepo;
        
        public AffiliateManagerAwin(IRepository<DB.AffiliateFile> fileRepo, IRepository<DB.AffiliateFieldMapping> fieldMappingRepo, IRepository<DBST.OfferImportAwin> awinRepo, IMapper mapper) 
            : base(fileRepo, fieldMappingRepo, mapper)
        {
            _awinRepo = awinRepo;
        }

        #endregion

        #region Public Methods
        
        public override async Task<Tuple<List<ST.OfferImportAwin>, List<ST.AwinCSVFileError>>> MapToSchemaAsync(List<ST.AwinCSVFile> records, ST.OfferImportFile offerImportFile, string cultureInfoCountry)
        {
            var awinRecords = new List<ST.OfferImportAwin>();
            var awinFileErrors = new List<ST.AwinCSVFileError>();

            await Task.WhenAll(records.Select(async rec =>
            {
                StringBuilder sb = new StringBuilder();
                var offerImportAwin = new ST.OfferImportAwin
                {
                    OfferImportFileId = offerImportFile.Id,
                    PromotionId = rec.PromotionID,
                    Advertiser = rec.Advertiser,
                    AdvertiserId = rec.AdvertiserID,
                    Type = rec.Type,
                    Code = rec.Code,
                    Description = rec.Description,
                    Categories = rec.Categories,
                    Regions = rec.Regions,
                    Terms = rec.Terms,
                    DeeplinkTracking = rec.DeeplinkTracking,
                    Deeplink = rec.Deeplink,
                    CommissionGroups = rec.CommissionGroups,
                    Commission = rec.Commission,
                    Exclusive = rec.Exclusive,
                    Title = rec.Title
                };


                try
                {
                    string[] formats =
                    {
                            "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "M/dd/yyyy hh:mm", "MM/dd/yyyy hh:mm",
                            "MM-dd-yyyy", "M/d/yyyy h:mm", "dd-MMM-yyyy HH:mm", "dd-MM-yyyy HH:mm",
                            "dd/MM/yyyy, HH:mm:ss", "dd/MM/yyyy, HH:mm", "M/dd/yyyy, hh:mm", "MM/dd/yyyy, hh:mm",
                            "MM-dd-yyyy", "M/d/yyyy, h:mm", "dd-MMM-yyyy, HH:mm", "dd-MM-yyyy, HH:mm",
                            "d/M/yyyy, h:m:s tt", "d/M/yyyy, H:m:s"
                        };
                    
                    if (!string.IsNullOrWhiteSpace(offerImportFile.CountryCode))
                    {
                        
                        if (string.IsNullOrWhiteSpace(cultureInfoCountry))
                        {
                            cultureInfoCountry = "en-GB"; //set default GB as countryCode culture
                        }
                    }
                    else
                    {
                        cultureInfoCountry = "en-GB"; //set default GB as countryCode culture
                    }
                    if (!string.IsNullOrWhiteSpace(rec.Starts))
                    {
                        DateTime start;
                        if (DateTime.TryParseExact(rec.Starts.Trim(), formats, new CultureInfo(cultureInfoCountry),
                            DateTimeStyles.None, out start))
                        {
                            offerImportAwin.Starts = start;
                        }
                        else
                        {
                            sb.Append("Invalid Start date format. Required format in dd/MM/yyyy HH:mm:ss");
                        }
                    }
                    else
                    {
                        sb.Append("Start Date cannot be empty");
                    }
                    if (!string.IsNullOrWhiteSpace(rec.Ends))
                    {
                        DateTime endsDate;
                        if (DateTime.TryParseExact(rec.Ends.Trim(), formats, new CultureInfo(cultureInfoCountry),
                            DateTimeStyles.None, out endsDate))
                        {
                            offerImportAwin.Ends = endsDate;
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(", Invalid End date format. Required format in dd/MM/yyyy HH:mm:ss");
                            }
                            else
                            {
                                sb.Append("Invalid End date format. Required format in dd/MM/yyyy HH:mm:ss");
                            }
                        }
                    }
                    else
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", End Date cannot be empty");
                        }
                        else
                        {
                            sb.Append("End Date cannot be empty");
                        }
                    }
                    //DateTime dateAdded;
                    if (!string.IsNullOrWhiteSpace(rec.DateAdded))
                    {
                        DateTime dateAdded;
                        if (DateTime.TryParseExact(rec.DateAdded.Trim(), formats, new CultureInfo(cultureInfoCountry),
                            DateTimeStyles.None, out dateAdded))
                        {
                            offerImportAwin.DateAdded = dateAdded;
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append(", Invalid DateAdded format. Required format in dd/MM/yyyy HH:mm:ss");
                            }
                            else
                            {
                                sb.Append("Invalid DateAdded format. Required format in dd/MM/yyyy HH:mm:ss");
                            }
                        }
                    }
                    else
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(", DateAdded cannot be empty");
                        }
                        else
                        {
                            sb.Append("DateAdded Date cannot be empty");
                        }
                    }
                    if (!string.IsNullOrEmpty(sb.ToString()))
                    {
                        throw new Exception(sb.ToString());
                    }

                    awinRecords.Add(offerImportAwin);
                }
                catch (Exception ex)
                {

                    var importAwinError = new ST.AwinCSVFileError
                    {
                        PromotionID = rec.PromotionID,
                        Advertiser = rec.Advertiser,
                        AdvertiserID = rec.AdvertiserID,
                        Type = rec.Type,
                        Code = rec.Code,
                        Description = rec.Description,
                        Starts = rec.Starts,
                        Ends = rec.Ends,
                        Categories = rec.Categories,
                        Regions = rec.Regions,
                        Terms = rec.Terms,
                        DeeplinkTracking = rec.DeeplinkTracking,
                        Deeplink = rec.Deeplink,
                        CommissionGroups = rec.CommissionGroups,
                        Commission = rec.Commission,
                        Exclusive = rec.Exclusive,
                        DateAdded = rec.DateAdded,
                        Title = rec.Title,
                        ErrorMessage = ex.Message
                    };
                    awinFileErrors.Add(importAwinError);
                }


                await Task.CompletedTask;
            }));

            return new Tuple<List<ST.OfferImportAwin>, List<ST.AwinCSVFileError>>(awinRecords, awinFileErrors);


        }

        public override List<ST.AwinCSVFile> ReadFileFromCSV(string filePath)
        {
            var offerImportAwins = new List<Services.Models.DTOs.StagingModels.OfferImportAwin>();


            List<ST.AwinCSVFile> records = new List<ST.AwinCSVFile>();

            //Stream reader will read .csv file in current folder
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                //Csv reader reads the stream
                using (var csvread = new CsvReader(streamReader))
                {
                    // Remove whitespace
                    csvread.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", string.Empty);
                    csvread.Configuration.MissingFieldFound = null;
                    csvread.Configuration.IgnoreBlankLines = false;
                    csvread.Configuration.BadDataFound = context =>
                    {
                    };
                    csvread.Configuration.ReadingExceptionOccurred = (ex) =>
                    {
                        throw new Exception("Error reading CSV file", ex);
                    };

                    //csvread will fetch all record in one go to the IEnumerable object record
                    records = csvread.GetRecords<ST.AwinCSVFile>().ToList();
                }
            }

            return records;
        }

        /// <summary>
        /// This is used to report errors raised when importing file records into the staging table
        /// It saves the <paramref name="errorList"/> records to the <paramref name="errorfile"/>
        /// This does not append, it will replace a file if it already exists and delete the file 
        /// if it already existed and <param name="errorList"> is empty 
        /// </summary>
        /// <param name="errorList"> list of records to be added to the file</param>
        /// <param name="errorfile"> file to be replaced</param>
        /// <returns></returns>
        public override int SaveErrorFile(List<ST.AwinCSVFileError> errorList, string errorfile)
        {

            int errorCount = 0;

            var path = Path.GetDirectoryName(errorfile);
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (errorList != null && errorList.Count > 0)
            {
                errorCount = errorList.Count;


                using (var writer = new StreamWriter(new FileStream(errorfile, FileMode.Create, FileAccess.Write, FileShare.Read)))
                using (var csvWriter = new CsvWriter(writer))
                {
                    //csvWriter.Configuration.Delimiter = ";";
                    csvWriter.Configuration.HasHeaderRecord = true;
                    csvWriter.Configuration.AutoMap<ST.AwinCSVFileError>();

                    csvWriter.WriteHeader<ST.AwinCSVFileError>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(errorList);

                    writer.Flush();
                }
            }
            else
            {
                if (File.Exists(errorfile))
                    //get rid of the existing file, this must be a duplicate file name and
                    //we don't want AppendToErrorFile adding new records to an old file
                    File.Delete(errorfile);
            }

            return errorCount;
        }

        /// <summary>
        /// We need this so we can also report on errors thaT HAPPEN in the offer mapping process 
        /// </summary>
        /// <param name="recordErrorList"></param>
        /// <param name="errorfile"></param>
        /// <returns></returns>
        public override Task<int> AppendToErrorFile(List<ST.OfferImportError> recordErrorList, string errorfile)
        {
            var awinFileErrors = new List<ST.AwinCSVFileError>();

            //Convert to awin csv file structure
            //it mainly empty but what can you do, the csv file was designed for staging import errors
            //And mapping errors don't include all the extra fields
            Task.WhenAll(recordErrorList.Select(async rec =>
            {
                StringBuilder sb = new StringBuilder();
                var importAwinError = new ST.AwinCSVFileError
                {
                    Advertiser = rec.ErrorMessage.Contains("Advertiser") ? rec.AffiliateValue : "",
                    Categories = rec.ErrorMessage.Contains("Categories")? rec.AffiliateValue:"",
                    Regions = rec.ErrorMessage.Contains("Regions") ? rec.AffiliateValue : "",
                    DateAdded = rec.ErrorDateTime.ToString("yyyy MM dd HH:mm:ss"),
                    ErrorMessage = rec.ErrorMessage
                };
                awinFileErrors.Add(importAwinError);
                await Task.CompletedTask;
            }));

            int errorCount = 0;
            if (awinFileErrors != null && awinFileErrors.Count > 0)
            {
                errorCount = awinFileErrors.Count;

                var path = Path.GetDirectoryName(errorfile);
                if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //Create or append to error file
                using (var writer = new StreamWriter(new FileStream(errorfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
                using (var csvWriter = new CsvWriter(writer))
                {
                    csvWriter.Configuration.HasHeaderRecord = true;
                    csvWriter.Configuration.AutoMap<ST.AwinCSVFileError>();

                    csvWriter.WriteHeader<ST.AwinCSVFileError>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(awinFileErrors);

                    writer.Flush();
                }
            }
            return Task.FromResult(errorCount);
        }


        public override int SaveStagingData(List<ST.OfferImportAwin> data)
        {
            int batchSize = 200;
            int recCount = 0;

            var dbAwinData = _mapper.Map<List<DBST.OfferImportAwin>>(data);
            foreach (var dbAwinRecord in dbAwinData)
            {
                _awinRepo.Create(dbAwinRecord);
                recCount++;
                if (recCount >= batchSize)
                {
                    recCount = 0;
                    _awinRepo.SaveChanges();
                }
            }
            if (recCount > 0)
                _awinRepo.SaveChanges();

            return data.Count;

        }

        public override  List<ST.OfferImportAwin> GetStagingData(int affiliateFileId)
        {
            var dbData = _awinRepo.Filter(x => x.OfferImportFileId == affiliateFileId).ToList();
            var dtoData = _mapper.Map<List<ST.OfferImportAwin>>(dbData);
            return dtoData;
        }

        #endregion
    }
}
