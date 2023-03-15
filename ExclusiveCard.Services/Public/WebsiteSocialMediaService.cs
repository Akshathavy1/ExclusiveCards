using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs.Public;
using dto = ExclusiveCard.Services.Models.DTOs.Public;

namespace ExclusiveCard.Services.Public
{
    public class WebsiteSocialMediaService : IWebsiteSocialMediaService
    {
        #region Private Members

        private readonly IWebsiteSocialMediaManager _manager;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WebsiteSocialMediaService(IMapper mapper, IWebsiteSocialMediaManager websiteSocialMediaManager)
        {
            _mapper = mapper;
            _manager = websiteSocialMediaManager;
        }

        #endregion
        #region Reads
        public async Task<List<dto.WebsiteSocialMediaLink>> GetAllAsync()
        {
            return _mapper.Map<List<dto.WebsiteSocialMediaLink>>(await _manager.GetAllAsync());
        }

        public async Task<List<WebsiteSocialMediaLink>> GetSocialMediaLinks(int id)
        {
            return _mapper.Map<List<dto.WebsiteSocialMediaLink>>(await _manager.GetSocialMediaLinks(id));
        }
        #endregion
    }
}
