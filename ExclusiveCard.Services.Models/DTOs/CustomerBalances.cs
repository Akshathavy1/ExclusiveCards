using System;

namespace ExclusiveCard.Services.Models.DTOs
{ 
    public class CustomerBalances
    {
        public decimal PendingAmount { get; set; }
        public decimal ConfirmedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal Invested { get; set; }
        public decimal Withdrawn { get; set; }
        public decimal CurrentValue { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public decimal DonatedAmount { get; set; }

    }
}
