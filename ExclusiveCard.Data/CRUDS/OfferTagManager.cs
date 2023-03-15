using Castle.Core.Logging;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferTagManager : IOfferTagManager
    {
        private readonly IRepository<OfferTag> _offerTagRepo;
        private readonly ExclusiveContext _ctx;
       
        public OfferTagManager(IRepository<OfferTag> offerTagRepo,ExclusiveContext ctx)
        {
            _offerTagRepo = offerTagRepo;
            _ctx = ctx;            
        }

        public async Task<OfferTag> Add(OfferTag offerTag)
        {
            _offerTagRepo.Create(offerTag);
            _offerTagRepo.SaveChanges();

            await Task.CompletedTask;

            return offerTag;            
        }

        public async Task Delete(int offerId)
        {
            List<OfferTag> offerTags = await _ctx.OfferTag.Where(x => x.OfferId == offerId).AsNoTracking().ToListAsync();
            DbSet<OfferTag> offerTagsSet = _ctx.Set<OfferTag>();
            offerTagsSet.RemoveRange(offerTags);
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<OfferTag>> GetAll(int id)
        {
            return await _ctx.OfferTag.Include(x => x.Tag).Where(x => x.OfferId == id).AsNoTracking().ToListAsync();
        }
    }
}
