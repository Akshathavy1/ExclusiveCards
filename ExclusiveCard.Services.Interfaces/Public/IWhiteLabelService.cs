using ExclusiveCard.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IWhiteLabelService
    {
        dto.WhiteLabelSettings GetSiteSettings(string url);
        string WhiteLabelString(string url, string sourceString);
        //IList<dto.WhiteLabelSettings> GetAllSiteSettings(int newsLetterId);
        dto.WhiteLabelSettings GetSiteSettingsById(int id);
        IList<dto.WhiteLabelSettings> GetAll();
        /// <summary>
        /// Finds Regional White label sites
        /// </summary>
        /// <returns>Regional white labels</returns>
        IList<dto.WhiteLabelSettings> GetRegionSites();
        Task<bool> Update(dto.WhiteLabelSettings settings);
        Task<List<dto.SponsorImages>> GetSponsorImagesById(int id);
    }
}
