
namespace ExclusiveCard.Services.Models.DTOs.Admin
{
   public class OfferSummary
    {
        public int OfferId { get; set; }
        public string MerchantName { get; set; }
        public string OfferShortDescription { get; set; }
        public string OfferLongDescription { get; set; }
        public short SearchRanking { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public short DisplayOrder {get; set;}
    }
}
