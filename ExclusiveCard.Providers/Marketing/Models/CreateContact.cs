using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class CreateContactsRequest
    {
        /// <summary>
        /// This is required but we will expect only one in out implementation.
        /// </summary>
        [JsonProperty("list_ids")]
        public List<string> ContactListIds { get; set; }
        [JsonProperty( "contacts")]
        public List<MarketingContact> Contacts { get; set; }
    }

    public class ContactResponse : Errors
    {
        [JsonProperty("job_id")]
        public string JobId { get; set; }
    }
}
