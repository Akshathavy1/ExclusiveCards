using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
   public class CashbackTransaction
    {
        public int Id { get; set; }

        public int? AffiliateId { get; set; }

        public int MembershipCardId { get; set; }

        public int? MerchantId { get; set; }

        public int? PartnerId { get; set; }

        public char AccountType { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string AffiliateTransactionReference { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal CashbackAmount { get; set; }

        public decimal PurchaseAmount { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CurrencyCode { get; set; }

        //public bool Credit { get; set; }

        [MaxLength(30)]
        [DataType("nvarchar")]
        public string Summary { get; set; }

        [MaxLength(70)]
        [DataType("nvarchar")]
        public string Detail { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ExpectedPaymentDate { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public DateTime? DateReceived { get; set; }
        //public DateTime? DatePaidOut { get; set; }

        public int? PartnerCashbackPayoutId { get; set; }

        public int? FileId { get; set; }

        public int? PaymentStatusId { get; set; }
    
    }
}
