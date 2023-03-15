using ExclusiveCard.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ITalkSportManager
    {
        Task<List<League>> GetAllLeague();
        Task<List<SiteCategory>> GetAllSiteCategory();
        Task<List<Charity>> GetAllCharity();
        Task<PagedResult<SiteClan>> SearchClub(int leagueId, int siteCategoryId, int page, int pageSize);
        Task<SiteClan> UpdateSiteClan(SiteClan clan);
        Task<SiteClan> GetSiteClanById(int id);
        Task<List<League>> GetLeagues(int whitelabelId);
    }
}
