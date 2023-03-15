using System;

namespace ExclusiveCard.Website.Models.Api
{
    public class ValidateLoginRequest
    {
        public string AppId { get; set; }

        public string UserName { get; set; }

        public Guid Token { get; set; }
    }
}