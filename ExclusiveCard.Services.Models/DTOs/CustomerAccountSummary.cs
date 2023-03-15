using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class CustomerAccountSummary
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string NiNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool IsDiamondCustomer { get; set; }

        public int MembershipCardId { get; set; }

        public string CardNumber { get; set; }

        public string CardStatus { get; set; }

        public string PlanType { get; set; }

        public string PlanName { get; set; }

        public int MembershipPlanId { get; set;  }
        
        public DateTime CardExpiryDate { get; set; }
        
        public int CardProviderId { get; set; }

        public int RewardPartnerId { get; set; }

        public string RewardKey { get; set; }

        public string RewardPassword { get; set; }

        public int? SiteClanId { get; set; }

        public CustomerBalances Balances { get; set; }

    }
}
