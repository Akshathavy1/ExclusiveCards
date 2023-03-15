using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IPartnerTransactionManager
    {
        Task UpdatePartnerTransactions(int partnerRewardId, int fileId, int paymentStatusId);
        Task<List<TamDataModel>> GetTransactionReport(int partnerId);

        Task<PagedResult<Files>> GetTransactionsAsync(int statusId, int partnerId, int page, int pageSize,
            TransactionSortOrder sortOrder);

        Task<PagedResult<CustomerWithdrawalDataModel>> GetPagedCustomerRewardWithdrawalsAsync(
            DateTime fromDate, DateTime toDate, int page, int pageSize);

        Task<List<CustomerWithdrawalDataModel>> GetCustomerRewardWithdrawalsAsync(int partnerId,
            DateTime fromDate, DateTime toDate);
    }
}