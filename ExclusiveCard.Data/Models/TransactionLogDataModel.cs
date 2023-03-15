using System;

namespace ExclusiveCard.Data.Models
{
    public class TransactionLogDataModel
    {
        public Int64 TotalRecord { get; set; }
        public Int64 RowNumber { get; set; }
        public DateTime Date { get; set; }
        public string Merchant { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public bool Invested { get; set; }
        public string Donated { get; set; }
        public string Summary { get; set; }
        public string PurchaseAmount { get; set; }
    }
}
