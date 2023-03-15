using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipPlanType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? TermsConditionsId { get; set; }
        public TermsConditions TermsConditions { get; set; }
        public ICollection<Data.Models.MembershipPlan> MembershipPlans { get; set; }
    }
}
