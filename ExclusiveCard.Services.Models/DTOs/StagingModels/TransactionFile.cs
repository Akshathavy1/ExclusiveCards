using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
    public class TransactionFile
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }

        public ICollection<CashbackTransaction> CashbackTransactions { get; set; }

        public ICollection<CashbackTransactionError> StagingCashbackTransactionErrors { get; set; }
    }
}
