using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("AffiliateFileMapping")]
   public class AffiliateFileMapping
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Affiliate")]
        public int AffiliateId { get; set; }

        [MaxLength(int.MaxValue)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        public virtual Affiliate Affiliate { get; set; }

        public virtual ICollection<AffiliateFieldMapping> AffiliateFieldMappings { get; set; }

        public virtual ICollection<AffiliateFile> AffiliateFiles { get; set; }
    }
}
