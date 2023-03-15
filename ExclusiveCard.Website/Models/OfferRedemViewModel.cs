using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.Website.Models
{
    public class OfferRedemViewModel
    {
        public int OfferId { get; set; }
        public int MembershipCardId { get; set; }
        public string UserId { get; set; }
        public int Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public bool Redeemed { get; set; }
    }
}
