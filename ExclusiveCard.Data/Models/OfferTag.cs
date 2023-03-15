using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("OfferTag", Schema = "Exclusive")]
    public class OfferTag
    {
        [Key, Column(Order = 0), ForeignKey("Offer")]
        public int OfferId { get; set; }
        [Key, Column(Order = 1), ForeignKey("Tag")]
        public int TagId { get; set; }

        public virtual Offer Offer { get;set;}

        public virtual Tag Tag { get; set; }
    }
}
