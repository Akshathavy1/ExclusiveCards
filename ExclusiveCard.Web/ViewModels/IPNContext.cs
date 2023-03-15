using Microsoft.AspNetCore.Http;
using System;

namespace ExclusiveCard.WebAdmin.Models
{
    public class IPNContext
    {
        public HttpRequest IPNRequest { get; set; }

        public string RequestBody { get; set; }

        public string Verification { get; set; } = String.Empty;
    }
}
