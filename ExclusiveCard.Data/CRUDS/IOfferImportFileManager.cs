using System.Collections.Generic;
using System.Threading.Tasks;
using ST = ExclusiveCard.Data.StagingModels;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferImportFileManager
    {
        Task<ST.OfferImportFile> Add(ST.OfferImportFile offerImportFile);
        Task<ST.OfferImportFile> Update(ST.OfferImportFile offerImportFile);
        ST.OfferImportFile Get(int affiliateId, int fileTypeId, int status);
        Task<ST.OfferImportFile> GetById(int id, int? status = 0);
        Task<List<ST.OfferImportFile>> GetAllAsync(int? importStatus);
    }
}