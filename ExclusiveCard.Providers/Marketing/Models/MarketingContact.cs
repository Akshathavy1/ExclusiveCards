using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class MarketingContact
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("address_line_1")]
        public string Address1 { get; set; }

        [JsonProperty("address_line_2")]
        public string Address2 { get; set; }

        [JsonProperty("postal_code")]
        public string PostCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }

    public class MarketingContactId
    {
        public string Id { get; set; }
    }

    public class DeleteContactQuery
    {
        [JsonProperty("ids")]
        public string Ids { get; set; }
    }

}
