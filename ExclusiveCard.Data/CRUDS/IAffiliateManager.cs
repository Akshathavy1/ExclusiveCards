using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IAffiliateManager
    {
        Task<Affiliate> Add(Affiliate affiliate);
        Task<Affiliate> Update(Affiliate affiliate);
        Task<Affiliate> DeleteAsync(Affiliate affiliate);
        Task<Affiliate> Get(int id);
        Task<List<Affiliate>> GetAll();
    }
}