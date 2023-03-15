using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("MembershipCardAffiliateReference",Schema = "Exclusive")]
    public class MembershipCardAffiliateReference
    {
        [Key, Column(Order = 0), ForeignKey("Affiliate")]
        public int AffiliateId { get; set; }

        [Key, Column(Order = 1), ForeignKey("MembershipCard")]
        public int MembershipCardId { get; set; }

        [DataType("nvarchar")]
        [MaxLength(int.MaxValue)]
        public string CardReference { get; set; }

        public virtual Affiliate Affiliate { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }
    }
}
