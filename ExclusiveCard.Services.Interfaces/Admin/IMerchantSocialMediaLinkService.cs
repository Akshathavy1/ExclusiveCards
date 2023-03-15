using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IMerchantSocialMediaLinkService
    {
        #region Write
        
        Task<MerchantSocialMediaLink> Add(MerchantSocialMediaLink link);

        Task<List<MerchantSocialMediaLink>> AddListAsync(List<MerchantSocialMediaLink> links);

        Task<MerchantSocialMediaLink> Update(MerchantSocialMediaLink link);

        #endregion

        #region Reads

        MerchantSocialMediaLink Get(int merchantId, int linkId);

        Task<List<MerchantSocialMediaLink>> GetAll(int merchantId);

        #endregion
    }
}