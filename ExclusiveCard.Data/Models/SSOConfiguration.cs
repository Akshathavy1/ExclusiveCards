using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("SSOConfiguration", Schema = "Exclusive")]
    public class SSOConfiguration
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string DestinationUrl { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string ClientId { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Metadata { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Certificate { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Issuer { get; set; }
    }
}