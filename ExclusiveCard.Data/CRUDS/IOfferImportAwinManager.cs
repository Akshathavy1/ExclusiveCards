using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferImportAwinManager
    {
        Task<StagingModels.OfferImportAwin> Add(StagingModels.OfferImportAwin offerImport);
        Task<StagingModels.OfferImportAwin> Update(StagingModels.OfferImportAwin offerImport);
        Task Delete(List<StagingModels.OfferImportAwin> awins);
        Task<List<StagingModels.OfferImportAwin>> AddToAwinAsync(List<StagingModels.OfferImportAwin> offerImportAwins);
        Task<List<StagingModels.OfferImportAwin>> GetAllAsync(int? importFileId);
    }
}