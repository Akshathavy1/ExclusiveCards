using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace ExclusiveCard.Data.Models
{
    [Table("AffiliateMappingRule", Schema ="Exclusive")]
   public class AffiliateMappingRule
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        [ForeignKey("Affiliate")]

        public int AffiliateId { get; set; }

        public bool IsActive { get; set; }

        public virtual Affiliate Affiliate { get; set; }

        public virtual ICollection<AffiliateMapping> AffiliateMappings { get; set; }

        [Obsolete("No navigation property required back to FieldMappings")]
        public virtual ICollection<AffiliateFieldMapping> AffiliateFieldMappings { get; set; }
    }
}
