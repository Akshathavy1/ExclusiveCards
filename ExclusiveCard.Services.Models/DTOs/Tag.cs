using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Tag
    {
        public int Id { get; set; }

        public string Tags { get; set; }

        public string TagType { get; set; }

        public bool IsActive { get; set; }

        public ICollection<OfferTag> OfferTags { get; set; }
    }
}
