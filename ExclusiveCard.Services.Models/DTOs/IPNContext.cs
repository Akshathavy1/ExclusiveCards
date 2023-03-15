using System;
using Microsoft.AspNetCore.Http;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class IPNContext
    {
        public HttpRequest IPNRequest { get; set; }

        public string RequestBody { get; set; }

        public string Verification { get; set; } = String.Empty;
    }
}
