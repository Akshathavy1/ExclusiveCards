using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ExclusiveCard.Data.Models
{
    [Table("Partner",Schema = "Exclusive")]
    public class Partner
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [ForeignKey("ContactDetail")]
        public int? ContactDetailId { get; set; }

        [ForeignKey("BankDetail")]
        public int? BankDetailsId { get; set; }

        public bool IsDeleted{ get; set; }
        public int Type { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string ImagePath { get; set; }
        [MaxLength(512)]
        [DataType("nvarchar")]
        public string ManagementURL { get; set; }

        [ForeignKey("IdentityUser")]
        [MaxLength(450)]
        [DataType("nvarchar")]
        public string AspNetUserId { get; set; }

        public string MembershipCardPrefix { get; set; }

        public virtual ContactDetail ContactDetail { get; set; }

        public virtual BankDetail BankDetail { get; set; }

        public virtual ICollection<MembershipPlan> MembershipPlans { get; set; }
        public virtual ICollection<CashbackTransaction> CashbackTransactions { get; set; }
        public virtual ICollection<CashbackSummary> CashbackSummaries { get; set; }
        public virtual ICollection<Files> Files { get; set; }
        public virtual ICollection<PartnerRewards> PartnerRewards { get; set; }

        public virtual ExclusiveUser IdentityUser { get; set; }

    }
}
