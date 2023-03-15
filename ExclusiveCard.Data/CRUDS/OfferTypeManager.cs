using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferTypeManager : IOfferTypeManager
    {
        private readonly ExclusiveContext _ctx;

        public OfferTypeManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<OfferType> Add(OfferType offerType)
        {
            DbSet<OfferType> offerTypes = _ctx.Set<OfferType>();
            offerTypes.Add(offerType);
            await _ctx.SaveChangesAsync();
            return offerType;
        }

        public async Task<OfferType> Update(OfferType offerType)
        {
            DbSet<OfferType> offerTypes = _ctx.Set<OfferType>();
            offerTypes.Update(offerType);
            await _ctx.SaveChangesAsync();
            return offerType;
        }

        public async Task<OfferType> Get(string offerTypeName)
        {
            return await _ctx.OfferType.FirstOrDefaultAsync(x => x.IsActive && x.Description == offerTypeName);
        }

        public async Task<List<OfferType>> GetAll()
        {
            return await _ctx.OfferType
                .Where(x => x.IsActive).OrderBy(x => x.Description).ToListAsync();
        }
    }
}
