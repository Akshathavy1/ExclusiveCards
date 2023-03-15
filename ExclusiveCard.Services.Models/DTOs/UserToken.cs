using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class UserToken
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public Guid Token { get; set; }

        public string Role { get; set; }

        public int MembershipPlanId { get; set; }
        public decimal CardCost { get; set; }
        public string Slug { get; set; }
        public int WhitelabelId { get; set; }

    }
}
