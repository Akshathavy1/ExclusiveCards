using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
   public  class Affiliate
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AffiliateFile> AffiliateFiles { get; set; }

       //public ICollection<AffiliateMapping> AffiliateMappings { get; set; }

    }
}
