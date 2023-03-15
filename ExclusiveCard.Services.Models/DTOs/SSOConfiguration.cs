using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class SSOConfiguration
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DestinationUrl { get; set; }

        public string ClientId { get; set; }

        public string Metadata { get; set; }

        public string Certificate { get; set; }

        public string Issuer { get; set; }
    }
}