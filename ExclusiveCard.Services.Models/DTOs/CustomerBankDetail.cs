using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class CustomerBankDetail
    {
        public int CustomerId { get; set; }

        public int BankDetailsId { get; set; }

        public bool MandateAccepted { get; set; }

        public DateTime? DateMandateAccepted { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public Customer Customer { get; set; }

        public BankDetail BankDetail { get; set; }
    }
}
