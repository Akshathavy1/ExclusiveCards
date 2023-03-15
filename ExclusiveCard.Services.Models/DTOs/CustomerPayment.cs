using System;

namespace ExclusiveCard.Services.Models.DTOs
{
  public  class CustomerPayment
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        public int? MembershipCardId { get; set; }

        public int? PaymentProviderId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }        
        
        public string CurrencyCode { get; set; }

        public string Details { get; set; }

        public int? CashbackTransactionId { get; set; }

        public int? PaymentNotificationId { get; set; }

        public string PaymentProviderRef { get; set; }

        public Customer Customer { get; set; }

        public MembershipCard MembershipCard { get; set; }

        public PaymentProvider PaymentProvider { get; set; }

        public CashbackTransaction CashbackTransaction { get; set; }

        public PaymentNotification PaymentNotification { get; set; }
    }
}
