namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferCountry
    {
        public int OfferId { get; set; }
        
        public string CountryCode { get; set; }

        public bool IsActive { get; set; }

        public Offer Offer { get; set; }
    }
}
