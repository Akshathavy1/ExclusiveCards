using ExclusiveCard.Enums;
using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class OfferListViewModel
    {
        public List<Services.Models.DTOs.Admin.OfferSummary> ListofOffers { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int CurrentPageNumber { get; set; }
        public OfferSortField SortField { get; set; }
        public string SortDirection { get; set; }
        public string SortIcon { get; set; }
        public OfferListViewModel(OfferListRequest req)
        {
            SortField = req.SortField;
            SortDirection = req.SortDirection;
            SortIcon = req.SortIcon;
            ListofOffers = new List<Services.Models.DTOs.Admin.OfferSummary>();
            PagingModel = new PagingViewModel();
        }
    }
}
