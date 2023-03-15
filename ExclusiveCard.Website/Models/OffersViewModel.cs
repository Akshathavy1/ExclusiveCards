using System;
using System.Collections.Generic;
using ExclusiveCard.Services.Models.DTOs.Public;

namespace ExclusiveCard.Website.Models
{
    public class OffersViewModel
    {
        public List<Services.Models.DTOs.OfferList> OfferLists { get; set; }
        public List<Services.Models.DTOs.Localisation> LocalisedData { get; set; }
        public List<OfferSummary> CashBackOfferSummaries { get; set; }
        public List<OfferSummary> VoucherCodeOfferSummaries { get; set; }
        public List<OfferSummary> SalesOfferSummaries { get; set; }
        public List<Tuple<OfferSummary, OfferSummary>> HighStreetOfferSummaries { get; set; }
        public List<OfferSummary> DailyDealsOfferSummaries { get; set; }
        public List<OfferSummary> EndingSoonOfferSummaries { get; set; }
        public List<Tuple<OfferSummary, OfferSummary>> RestaurantOfferSummaries { get; set; }
        

        public bool ShowCashbackPanel { get; set; }
        public bool ShowVoucherPanel { get; set; }
        public bool ShowSalesPanel { get; set; }
        public bool ShowHighStreetPanel { get; set; }
        public bool ShowRestuarantPanel { get; set; }
        public bool ShowDealsPanel { get; set; }
        public bool ShowEndingSoonPanel { get; set; }

        public OffersViewModel()
        {
            OfferLists = new List<Services.Models.DTOs.OfferList>();
            CashBackOfferSummaries = new List<OfferSummary>();
            VoucherCodeOfferSummaries = new List<OfferSummary>();
            SalesOfferSummaries = new List<OfferSummary>();
            HighStreetOfferSummaries = new List<Tuple<OfferSummary, OfferSummary>>();
            RestaurantOfferSummaries = new List<Tuple<OfferSummary, OfferSummary>>();
            DailyDealsOfferSummaries = new List<OfferSummary>();
            EndingSoonOfferSummaries = new List<OfferSummary>();
            LocalisedData = new List<Services.Models.DTOs.Localisation>();
            
        }
    }
}
