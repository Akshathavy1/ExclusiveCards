namespace ExclusiveCard.Services.Models.DTOs.Public
{
    public class OfferSummary
    {
        public int OfferTypeId { get; set; }

        public int OfferId { get; set; }

        public string OfferHeadline { get; set; }

        public string OfferShortDescription { get; set; }

        public string OfferLongDescription { get; set; }

        public string OfferTerms { get; set; }

        public string OfferInstructions { get; set; }

        public string OfferExclusions { get; set; }

        public string OfferCode { get; set; }

        public int MerchantId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantLogoPath { get; set; }

        public bool DeepLinkAvailable { get; set; }

        public string OfferTypeDescription { get; set; }

        public int? TimePending { get; set; }

        public short Rank { get; set; }

        public string RedemptionAccountNumber { get; set; }

        public string RedemptionProductCode { get; set; }

        public bool IsSSOConfigured { get; set; }
    }
}