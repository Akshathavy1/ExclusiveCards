using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using ST = ExclusiveCard.Services.Models.DTOs.StagingModels;
using ExclusiveCard.Enums;


namespace ExclusiveCard.Managers
{
    public interface IOfferImportManager
    {
        ST.OfferImportFile AddImportFile(ST.OfferImportFile offerImportFile);

        List<ST.OfferImportFile> GetImportFiles(params Import[] fileStatuses);
        List<ST.OfferImportFile> GetImportFiles(Import fileStatus, bool noTrack = false);
        ST.OfferImportFile GetLatestImportFile(int affiliateId, int fileTypeId, bool noTrack = false);
        void UpdateOfferImportFileRecord(ST.OfferImportFile dtoFile, Import fileStatusId);

        Task UploadFileToStagingAsync(ST.OfferImportFile file);

        Task MigrateOffersFromStagingAsync(ST.OfferImportFile file);

    }
}
