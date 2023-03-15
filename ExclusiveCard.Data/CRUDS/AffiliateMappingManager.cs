using ExclusiveCard.Data.Context;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class AffiliateMappingManager : IAffiliateMappingManager
    {
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public AffiliateMappingManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<AffiliateMapping> Add(AffiliateMapping affiliateMapping)
        {
            try
            {
                DbSet<AffiliateMapping> affiliateMappings = _ctx.Set<AffiliateMapping>();
                affiliateMappings.Add(affiliateMapping);
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
            return affiliateMapping;
        }

        public async Task<AffiliateMapping> Update(AffiliateMapping affiliateMapping)
        {
            try
            {
                DbSet<AffiliateMapping> affiliateMappings = _ctx.Set<AffiliateMapping>();
                affiliateMappings.Update(affiliateMapping);
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
            return affiliateMapping;
        }

        public async Task<AffiliateMapping> Get(int id)
        {
            try
            {
                return await _ctx.AffiliateMapping.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<AffiliateMapping> GetAffiliateMapping(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj)
        {
            AffiliateMapping affiliateMapping = null;
            try
            {
                switch (matchTypeId)
                {
                    case AffiliateMatchTypes.Equals:
                        affiliateMapping = await _ctx.AffiliateMapping.FirstOrDefaultAsync(x =>
                        x.AffiliateMappingRuleId == affiliateMappingRuleId
                        && x.AffilateValue == affiliateValueObj.ToString()); //x.AffiliateFileId == affiliateFileId &&
                        break;
                    case AffiliateMatchTypes.LikeB:
                        affiliateMapping = await _ctx.AffiliateMapping.FirstOrDefaultAsync(x =>
                        x.AffiliateMappingRuleId == affiliateMappingRuleId
                        && affiliateValueObj.ToString().Contains(x.AffilateValue)); //x.AffiliateFileId == affiliateFileId &&
                        break;
                    case AffiliateMatchTypes.LikeA:
                        affiliateMapping = await _ctx.AffiliateMapping.FirstOrDefaultAsync(x =>
                        x.AffiliateMappingRuleId == affiliateMappingRuleId
                        && x.AffilateValue.Contains(affiliateValueObj.ToString())); //x.AffiliateFileId == affiliateFileId &&
                        break;
                    case AffiliateMatchTypes.StartsWith:
                        affiliateMapping = await _ctx.AffiliateMapping.FirstOrDefaultAsync(x =>
                        x.AffiliateMappingRuleId == affiliateMappingRuleId
                        && x.AffilateValue.StartsWith(affiliateValueObj.ToString())); //x.AffiliateFileId == affiliateFileId &&
                        break;
                    case AffiliateMatchTypes.EndsWith:
                        affiliateMapping = await _ctx.AffiliateMapping.FirstOrDefaultAsync(x =>
                        x.AffiliateMappingRuleId == affiliateMappingRuleId
                        && x.AffilateValue.EndsWith(affiliateValueObj.ToString())); //x.AffiliateFileId == affiliateFileId &&
                        break;
                    case AffiliateMatchTypes.Custom:

                        break;
                }
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
            return affiliateMapping;
        }

        public async Task<AffiliateMapping> GetByAffiliateValue(int affiliateMappingRuleId, string affiliateValue)
        {
            try
            {
                return await _ctx.AffiliateMapping.FirstOrDefaultAsync(x => x.AffiliateMappingRuleId == affiliateMappingRuleId && x.AffilateValue.ToLower() == affiliateValue.ToLower());
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

        public List<Type> GetEntityTypes()
        {
            return _ctx.Model.GetEntityTypes().Select(x => x.ClrType).ToList();
        }

        public async Task<List<AffiliateMapping>> GetAffiliateMappingList(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj)
        {
            List<AffiliateMapping> affiliateMappings = null;
            switch (matchTypeId)
            {
                case AffiliateMatchTypes.Equals:
                    affiliateMappings = await _ctx.AffiliateMapping.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue == affiliateValueObj.ToString()).ToListAsync(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.LikeB:
                    affiliateMappings = await _ctx.AffiliateMapping.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && affiliateValueObj.ToString().Contains(x.AffilateValue)).ToListAsync(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.LikeA:
                    affiliateMappings = await _ctx.AffiliateMapping.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.Contains(affiliateValueObj.ToString())).ToListAsync(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.StartsWith:
                    affiliateMappings = await _ctx.AffiliateMapping.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.StartsWith(affiliateValueObj.ToString())).ToListAsync(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.EndsWith:
                    affiliateMappings = await _ctx.AffiliateMapping.Where(x =>
                    x.AffiliateMappingRuleId == affiliateMappingRuleId
                    && x.AffilateValue.EndsWith(affiliateValueObj.ToString())).ToListAsync(); //x.AffiliateFileId == affiliateFileId &&
                    break;
                case AffiliateMatchTypes.Custom:

                    break;
            }
            return affiliateMappings;
        }

        public async Task<List<AffiliateMapping>> GetAll()
        {
            return await _ctx.AffiliateMapping.ToListAsync();
        }
    }
}
