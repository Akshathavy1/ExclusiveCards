using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMerchantBranchManager
    {
        Task<MerchantBranch> Add(MerchantBranch branch);
        Task<List<MerchantBranch>> GetAll(int merchantId, bool includeContacts);
        Task<MerchantBranch> Get(int id, bool includeContact, bool includeMerchant);
        Task<MerchantBranch> Update(MerchantBranch branch);
        Task<PagedResult<MerchantBranch>> GetBranchPagedList(int merchantId, int page, int pageSize);
    }
}