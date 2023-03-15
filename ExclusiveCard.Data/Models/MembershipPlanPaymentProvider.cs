using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{

    [Table("MembershipPlanPaymentProvider", Schema = "Exclusive")]
    public  class MembershipPlanPaymentProvider
    {
        [Key, Column(Order = 0), ForeignKey("MembershipPlan")]
        public int MembershipPlanId { get; set; }

        [Key, Column(Order = 1), ForeignKey("PaymentProvider")]
        public int PaymentProviderId { get; set; }

        [DataType("nvarchar")]
        [MaxLength(50)]
        public string SubscribeAppRef { get; set; }
        [DataType("nvarchar")]
        [MaxLength(50)]
        public string SubscribeAppAndCardRef { get; set; }
        [DataType("nvarchar")]
        [MaxLength(50)]
        public string OneOffPaymentRef { get; set; }

        public virtual MembershipPlan MembershipPlan { get; set; }

        public virtual PaymentProvider PaymentProvider { get; set; }
    }
}
