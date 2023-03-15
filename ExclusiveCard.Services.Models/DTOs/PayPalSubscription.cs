using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class PayPalSubscription
    {
        public int Id { get; set; }
        
        public int CustomerId { get; set; }
        
        public string PayPalId { get; set; }
        
        public int PayPalStatusId { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        public decimal NextPaymentAmount { get; set; }
        
        public string PaymentType { get; set; }

        public int? MembershipPlanId { get; set; }

        public Customer Customer { get; set; }

        public Status PayPalStatus { get; set; }

        public MembershipPlan MembershipPlan { get; set; }
    }
}
