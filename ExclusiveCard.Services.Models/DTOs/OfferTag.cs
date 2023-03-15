namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferTag
    {
        public int OfferId { get; set; }
        public int TagId { get; set; }

        public Offer Offer { get; set; }

        public Tag Tag { get; set; }
    }
}
