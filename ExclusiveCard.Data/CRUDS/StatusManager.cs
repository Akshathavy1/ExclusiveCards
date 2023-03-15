using System;
using ExclusiveCard.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Constants;
using Microsoft.Extensions.Caching.Memory;
using Status = ExclusiveCard.Data.Models.Status;

namespace ExclusiveCard.Data.CRUDS
{
    public class StatusManager : IStatusManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;
        private readonly IMemoryCache _cache;

        #endregion

        #region Constructor

        public StatusManager(ExclusiveContext ctx, IMemoryCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }

        #endregion

        public async Task<Status> Add(Status status)
        {
            DbSet<Status> statusesSet = _ctx.Set<Status>();
            statusesSet.Add(status);
            await _ctx.SaveChangesAsync();

            return status;
        }

        public async Task<List<Status>> AddRange(List<Status> status)
        {
            DbSet<Status> statusesSet = _ctx.Set<Status>();
            statusesSet.AddRange(status);
            await _ctx.SaveChangesAsync();

            return status;
        }

        public async Task<List<Status>> GetAll(string type)
        {
            List<Status> status = _cache.Get<List<Status>>(Keys.Status);
            if (status == null)
            {
                IQueryable<Status> statusQuery = _ctx.Status;
                statusQuery = statusQuery.Where(x => x.IsActive);

                status = await statusQuery.OrderBy(x => x.Name).ToListAsync();
                _cache.Set(Keys.Status, status, DateTimeOffset.UtcNow.AddHours(24));
            }
            if (type != null)
            {
                status = status?.Where(x => x.Type == type).ToList();
            }

            return status;
        }

        public async Task DeleteRangeAsync()
        {
            DbSet<Status> status = _ctx.Set<Status>();
            List<Status> stats = await status.ToListAsync();
            status.RemoveRange(stats);
            await _ctx.SaveChangesAsync();
        }
    }
}
