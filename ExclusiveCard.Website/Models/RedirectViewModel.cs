namespace ExclusiveCard.Website.Models
{
    public class RedirectViewModel
    {
        public int OfferId { get; set; }
        public int MerchantId { get; set; }
        public int MembershipCardId { get; set; }
        public int AffiliateId { get; set; }
        public string MerchantLogoPath { get; set; }
        public string MerchantName { get; set; }
        public string DeepLinkUrl { get; set; }
        public string MembershipCardReference { get; set; }
        public bool RedirectAllowed { get; set; }
        public string MembershipCardNumber { get; set; }
        public string Token { get; set; }
    }
}
