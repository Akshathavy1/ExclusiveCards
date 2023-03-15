using System.Collections.Generic;
using System.Linq;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Data.SqlClient;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class AffiliateMappingRuleManager : IAffiliateMappingRuleManager
    {
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public AffiliateMappingRuleManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<AffiliateMappingRule> Add(AffiliateMappingRule affiliateMappingRule)
        {
            try
            {
                DbSet<AffiliateMappingRule> affiliateMappingRules = _ctx.Set<AffiliateMappingRule>();
                affiliateMappingRules.Add(affiliateMappingRule);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return affiliateMappingRule;
        }

        public async Task<AffiliateMappingRule> Update(AffiliateMappingRule affiliateMappingRule)
        {
            try
            {
                DbSet<AffiliateMappingRule> affiliateMappingRules = _ctx.Set<AffiliateMappingRule>();
                affiliateMappingRules.Update(affiliateMappingRule);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return affiliateMappingRule;
        }

        public async Task<AffiliateMappingRule> Get(int id)
        {
            try
            {
                return await _ctx.AffiliateMappingRule.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return null;
        }

        public async Task<AffiliateMappingRule> GetByDesc(string desc, int? affiliateId = null)
        {
            try
            {
                IQueryable<AffiliateMappingRule> query = _ctx.AffiliateMappingRule
                    .Include(x => x.AffiliateMappings);
                if (affiliateId.HasValue)
                {
                    return await query.FirstOrDefaultAsync(x => x.Description == desc && x.AffiliateId == affiliateId);
                }
                return await query.FirstOrDefaultAsync(x => x.Description == desc);
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return null;
        }

        public async Task<List<AffiliateMappingRule>> GetAllMappingRules(string desc, int? affiliateId = null)
        {
            try
            {
                IQueryable<AffiliateMappingRule> query = _ctx.AffiliateMappingRule;
                //.Include(x => x.AffiliateMappings);
                if (affiliateId.HasValue)
                {
                    return await query.Where(x => x.Description == desc && x.AffiliateId == affiliateId).ToListAsync();
                }

                return await query.Where(x => x.Description == desc).ToListAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return null;
        }
    }
}
