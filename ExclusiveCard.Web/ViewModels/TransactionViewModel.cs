using System;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Amount { get; set; }
        public string Payee { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
    }
}
