using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("EmailsSent")]
    public class EmailsSent
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("EmailTemplate")]
        public int EmailTemplateId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [MaxLength(512)]
        [Column(TypeName = "nvarchar(512)")]
        public string EmailTo { get; set; }
        public DateTime DateSent { get; set; }

        public virtual EmailTemplate EmailTemplate { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
