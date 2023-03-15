using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IPartnerRewardWithdrawalManager
    {
        Task<PartnerRewardWithdrawal> AddAsync(PartnerRewardWithdrawal data);
        Task<PartnerRewardWithdrawal> UpdateAsync(PartnerRewardWithdrawal data);
        Task<List<PartnerRewardWithdrawal>> BulkUpdateAsync(List<PartnerRewardWithdrawal> data);
        Task<PartnerRewardWithdrawal> UpdateErrorAsync(int partnerRewardId, int errorStatus);

        Task<PartnerRewardWithdrawal> UpdateConfirmationAsync(int partnerRewardId, int successStatus,
            decimal amountConfirmed);

        Task<PartnerRewardWithdrawal> GetByIdAsync(int id);
        Task<PartnerRewardWithdrawal> GetByBankDetailIdAsync(int id);
        Task<List<TamWithdrawalDataModel>> GetWithdrawalReport(int statusId);
        Task<List<PartnerRewardWithdrawal>> GetAllAsync();
        Task<List<PartnerRewardWithdrawal>> GetAllPendingAsync();
        Task<WithdrawalRequestModel> GetWithdrawalDataForRequest(int membershipCardId);

        Task<PagedResult<TransactionLogDataModel>> GetTransactionLog(string userId, int page, int pageSize,
            TransactionLogSortOrder sortOrder);

        Task<PagedResult<TransactionLogDataModel>> GetWithdrawalLog(string userId, int page, int pageSize);

        Task<PagedResult<PartnerRewardWithdrawal>> GetWithdrawalsForPayments(int partnerId,
            int status, int page, int pageSize, WithdrawalSortOrder sortOrder);

        Task<PagedResult<PartnerRewardWithdrawal>> GetCustomerWithdrawal(int statusId, int page, int pageSize,
            WithdrawalSortOrder sortOrder, DateTime startDate, DateTime endDate);
    }
}