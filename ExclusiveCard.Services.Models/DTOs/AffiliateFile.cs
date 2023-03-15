using System.Collections.Generic;
using ExclusiveCard.Services.Models.DTOs.StagingModels;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class AffiliateFile
    {
        public int Id { get; set; }
        
        public int AffiliateId { get; set; }
        
        public string FileName { get; set; }
        
        public string Description { get; set; }
        
        public string StagingTable { get; set; }

        public int AffiliateFileMappingId { get; set; }

        public Affiliate Affiliate { get; set; }

        public AffiliateFileMapping AffiliateFileMapping { get; set; }


    }
}
