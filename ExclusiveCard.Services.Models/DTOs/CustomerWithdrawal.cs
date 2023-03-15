using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class CustomerWithdrawal
    {
        public Int64 TotalRecord { get; set; }
        public Int64 RowNumber { get; set; }
        public string ContactName { get; set; }
        public string EmailAddress { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public int AccountCode { get; set; }
        public int TaxType { get; set; }
        public int TaxAmount { get; set; }
        public string Currency { get; set; }
    }
}
