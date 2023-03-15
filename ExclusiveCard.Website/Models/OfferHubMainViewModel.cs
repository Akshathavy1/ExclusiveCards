using ExclusiveCard.Data.Models;
using System.Collections.Generic;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.Models
{
    public class OfferHubMainViewModel
    {
        public List<OfferHubViewModel> OfferHubs { get; set; }
        public List<OfferHubViewModel> BestCashbackOffers { get; set; }
        public List<OfferHubViewModel> HomepageOffers { get; set; }
        public List<dto.Category> Categories { get; set; }
        public List<OfferMerchantBranch> OfferMerchantBranches { get; set; }

        public OfferHubMainViewModel()
        {
            OfferHubs = new List<OfferHubViewModel>();
            BestCashbackOffers = new List<OfferHubViewModel>();
            HomepageOffers = new List<OfferHubViewModel>();
            Categories = new List<dto.Category>();
        }
    }
}
