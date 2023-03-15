using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Localisation", Schema = "CMS")]
    public class Localisation
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(512)]
        [DataType("nvarchar")]
        public string LocalisedText { get; set; }
        [MaxLength(512)]
        [DataType("nvarchar")]
        public string Context { get; set; }

        [MaxLength(5)]
        [DataType("nvarchar")]
        public string LocalisationCode { get; set; }

    }
}
