using System;

namespace ExclusiveCard.Services.Models.DTOs.Public
{
    public class TransactionSummary
    {
        public DateTime? TransactionDate { get; set; }
        public string AffiliateTransactionReference { get; set; }
        public string Status { get; set; }
        public string Merchant { get; set; }
        public string Description { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal CashbackAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal AmountPaidOut { get; set; }
        public string DonationDescription { get; set; } = "Donation";
        public decimal Donation { get; set; }
    }
}
