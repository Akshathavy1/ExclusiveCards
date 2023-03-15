using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.Models.Api
{
    public class CreateAccountRequest
    {
        public string AppId { get; set; }
        public string Token { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address1 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public List<string> MarketingPreferences { get; set; }
    }
}
