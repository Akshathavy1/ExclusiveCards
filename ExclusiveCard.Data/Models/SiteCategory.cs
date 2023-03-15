using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ExclusiveCard.Data.Models
{
    [Table("SiteCategory", Schema = "CMS")]
    public class SiteCategory
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        [DataType("nvarchar")]
        public string Description { get; set; }
    }
}
