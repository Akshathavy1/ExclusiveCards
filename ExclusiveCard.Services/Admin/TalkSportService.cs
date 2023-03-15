using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using NLog;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using System;

namespace ExclusiveCard.Services.Admin
{
    public class TalkSportService : ITalkSportService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ITalkSportManager _talkSportManager;
       
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public TalkSportService(IMapper mapper, ITalkSportManager talkSportManager)
        {
            _mapper = mapper;
            _talkSportManager = talkSportManager;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion
        public async Task<List<Models.DTOs.League>> GetAllLeagues()
        {
            var leagues = await _talkSportManager.GetAllLeague();
            return  _mapper.Map<List<Models.DTOs.League>>(leagues);
        }

        public async Task<List<Models.DTOs.SiteCategory>> GetAllSiteCategory()
        {
            var siteCategory = await _talkSportManager.GetAllSiteCategory();
            return _mapper.Map<List<Models.DTOs.SiteCategory>>(siteCategory);
        }

        public async Task<List<Models.DTOs.Charity>> GetAllCharity()
        {
            var siteCharity = await _talkSportManager.GetAllCharity();
            return _mapper.Map<List<Models.DTOs.Charity>>(siteCharity);
        }

        public async Task<PagedResult<SiteClan>> SearchClub(int leagueId, int siteCategoryId, int page, int pageSize)
        {
            var siteClan = await _talkSportManager.SearchClub(leagueId, siteCategoryId, page,  pageSize);
            return _mapper.Map<PagedResult<Models.DTOs.SiteClan>>(siteClan);
        }

        public async Task<Models.DTOs.SiteClan> UpdateSiteClan(SiteClan clan)
        {
            var res= _mapper.Map<Data.Models.SiteClan>(clan);
            var siteClan = await _talkSportManager.UpdateSiteClan(res);
            return _mapper.Map<Models.DTOs.SiteClan>(siteClan);
        }

        public async Task<Models.DTOs.SiteClan> GetSiteClanById(int id)
        {
            var siteClan = await _talkSportManager.GetSiteClanById(id);
            return _mapper.Map<Models.DTOs.SiteClan>(siteClan);
        }

        public async Task<List<Models.DTOs.League>> GetLeagues(int whitelabelId)
        {
            return MapToLeagues(await _talkSportManager.GetLeagues(whitelabelId));
        }


        private List<League> MapToLeagues(List<Data.Models.League> leagues)
        {
            try
            {

                List<League> list = new List<League>();
                foreach (var item in leagues)
                {
                    League league = new League();
                    List<SiteClan> siteClanList = new List<SiteClan>();

                    league.Description = item.Description;


                    if (item.SiteClan.Count > 0)
                    {
                        foreach (var clan in item.SiteClan)
                        {
                            SiteClan siteClan = new SiteClan();
                            siteClan.Description = clan.Description;
                            siteClan.PrimaryColour = clan.PrimaryColour;
                            siteClan.SecondaryColour = clan.SecondaryColour;
                            siteClan.Id = clan.Id;
                            siteClanList.Add(siteClan);
                        }
                        league.SiteClan = siteClanList;
                    }

                    list.Add(league);
                }


                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
