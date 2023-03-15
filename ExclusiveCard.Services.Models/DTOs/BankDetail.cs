using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class BankDetail
    {
        public int Id { get; set; }

        public string BankName { get; set; }

        public int? ContactDetailId { get; set; }

        public string SortCode { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public bool IsDeleted { get; set; }

        public ContactDetail ContactDetail { get; set; }
        public CustomerBankDetail CustomerBankDetail { get; set; }



    }
}
