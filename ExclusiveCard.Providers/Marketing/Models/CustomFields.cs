using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class CustomFieldRequestModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("field_type")]
        public string FiledType { get; set; }
    }

    public class CustomFieldResponseList
    {
        [JsonProperty("custom_fields")]
        public List<CustomFieldResponse> CustomFields { get; set; }

        public CustomFieldResponseList()
        {
            CustomFields = new List<CustomFieldResponse>();
        }
    }

    public class CustomFieldResponse : Errors
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("field_type")]
        public string FiledType { get; set; }
        [JsonProperty("_metadata")]
        public MetaData MetaData { get; set; }
    }

    public class MetaData
    {
        public string self;
    }
}
