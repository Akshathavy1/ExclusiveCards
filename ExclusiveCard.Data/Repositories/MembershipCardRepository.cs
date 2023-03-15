using ExclusiveCard.Data.Context;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Data.Repositories
{
    public class MembershipCardRepository : Repository<MembershipCard>, IMembershipCardRepository
    {
        private ExclusiveContext _exclusiveContext;

        public MembershipCardRepository(ExclusiveContext context)
            : base(context)
        {
            _exclusiveContext = context;
        }

        /// <summary>
        /// This method creates a Membership Card record AND the associated CashbackSummary records it also requires
        /// Creating the cashback summary records here avoids having to hope the services remember to always do this 
        /// after creating a card.  
        /// </summary>
        /// <param name="card">The membership card entity to add</param>
        /// <param name="planType">An enum value for the type of plan. DIfferent plan types need different combinations of CashbackSummary records</param>
        public void CreateMembershipCard(MembershipCard card, MembershipPlanTypeEnum planType)
        {
            // first, save the membership card using base repo
            base.Create(card);

            // Now we need to add cashback summary records,
            switch (planType)
            {
                case MembershipPlanTypeEnum.PartnerReward:
                    CreatePartnerRewardSummaryRecords(card.Id);
                    break;

                case MembershipPlanTypeEnum.CustomerCashback:
                    CreateCashbackSummaryRecords(card.Id);
                    break;

                case MembershipPlanTypeEnum.Donation:
                    CreateDonationSummaryRecords(card.Id);
                    break;


                case MembershipPlanTypeEnum.BenefitRewards:
                    CreatePartnerSummaryRecords(card.Id);
                    break;

                default:
                    throw new Exception("Unable to create cashback summary records - plan type not supported");

            }
        }

        public void CreateCashBackTransactions(CashbackTransaction cashBackTransaction)
        {
            try
            {
                DbSet<CashbackTransaction> req = _exclusiveContext.Set<CashbackTransaction>();
                req.Add(cashBackTransaction);
                _exclusiveContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
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

            CreateDeductionSummary(membershipCardId);

            CreateBenefitSummary(membershipCardId);
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
            _exclusiveContext.CashbackSummary.Add(rewardSummary);
            
        }

        private void CreateDeductionSummary(int membershipCardId)
        {
            var rewardSummary = CreateSummaryRecord(membershipCardId);
            rewardSummary.AccountType = 'D';
            _exclusiveContext.CashbackSummary.Add(rewardSummary);
        }

        private void CreateCashbackSummary(int membershipCardId)
        {
            // create reward account
            var rewardSummary = CreateSummaryRecord(membershipCardId);
            rewardSummary.AccountType = 'C';
            _exclusiveContext.CashbackSummary.Add(rewardSummary);
        }

        private void CreateBenefitSummary(int membershipCardId)
        {
            // create benefit reward account
            var rewardSummary = CreateSummaryRecord(membershipCardId);
            rewardSummary.AccountType = 'B';
            _exclusiveContext.CashbackSummary.Add(rewardSummary);
        }

    }
}
