namespace ExclusiveCard.Website.Models
{
    public class PaymentViewModel
    {
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public bool PayPal { get; set; }
        public bool Cashback { get; set; }
        public decimal MembershipCard { get; set; }
    }
}
