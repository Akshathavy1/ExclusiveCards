using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
   public interface IAffiliateFileMappingService
    {
        #region Writes

        Task<AffiliateFileMapping> Add(AffiliateFileMapping affiliateFileMapping);

        Task<AffiliateFileMapping> Update(AffiliateFileMapping affiliateFileMapping);

        #endregion

        #region Reads

        //Task<AffiliateFileMapping> Get(int id);

        #endregion

    }
}
