using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMerchantSocialMediaLinkManager
    {
        Task<MerchantSocialMediaLink> Add(MerchantSocialMediaLink medialLink);
        Task<List<MerchantSocialMediaLink>> AddListAsync(List<MerchantSocialMediaLink> merchantSocialMediaLinks);
        Task<MerchantSocialMediaLink> Update(MerchantSocialMediaLink medialLink);
        Task<List<MerchantSocialMediaLink>> GetAll(int merchantId);
        MerchantSocialMediaLink Get(int merchantId, int socialMediaId);
    }
}