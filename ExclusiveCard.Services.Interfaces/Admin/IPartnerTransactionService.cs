using System;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IPartnerTransactionService
    {
        Task<Models.DTOs.Files> AddAsync(Models.DTOs.Files file);
        Task<Models.DTOs.Files> UpdateAsync(Models.DTOs.Files file);

        Task<List<Models.DTOs.TamDataModel>> GetTransactionReport(int partnerId);
        Task<Models.DTOs.Files> GetByIdAsync(int id);
        Task<Models.DTOs.Files> GetByNameAsync(string name);
        Task<List<Models.DTOs.Files>> GetAllAsync();
        bool CheckIfFileExistsWithProcessingState(int partnerId);

        Task<Models.DTOs.ProcessResult> ProcessPartnerFile(int partnerId, Models.DTOs.ExternalFile externalFile,
            string errorBlob, string processedBlob, string partnerName);
        Task<Models.DTOs.ProcessResult> ProcessTAMPositionFile(Models.DTOs.ExternalFile externalFile, ILogger logger = null);

        Task<Models.DTOs.PagedResult<Models.DTOs.Files>> GetTransactionsAsync(int statusId, int partnerId, int page,
            int pageSize, Enums.TransactionSortOrder sortOrder);

        Task<Models.DTOs.PagedResult<Models.DTOs.Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, int page, int pageSize, Enums.TransactionSortOrder sortOrder);

        Task<List<Models.DTOs.Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, Enums.TransactionSortOrder sortOrder);

        Task<Models.DTOs.PagedResult<Models.DTOs.CustomerWithdrawal>> GetPagedCustomerRewardWithdrawalsAsync(DateTime fromDate, DateTime toDate, int page, int pageSize);

        Task<List<Models.DTOs.CustomerWithdrawal>> GetCustomerRewardWithdrawalsAsync(int partnerId, DateTime fromDate, DateTime toDate);

        Task<Models.DTOs.ProcessResult> ProcessWithdrawalFile(int partnerId,
            Models.DTOs.ExternalFile externalFile, string errorBlob, string processedBlob, string partnerName);
        Task<Models.DTOs.PagedResult<Models.DTOs.Files>> GetPagedFileResults(int page, int pageSize,
            int? state, string type, DateTime? createdFrom, DateTime? createdTo);
    }
}