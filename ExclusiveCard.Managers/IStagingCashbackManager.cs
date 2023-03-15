using System;
using System.Collections.Generic;
using System.Text;
using st = ExclusiveCard.Data.StagingModels;
using dto = ExclusiveCard.Services.Models.DTOs;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public interface IStagingCashbackManager
    {
        int CreateTransactionFile(dto.StagingModels.TransactionFile file);

        Task<List<dto.StagingModels.TransactionFile>> GetTransactionFilesAsync(Enums.StagingTransactionFiles fileStatus);

        void SetTransactionFileStatus(int fileId, Enums.StagingTransactionFiles fileStatus);

        int CreateTransaction(dto.StagingModels.CashbackTransaction transaction);

        Task<List<dto.StagingModels.CashbackTransaction>> GetTransactionsAsync(Enums.StagingCashbackTransactions status);

        void SetTransactionStatus(int transactionId, Enums.StagingCashbackTransactions status);

        void UpdateTransaction(dto.StagingModels.CashbackTransaction transaction);

        void CreateError(dto.StagingModels.CashbackTransactionError error);



    }
}
