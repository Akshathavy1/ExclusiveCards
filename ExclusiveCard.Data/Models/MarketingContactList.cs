using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("ContactLists", Schema = "Marketing")]
    public class MarketingContactList
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("WhiteLabelSettings")]
        public int WhiteLabelId { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string ContactListReference { get; set; }

        [MaxLength(270)]
        [DataType("nvarchar")]
        public string ContactListName { get; set; }

        public virtual WhiteLabelSettings WhiteLabelSettings { get; set; }
    }
}
