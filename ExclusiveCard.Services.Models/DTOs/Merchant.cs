using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Merchant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ContactDetailsId { get; set; }

        public string ContactName { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string Terms { get; set; }

        public string WebsiteUrl { get; set; }

        public bool IsDeleted { get; set; }
        public bool FeatureImageOfferText { get; set; }
        public string BrandColour { get; set; }

        public ContactDetail ContactDetail { get; set; }

        public ICollection<MerchantSocialMediaLink> MerchantSocialMediaLinks { get; set; }

        public ICollection<MerchantBranch> MerchantBranches { get; set; }

        public ICollection<MerchantImage> MerchantImages { get; set; }

        public ICollection<Offer> Offers { get; set; }

        public ICollection<CashbackTransaction> CashbackTransactions { get; set; }
        public ICollection<CategoryFeatureDetail> CategoryFeatureDetails { get; set; }
    }
}
