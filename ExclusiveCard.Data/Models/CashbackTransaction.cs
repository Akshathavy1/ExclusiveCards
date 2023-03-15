using System;
using System.Collections.Generic;

namespace ExclusiveCard.Data.Models
{
    public partial class CashbackTransaction
    {
        public int Id { get; set; }
        public int? AffiliateId { get; set; }
        public int MembershipCardId { get; set; }
        public int? PartnerId { get; set; }
        public char AccountType { get; set; }
        public string AffiliateTransactionReference { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal PurchaseAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ExpectedPaymentDate { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public DateTime? DateReceived { get; set; }
        public int? PartnerCashbackPayoutId { get; set; }
        public decimal CashbackAmount { get; set; }
        public int? MerchantId { get; set; }
        public int? FileId { get; set; }
        public int? PaymentStatusId { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }
       

    }
}
