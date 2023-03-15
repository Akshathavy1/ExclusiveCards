namespace ExclusiveCard.Services.Models.DTOs
{
    public class MerchantSocialMediaLink
    {
        public int Id { get; set; }

        public int MerchantId { get; set; }

        public int SocialMediaCompanyId { get; set; }

        public string SocialMediaURI { get; set; }

        public Merchant Merchant { get; set; }

        public SocialMediaCompany SocialMediaCompany { get; set; }
    }
}
