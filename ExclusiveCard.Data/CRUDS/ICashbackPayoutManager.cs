using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ICashbackPayoutManager
    {
        Task<CashbackPayout> Add(CashbackPayout cashbackPayout);
        Task<CashbackPayout> Update(CashbackPayout cashbackPayout);
        Task<CashbackPayout> Get(int id);
        Task<CashbackPayout> GetByCustomerPartnerCurrency(int customerId, string currencyCode);
        Task<CashbackPayout> GetByMembershipCard(int membershipCardId);
        Task<List<CashbackPayout>> GetAll();
        Task<WithdrawalRequestModel> GetCashoutDataForRequest(string userId);

        Task<PagedResult<CashbackPayout>> GetCashbackPaidoutData(int statusId, int page, int pageSize,
            WithdrawalSortOrder sortOrder);

        decimal GetWithdrawnAmount(int customerId);

        Task<PagedResult<CashbackTransaction>> GetAllPagedFinancialReportSearch(int statusId, DateTime startDate,
            DateTime endDate, int page, int pageSize);

        List<SPFinancialReport> GetPagedFinancialReportSearch(int statusId, DateTime? startDate, DateTime? endDate,
            int page, int pageSize);

        List<SPFinancialReport> GetAllPagedFinancialReport(DateTime startDate, DateTime endDate);
    }
}