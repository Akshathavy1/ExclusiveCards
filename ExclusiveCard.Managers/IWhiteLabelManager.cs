using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface IWhiteLabelManager
    {
        IList<dto.WhiteLabelSettings> GetAll();

        /// <summary>
        /// Get all white labels
        /// </summary>
        /// <returns></returns>
        Task<IList<dto.WhiteLabelSettings>> GetAllAsync();

        /// <summary>
        /// Finds Regional White label sites
        /// </summary>
        /// <returns>Regional white labels</returns>
        IList<dto.WhiteLabelSettings> GetRegionSites();

        dto.WhiteLabelSettings GetByUrl(string url);

        dto.WhiteLabelSettings GetSiteSettingsById(int id);

        Task<IList<dto.WhiteLabelSettings>> GetWhiteLabelToBeAddedToSendGrid();

        Task<dto.WhiteLabelSettings> Update(dto.WhiteLabelSettings settings);

        Task<List<dto.SponsorImages>> GetSponsorImagesById(int id);
    }
}