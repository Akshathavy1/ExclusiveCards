//using ExclusiveCard.Data.Context;
//using ST = ExclusiveCard.Data.StagingModels;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//namespace ExclusiveCard.Data.Managers
//{
//    public class StagingOfferCategoryManager : IStagingOfferCategoryManager
//    {
//        private readonly ExclusiveContext _ctx;
//        public StagingOfferCategoryManager(ExclusiveContext ctx)
//        {
//            _ctx = ctx;
//        }

//        public async Task<ST.OfferCategory> Add(ST.OfferCategory offerCategory)
//        {
//            DbSet<ST.OfferCategory> offerCategories = _ctx.Set<ST.OfferCategory>();
//            offerCategories.Add(offerCategory);
//            await _ctx.SaveChangesAsync();

//            return offerCategory;
//        }

//        public async Task<ST.OfferCategory> Update(ST.OfferCategory offerCategory)
//        {
//            DbSet<ST.OfferCategory> offerCategories = _ctx.Set<ST.OfferCategory>();
//            offerCategories.Update(offerCategory);
//            await _ctx.SaveChangesAsync();

//            return offerCategory;
//        }
//    }
//}
