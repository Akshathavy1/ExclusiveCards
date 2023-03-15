using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs.StagingModels;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IOfferImportFileService
    {
        Task<OfferImportFile> Add(OfferImportFile offerImportFile);

        Task<OfferImportFile> Update(OfferImportFile offerImportFile);

        OfferImportFile Get(int affiliateId, int fileTypeId, int status);

        Task<OfferImportFile> GetById(int id, int? status = 0);

        Task<List<OfferImportFile>> GetAllAsync(int? importStatus);
    }
}
