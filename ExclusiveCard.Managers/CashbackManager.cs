using AutoMapper;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ExclusiveCard.Enums;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    /// <summary>
    /// The CashbackManager takes care of cashback payments made by affiliates,
    /// and the cashback balances held by customers. 
    /// This includes management of the CashbackTransactions, CashbackSummary and CashbackPayout  tables
    /// </summary>
    public class CashbackManager : ICashbackManager
    {
        #region Constructor and private fields

        private const string DEFAULT_CURRENCY_CODE = "GBP";
        private const string ACCOUNT_BOOST_DETAIL = "Investment";
        private const string ACCOUNT_BOOST_SUMMARY = "Account Boost Deposit";

        private readonly IRepository<CashbackTransaction> _cashbackRepo;
        private readonly IRepository<CashbackSummary> _cashbackSummaryRepo;
        private readonly IRepository<MembershipCard> _cardRepo;
        private readonly IMapper _mapper;

        public CashbackManager(IRepository<CashbackTransaction> cashbackRepo, IRepository<CashbackSummary> cashbacksummaryRepo, 
                               IRepository<MembershipCard> cardRepo, IMapper mapper)
        {
            _cashbackRepo = cashbackRepo;
            _cashbackSummaryRepo = cashbacksummaryRepo;
            _cardRepo = cardRepo;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        #region Cashback Summaries and Balances

        /// <summary>
        /// Create the cashback summary records that are required for each new customer account
        /// The plantype will determine which combination of (R)eward, (D)eduction and (C)ashback account types needed
        /// </summary>
        /// <param name="membershipCardId">Card to add the summaries too - each memnership card gets their own set</param>
        /// <param name="planType">Type of membership Plan</param>
        public void CreateSummaryRecords(int membershipCardId, MembershipPlanTypeEnum planType)
        {
            switch (planType)
            {
                case MembershipPlanTypeEnum.PartnerReward:
                    CreatePartnerRewardSummaryRecords(membershipCardId);
                    break;

                case MembershipPlanTypeEnum.CustomerCashback:
                    CreateCashbackSummaryRecords(membershipCardId);
                    break;

                case MembershipPlanTypeEnum.Donation:
                    CreateDonationSummaryRecords(membershipCardId);
                    break;


                case MembershipPlanTypeEnum.BenefitRewards:
                    CreatePartnerSummaryRecords(membershipCardId);
                    break;

                default:
                    throw new Exception("Unable to create cashback summary records - plan type not supported");

            }

            _cashbackSummaryRepo.SaveChanges();
        }

        //ReceivedAmount: This is only the total received ever, it does not include cashback withdrawn or invested
        public DTOs.CustomerBalances GetCustomerBalances(int customerId, char accountType = 'R', string currencyCode = DEFAULT_CURRENCY_CODE)
        {
            DTOs.CustomerBalances dtoSummary = null;

            var cards = _cardRepo.Include(c => c.CashbackSummaries).Where(x => x.CustomerId == customerId);
            if (cards != null)
            {
                dtoSummary = new DTOs.CustomerBalances() { PendingAmount = 0, ConfirmedAmount = 0, ReceivedAmount = 0, Invested = 0 };
                foreach(var card in cards)
                {
                    var summary = card.CashbackSummaries.FirstOrDefault(a => a.AccountType == accountType && a.CurrencyCode == currencyCode);
                    
                    if (summary != null)
                    {
                        dtoSummary.PendingAmount += (decimal)summary.PendingAmount;
                        dtoSummary.ConfirmedAmount += (decimal)summary.ConfirmedAmount;
                        dtoSummary.ReceivedAmount += (decimal)summary.ReceivedAmount;
                        dtoSummary.Invested += (decimal)summary.PaidAmount;
                    }
                }
                
            }

            return dtoSummary;
        }

        /// <summary>
        /// Updates the cashback summary record with the totals supplied
        /// The values are added onto the existing totals (they do not just replace them)
        /// Leave any value null to keep the existing total as is
        /// </summary>
        /// <param name="membershipCardId"></param>
        /// <param name="accountType"></param>
        /// <param name="pendingAmount"></param>
        /// <param name="confirmedAmount"></param>
        /// <param name="receivedAmount"></param>
        public void AddToCustomerBalance(int membershipCardId, char accountType = 'R', decimal? pendingAmount = null, decimal? confirmedAmount = null, decimal? receivedAmount = null, decimal? paidAmount = null, string currencyCode = DEFAULT_CURRENCY_CODE)
        {
            var summary = _cashbackSummaryRepo.Get(x => x.MembershipCardId == membershipCardId && x.AccountType == accountType && x.CurrencyCode == currencyCode);
            if (summary != null)
            {
                summary.PendingAmount = pendingAmount == null ? summary.PendingAmount : summary.PendingAmount + (decimal)pendingAmount;
                summary.ConfirmedAmount = confirmedAmount == null ? summary.ConfirmedAmount : summary.ConfirmedAmount + (decimal)confirmedAmount;
                summary.ReceivedAmount = receivedAmount == null ? summary.ReceivedAmount : summary.ReceivedAmount + (decimal)receivedAmount;
                summary.PaidAmount = paidAmount == null ? summary.PaidAmount : summary.PaidAmount + (decimal)paidAmount;

                _cashbackSummaryRepo.Update(summary);
                _cashbackSummaryRepo.SaveChanges();
            }
            else
            {
                summary = new CashbackSummary()
                {
                    MembershipCardId = membershipCardId,
                    AccountType = accountType,
                    CurrencyCode = currencyCode,

                    PendingAmount = pendingAmount == null ? 0 : (decimal)pendingAmount,
                    ConfirmedAmount = confirmedAmount == null ? 0 : (decimal)confirmedAmount,
                    ReceivedAmount = receivedAmount == null ? 0 : (decimal)receivedAmount,
                    PaidAmount = paidAmount == null ? 0 : (decimal)paidAmount
                };
                _cashbackSummaryRepo.Create(summary);
                _cashbackSummaryRepo.SaveChanges();
            }
        }

        #endregion

        #region Cashback Transactions

        /// <summary>
        /// Creates cashback transactions from a source transaction
        /// As of writing the only source for this is for new trasactions arriving on the Strackr Cashback Report 
        /// but other sources can be added as required. This method does not care where the source is. 
        /// This method assumes the source transaction's cashback amount is the full amount of cashback on an original affiliate transaction
        /// Dependent on the type of membership plan, this amount will be split across multiple new transactions (e.g. R and D type), based on 
        /// the % values held in the plan. For Partner Rewards (type 4) , two transactions will be created from the source.
        /// Other plan types could be included in the future, but as of May 2020  obsolete plan types 1, 2 and 3 are no longer supported
        /// </summary>
        /// <param name="cashbackTran">The source transaction, from which to create the actual Cashback transaction(s)</param>
        /// <param name="plan">The membership plan that dictates the number, type and value of cashback txns created</param>
        public void CreateCashbackTransactions(DTOs.CashbackTransaction cashbackTran, DTOs.MembershipPlan plan)
        {
            int planId = plan?.MembershipPlanTypeId ?? 0;
            if (planId != (int)MembershipPlanTypeEnum.PartnerReward && planId != (int)MembershipPlanTypeEnum.BenefitRewards)
                throw new Exception("Unable to create new cashback transactions - plan type not supported. Plan type = " + planId.ToString() +  ", affilliate Ref = " + cashbackTran.AffiliateTransactionReference);

            var amounts = GetCashbackValuesForPlan(cashbackTran, plan);
            decimal amountR = amounts.Item1;
            decimal amountD = amounts.Item2;
            decimal amountB = amounts.Item3; //BenefactorPercentage

            // Update the Cashback summary balances

            if (amountR != 0)
            {
                // Create the R(eward) transaction for customer
                var txnR = _mapper.Map<CashbackTransaction>(cashbackTran);
                txnR.CashbackAmount = amountR;
                txnR.AccountType = 'R';
                _cashbackRepo.Create(txnR);

                UpdateCashbackSummaryValues(cashbackTran.MembershipCardId, (Cashback)cashbackTran.StatusId, amountR, 'R', cashbackTran.CurrencyCode);
            }

            if (amountD != 0)
            {
                // Create the D(eduction) transaction 
                var txnD = _mapper.Map<CashbackTransaction>(cashbackTran);
                txnD.CashbackAmount = amountD;
                txnD.AccountType = 'D';
                _cashbackRepo.Create(txnD);

                UpdateCashbackSummaryValues(cashbackTran.MembershipCardId, (Cashback)cashbackTran.StatusId, amountD, 'D', cashbackTran.CurrencyCode);
            }

            if (amountB != 0)
            {
                //Create the B(enefactor) transaction
                var txnB = _mapper.Map<CashbackTransaction>(cashbackTran);
                txnB.CashbackAmount = amountB;
                txnB.AccountType = 'B';
                _cashbackRepo.Create(txnB);
                _cashbackRepo.SaveChanges();

                // Update the Cashback summary balance
                UpdateCashbackSummaryValues(cashbackTran.MembershipCardId, (Cashback)cashbackTran.StatusId, amountB, 'B', cashbackTran.CurrencyCode);
            }

        }

        public Tuple<decimal, decimal, decimal> GetCashbackValuesForPlan(DTOs.CashbackTransaction cashbackTran, DTOs.MembershipPlan plan)
        {
            // Work out the % split
            decimal amountR = Math.Round(cashbackTran.CashbackAmount * Convert.ToDecimal(plan.CustomerCashbackPercentage / 100), 2);
            decimal amountD = Math.Round(cashbackTran.CashbackAmount * Convert.ToDecimal(plan.DeductionPercentage / 100), 2);
            decimal amountB = Math.Round(cashbackTran.CashbackAmount * Convert.ToDecimal(plan.BenefactorPercentage / 100), 2);

            // Look out for lost pennies
            if (amountR + amountD + amountB != cashbackTran.CashbackAmount)
            {
                // if there is a penny left, give it to customer (don't tell Neil)
                if (cashbackTran.CashbackAmount - amountR - amountD -amountB == 0.01M)
                    amountR += 0.01M;

                // if there is a penny missing, take it off the Deduction amount
                else if (cashbackTran.CashbackAmount - amountR - amountD - amountB == -0.01M)
                    amountD -= 0.01M;

                // Check again and if that still isn't right, throw an error
                if (amountR + amountD + amountB != cashbackTran.CashbackAmount)
                    throw new Exception("Unable to create new cashback transactions - the % splits do not equal the source amount. AffiliateRef = " + cashbackTran.AffiliateTransactionReference);
            }

            return new Tuple<decimal, decimal, decimal>(amountR, amountD, amountB);
        }

        public async Task<List<DTOs.CashbackTransaction>> GetCashbackTransactionsByAffiliateRefAsync(string affiliateRef)
        {
            List<DTOs.CashbackTransaction> txns = null;

            var dbTxns =  await _cashbackRepo.FilterNoTrackAsync(x => x.AffiliateTransactionReference == affiliateRef);
            if (dbTxns != null)
            {
                txns = _mapper.Map<List<DTOs.CashbackTransaction>>(dbTxns.ToList());
            }

            return txns;
        }


        public void UpdateCashbackTransactionStatus(int txnId, Cashback status, decimal cashbackAmount,  DateTime txnDate, DateTime? receivedDate,  DateTime? paymentDate, DateTime? confirmedDate )
        {
            var dbTxn = _cashbackRepo.GetById(txnId);
            var oldStatus = (Cashback)dbTxn.StatusId;

            decimal pending = 0;
            decimal confirmed = 0;
            decimal received = 0;

            // Update cashback summary dependent on status change
            if (status != oldStatus)
            {
                if (oldStatus == Cashback.Received)
                    throw new Exception("Unable to process cashback txn update, attempt to revert Received payment to " + status.ToString());

                else if (oldStatus == Cashback.Pending)
                {
                    pending -= dbTxn.CashbackAmount;
                    switch (status)
                    {
                        case Cashback.Confirmed:
                            confirmed += cashbackAmount;
                            break;
                        case Cashback.Received:
                            received += cashbackAmount;
                            break;
                    }
                }
                else if (oldStatus == Cashback.Confirmed)
                {
                    confirmed -= dbTxn.CashbackAmount;
                    switch (status)
                    {
                        case Cashback.Pending:
                            pending += cashbackAmount;
                            break;
                        case Cashback.Received:
                            received += cashbackAmount;
                            break;
                    }
                }

                // Update the cashback summaries
                AddToCustomerBalance(dbTxn.MembershipCardId, dbTxn.AccountType, pendingAmount: pending, confirmedAmount: confirmed, receivedAmount: received, currencyCode: dbTxn.CurrencyCode);
            }

            dbTxn.StatusId = (int)status;
            dbTxn.CashbackAmount = cashbackAmount;
            dbTxn.TransactionDate = txnDate;
            dbTxn.ExpectedPaymentDate = paymentDate;
            dbTxn.DateReceived = receivedDate;
            dbTxn.DateConfirmed = confirmedDate;
            _cashbackRepo.Update(dbTxn);
            _cashbackRepo.SaveChanges();
            
        }
        #endregion


        public void AddAccountBoost(int membershipCardId, decimal transactionAmount, DateTime transactionDate, string currencyCode)
        {
            // find the membership card
            var card = _cardRepo.Include(c => c.MembershipPlan).FirstOrDefault(x => x.Id == membershipCardId && x.IsActive && !x.IsDeleted && x.StatusId == (int)MembershipCardStatus.Active);
            if (card == null)
                throw new Exception("AddAccountBoost Failed - unable to find an active membership card, membershipcardId = " + membershipCardId.ToString());

            var plan = card.MembershipPlan;
            if (plan == null)
                throw new Exception("AddAccountBoost failed - unable to find a membership plan");

            // Calculate values of the cashback 
            decimal cashbackAmount = transactionAmount - plan.PaymentFee;

            // Get Plan Type
            char accountType = 'C';
            switch (plan.MembershipPlanTypeId)
            {
                case (int)MembershipPlanTypeEnum.CustomerCashback:
                case (int)MembershipPlanTypeEnum.Donation:
                    accountType = 'C';
                    break;

                case (int)MembershipPlanTypeEnum.BenefitRewards:
                case (int)MembershipPlanTypeEnum.PartnerReward:
                    accountType = 'R';
                    break;
            }

            DateTime dateReceived = DateTime.UtcNow;

            // Create cashback txn
            CreateAccountBoostTransaction(accountType, cashbackAmount, membershipCardId, transactionAmount, transactionDate, currencyCode, dateReceived, plan.PartnerId);
            // Create deduction txn
            CreateAccountBoostTransaction('D', plan.PaymentFee, membershipCardId, transactionAmount, transactionDate, currencyCode, dateReceived, plan.PartnerId);

        }


        #endregion

        #region  Private Methods

        private void CreateAccountBoostTransaction(char accountType, decimal cashbackAmount, int membershipCardId, decimal transactionAmount, DateTime transactionDate, string currencyCode, DateTime dateReceived, int? partnerId)
        {
            var cashbackTrans = new CashbackTransaction()
            {
                MembershipCardId = membershipCardId,
                PartnerId = partnerId,
                AccountType = accountType,
                TransactionDate = transactionDate,
                PurchaseAmount = transactionAmount,
                CurrencyCode = currencyCode,
                Summary = ACCOUNT_BOOST_SUMMARY,
                Detail = ACCOUNT_BOOST_DETAIL,
                StatusId = (int)Cashback.UserPaid,
                DateReceived = dateReceived,
                CashbackAmount = cashbackAmount
            };
            _cashbackRepo.Create(cashbackTrans);

            AddToCustomerBalance(membershipCardId, accountType, paidAmount: cashbackAmount);
        }

        private void CreatePartnerRewardSummaryRecords(int membershipCardId)
        {
            CreateRewardSummary(membershipCardId);

            CreateDeductionSummary(membershipCardId);
        }

        private void CreateCashbackSummaryRecords(int membershipCardId)
        {
            CreateCashbackSummary(membershipCardId);
        }

        private void CreatePartnerSummaryRecords(int membershipCardId)
        {
            CreateRewardSummary(membershipCardId);
            CreateBenefitRewardsSummary(membershipCardId);
        }

        private void CreateDonationSummaryRecords(int membershipCardId)
        {
            CreateCashbackSummary(membershipCardId);

            CreateDeductionSummary(membershipCardId);
        }

        private CashbackSummary CreateSummaryRecord(int membershipCardId)
        {
            var summary = new CashbackSummary()
            {
                MembershipCardId = membershipCardId,
                CurrencyCode = "GBP", // No other currency supported yet
            };

            return summary;
        }

        private void CreateRewardSummary(int membershipCardId)
        {
            // create reward account
            var rewardSummary = CreateSummaryRecord(membershipCardId);
            rewardSummary.AccountType = 'R';
            _cashbackSummaryRepo.Create(rewardSummary);
        }

        private void CreateDeductionSummary(int membershipCardId)
        {
            var rewardSummary = CreateSummaryRecord(membershipCardId);
            rewardSummary.AccountType = 'D';
            _cashbackSummaryRepo.Create(rewardSummary);
        }

        private void CreateCashbackSummary(int membershipCardId)
        {
            // create reward account
            var rewardSummary = CreateSummaryRecord(membershipCardId);
            rewardSummary.AccountType = 'C';
            _cashbackSummaryRepo.Create(rewardSummary);
        }

        private void CreateBenefitRewardsSummary(int membershipCardId)
        {
            // create reward account
            var BenefitRewardsSummary = CreateSummaryRecord(membershipCardId);
            BenefitRewardsSummary.AccountType = 'B';
            _cashbackSummaryRepo.Create(BenefitRewardsSummary);
        }


        private void UpdateCashbackSummaryValues(int cardId, Cashback status, decimal amount, char accountType, string currencyCode)
        {
            switch (status)
            {
                case Cashback.Pending:
                    AddToCustomerBalance(cardId, accountType, pendingAmount: amount);
                    break;

                case Cashback.Confirmed:
                    AddToCustomerBalance(cardId, accountType, confirmedAmount: amount);
                    break;

                case Cashback.Received:
                    AddToCustomerBalance(cardId, accountType, receivedAmount: amount);
                    break;

                case Cashback.UserPaid:
                    AddToCustomerBalance(cardId, accountType, paidAmount: amount);
                    break;

                default:  // do nothing if status not recognised
                    break;

            }
        }

        #endregion
    }
}
