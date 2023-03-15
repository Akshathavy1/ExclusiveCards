using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class SiteClanManager : ISiteClanManager
    {
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public SiteClanManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<List<League>> GetAllLeague()
        {
            return await _ctx.League.OrderBy(x => x.Description).ToListAsync();
        }

        public async Task<List<SiteCategory>> GetAllSiteCategory()
        {
            return await _ctx.SiteCategory.ToListAsync();
        }

        public async Task<List<Charity>> GetAllCharity()
        {
            return await _ctx.Charity.OrderBy(x=>x.CharityName).ToListAsync();
        }

        public async Task<SiteClan> GetSiteClanById(int id)
        {
            return await _ctx.SiteClan.Include(y=>y.Charity).Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<PagedResult<SiteClan>> SearchClub(int leagueId, int siteCategoryId,int page,int pageSize)
        {
            IQueryable<SiteClan> query = _ctx.SiteClan;
            query= query.Where(x=>x.LeagueId== leagueId && x.SiteCategoryId==siteCategoryId);
            return await query.AsNoTracking().GetPaged(page, pageSize);
        }

        public async Task<SiteClan> UpdateSiteClan(SiteClan clan)
        {
            try
            {
                DbSet<SiteClan> siteClan = _ctx.Set<SiteClan>();
                siteClan.Update(clan);
                await _ctx.SaveChangesAsync();
                return clan;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                Console.WriteLine(e);
                throw;
            }

        }


        public async Task<List<League>> GetLeagues(int whitelabelId)
        {

            try
            {
                //List<League> leagues = null;
                //leagues = await _ctx.League.Include(x => x.SiteClan).ToListAsync();
                //return leagues;

                List<League> leag = null;
                IQueryable<League> leagues = _ctx.League.Include(x => x.SiteClan).ThenInclude(x=>x.SiteOwner);
                if (whitelabelId > 0)
                {
                    leagues = leagues.Where(x => x.SiteClan.All(i => i.WhiteLabelId == whitelabelId));
                }
                leag = await leagues.ToListAsync();
                return leag;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
