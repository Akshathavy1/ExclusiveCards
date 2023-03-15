using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("OfferCategory", Schema = "Exclusive")]
    public class OfferCategory
    {
        public int OfferId { get; set; }
        public int CategoryId { get; set; }

        public virtual Offer Offer { get; set; }

        public virtual Category Category { get; set; }
    }
}
