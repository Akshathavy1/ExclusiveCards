using ExclusiveCard.Website.Helpers;
using Newtonsoft.Json;

namespace ExclusiveCard.Website.Models
{
    //This should have been a service model dto
    //This copy here is only used by the old app
    //TODO: refactor app login and remove this
    public class LoginToken
    {
        [JsonProperty(PropertyName = "p1")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "p2")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "p3")]
        public string RemoteIp { get; set; }

        /// <summary>
        /// Expiry Date as UTC DateTime Ticks
        /// </summary>
        [JsonProperty(PropertyName = "p4")]
        public long ExpiresTimestamp { get; set; }

        public string ToEncryptedString()
        {
            string encryptedString = EncryptionHelper.EncryptToken(this);
            return System.Net.WebUtility.UrlEncode(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(encryptedString)));
        }

        public static LoginToken FromEncryptedString(string encryptedString)
        {
            string urlDecodedString = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Net.WebUtility.UrlDecode(encryptedString)));
            return EncryptionHelper.DecryptToken<LoginToken>(urlDecodedString);
        }
    }
}
