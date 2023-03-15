using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("PartnerRewards", Schema = "Exclusive")]
    public class PartnerRewards
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        [DataType("nvarchar")]
        public string RewardKey { get; set; }
        [ForeignKey("Partner")]
        public int? PartnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal LatestValue { get; set; }
        public DateTime? ValueDate { get; set; }
        public decimal TotalConfirmedWithdrawn { get; set; }
        [MaxLength(255)]
        [DataType("nvarchar")]
        public string Password { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual ICollection<MembershipCard> MembershipCards { get; set; }
        public virtual ICollection<PartnerRewardWithdrawal> PartnerRewardWithdrawals { get; set; }
    }
}
