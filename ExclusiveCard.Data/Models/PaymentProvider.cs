using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("Paymentprovider", Schema = "Exclusive")]
    public class PaymentProvider
    {
        [Key]

        public int Id { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<PaymentNotification> PaymentNotifications { get; set; }

        public virtual ICollection<CustomerPayment> CustomerPayment { get; set; }

        public virtual ICollection<MembershipPlanPaymentProvider> MembershipPlanPaymentProvider { get; set; }
    }
}
