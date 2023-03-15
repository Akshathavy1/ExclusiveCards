using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
   
    [Table("SponsorImages", Schema = "CMS")]
    public class SponsorImages
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        [DataType("varchar")]
        public string File { get; set; }
        public int WhiteLabelId { get; set; }
    }
}
