using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class PartnerManager : IPartnerManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public PartnerManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }
        
        #endregion

        public async Task<Partner> Add(Partner partner)
        {
            DbSet<Partner> partners = _ctx.Set<Partner>();
            partners.Add(partner);
            await _ctx.SaveChangesAsync();

            return partner;
        }

        public async Task<Partner> Update(Partner partner)
        {
            DbSet<Partner> partners = _ctx.Set<Partner>();
            partners.Update(partner);
            await _ctx.SaveChangesAsync();

            return partner;
        }

        public async Task<Partner> GetByIdAsync(int id)
        {
            return await _ctx.Partner.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Partner> GetByNameAsync(string name)
        {
            return await _ctx.Partner.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Partner>> GetAllAsync(int? type)
        {
            IQueryable<Partner> query = _ctx.Partner;

            if (type.HasValue)
            {
                query = query.Where(x => x.Type == type);
            }

            return await query.ToListAsync();
        }
    }
}
