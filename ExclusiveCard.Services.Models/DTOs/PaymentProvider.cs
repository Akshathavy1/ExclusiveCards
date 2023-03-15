using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
   public  class PaymentProvider
    {

        public int Id { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public  ICollection<PaymentNotification> PaymentNotifications { get; set; }

        public  ICollection<CustomerPayment> CustomerPayment { get; set; }

        public  ICollection<MembershipPlanPaymentProvider> MembershipPlanPaymentProvider { get; set; }
    }
}
