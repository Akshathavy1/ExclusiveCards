using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("Contacts", Schema = "Marketing")]
    public class MarketingContact
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int ExclusiveCustomerId { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string ContactReference { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
