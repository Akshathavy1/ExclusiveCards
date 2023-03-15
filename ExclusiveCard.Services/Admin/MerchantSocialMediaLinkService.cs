using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class MerchantSocialMediaLinkService : IMerchantSocialMediaLinkService
    {
        #region Private members

        private readonly IMerchantSocialMediaLinkManager _manager;

        #endregion

        #region Constuctor

        public MerchantSocialMediaLinkService(IMerchantSocialMediaLinkManager manager)
        {
            _manager = manager;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.MerchantSocialMediaLink> Add(Models.DTOs.MerchantSocialMediaLink link)
        {
            MerchantSocialMediaLink req = MapToData(link);
            return MapToDto(
                await _manager.Add(req));
        }

        public async Task<List<Models.DTOs.MerchantSocialMediaLink>> AddListAsync (List<Models.DTOs.MerchantSocialMediaLink> links)
        {
            List<MerchantSocialMediaLink> req = MapToDataList(links);
            return MapToDtoList(await _manager.AddListAsync(req));
        }

        public async Task<Models.DTOs.MerchantSocialMediaLink> Update(Models.DTOs.MerchantSocialMediaLink link)
        {
            MerchantSocialMediaLink req = MapToData(link);
            return MapToDto(
                await _manager.Update(req));
        }

        #endregion

        #region Reads

        public Models.DTOs.MerchantSocialMediaLink Get(int merchantId, int linkId)
        {
            return ManualMappings.MapMerchantSocialMediaLink(_manager.Get(merchantId, linkId));
        }

        public async Task<List<Models.DTOs.MerchantSocialMediaLink>> GetAll(int merchantId)
        {
            return ManualMappings.MapMerchantSocialMediaLinks(await _manager.GetAll(merchantId));
        }

        #endregion

        #region Private Members

        private List<MerchantSocialMediaLink> MapToDataList(List<Models.DTOs.MerchantSocialMediaLink> merchantSocialMediaLinks)
        {
            List<MerchantSocialMediaLink> merchantSocialLinks = new List<MerchantSocialMediaLink>();
            foreach(Models.DTOs.MerchantSocialMediaLink merchantSocialMediaLink in merchantSocialMediaLinks)
            {
                merchantSocialLinks.Add(MapToData(merchantSocialMediaLink));
            }
            return merchantSocialLinks;
        }

        private MerchantSocialMediaLink MapToData(Models.DTOs.MerchantSocialMediaLink merchantSocialMediaLink)
        {
            if (merchantSocialMediaLink == null)
                return null;

            MerchantSocialMediaLink mediaLink = new MerchantSocialMediaLink
            {
                MerchantId = merchantSocialMediaLink.MerchantId,
                SocialMediaCompanyId = merchantSocialMediaLink.SocialMediaCompanyId,
                SocialMediaURI = merchantSocialMediaLink.SocialMediaURI
            };
            if (merchantSocialMediaLink.Id > 0)
            {
                mediaLink.Id = merchantSocialMediaLink.Id;
            }

            return mediaLink;
        }

        public List<Models.DTOs.MerchantSocialMediaLink> MapToDtoList(List<MerchantSocialMediaLink> merchantSocialMediaLinks)
        {
            List<Models.DTOs.MerchantSocialMediaLink> merchantSocialLinks = new List<Models.DTOs.MerchantSocialMediaLink>();
            foreach(MerchantSocialMediaLink merchantSocialMediaLink in merchantSocialMediaLinks)
            {
                merchantSocialLinks.Add(MapToDto(merchantSocialMediaLink));
            }
            return merchantSocialLinks;
        }

        private Models.DTOs.MerchantSocialMediaLink MapToDto(MerchantSocialMediaLink merchantSocialMediaLink)
        {
            if (merchantSocialMediaLink == null)
                return null;

            Models.DTOs.MerchantSocialMediaLink mediaLink = new Models.DTOs.MerchantSocialMediaLink
            {
                MerchantId = merchantSocialMediaLink.MerchantId,
                SocialMediaCompanyId = merchantSocialMediaLink.SocialMediaCompanyId,
                SocialMediaURI = merchantSocialMediaLink.SocialMediaURI
            };
            if (merchantSocialMediaLink.Id > 0)
            {
                mediaLink.Id = merchantSocialMediaLink.Id;
            }

            return mediaLink;
        }

        #endregion
    }
}
