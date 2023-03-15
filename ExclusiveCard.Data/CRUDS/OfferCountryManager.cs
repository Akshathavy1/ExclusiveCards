using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ExclusiveCard.Data.CRUDS
{
    public class OfferCountryManager : IOfferCountryManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public OfferCountryManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task<OfferCountry> Add(OfferCountry offerCountry)
        {
            DbSet<OfferCountry> offerCountries = _ctx.Set<OfferCountry>();
            offerCountries.Add(offerCountry);
            await _ctx.SaveChangesAsync();

            return offerCountry;
        }

        public async Task<OfferCountry> Update(OfferCountry offerCountry)
        {
            DbSet<OfferCountry> offerCountries = _ctx.Set<OfferCountry>();
            offerCountries.Update(offerCountry);
            await _ctx.SaveChangesAsync();

            return offerCountry;
        }

        public async Task Delete(int offerId)
        {
            List<OfferCountry> offerCountries = await _ctx.OfferCountry.Where(x => x.OfferId == offerId).ToListAsync();
            DbSet<OfferCountry> offerCountrySet = _ctx.Set<OfferCountry>();
            offerCountrySet.RemoveRange(offerCountries);
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<OfferCountry>> GetAll(int offerId)
        {
            List<OfferCountry> offerCountries;

            IQueryable<OfferCountry> offerCountryQuery = _ctx.OfferCountry;
            offerCountries = await offerCountryQuery.Where(x => x.IsActive && x.OfferId == offerId).ToListAsync();

            return offerCountries;
        }
    }
}
