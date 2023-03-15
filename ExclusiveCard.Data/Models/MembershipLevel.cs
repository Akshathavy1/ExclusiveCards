using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ExclusiveCard.Data.Models
{
    [Table("MembershipLevel", Schema = "Exclusive")]
    public class MembershipLevel
    {
        [Key]
        public int Id { get; set; }
        [DataType("nvarchar")]
        [MaxLength(50)]
        public string Description { get; set; }
        public int Level { get; set; }

        public virtual ICollection<MembershipPlan> MembershipPlans { get; set; }
    }
}
