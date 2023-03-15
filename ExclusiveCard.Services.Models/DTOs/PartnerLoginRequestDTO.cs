using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class PartnerLoginRequestDTO
    {
        public int PartnerId { get; set; }
        public string PartnerSecret { get; set; }
    }
}
