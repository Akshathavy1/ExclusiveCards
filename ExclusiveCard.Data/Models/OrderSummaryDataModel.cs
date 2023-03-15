namespace ExclusiveCard.Data.Models
{
    public class OrderSummaryDataModel
    {
        public int MembershipPlanId { get; set; }
        public string OrderName { get; set; }
        public decimal CardPrice { get; set; }
        public string SubscriptionAppAndCardRef { get; set; }
        public string SubscriptionAppRef { get; set; }
        public string OneOffPaymentRef { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal PaymentFee { get; set; }
    }
}
