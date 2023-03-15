using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class AffiliateManager : IAffiliateManager
    {
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public AffiliateManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<Affiliate> Add(Affiliate affiliate)
        {
            try
            {
                DbSet<Affiliate> affiliates = _ctx.Set<Affiliate>();
                affiliates.Add(affiliate);
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
            return affiliate;
        }

        public async Task<Affiliate> Update(Affiliate affiliate)
        {
            try
            {
                DbSet<Affiliate> affiliates = _ctx.Set<Affiliate>();
                affiliates.Update(affiliate);
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
            return affiliate;
        }

        public async Task<Affiliate> DeleteAsync(Affiliate affiliate)
        {
            try
            {
                DbSet<Affiliate> affiliates = _ctx.Set<Affiliate>();
                affiliates.Remove(affiliate);
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
            return affiliate;
        }

        public async Task<Affiliate> Get(int id)
        {
            try
            {
                return await _ctx.Affiliate.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<List<Affiliate>> GetAll()
        {
            try
            {
                return await _ctx.Affiliate.Include(x => x.AffiliateFiles).ToListAsync();
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
