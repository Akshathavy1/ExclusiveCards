using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IPartnerRewardWithdrawalService
    {
        Task<dto.PartnerRewardWithdrawal> AddAsync(dto.PartnerRewardWithdrawal reward);
        Task<dto.PartnerRewardWithdrawal> UpdateAsync(dto.PartnerRewardWithdrawal reward);
        Task<List<dto.PartnerRewardWithdrawal>> BulkUpdateAsync(List<dto.PartnerRewardWithdrawal> reward);
        Task<dto.PartnerRewardWithdrawal> UpdateErrorAsync(int partnerRewardId, int errorStatus);
        Task<dto.PartnerRewardWithdrawal> UpdateConfirmationAsync(int partnerRewardId, int successStatus,
            decimal amountConfirmed);

        Task<dto.PartnerRewardWithdrawal> GetByIdAsync(int id);
        Task<dto.PartnerRewardWithdrawal> GetByBankDetailIdAsync(int id);
        Task<List<dto.TamWithdrawalDataModel>> GetWithdrawalReport(int statusId);
        Task<List<dto.PartnerRewardWithdrawal>> GetAllAsync();
        Task<List<dto.PartnerRewardWithdrawal>> GetAllPendingAsync();
        Task<dto.WithdrawalRequestModel> GetWithdrawalDataForRequest(int membershipCardId);
        Task<dto.PagedResult<dto.TransactionLog>> GetTransactionLog(string userId, int page,
            int pageSize, Enums.TransactionLogSortOrder sortOrder);
        Task<dto.PagedResult<dto.TransactionLog>> GetWithdrawalLog(string userId, int page,
            int pageSize);
        Task<dto.PagedResult<dto.PartnerRewardWithdrawal>> GetWithdrawalsForPayments(int partnerId,
            int status, int page, int pageSize, Enums.WithdrawalSortOrder sortOrder);

    }
}