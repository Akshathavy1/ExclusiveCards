using System;
using System.ComponentModel;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class PartnerTransactionsViewModel
    {
        public int Id { get; set; }
        [DisplayName("Date")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Filename")]
        public string FileName { get; set; }
        [DisplayName("Status")]
        public string PaymentStatus { get; set; }
        [DisplayName("Total")]
        public string Amount { get; set; }
    }
}
