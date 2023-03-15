using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs.Public;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IWebsiteSocialMediaService
    {
        #region Reads
        //Get all social media links available for a country
        Task<List<dto.WebsiteSocialMediaLink>> GetAllAsync();

        Task<List<dto.WebsiteSocialMediaLink>> GetSocialMediaLinks(int id);
        #endregion
    }
}
