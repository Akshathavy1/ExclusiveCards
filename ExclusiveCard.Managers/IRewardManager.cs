using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Managers
{
    public interface IRewardManager
    {
        int CreatePartnerReward(int partnerId, Customer customer);

        CustomerBalances GetRewardSummary(int partnerRewardId, CustomerBalances  rewardSummary = null);

        PartnerRewards GetPartnerRewards(int partnerRewardId);
    }
}
