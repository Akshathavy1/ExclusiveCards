using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Website.Models
{
    public class OfferAccessDeniedViewModel
    {
        public bool IsLoggedIn { get; set; }
        public bool IsDiamondMemberNeeded { get; set; }
        public string OfferMerchant { get; set; }
        public string ShortDescription { get; set; }
    }
}
