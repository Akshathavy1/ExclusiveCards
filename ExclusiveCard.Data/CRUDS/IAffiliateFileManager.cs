using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IAffiliateFileManager
    {
        Task<AffiliateFile> Add(AffiliateFile affiliateFile);
        Task<AffiliateFile> Update(AffiliateFile affiliateFile);
        Task<AffiliateFile> Get(int id);
        Task<List<AffiliateFile>> GetByAffiliateAsync(int id);
        Task<List<AffiliateFile>> GetAll();
    }
}