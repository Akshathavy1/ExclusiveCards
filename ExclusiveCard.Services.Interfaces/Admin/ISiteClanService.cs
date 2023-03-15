using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface ISiteClanService
    {
        Task<List<Models.DTOs.League>> GetAllLeagues();
        Task<List<Models.DTOs.SiteCategory>> GetAllSiteCategory();
        Task<List<Models.DTOs.Charity>> GetAllCharity();
        Task<PagedResult<Models.DTOs.SiteClan>> SearchClub(int leagueId, int siteCategoryId, int page, int pageSize);
        Task<Models.DTOs.SiteClan> UpdateSiteClan(SiteClan clan);
        Task<Models.DTOs.SiteClan> GetSiteClanById(int id);
        Task<List<Models.DTOs.League>> GetLeagues(int whitelabelId);
    }
}
