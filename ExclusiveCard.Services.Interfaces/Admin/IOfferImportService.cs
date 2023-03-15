using dto = ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IOfferImportService
    {
        dto.StagingModels.OfferImportFile AddImportFile(dto.StagingModels.OfferImportFile offerImportFile);

        dto.StagingModels.OfferImportFile GetImportFile(int affiliateId, int fileTypeId, int status);

        dto.StagingModels.OfferImportFile GetLatestImportFile(int affiliateId, int fileTypeId);

        Task CompleteImport(int affiliateId, int fileTypeId);

        Task UploadToStaging();

        Task MigrateFromStaging();

        Task Import();
    }
}
