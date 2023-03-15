using System;
using System.ComponentModel;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CustomerWithdrawViewModel
    {
        [DisplayName("Contact Name")]
        public string ContactName { get; set; }
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
        [DisplayName("PO Address 1")]
        public string POAddressLine1 { get; set; }
        [DisplayName("PO Address 2")]
        public string POAddressLine2 { get; set; }
        [DisplayName("PO Address 3")]
        public string POAddressLine3 { get; set; }
        [DisplayName("PO Address 4")]
        public string POAddressLine4 { get; set; }
        [DisplayName("PO City")]
        public string POCity { get; set; }
        [DisplayName("PO Region")]
        public string PORegion { get; set; }
        [DisplayName("PO Postal Code")]
        public string POPostalCode { get; set; }
        [DisplayName("PO Country")]
        public string POCountry { get; set; }
        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set; }
        [DisplayName("Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }
        [DisplayName("Total")]
        public decimal Total { get; set; }
        [DisplayName("Inventory Item Code")]
        public string InventoryItemCode { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Unit Amount")]
        public decimal UnitAmount { get; set; }
        [DisplayName("Account Code")]
        public int AccountCode { get; set; }
        [DisplayName("Tax Type")]
        public int TaxType { get; set; }
        [DisplayName("Tax Amount")]
        public int TaxAmount { get; set; }
        [DisplayName("Tracking Name 1")]
        public string TrackingName1 { get; set; }
        [DisplayName("Tracking Option 1")]
        public string TrackingOption1 { get; set; }
        [DisplayName("Tracking Name 2")]
        public string TrackingName2 { get; set; }
        [DisplayName("Tracking Option 2")]
        public string TrackingOption2 { get; set; }
        [DisplayName("Currency")]
        public string Currency { get; set; }
    }
}
