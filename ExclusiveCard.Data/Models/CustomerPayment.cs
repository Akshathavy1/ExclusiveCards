using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ExclusiveCard.Data.Models
{
    [Table("CustomerPayment", Schema = "Exclusive")]
    public class CustomerPayment
    {
        [Key]
        public int Id {get; set;}

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }

        [ForeignKey("Membershipcard")]
        public int? MembershipCardId { get; set; }

        [ForeignKey("PaymentProvider")]
        public int? PaymentProviderId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CurrencyCode { get; set; }

        [MaxLength(int.MaxValue)]
        [DataType("nvarchar")]
        public string Details { get; set; }

        [ForeignKey("CashbackTransaction")]
        public int? CashbackTransactionId { get; set; }

        [ForeignKey("PaymentNotification")]
        public int? PaymentNotificationId { get; set;}

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string PaymentProviderRef { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }

        public virtual PaymentProvider PaymentProvider {get; set;}

        public virtual CashbackTransaction CashbackTransaction { get; set; }

        public virtual PaymentNotification PaymentNotification { get; set; }
    }
}
