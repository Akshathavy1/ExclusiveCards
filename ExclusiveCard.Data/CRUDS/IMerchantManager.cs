using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMerchantManager
    {
        Task<Merchant> Add(Merchant merchant);
        Task<Merchant> Update(Merchant merchant);
        Merchant Get(int id, bool includeBranch, bool includeBranchContact, bool includeImage, bool includeSocialMedia);
        Merchant GetByName(string name);
        Task<List<Merchant>> GetAll(bool includeBranch, bool includeImage, bool includeSocialMedia);

        Task<PagedResult<Merchant>> GetPagedListAsync(string searchText, int page, int pageSize,
            MerchantsSortOrder sortOrder);
        //Task<List<Merchant>> GetAllMerchants(bool includeMerchants);
        List<Merchant> GetMerchants();
    }
}