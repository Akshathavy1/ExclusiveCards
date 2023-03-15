using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{

  [Obsolete("Should not be required, duplicates AffiliateFile table")]
  public  class AffiliateFileMapping
    {
        public int Id { get; set; }

        public int AffiliateId { get; set; }

        public string Description { get; set; }


    }
}
