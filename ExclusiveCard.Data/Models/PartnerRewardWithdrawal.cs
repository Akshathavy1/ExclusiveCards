
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("PartnerRewardWithdrawal", Schema = "Exclusive")]
    public class PartnerRewardWithdrawal
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("PartnerReward")]
        public int? PartnerRewardId { get; set; }
        [ForeignKey("WithdrawalStatus")]
        public int StatusId { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal? ConfirmedAmount { get; set; }
        [ForeignKey("File")]
        public int? FileId { get; set; }
        [ForeignKey("BankDetail")]
        public int BankDetailId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime ChangedDate { get; set; }
        [MaxLength(450)]
        [DataType("nvarchar")]
        public string UpdatedBy { get; set; }

        public virtual PartnerRewards PartnerReward { get; set; }
        public virtual Status WithdrawalStatus { get; set; }
        public virtual Files File { get; set; }
        public virtual BankDetail BankDetail { get; set; }
    }
}
