namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferCategory
    {
        public int OfferId { get; set; }
        public int CategoryId { get; set; }

        public Offer Offer { get; set; }

        public Category Category { get; set; }
    }
}
