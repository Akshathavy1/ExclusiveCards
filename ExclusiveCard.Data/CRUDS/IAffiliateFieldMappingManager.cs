using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IAffiliateFieldMappingManager
    {
        Task<AffiliateFieldMapping> Add(AffiliateFieldMapping affiliateFieldMapping);
        Task<AffiliateFieldMapping> Update(AffiliateFieldMapping affiliateFieldMapping);
        Task<AffiliateFieldMapping> Get(int id);
        Task<List<AffiliateFieldMapping>> GetAll(int affiliateFileMappingid);
    }
}