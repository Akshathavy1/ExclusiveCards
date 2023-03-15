using DTOs = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public interface ICashbackManager
    {

        #region Cashback summaries and balances

        void CreateSummaryRecords(int membershipCardId, MembershipPlanTypeEnum planType);

        DTOs.CustomerBalances GetCustomerBalances(int customerId, char accountType = 'R', string currencyCode = "GBP");

        void AddToCustomerBalance(int membershipCardId, char AccountType = 'R', decimal? pendingAmount = null, decimal? confirmedAmount = null, decimal? receivedAmount = null, decimal? paidAmount = null, string currencyCode = "GBP");

        #endregion

        #region Cashback Transactions

        void CreateCashbackTransactions(DTOs.CashbackTransaction cashbackTran, DTOs.MembershipPlan plan);

        Task<List<DTOs.CashbackTransaction>> GetCashbackTransactionsByAffiliateRefAsync(string affiliateRef);

        Tuple<decimal, decimal, decimal> GetCashbackValuesForPlan(DTOs.CashbackTransaction cashbackTran, DTOs.MembershipPlan plan);

        void UpdateCashbackTransactionStatus(int txnId, Cashback status, decimal cashbackAmount, DateTime txnDate, DateTime? receivedDate, DateTime? paymentDate, DateTime? confirmedDate);

        #endregion
        void AddAccountBoost(int membershipCardId, decimal transactionAmount, DateTime transactionDate, string currencyCode);

    }
}
