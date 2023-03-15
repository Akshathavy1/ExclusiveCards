using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class PartnerRewardWithdrawal
    {
        public int Id { get; set; }
        public int? PartnerRewardId { get; set; }
        public int StatusId { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal? ConfirmedAmount { get; set; }
        public int? FileId { get; set; }
        public int BankDetailId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime ChangedDate { get; set; }
        public string UpdatedBy { get; set; }

        public PartnerRewards PartnerReward { get; set; }
        public Status WithdrawalStatus { get; set; }
        public Files File { get; set; }
        public BankDetail BankDetail { get; set; }

        public Customer Customer { get; set; }

    }
}
