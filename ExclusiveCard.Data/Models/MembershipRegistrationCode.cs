using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("MembershipRegistrationCode")]
    public class MembershipRegistrationCode
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MembershipPlan")]
        public int MembershipPlanId { get; set; }

        [MaxLength(30)]
        [DataType("nvarchar")]
        public string RegistartionCode { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public int NumberOfCards { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string EmailHostName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int? RegistrationCodeSummaryId { get; set; }

        public virtual MembershipPlan MembershipPlan { get; set; }
        public virtual RegistrationCodeSummary RegistrationCodeSummary { get; set; }

        public virtual ICollection<MembershipPendingToken> MembershipPendingTokens { get; set; }
        
        public virtual ICollection<MembershipCard> MembershipCards { get; set; }
    }
}
