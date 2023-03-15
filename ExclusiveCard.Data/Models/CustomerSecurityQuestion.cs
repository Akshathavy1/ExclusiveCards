using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("CustomerSecurityQuestion",Schema = "Exclusive")]
    public class CustomerSecurityQuestion
    {
        [Key, Column(Order = 0), ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [Key, Column(Order = 1), ForeignKey("SecurityQuestion")]
        public int SecurityQuestionId { get; set; }

        [MaxLength(500)]
        [DataType("nvarchar")]
        public string Answer { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual SecurityQuestion SecurityQuestion { get; set; }
    }
}
