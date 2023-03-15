using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
   public class AffiliateMappingRule
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int AffiliateId { get; set; }

        public bool IsActive { get; set; }

        public ICollection<AffiliateMapping> AffiliateMappings { get; set; }

    }
}
