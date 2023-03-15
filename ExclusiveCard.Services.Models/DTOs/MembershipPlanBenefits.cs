namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipPlanBenefits
    {
        public int Id { get; set; }

        public int MembershipPlanId { get; set; }

        public int DisplayOrder { get; set; }

        public string Description { get; set; }

        public MembershipPlan MembershipPlan { get; set; }
    }
}
