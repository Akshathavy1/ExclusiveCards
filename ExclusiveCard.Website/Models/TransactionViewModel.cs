using System;

namespace ExclusiveCard.Website.Models
{
    public class TransactionViewModel
    {
        public DateTime Date { get; set; }
        public string Merchant { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public bool Invested { get; set; }
        public string Donated { get; set; }
    }
}
