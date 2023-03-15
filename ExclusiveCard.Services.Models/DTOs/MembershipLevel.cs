using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipLevel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }

        public ICollection<MembershipPlan> MembershipPlans { get; set; }
    }
}
