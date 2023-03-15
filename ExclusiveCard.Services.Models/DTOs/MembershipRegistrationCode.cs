using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipRegistrationCode
    {
        public int Id { get; set; }

        public int MembershipPlanId { get; set; }

        public string RegistartionCode { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public int NumberOfCards { get; set; }

        public string EmailHostName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public  MembershipPlan MembershipPlan { get; set; }

        public ICollection<MembershipPendingToken> MembershipPendingTokens { get; set; }

        public ICollection<MembershipCard> MembershipCards { get; set; }
    }
}
