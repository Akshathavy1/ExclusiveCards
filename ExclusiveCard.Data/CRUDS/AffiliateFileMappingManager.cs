using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    [Obsolete("Replaced by ExclusiveCard.Managers.AffiliateManager")]
   public class AffiliateFileMappingManager : IAffiliateFileMappingManager
    {
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public AffiliateFileMappingManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<AffiliateFileMapping> Add(AffiliateFileMapping affiliateFileMapping)
        {
            try
            {
                DbSet<AffiliateFileMapping> affiliateFileMappings = _ctx.Set<AffiliateFileMapping>();
                affiliateFileMappings.Add(affiliateFileMapping);
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
            return affiliateFileMapping;
        }

        public async Task<AffiliateFileMapping> Update(AffiliateFileMapping affiliateFileMapping)
        {
            try
            {
                DbSet<AffiliateFileMapping> affiliateFileMappings = _ctx.Set<AffiliateFileMapping>();
                affiliateFileMappings.Update(affiliateFileMapping);
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
            return affiliateFileMapping;
        }

        //public async Task<AffiliateFieldMapping> Get(int id)
        //{
        //    try
        //    {
        //        // Removed becuase of compile error after fixing the data entity definition in ExclusiveContext.
        //        return null;  // await _ctx.AffiliateFileMapping.FirstOrDefaultAsync(x => x.Id == id);
        //    }
        //    catch (DbUpdateException e)
        //    {
        //        _logger.Error(e);
        //    }
        //    catch (SqlException e)
        //    {
        //        _logger.Error(e);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error(e);
        //    }
        //    finally
        //    {
        //        _ctx.Dispose();
        //    }
        //    return null;
        //}
    }
}
