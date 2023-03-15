using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("EmailCustomFields", Schema = "Marketing")]
    public class EmailCustomField
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string ExclusiveField { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string CustomName { get; set; }

        [MaxLength(6)]
        [DataType("nvarchar")]
        public string FieldType { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string SubstitutionTag { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string SendGridId { get; set; }
    }
}
