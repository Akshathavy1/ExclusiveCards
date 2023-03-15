using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class SearchResponse : Errors
    {
        [JsonProperty("result")]
        public List<SearchResult> SearchResult { get; set; }

        public SearchResponse()
        {
            SearchResult = new List<SearchResult>();
        }
    }
    public class SearchResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("list_ids")]
        public List<string> ListIds { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
    }

    public class SearchContact
    {
        [JsonProperty("query")]
        public string Query { get; set; }
    }

    public class SearchContactEmails
    {
        [JsonProperty("emails")]
        public List<string> EmailAddresses { get; set; }
    }
}
