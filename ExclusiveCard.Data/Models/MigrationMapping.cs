using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{

    [Table("MigrationMapping", Schema = "Exclusive")]
    public class MigrationMapping
    {

        [Key]
        public int Id { get; set; }

        public long OldId { get; set; }

        public int NewId { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string EntityType { get; set; }
    }
}
