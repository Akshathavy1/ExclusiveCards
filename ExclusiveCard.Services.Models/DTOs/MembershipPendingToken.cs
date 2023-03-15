using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipPendingToken
    {
        public int Id { get; set; }

        public int MembershipRegistrationCodeId { get; set; }

        public Guid Token { get; set; }

        public DateTime DateCreated { get; set; }

        public  MembershipRegistrationCode MembershipRegistrationCode { get; set; }
    }
}
