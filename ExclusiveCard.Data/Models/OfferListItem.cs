using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("OfferListItems", Schema = "CMS")]
    public class OfferListItem
    {
        [Key, Column(Order = 0), ForeignKey("OfferList")]
        public int OfferListId { get; set; }

        [Key, Column(Order = 1), ForeignKey("Offer")]
        public int OfferId { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string ExcludedCountries { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        [Key, Column(Order = 2)]
        public string CountryCode { get; set; }

        public short DisplayOrder { get; set; }

        public DateTime? DisplayFrom { get; set; }

        public DateTime? DisplayTo { get; set; }

        public virtual OfferList OfferList { get; set; }

        public virtual Offer Offer { get; set; }
    }
}