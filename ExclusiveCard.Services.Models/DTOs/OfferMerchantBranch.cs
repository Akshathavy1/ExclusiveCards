namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferMerchantBranch
    {
        public int OfferId { get; set; }

        public int MerchantBranchId { get; set; }

        public Offer Offer { get; set; }

        public MerchantBranch MerchantBranch { get; set; }
    }
}
