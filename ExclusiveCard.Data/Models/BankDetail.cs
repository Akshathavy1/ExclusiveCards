using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("BankDetail", Schema="Exclusive")]
    public class BankDetail
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string BankName { get; set; }

        [ForeignKey("ContactDetail")]
        public int? ContactDetailId { get; set; }

        [MaxLength(16)]
        [DataType("nvarchar")]
        public string SortCode { get; set; }

        [MaxLength(16)]
        [DataType("nvarchar")]
        public string AccountNumber { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string AccountName { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ContactDetail ContactDetail { get; set; }

        public virtual ICollection<CustomerBankDetail> CustomerBankDetails { get; set; }

        public virtual ICollection<Partner> Partners { get; set; }

        public virtual ICollection<CashbackPayout> CashbackPayouts { get; set; }
        public virtual ICollection<PartnerRewardWithdrawal> PartnerRewardWithdrawals { get; set; }
    }
}
