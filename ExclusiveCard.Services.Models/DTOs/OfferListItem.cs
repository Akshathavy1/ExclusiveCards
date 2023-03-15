using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferListItem
    {
        public int OfferListId { get; set; }

        public int OfferId { get; set; }

        public string ExcludedCountries { get; set; }

        public string CountryCode { get; set; }

        public short DisplayOrder { get; set; }

        public DateTime? DisplayFrom { get; set; }

        public DateTime? DisplayTo { get; set; }

        public OfferList OfferList { get; set; }

        public Offer Offer { get; set; }
    }
}
