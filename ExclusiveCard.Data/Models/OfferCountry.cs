using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("OfferCountry", Schema = "Exclusive")]
    public class OfferCountry
    {
        public int OfferId { get; set; }
        public string CountryCode { get; set; }

        public bool IsActive { get; set; }

        public virtual Offer Offer { get; set; }
    }
}
