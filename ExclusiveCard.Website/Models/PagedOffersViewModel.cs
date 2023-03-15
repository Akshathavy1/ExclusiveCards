using ExclusiveCard.Services.Models.DTOs.Public;
using System.Collections.Generic;

namespace ExclusiveCard.Website.Models
{
    public class PagedOffersViewModel
    {
        public List<OfferSummary> OfferSummaries { get; set; }

        public PagingViewModel PagingView { get; set; }

        public int? CurrentPageNumber { get; set; }
        public PagedOffersViewModel()
        {
            OfferSummaries = new List<OfferSummary>();
            PagingView = new PagingViewModel();
        }
    }
}
