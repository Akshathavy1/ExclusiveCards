using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("MembershipPlanBenefits", Schema = "Exclusive")]
    public class MembershipPlanBenefits
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MembershipPlan")]
        public int MembershipPlanId { get; set; }

        public int DisplayOrder { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        public virtual MembershipPlan MembershipPlan { get; set; }
    }
}
