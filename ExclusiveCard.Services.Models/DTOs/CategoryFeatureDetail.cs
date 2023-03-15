namespace ExclusiveCard.Services.Models.DTOs
{
    public class CategoryFeatureDetail
    {
        public int CategoryId { get; set; }
        public string CountryCode { get; set; }
        public int FeatureMerchantId { get; set; }
        public string SelectedImage { get; set; }
        public string UnselectedImage { get; set; }

        public Category Category { get; set; }
        public Merchant Merchant { get; set; }
    }
}
