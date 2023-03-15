using ExclusiveCard.Data.Models;
using System.Collections.Generic;
using System.ComponentModel;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.Models
{
    public class MerchantOffersViewModel
    {
        [DisplayName("Terms:")]
        public string Terms { get; set; }
        public dto.Merchant Merchant { get; set; }
        public List<dto.Public.OfferSummary> Cashback { get; set; }
        public List<dto.Public.OfferSummary> Voucher { get; set; }
        public List<dto.Public.OfferSummary> HighStreet { get; set; }
        public List<dto.Public.OfferSummary> HighStreetRestaurant { get; set; }
        public List<dto.Public.OfferSummary> Sales { get; set; }

        // IB: Rename the standard property to FeaturedOffer, as this may now be either a local offer or a standard cashback offer
        public dto.Public.OfferSummary FeaturedOffer { get; set; }
        // IB: Add list for Local Offers
        public List<dto.Public.OfferSummary> LocalOffers { get; set; }

        public List<dto.Public.OfferSummary> DailyDeals { get; set; }
        public List<dto.Public.OfferSummary> EndingSoon { get; set; }
        public List<dto.MerchantBranch> MerchantBranches { get; set; }
        public PagedBranchContactViewModel PagedMerchantBranch { get; set;}
        public List<dto.Public.OfferSummary> RelatedOffers { get; set; }
        public string Logo { get; set; }
        public string FeatureImage { get; set; }

        public MerchantOffersViewModel()
        {
            Merchant = new dto.Merchant();
            Cashback = new List<dto.Public.OfferSummary>();
            Voucher = new List<dto.Public.OfferSummary>();
            HighStreetRestaurant = new List<dto.Public.OfferSummary>();
            HighStreet = new List<dto.Public.OfferSummary>();
            Sales = new List<dto.Public.OfferSummary>();
            FeaturedOffer = new dto.Public.OfferSummary();
            LocalOffers = new List<dto.Public.OfferSummary>();
            DailyDeals = new List<dto.Public.OfferSummary>();
            EndingSoon = new List<dto.Public.OfferSummary>();
            
            MerchantBranches = new List<dto.MerchantBranch>();
            PagedMerchantBranch = new PagedBranchContactViewModel();
            RelatedOffers = new List<dto.Public.OfferSummary>();
        }
    }
}
