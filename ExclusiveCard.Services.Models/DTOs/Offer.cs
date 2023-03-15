using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Offer
    {
        public int Id { get; set; }

        public int MerchantId { get; set; }

        public int? AffiliateId { get; set; }

        public int OfferTypeId { get; set; }

        public int StatusId { get; set; }

        public int? SSOConfigId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool Validindefinately { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string Instructions { get; set; }

        public string Terms { get; set; }

        public string Exclusions { get; set; }

        public string LinkUrl { get; set; }

        public string OfferCode { get; set; }

        public bool Reoccuring { get; set; }

        public short SearchRanking { get; set; }

        public DateTime Datecreated { get; set; }

        public string Headline { get; set; }

        public string AffiliateReference { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string RedemptionAccountNumber { get; set; }

        public string RedemptionProductCode { get; set; }

        public string ProductCode { get; set; }

        public Merchant Merchant { get; set; }

        public OfferType OfferType { get; set; }

        public Status Status { get; set; }

        public Affiliate Affiliate { get; set; }

        public SSOConfiguration SSOConfiguration { get; set; }

        public ICollection<OfferMerchantBranch> OfferMerchantBranches { get; set; }

        public ICollection<OfferCountry> OfferCountries { get; set; }

        public ICollection<OfferCategory> OfferCategories { get; set; }

        public ICollection<OfferTag> OfferTags { get; set; }

        public ICollection<OfferListItem> OfferListItems { get; set; }

        public ICollection<ClickTracking> ClickTrackings { get; set; }

        public ICollection<OfferRedemption> OfferRedemptions { get; set; }

        public int? DisplayType { get; set; }
    }
}