using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ExclusiveCard.Data.StagingModels;
using System;

namespace ExclusiveCard.Data.Models
{
    [Table("AffiliateFile",Schema ="Exclusive")]
    public class AffiliateFile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Affiliate")]
        public int AffiliateId { get; set; }

        [MaxLength(250)]
        [DataType("nvarchar")]
        public string FileName { get; set; }

        [MaxLength(500)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string StagingTable { get; set; }

        
        [ForeignKey("AffiliateFileMapping")]
        public int AffiliateFileMappingId { get; set; }


        public virtual Affiliate Affiliate { get; set; }

        [Obsolete("This property is duplicate of AffiliateFile")]
        public virtual AffiliateFileMapping AffiliateFileMapping { get; set; }
        //public virtual ICollection<AffiliateMapping> AffiliateMappings { get; set; }
        //public virtual ICollection<AffiliateFieldMapping> AffiliateFieldMappings { get; set; }

        [Obsolete("this navigation property simply not required")]
        public virtual ICollection<OfferImportFile> OfferImportFiles { get; set; }

       // public virtual ICollection<AffiliateFieldMapping> AffiliateFieldMappings { get; set; }

    }
}
