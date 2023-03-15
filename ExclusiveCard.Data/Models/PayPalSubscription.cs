using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("PayPalSubscription", Schema = "Exclusive")]
    public class PayPalSubscription
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [MaxLength(30)]
        [DataType("nvarchar")]
        public string PayPalId { get; set; }
        [ForeignKey("PayPalStatus")]
        public int PayPalStatusId { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public decimal NextPaymentAmount { get; set; }
        [MaxLength(30)]
        [DataType("nvarchar")]
        public string PaymentType { get; set; }
        [ForeignKey("MembershipPlan")]
        public int? MembershipPlanId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Status PayPalStatus { get; set; }
        public virtual MembershipPlan MembershipPlan { get; set; }
    }
}
