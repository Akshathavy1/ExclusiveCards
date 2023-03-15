using ExclusiveCard.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    [DataContract]
    public class AppleResponse
    {
        [DataMember(Name = "status")]
        public AppleStatus Status { get; set; }

        [DataMember(Name = "environment")]
        public string Environment { get; set; }

        [DataMember(Name = "receipt")]
        public JObject Receipt { get; set; }

        [DataMember(Name = "paymentNotification")] 
        public int PaymentNotificationId { get; set; }
    }
}
