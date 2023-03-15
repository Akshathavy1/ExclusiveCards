using System;

namespace ExclusiveCard.WebAdmin.Models.Api
{
    public class LoginResponse
    {
        public string AppId { get; set; }

        public string UserName { get; set; }
        
        public Guid Token { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
