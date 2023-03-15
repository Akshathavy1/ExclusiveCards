using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class PartnerRewardManager : IPartnerRewardManager
    {
        #region Private members and constructor

        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public PartnerRewardManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Writes

        public async Task<PartnerRewards> AddAsync(PartnerRewards reward)
        {
            try
            {
                DbSet<PartnerRewards> rewards = _ctx.Set<PartnerRewards>();
                rewards.Add(reward);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return reward;
        }

        public async Task<PartnerRewards> UpdateAsync(PartnerRewards reward)
        {
            try
            {
                DbSet<PartnerRewards> rewards = _ctx.Set<PartnerRewards>();
                rewards.Update(reward);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return reward;
        }

        #endregion

        #region Reads

        public async Task<PartnerRewards> GetByIdAsync(int id)
        {
            return await _ctx.PartnerRewards.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PartnerRewards> GetByRewardKey(string key)
        {
            return await _ctx.PartnerRewards
                .Include(x => x.Partner)
                .Include(x => x.MembershipCards)
                .ThenInclude(y => y.Customer)
                .FirstOrDefaultAsync(x => x.RewardKey == key);
        }

        public async Task<List<PartnerRewards>> GetAllAsync()
        {
            return await _ctx.PartnerRewards.ToListAsync();
        }

        #endregion
    }
}
