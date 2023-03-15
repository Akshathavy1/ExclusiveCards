using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MerchantBranch
    {
        public int Id { get; set; }
        
        public int? ContactDetailsId { get; set; }

        public int MerchantId { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string Notes { get; set; }

        public bool Mainbranch { get; set; }

        public short DisplayOrder { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Merchant Merchant { get; set; }

        public ContactDetail ContactDetail { get; set; }

        public ICollection<OfferMerchantBranch> OfferMerchantBranches { get; set; }
    }
}
