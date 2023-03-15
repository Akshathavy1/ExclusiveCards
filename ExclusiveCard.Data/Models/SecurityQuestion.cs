using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("SecurityQuestion",Schema = "Exclusive")]
   public class SecurityQuestion
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        [DataType("nvarchar")]
        public string Question { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<CustomerSecurityQuestion> CustomerSecurityQuestions { get; set; }
    }
}
