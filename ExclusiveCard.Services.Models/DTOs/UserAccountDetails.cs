using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class UserAccountDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string PostCode { get; set; }

        public int SecurityQuestionId { get; set; }

        public string SecurityAnswer { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string NINumber { get; set; }
        public bool MarketingPreferences { get; set; }
        public int? MembershipLevelId { get; set; }
        public string UserWhiteLabelUrl { get; set; }
        public int? SiteClanId { get; set; }
    }
}
