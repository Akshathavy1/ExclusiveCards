using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class PartnerRewards
    {
        public int Id { get; set; }
        public string RewardKey { get; set; }
        public int? PartnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal LatestValue { get; set; }
        public DateTime? ValueDate { get; set; }
        public decimal TotalConfirmedWithdrawn { get; set; }
        public string Password { get; set; }

        public PartnerDto Partner { get; set; }

        public ICollection<MembershipCard> MembershipCards { get; set; }
        public ICollection<PartnerRewardWithdrawal> PartnerRewardWithdrawals { get; set; }
    }
}
