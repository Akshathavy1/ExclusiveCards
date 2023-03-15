using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
   public class OfferSearchCriteria
    {
        public string OfferListName { get; set; }
        public int? MerchantId { get; set; }
        public int? AffiliateId { get; set; }
        public int? OfferType { get; set; }
        public int? OfferStatus { get; set; }
        public string KeyWord { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CountryCode { get; set; }
        public List<int> Categories { get; set; }
        public string MerchantName { get; set; }
        public List<int> OfferTypes { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int SortOrder { get; set; }
        public int? OfferId { get; set; }
        public bool FirstOnly { get; set; }
        public int? ExcludedMerchantId { get; set; }

        public OfferSearchCriteria()
        {
            Categories = new List<int>();
            OfferTypes = new List<int>();
            FirstOnly = false;
        }
    }
}
