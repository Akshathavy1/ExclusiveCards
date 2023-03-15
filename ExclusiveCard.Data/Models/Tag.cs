using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Tag", Schema = "Exclusive")]
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(50)]
        [DataType("nvarchar")]
        public string Tags { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string TagType { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<OfferTag> OfferTags { get; set; }

    }
}
