using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface ICashbackPayoutService
    {
        Task<CashbackPayout> Add(CashbackPayout cashbackPayout);

        Task<CashbackPayout> Update(CashbackPayout cashbackPayout);

        Task<CashbackPayout> Get(int id);

        Task<CashbackPayout> GetByCustomerPartnerCurrency(int customerId, string currencyCode);

        //Task<List<CashbackPayout>> GetAllAsync();

        //Task<CashbackPayout> GetByMembershipCardId(int membershipCardId);

        Task<WithdrawalRequestModel> GetCashoutDataForRequest(string userId);
        Task<PagedResult<CashbackPayout>> GetCashbackPaidoutData(int statusId, int page, int pageSize, Enums.WithdrawalSortOrder sortOrder);

        PagedResult<FinancialReportSummary> GetAllPagedFinancialReport(DateTime startDate, DateTime endDate);
        PagedResult<FinancialReportSummary> GetPagedFinancialReportSearch(int statusId, DateTime? startDate, DateTime? endDate, int page, int pageSize);
    }
}
