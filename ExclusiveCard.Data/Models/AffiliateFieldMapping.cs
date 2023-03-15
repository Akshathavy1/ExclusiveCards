using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("AffiliateFieldMapping", Schema = "Exclusive")]
    public class AffiliateFieldMapping
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("AffiliateFile")]
        //public int AffiliateFileId { get; set; }

        [ForeignKey("AffiliateFileMapping")]
        public int AffiliateFileMappingId { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string AffiliateFieldName { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string ExclusiveTable { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string ExclusiveFieldName { get; set; }

        public bool IsList { get; set; }

        [MaxLength(8)]
        [DataType("nvarchar")]
        public string Delimiter { get; set; }


        [ForeignKey("AffiliateMappingRule")]
        public int? AffiliateMappingRuleId { get; set; }

        public int AffiliateTransformId { get; set; }

        public int AffiliateMatchTypeId { get; set; }

        //public virtual AffiliateFile AffiliateFile  { get; set;}
        public virtual AffiliateMappingRule AffiliateMappingRule { get; set; }

        public virtual AffiliateFileMapping AffiliateFileMapping { get; set; }
    }
}
