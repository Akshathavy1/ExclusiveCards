namespace ExclusiveCard.Data.Models
{
    public class RedemptionDataModel
    {
        public string FileName { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerRef { get; set; }
        public string Name { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string Add3 { get; set; }
        public string Add4 { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public int Value { get; set; }
        public int Total { get; set; }
        public string ActivationPIN { get; set; }
        public string CustomerNotes { get; set; }
    }
}
