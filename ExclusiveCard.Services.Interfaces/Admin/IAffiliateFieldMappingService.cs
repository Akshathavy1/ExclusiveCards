using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IAffiliateFieldMappingService
    {
        #region Writes

        Task<AffiliateFieldMapping> Add(AffiliateFieldMapping affiliateFieldMapping);

        Task<AffiliateFieldMapping> Update(AffiliateFieldMapping affiliateFieldMapping);

        #endregion

        #region Reads

        Task<AffiliateFieldMapping> Get(int id);

        Task<List<AffiliateFieldMapping>> GetAll(int affiliateFileMappingid);

        #endregion
    }
}
