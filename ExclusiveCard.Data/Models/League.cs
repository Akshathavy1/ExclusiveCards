using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("League", Schema = "CMS")]
    public class League
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string ImagePath { get; set; }

        [ForeignKey("SiteCategory")]
        public int SiteCategoryId { get; set; }

        public virtual ICollection<SiteClan> SiteClan { get; set; }
        public virtual SiteCategory SiteCategory { get; set; }

    }
}
