using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class LoginUserToken
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Token { get; set; }
        public Guid TokenValue { get; set; }

        public ExclusiveUser IdentityUser { get; set; }
    }
}
