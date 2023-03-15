using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
   public class PaymentNotification
    {
     
        public int Id { get; set; }

        public int PaymentProviderId { get; set; }

        public string CustomerPaymentProviderId { get; set; }

        public string TransactionType { get; set; }

        public DateTime DateReceived { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string FullMessage { get; set; }

        public PaymentProvider PaymentProvider { get; set; }

        public ICollection<CustomerPayment> CustomerPayment { get; set; }
    }
}
