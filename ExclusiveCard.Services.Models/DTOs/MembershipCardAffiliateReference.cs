namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipCardAffiliateReference
    {
        public int AffiliateId { get; set; }

        public int MembershipCardId { get; set; }

        public string CardReference { get; set; }

        public Affiliate Affiliate { get; set; }

        public MembershipCard MembershipCard { get; set; }
    }
}
