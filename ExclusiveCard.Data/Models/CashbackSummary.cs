using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
   [Table("CashbackSummary",Schema ="Exclusive")]
    public class CashbackSummary
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Affiliate")]
        public int? AffiliateId { get; set; }

        [ForeignKey("MembershipCard")]
        public int MembershipCardId { get; set; }

        [ForeignKey("Partner")]

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
