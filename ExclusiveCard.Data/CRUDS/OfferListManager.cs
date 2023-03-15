using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferListManager : IOfferListManager
    {
        private readonly ExclusiveContext _ctx;

        public OfferListManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<OfferList> Add(OfferList offerList)
        {
            DbSet<OfferList> offerLists = _ctx.Set<OfferList>();
            offerLists.Add(offerList);
            await _ctx.SaveChangesAsync();
            return offerList;
        }
        
        public async Task<OfferList> Update(OfferList offerList)
        {
            DbSet<OfferList> offerLists = _ctx.Set<OfferList>();
            offerLists.Update(offerList);
            await _ctx.SaveChangesAsync();
            return offerList;
        }

        public async Task<OfferList> Get(string listName, string countryCode)
        {
            IQueryable<OfferList> offerListQuery = _ctx.OfferList
                .Include(a => a.OfferListItems.Where(b =>
                    b.DisplayFrom <= DateTime.UtcNow && b.DisplayTo >= DateTime.UtcNow &&
                    !b.ExcludedCountries.Contains(countryCode) && 
                    (b.Offer.Validindefinately || (b.Offer.ValidFrom != null && b.Offer.ValidTo != null && b.Offer.ValidFrom <= DateTime.UtcNow 
                    && b.Offer.ValidTo >= DateTime.UtcNow)) && b.Offer.OfferCountries.All(c => c.IsActive && c.CountryCode == countryCode)))
                    .ThenInclude(d => d.Offer).ThenInclude(e => e.Merchant);

            var offerList = await offerListQuery.FirstOrDefaultAsync(x => x.IsActive && x.ListName == listName);
            return offerList;
        }

        public async Task<List<OfferList>> GetAll(string countryCode)
        {
            //IQueryable<OfferList> offerListQuery = ctx.OfferList;
                /*.Include(a => a.OfferListItems.Where(b =>
                    b.DispalyFrom <= DateTime.UtcNow && b.DispalyTo >= DateTime.UtcNow &&
                    !b.ExcludedCountries.Contains(countryCode)
                     && (b.Offer.Validindefinately || (b.Offer.ValidFrom != null && b.Offer.ValidTo != null && b.Offer.ValidFrom <= DateTime.UtcNow
                                                   && b.Offer.ValidTo >= DateTime.UtcNow)) && b.Offer.OfferCountries.All(c => c.IsActive && c.CountryCode == countryCode)));
                .ThenInclude(d => d.Offer).ThenInclude(e => e.Merchant).ThenInclude(f => f.MerchantImages.FirstOrDefault());*/

            return await _ctx.OfferList.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<List<OfferList>> GetAll()
        {
            return await _ctx.OfferList.Where(x => x.IsActive).ToListAsync();
        }
        
        public async Task<OfferList> GetByName(string listName)
        {
            IQueryable<OfferList> offerListQuery =
                _ctx.OfferList.Where(x => x.ListName.Trim().ToLower() == listName.Trim().ToLower());

            var offerList = await offerListQuery.FirstOrDefaultAsync(x => x.IsActive);
            return offerList;
        }

        
    }
}
