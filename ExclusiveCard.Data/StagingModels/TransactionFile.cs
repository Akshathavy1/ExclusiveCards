using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.StagingModels
{
    [Table("TransactionFile", Schema ="Staging")]
    public class TransactionFile
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string FileName { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        public virtual Status Status { get; set; }

        public virtual ICollection<CashbackTransaction> CashbackTransactions { get; set; }

        public virtual ICollection<CashbackTransactionError> StagingCashbackTransactionErrors { get; set; }
    }
}
