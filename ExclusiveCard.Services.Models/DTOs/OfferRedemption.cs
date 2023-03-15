using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferRedemption
    {
        public int MembershipCardId { get; set; }
        public int OfferId { get; set; }
        public int State { get; set; } //(Requested or Complete)
        public int? FileId { get; set; }//(FK to file table)
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CustomerRef { get; set; }

        public MembershipCard MembershipCard { get; set; }
        public Offer Offer { get; set; }
        public Status Status { get; set; }
        public Files File { get; set; }
    }
}
