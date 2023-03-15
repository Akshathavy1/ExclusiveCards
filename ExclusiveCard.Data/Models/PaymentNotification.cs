using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ExclusiveCard.Data.Models
{

    [Table("PaymentNotification", Schema = "Exclusive")]
    public class PaymentNotification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PaymentProvider")]
        public int PaymentProviderId { get; set; }

        public string CustomerPaymentProviderId { get; set; }

        public string TransactionType { get; set; }

        public DateTime DateReceived { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string FullMessage { get; set; }

        public virtual PaymentProvider PaymentProvider { get; set; }

        public virtual ICollection<CustomerPayment> CustomerPayment { get; set; }

    }
}
