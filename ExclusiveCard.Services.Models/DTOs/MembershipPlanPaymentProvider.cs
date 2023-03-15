namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipPlanPaymentProvider
    {
        public int MembershipPlanId { get; set; }

        public int PaymentProviderId { get; set; }

        public string SubscribeAppRef { get; set; }

        public string SubscribeAppAndCardRef { get; set; }
        public string OneOffPaymentRef { get; set; }

        public MembershipPlan MembershipPlan { get; set; }

        public PaymentProvider PaymentProvider { get; set; }

    }
}
