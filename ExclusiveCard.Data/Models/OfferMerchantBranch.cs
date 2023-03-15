using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{

    [Table("OfferMerchantBranch", Schema = "Exclusive")]
    public class OfferMerchantBranch
    {
        [Key, Column(Order = 0), ForeignKey("Offer")]
        public int OfferId { get; set; }

        [Key, Column(Order = 1), ForeignKey("MerchantBranch")]
        public int MerchantBranchId { get; set; }

        public virtual Offer Offer { get; set; }

        public virtual MerchantBranch MerchantBranch { get; set; }
    }
}
