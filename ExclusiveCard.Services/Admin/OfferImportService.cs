using ExclusiveCard.Enums;
using ExclusiveCard.Managers;
using ExclusiveCard.Services.Interfaces.Admin;
using NLog;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
    public class OfferImportService : IOfferImportService
    {
        #region Private fields and constructor

        IOfferImportManager _offerImportManager;
        readonly ILogger _logger;

        public OfferImportService(IOfferImportManager offerImportManager)
        {
            _offerImportManager = offerImportManager;

            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves details about the import file to Staging.OfferImportFile
        /// </summary>
        /// <param name="offerImportFile"></param>
        /// <returns></returns>
        public dto.StagingModels.OfferImportFile AddImportFile(dto.StagingModels.OfferImportFile offerImportFile)
        {
           var result = _offerImportManager.AddImportFile(offerImportFile);
           return result;
        }

        /// <summary>
        /// Mark the current file as complete
        /// Called from the admin tool by the user to acknowledge they 
        /// have finnished with the current file.
        /// Once actioned, downloading the error file will not be available on the 
        /// admin tool but further files can now be imported
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="fileTypeId"></param>
        /// <returns></returns>
        public async Task CompleteImport(int affiliateId, int fileTypeId)
        {
            Import[] statuses = new Import[] { Enums.Import.Failed, Enums.Import.Processed };
            var importedFiles = _offerImportManager.GetImportFiles(statuses);
            var result = importedFiles.FindLast(i =>
                                                i.AffiliateFile.AffiliateId == affiliateId &&
                                                i.AffiliateFile.Id == fileTypeId &&
                                                i.AffiliateFileId == fileTypeId);
            if(result != null)
                _offerImportManager.UpdateOfferImportFileRecord(result, Enums.Import.Complete);

            await Task.CompletedTask;
        }


        public dto.StagingModels.OfferImportFile GetImportFile(int affiliateId, int fileTypeId, int status)
        {
            var importedFiles = _offerImportManager.GetImportFiles((Import)status, true);

            var result = importedFiles.FindLast(i =>
                                                i.AffiliateFile.AffiliateId == affiliateId &&
                                                i.AffiliateFile.Id == fileTypeId &&
                                                i.AffiliateFileId == fileTypeId );
            return result;
        }

        public dto.StagingModels.OfferImportFile GetLatestImportFile(int affiliateId, int fileTypeId)
        {
            var result = _offerImportManager.GetLatestImportFile(affiliateId, fileTypeId, true);
            return result;
        }

        /// <summary>
        /// STEP 1
        /// Loads an affiliate CSV file into the staging data table, as the 1st step in the Offer import process
        /// Each affiliate has its own bespoke format of data, and its own staging table in this format.
        /// If any rows are invalid, they are written out to an error file name [filename]_Error.csv, in the same dir as original file.
        /// An invalid row does not stop the rest of the file from being loaded.
        /// </summary>
        /// <returns></returns>
        public async Task UploadToStaging()
        {
            var files = _offerImportManager.GetImportFiles(Enums.Import.New, true);
            if (files != null)
            {
                foreach (var file in files)
                {
                    await _offerImportManager.UploadFileToStagingAsync(file);
                }
            }
        }

        /// <summary>
        /// STEP 2
        /// Copies valid staging offer records to the offer table
        /// </summary>
        /// <returns></returns>
        public async Task MigrateFromStaging()
        {
            var files = _offerImportManager.GetImportFiles(Enums.Import.Uploaded, true);
            if (files != null)
            {
                foreach (var file in files)
                {
                   await _offerImportManager.MigrateOffersFromStagingAsync(file);
                }
            }
        }

        /// <summary>
        /// STEPS 1 & 2 COMBINED
        /// </summary>
        /// <returns></returns>
        public async Task Import()
        {
            var files = _offerImportManager.GetImportFiles(Enums.Import.New, true);
            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        await _offerImportManager.UploadFileToStagingAsync(file);

                        await _offerImportManager.MigrateOffersFromStagingAsync(file);
                    }
                    catch(System.Exception ex)
                    {
                        _logger.Error(ex, "Error importing");
                    }
                }
            }

        }

        #endregion


    }
}
