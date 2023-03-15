using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class ClickTrackingManager : IClickTrackingManager
    {
        private readonly ExclusiveContext _ctx;

        public ClickTrackingManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ClickTracking> Add(ClickTracking clickTracking)
        {
            DbSet<ClickTracking> clickTrackings = _ctx.Set<ClickTracking>();
            clickTrackings.Add(clickTracking);
            await _ctx.SaveChangesAsync();

            return clickTracking;
        }

        public async Task<ClickTracking> Update(ClickTracking clickTracking)
        {
            DbSet<ClickTracking> clickTrackings = _ctx.Set<ClickTracking>();
            clickTrackings.Update(clickTracking);
            await _ctx.SaveChangesAsync();

            return clickTracking;
        }

        public ClickTracking Get(int id)
        {
            return _ctx.ClickTracking.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }
    }
}
