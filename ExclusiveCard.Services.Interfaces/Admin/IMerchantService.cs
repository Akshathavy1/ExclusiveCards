using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IMerchantService
    {
        #region Writes

        Task<Merchant> Add(Merchant merchant);

        Task<Merchant> Update(Merchant merchant);

        #endregion

        #region Reads

        Merchant Get(int id, bool includeBranch = false, bool includeBranchContact = false,
            bool includeImage = false, bool includeSocialMedia = false);

        Merchant GetByName(string name);

        Task<List<Merchant>> GetAll(
            bool includeBranch = false, bool includeImage = false, bool includeSocialMedia = false);

        Task<PagedResult<Merchant>> GetPagedResult(string searchText, int pageNo, int pageSize, MerchantsSortOrder sortOrder);
       List<string> GetAllMerchants();
        #endregion
    }
}
