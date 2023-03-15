//using ExclusiveCard.Data.Context;
//using ST = ExclusiveCard.Data.StagingModels;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;

//namespace ExclusiveCard.Data.Managers
//{
//    public class StagingOfferCountryManager : IStagingOfferCountryManager
//    {
//        #region Private Member

//        private readonly ExclusiveContext _ctx;

//        #endregion

//        #region Constructor

//        public StagingOfferCountryManager(ExclusiveContext ctx)
//        {
//            _ctx = ctx;
//        }

//        #endregion

//        public async Task<ST.OfferCountry> Add(ST.OfferCountry offerCountry)
//        {
//            DbSet<ST.OfferCountry> offerCountries = _ctx.Set<ST.OfferCountry>();
//            offerCountries.Add(offerCountry);
//            await _ctx.SaveChangesAsync();
//            return offerCountry;
//        }

//        public async Task<ST.OfferCountry> Update(ST.OfferCountry offerCountry)
//        {
//            DbSet<ST.OfferCountry> offerCountries = _ctx.Set<ST.OfferCountry>();
//            offerCountries.Update(offerCountry);
//            await _ctx.SaveChangesAsync();

//            return offerCountry;
//        }

//        public async Task Delete(int offerId)
//        {
//            List<ST.OfferCountry> offerCountries = await _ctx.StagingOfferCountry.Where(x => x.OfferId == offerId).ToListAsync();
//            DbSet<ST.OfferCountry> offerCountriesSet = _ctx.Set<ST.OfferCountry>();
//            offerCountriesSet.RemoveRange(offerCountries);
//            await _ctx.SaveChangesAsync();
//        }
//    }
//}
