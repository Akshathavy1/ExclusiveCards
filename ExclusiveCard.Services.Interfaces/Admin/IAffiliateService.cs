using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IAffiliateService
    {
        #region Writes

        Task<Affiliate> Add(Affiliate affiliate);

        Task<Affiliate> Update(Affiliate affiliate);

        Task<Affiliate> DeleteAsync(Affiliate affiliate);

        #endregion

        #region Reads

        Task<Affiliate> Get(int id);

        Task<List<Affiliate>> GetAll();

        #endregion
    }
}
