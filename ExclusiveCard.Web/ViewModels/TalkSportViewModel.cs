using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class TalkSportViewModel
    {
        public List<SelectListItem> Leagues { get; set; }
        public List<SelectListItem> SiteCategories { get; set; }
        public List<SelectListItem> Charity { get; set; }
        public PagedResult<SiteClan> SiteClan { get; set; }
        public int? Page { get; set; }
        public int LeagueId { get; set; }
        public int SiteCategoryId { get; set; }
        public int CharityId { get; set; }

        public TalkSportViewModel()
        {
            SiteClan = new PagedResult<SiteClan>();
            Paging = new PagingViewModel();
        }
        public PagingViewModel Paging { get; set; }
        public int? CurrentPageNumber { get; set; }

       
    }
}
