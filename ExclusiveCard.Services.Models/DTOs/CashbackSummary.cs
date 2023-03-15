using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
   public class CashbackSummary
    {
        public int Id { get; set; }

        public int? AffiliateId { get; set; }

        public int MembershipCardId { get; set; }

        public int? PartnerId { get; set; }

        public char AccountType { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CurrencyCode { get; set; }

        public decimal PendingAmount { get; set; }

        public decimal ConfirmedAmount { get; set; }

        public decimal ReceivedAmount { get; set; }

        public decimal FeeDue { get; set; }

        public decimal PaidAmount { get; set; }


    }
}
