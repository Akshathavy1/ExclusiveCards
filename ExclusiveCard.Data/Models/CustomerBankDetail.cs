using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("CustomerBankDetail",Schema="Exclusive")]
    public class CustomerBankDetail
    {
        [Key, Column(Order = 0), ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Key, Column(Order = 1), ForeignKey("BankDetail")]
        public int BankDetailsId { get; set; }

        public bool MandateAccepted { get; set; }

        public DateTime? DateMandateAccepted { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual BankDetail BankDetail { get; set; }
    }
}
