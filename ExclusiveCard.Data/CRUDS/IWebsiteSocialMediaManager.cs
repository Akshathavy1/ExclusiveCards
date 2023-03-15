using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IWebsiteSocialMediaManager
    {
        Task<List<WebsiteSocialMediaLink>> GetAllAsync();
        Task<List<WebsiteSocialMediaLink>> GetSocialMediaLinks(int id);

    }
}