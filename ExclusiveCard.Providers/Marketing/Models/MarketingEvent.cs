using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class MarketingEvent
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("categories")]
        public List<string> Categories { get; set; }
        [JsonProperty("send_at")]
        public string SendAt { get; set; }
        [JsonProperty("send_to")]
        public MarketingEventSendTo SendTo { get; set; }
        [JsonProperty("email_config")]
        public MarketingEventEmailConfig EmailConfig { get; set; }
        
    }

    public class MarketingEventSendTo
    {
        [JsonProperty("list_ids")]
        public List<string> ListIds { get; set; }
        [JsonProperty("segment_ids")]
        public List<string> SegmentIds { get; set; }
        [JsonProperty("all")]
        public bool All { get; set; }
    }
    
    public class MarketingEventEmailConfig
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("html_content")]
        public string HtmlContent { get; set; }
        [JsonProperty("plain_content")]
        public string PlainContent { get; set; }
        [JsonProperty("generate_plain_content")]
        public bool GeneratePlainContent { get; set; }
       
        [JsonProperty("editor")]
        public string Editor { get; set; }
        [JsonProperty("custom_unsubscribe_url")]
        public string CustomUnsubscribeUrl { get; set; }
        [JsonProperty("sender_id")]
        public int? SenderId { get; set; }
       
    }

    public class MarketingEventResponses
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("categories")]
        public List<string> Categories { get; set; }
        [JsonProperty("send_at")]
        public string SendAt { get; set; }
        [JsonProperty("send_to")]
        public MarketingEventSendTo SendTo { get; set; }
        [JsonProperty("email_config")]
        public MarketingEventEmailConfig EmailConfig { get; set; }

    }

    public class MarketingEventSchedule
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("categories")]
        public List<string> Categories { get; set; }
        [JsonProperty("send_at")]
        public string SendAt { get; set; }
        [JsonProperty("send_to")]
        public MarketingEventSendTo SendTo { get; set; }
        [JsonProperty("email_config")]
        public MarketingEventEmailConfig EmailConfig { get; set; }

    }
    public class ContactsCount
    {
        [JsonProperty("contact_count")]
        public int Count { get; set; }
        [JsonProperty("billable_count")]
        public int BillableCount { get; set; }

    }
    
}
