using ExclusiveCard.Data.StagingModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Status", Schema = "Exclusive")]
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string Name { get; set; }
        [MaxLength(50)]
        [DataType("nvarchar")]
        public string Type { get; set; }
        public bool IsActive { get; set; }

    }
}
