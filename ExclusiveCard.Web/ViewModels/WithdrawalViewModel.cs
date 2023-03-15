
using System;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class WithdrawalViewModel
    {
        public int Id { get; set; }
        public int PartnerRewardId { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public decimal ConfirmedAmount { get; set; }
        public string Status { get; set; }
        public DateTime? PayOutDate { get; set; }
    }
}
