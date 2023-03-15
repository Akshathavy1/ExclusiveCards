using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("MembershipPlanType", Schema = "Exclusive")]
    public class MembershipPlanType
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("TermsConditions")]
        public int? TermsConditionsId { get; set; }
        public virtual TermsConditions TermsConditions { get; set; }
        public virtual ICollection<MembershipPlan> MembershipPlans { get; set; }
    }
}
