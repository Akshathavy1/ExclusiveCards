using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IMerchantBranchService
    {
        #region Writes

        Task<MerchantBranch> Add(MerchantBranch branch);

        Task<MerchantBranch> Update(MerchantBranch branch);

        #endregion

        #region Reads

        Task<MerchantBranch> Get(int id, bool includeContact = false, bool includeMerchant = false);

        Task<List<MerchantBranch>> GetAll(int merchantId, bool includeContacts = false);

        Task<PagedResult<MerchantBranch>> GetPagedResult(int merchantId, int pageNo, int pageSize);

        #endregion
    }
}
