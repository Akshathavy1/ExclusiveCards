using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
   public interface IAffiliateFileService
    {
        #region Writes

        Task<AffiliateFile> Add(AffiliateFile affiliateFile);

        Task<AffiliateFile> Update(AffiliateFile affiliateFile);

        #endregion

        #region Reads

        Task<AffiliateFile> Get(int id);

        Task<List<AffiliateFile>> GetByAffiliateAsync(int id);

        Task<List<AffiliateFile>> GetAll();

        #endregion
    }
}
