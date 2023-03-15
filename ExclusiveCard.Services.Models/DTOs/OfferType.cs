using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferType
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int? ActionTextLocalisationId { get; set; }

        public int? TitleLocalisationId { get; set; }

        public int SearchRanking { get; set; }

        public ICollection<Offer> Offers { get; set; }
        public Localisation ActionTextLocalisation { get; set; }
        public Localisation TitleLocalisation { get; set; }
        public string ListName { get; set; }
    }
}
