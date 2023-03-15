using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class ClickTracking
    {
        public int Id { get; set; }

        public int OfferId { get; set; }

        public int MembershipCardId { get; set; }

        public int AffiliateId { get; set; }

        public string DeeplinkURL { get; set; }

        public DateTime DateTime { get; set; }

        public virtual Offer Offer { get; set; }
        public virtual MembershipCard MembershipCard { get; set; }
        public virtual Affiliate Affiliate { get; set; }
    }
}
