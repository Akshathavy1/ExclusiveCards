using ExclusiveCard.Services.Models.DTOs;
using System.Collections.Generic;

namespace ExclusiveCard.Website.Models
{
    public class OffersResultViewModel
    {
        public List<Localisation> LocalisedData { get; set; }
        public PagedOffersViewModel StandardOffers { get; set; }
        public PagedOffersViewModel CashbackOffers { get; set; }
        public PagedOffersViewModel VoucherCodeOffers { get; set; }
        public PagedOffersViewModel SalesOffers { get; set; }
        public PagedOffersViewModel HighStreetandRestaurantOffers { get; set; }

        public OffersResultViewModel()
        {
            StandardOffers = new PagedOffersViewModel();
            CashbackOffers = new PagedOffersViewModel();
            VoucherCodeOffers = new PagedOffersViewModel();
            SalesOffers = new PagedOffersViewModel();
            HighStreetandRestaurantOffers = new PagedOffersViewModel();
            LocalisedData = new List<Localisation>();
        }
    }
}
