using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class PaymentSubscription
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string SubscriptionId { get; set; }

        public int StatusId { get; set; }

        public DateTime NextPaymentDate { get; set; }

        public decimal NextPaymentAmount { get; set; }

        public string PaymentType { get; set; }

        public int MembershipPlanId { get; set; }
    }
}
