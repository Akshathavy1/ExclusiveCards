using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class TermsConditions
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public DateTime ValidFrom { get; set; }

        public ICollection<MembershipPlanType> MembershipPlanTypes { get; set; }
        public ICollection<MembershipCard> MembershipCards { get; set; }
    }
}
