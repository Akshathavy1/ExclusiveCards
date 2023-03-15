using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class LoginToken
    {
        public string AspNetUserId { get; set; }

        /// <summary>
        /// Expiry Date as UTC DateTime Ticks
        /// </summary>
        public DateTime ExpiresTimestamp { get; set; }

        public Guid TokenValue { get; set; }

        public string ToBase64String()
        {
            string tokenJson = JsonConvert.SerializeObject(this, Formatting.None);
            return System.Net.WebUtility.UrlEncode(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tokenJson)));
        }

        public static LoginToken FromBase64String(string base64String)
        {
            string urlDecodedString = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Net.WebUtility.UrlDecode(base64String)));
            return JsonConvert.DeserializeObject<LoginToken>(urlDecodedString);
        }
    }
}
