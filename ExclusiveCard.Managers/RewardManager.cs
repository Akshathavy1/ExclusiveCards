using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AutoMapper;
using NLog;
using db = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Repositories;
using Microsoft.EntityFrameworkCore;


namespace ExclusiveCard.Managers
{
    /// <summary>
    /// The RewardManager looks after entitlement to and management of Rewards 
    /// Rewards are defined as benefits provided by a third party "Reward Partner" to customers.
    /// They can include Savings accounts (e.g. TAM), money off insurance renewals (e.g. an insurance company who haven't signed up yet), 
    /// loyalty card schemes (like a tesco club card) etc etc etc
    /// Reward transactions are tracked and paid out to individual membership cards but the display of balances, 
    /// transfer of rewards to/from reward partner is done at a Customer account level. 
    /// </summary>
    public class RewardManager : IRewardManager
    {
        #region Private fields and constructor

        private readonly IRepository<db.PartnerRewards> _rewardRepo;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RewardManager(IRepository<db.PartnerRewards> rewardRepo, IMapper mapper)
        {
            _rewardRepo = rewardRepo;
            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region public methods

        public int CreatePartnerReward(int partnerId, dto.Customer customer)
        {
            string key = GenerateUniqueKey(customer);

            db.PartnerRewards reward = new db.PartnerRewards
            {
                RewardKey = key,
                PartnerId = partnerId,
                CreatedDate = DateTime.UtcNow,
                LatestValue = 0,
                TotalConfirmedWithdrawn = 0
            };

            int retryCount = 10;
            bool success = false;

            // In case of rare clashes where two users with same initials attempt to create record in same second, 
            // allow 3 retries with seconds hacked before abandoning. 
            do
            {

                try
                {
                    _rewardRepo.Create(reward);
                    _rewardRepo.SaveChanges();
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex, "Insert Partner Reward failed");
                    reward.RewardKey = GenerateUniqueKey(customer, 70 - retryCount);
                    retryCount -= 1;                    
                }
            } while (success == false && retryCount > 0);
            if (!success)
                throw new Exception("Unable to create the partner reward after 10 retries");

            return reward.Id;

        }

        public dto.CustomerBalances GetRewardSummary(int partnerRewardId, dto.CustomerBalances rewardSummary = null)
        {
            var partnerReward = _rewardRepo.GetById(partnerRewardId);
            if (partnerReward != null)
            {
                if (rewardSummary == null)
                    rewardSummary = new dto.CustomerBalances();

                rewardSummary.LastUpdatedDate = partnerReward.ValueDate;
                rewardSummary.Withdrawn = partnerReward.TotalConfirmedWithdrawn;
                // If we have not got a last updated balance, calculate current value 
                if (rewardSummary.LastUpdatedDate == null)
                {
                    rewardSummary.CurrentValue = rewardSummary.ReceivedAmount + rewardSummary.Invested - rewardSummary.Withdrawn;
                }
                else
                    rewardSummary.CurrentValue = partnerReward.LatestValue;
                
                
            }

            return rewardSummary;
        }

        public dto.PartnerRewards GetPartnerRewards(int partnerRewardId)
        {
            dto.PartnerRewards partnerRewards = null;

            var dbPartnerRewards = _rewardRepo.GetById(partnerRewardId);
            if (dbPartnerRewards != null)
            {
                partnerRewards = _mapper.Map<dto.PartnerRewards>(dbPartnerRewards);
            }

            return partnerRewards;
        }

        #endregion

        #region private methods

        private string GenerateUniqueKey(dto.Customer customer, int seconds = -1)
        {
            DateTime current = DateTime.UtcNow;
            string month = current.Month.ToString().Length == 1?  $"0{current.Month.ToString()}" : current.Month.ToString();
            string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();
            if (seconds == -1)
                seconds = current.Second;
            
            // Generate reward key based on date time stamp and current user initials 
            string key = $"{current.Year}{month}{day}{current.Hour}{current.Minute}{seconds}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}";

            return key;

        }

        #endregion
    }
}
