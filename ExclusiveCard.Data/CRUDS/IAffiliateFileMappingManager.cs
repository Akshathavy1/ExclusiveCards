using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IAffiliateFileMappingManager
    {
        Task<AffiliateFileMapping> Add(AffiliateFileMapping affiliateFileMapping);
        Task<AffiliateFileMapping> Update(AffiliateFileMapping affiliateFileMapping);
        //Task<AffiliateFieldMapping> Get(int id);
    }
}