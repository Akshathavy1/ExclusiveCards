using ExclusiveCard.Data.Context;
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
    public class AffiliateFieldMappingManager : IAffiliateFieldMappingManager
    {
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;


        public AffiliateFieldMappingManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<AffiliateFieldMapping> Add(AffiliateFieldMapping affiliateFieldMapping)
        {
            try
            {
                DbSet<AffiliateFieldMapping> affiliateFieldMappings = _ctx.Set<AffiliateFieldMapping>();
                affiliateFieldMappings.Add(affiliateFieldMapping);
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
            return affiliateFieldMapping;
        }

        public async Task<AffiliateFieldMapping> Update(AffiliateFieldMapping affiliateFieldMapping)
        {
            try
            {
                DbSet<AffiliateFieldMapping> affiliateFieldMappings = _ctx.Set<AffiliateFieldMapping>();
                affiliateFieldMappings.Update(affiliateFieldMapping);
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
            return affiliateFieldMapping;
        }

        public async Task<AffiliateFieldMapping> Get(int id)
        {
            try
            {
                return await _ctx.AffiliateFieldMapping.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<List<AffiliateFieldMapping>> GetAll(int affiliateFileMappingid)
        {
            try
            {
                return await _ctx.AffiliateFieldMapping.Where(x => x.AffiliateFileMappingId == affiliateFileMappingid).ToListAsync();
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
