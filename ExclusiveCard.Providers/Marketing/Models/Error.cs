using System;
using System.Collections.Generic;
using System.Text;
using ExclusiveCard.Data.Models;
using Newtonsoft.Json;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class Error
    {
        [JsonProperty("message")]
        public string  Message { get; set; }
        [JsonProperty("field")]
        public string Field { get; set; }
        [JsonProperty("error_id")]
        public  string ErrorId { get; set; }
        [JsonProperty("paramater")]
        public string Parameter { get; set; }
    }

    public class Errors
    {
        [JsonProperty("errors")]
        public List<Error> errors { get; set; }
        public Errors()
        {
            errors = new List<Error>();
        }
    }
}
