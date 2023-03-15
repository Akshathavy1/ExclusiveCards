using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("TermsConditions", Schema = "Exclusive")]
    public class TermsConditions
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Url { get; set; }

        public DateTime ValidFrom { get; set; }

        public virtual ICollection<MembershipPlanType> MembershipPlanTypes { get; set; }
        public virtual ICollection<MembershipCard> MembershipCards { get; set; }
    }
}
