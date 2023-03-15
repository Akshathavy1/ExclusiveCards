using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
   [Table("AffiliateMapping", Schema ="Exclusive")]
   public class AffiliateMapping
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AffiliateMappingRule")]
        public int AffiliateMappingRuleId { get; set; }

        [MaxLength(int.MaxValue)]
        [DataType("nvarchar")]
        public string AffilateValue { get; set; }

        public string ExclusiveValue { get; set; }

    }
}
