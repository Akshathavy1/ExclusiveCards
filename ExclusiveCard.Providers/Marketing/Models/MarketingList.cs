using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class MarketingList
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class MarketingListResponse : Errors
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contact_count")]
        public int ContactCount { get; set; }
    }

    public class MarketingListResponseBulk
    {
        [JsonProperty("result")]
        public List<MarketingListResponse> marketingListResponse { get; set; }

        public MarketingListResponseBulk()
        {
            marketingListResponse = new List<MarketingListResponse>();
        }
    }
}
