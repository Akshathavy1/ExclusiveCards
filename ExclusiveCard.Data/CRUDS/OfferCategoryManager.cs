using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferCategoryManager : IOfferCategoryManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public OfferCategoryManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }
        
        #endregion

        public async Task<OfferCategory> Add(OfferCategory offerCategory)
        {
            DbSet<OfferCategory> offerCategories = _ctx.Set<OfferCategory>();
            offerCategories.Add(offerCategory);
            await _ctx.SaveChangesAsync();
            return offerCategory;
        }

        public async Task<OfferCategory> Update(OfferCategory offerCategory)
        {
            DbSet<OfferCategory> offerCategories = _ctx.Set<OfferCategory>();
            offerCategories.Update(offerCategory);
            await _ctx.SaveChangesAsync();

            return offerCategory;
        }

        public async Task Delete(int offerId)
        {
            var deleteOfferCategory = false;
            while (!deleteOfferCategory)
            {
                try
                {
                    List<OfferCategory> offerCategories = await _ctx.OfferCategory.Where(x => x.OfferId == offerId).ToListAsync();
                    DbSet<OfferCategory> offerCategorySet = _ctx.Set<OfferCategory>();
                    offerCategorySet.RemoveRange(offerCategories);
                    await _ctx.SaveChangesAsync();
                    deleteOfferCategory = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                }
            }
        }

        public async Task<List<OfferCategory>> GetAll(int offerId)
        {
            List<OfferCategory> offerCategories;
            IQueryable<OfferCategory> offerCategoriesQuery = _ctx.OfferCategory;
            offerCategories = await offerCategoriesQuery.Where(x => x.OfferId == offerId).ToListAsync();
            return offerCategories;
        }
    }
}
