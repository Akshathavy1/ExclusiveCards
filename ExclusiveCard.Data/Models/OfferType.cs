using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("OfferType", Schema = "Exclusive")]
    public  class OfferType
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("ActionTextLocalisation")]
        public int? ActionTextLocalisationId { get; set; }

        [ForeignKey("TitleLocalisation")]
        public int? TitleLocalisationId { get; set; }

        public int SearchRanking { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }
        public virtual Localisation ActionTextLocalisation { get; set; }
        public virtual Localisation TitleLocalisation { get; set; }
    }
}
