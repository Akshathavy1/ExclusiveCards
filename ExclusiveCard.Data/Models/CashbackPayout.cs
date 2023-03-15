using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("CashbackPayout", Schema = "Exclusive")]
    public class CashbackPayout
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public DateTime? PayoutDate { get; set; }
        public decimal Amount { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CurrencyCode {get; set;}

        [ForeignKey("BankDetail")]
        public int? BankDetailId {get; set;}
        public DateTime RequestedDate { get; set; }
        public DateTime ChangedDate { get; set; }
        [DataType("nvarchar")]
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        public virtual BankDetail BankDetail { get; set; }
        public virtual Status Status { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
