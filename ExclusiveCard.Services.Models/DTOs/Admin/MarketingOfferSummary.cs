using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs.Admin
{
    public class MarketingOfferSummary
    {
        public string MerchantName { get; set; }
        public string ImagePath { get; set; }
        public string Heading { get; set; }
        public string OfferShortDescription { get; set; }
        public string OfferLongDescription { get; set; }
        public int OfferId { get; set; }
        public int MerchantId { get; set; }
        public string CountryCode { get; set; }
    }
}
