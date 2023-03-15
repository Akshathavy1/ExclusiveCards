using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Charity", Schema = "CMS")]
    public class Charity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        [DataType("nvarchar")]
        public string CharityName { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string CharityUrl { get; set; }
    }
}
