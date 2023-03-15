using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Website.Models
{
    public class SSOViewModel
    {
        public int CustomerId { get; set; }

        public int SSOConfigId { get; set; }

        public string ReturnUrl { get; set; }

        public string OfferUrl { get; set; }

        public string ProductCode { get; set; }

        public string MerchantName { get; set; }

        public string AcsUrl { get; set; }

        public string SamlResponseBase64 { get; set; }
    }
}