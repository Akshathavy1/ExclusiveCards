using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Enums;
using db=ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferListItemManager : IOfferListItemManager
    {
        #region Private Member

        private readonly IRepository<db.OfferListItem> _offerListItemRepo;
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        #endregion Private Member

        #region Constructor

        public OfferListItemManager(IRepository<db.OfferListItem> offerListItemRepo, ExclusiveContext ctx)
        {
            _offerListItemRepo = offerListItemRepo;
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion Constructor

        #region Writes

        public async Task<db.OfferListItem> Add(db.OfferListItem offerListItem)
        {
            try
            {
                _offerListItemRepo.Create(offerListItem);
                _offerListItemRepo.SaveChanges();

                await Task.CompletedTask;
                return offerListItem;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return offerListItem;
            }
        }

        public async Task<List<db.OfferListItem>> AddListOfferListItems(List<db.OfferListItem> offerListItem)
        {
            try
            {
                foreach (db.OfferListItem item in offerListItem)
                {
                    _offerListItemRepo.Create(item);
                    _offerListItemRepo.SaveChanges();
                }

                await Task.CompletedTask;

                return offerListItem;

            }
            catch (Exception e)
            {
                _logger.Error(e);
                return offerListItem;
            }
        }

        public async Task<db.OfferListItem> Update(db.OfferListItem offerListItem)
        {
            _offerListItemRepo.Update(offerListItem);
            _offerListItemRepo.SaveChanges();

            await Task.CompletedTask;

            return offerListItem;
        }

        public async Task<db.OfferListItem> Delete(db.OfferListItem offerListItem, bool saveChanges = true)
        {
            if (saveChanges)
            {
                _offerListItemRepo.Delete(offerListItem);
                _offerListItemRepo.SaveChanges();
            }

            await Task.CompletedTask;

            return offerListItem;
        }

        public async Task DeleteByOfferListId(int id)
        {
            List<db.OfferListItem> offerListItems = await _ctx.OfferListItem.Where(x => x.OfferListId == id).AsNoTracking().ToListAsync();
            if (offerListItems.Count != 0)
            {
                foreach (var item in offerListItems)
                {
                    _ctx.Entry(item).State = EntityState.Deleted;
                }

                await _ctx.SaveChangesAsync();
            }
        }

        //Reorder offers
        public async Task<bool> ReorderOffersOnSelectedPostition(int itemListId, string countryCode, int order, int increment)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    List<db.OfferListItem> offerListItems;

                    offerListItems = await _ctx.OfferListItem.OrderBy(x => x.DisplayOrder).AsNoTracking()
                        .Where(x => x.OfferListId == itemListId && x.CountryCode == countryCode &&
                                    x.DisplayOrder > order).ToListAsync();

                    foreach (db.OfferListItem offer in offerListItems)
                    {
                        offer.DisplayOrder = Convert.ToInt16(offer.DisplayOrder + increment);
                        await Update(offer);
                    }

                    scope.Complete();
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }

        #endregion Writes

        #region Reads

        public async Task<List<db.OfferListItem>> GetAllByOfferId(int offerId)
        {
            IQueryable<db.OfferListItem> offerListItemQuery = _ctx.OfferListItem;
            var offerListItems = await offerListItemQuery.AsNoTracking()
                .Where(x => x.OfferId == offerId).OrderBy(x => x.DisplayOrder).ToListAsync();
            return offerListItems;
        }

        public async Task<db.OfferListItem> GetByOfferIdCountryandListId(int offerId, int itemListId, string countryCode)
        {
            IQueryable<db.OfferListItem> offerListItemQuery = _ctx.OfferListItem;
            var offerListItems = await offerListItemQuery.AsNoTracking().OrderBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync(x => x.OfferId == offerId && x.OfferListId == itemListId && x.CountryCode == countryCode
                                          && x.Offer.Status.Name == Constants.Status.Active && x.Offer.Status.IsActive);
            return offerListItems;
        }

        public async Task<List<db.OfferListItem>> GetByCountryandListId(int itemListId, string countryCode)
        {
            IQueryable<db.OfferListItem> offerListItemQuery = _ctx.OfferListItem;
            var offerListItems = await offerListItemQuery.AsNoTracking()
                .Where(x => x.OfferListId == itemListId && x.CountryCode == countryCode
                                                       && x.Offer.Status.Name == Constants.Status.Active && x.Offer.Status.IsActive)
                .OrderBy(x => x.DisplayOrder).ToListAsync();
            return offerListItems;
        }

        public async Task<List<db.OfferListItem>> GetAll()
        {
            IQueryable<db.OfferListItem> offerListItemQuery = _ctx.OfferListItem;
            var offerListItems = await offerListItemQuery.AsNoTracking().OrderBy(x => x.DisplayOrder).ToListAsync();
            return offerListItems;
        }

        //Get List<offerListItem> for each offerlist Id
        public async Task<List<db.OfferListItem>> GetAllByOfferListId(int offerListId)
        {
            IQueryable<db.OfferListItem> offerListItemQuery = _ctx.OfferListItem;

            var offerListItems = await offerListItemQuery.AsNoTracking()
                .Where(x => x.OfferListId == offerListId).OrderBy(x => x.DisplayOrder).ToListAsync();
            return offerListItems;
        }

        //Get Offer Summary for each OfferList, countryCode
        public async Task<List<db.OfferListItem>> GetByOfferListIdForCountry(int offerListId, string countryCode, int pageSize, int? merchantId)
        {
            IQueryable<db.OfferListItem> offerListItemQuery = _ctx.OfferListItem.AsNoTracking()
                .Where(a => a.OfferListId == offerListId && a.CountryCode == countryCode &&
                a.Offer.Status.Name == Constants.Status.Active && a.Offer.Status.IsActive && a.Offer.Status.Type == Constants.StatusType.Offer &&
                (a.Offer.Validindefinately
                || (a.Offer.ValidFrom != null && a.Offer.ValidTo != null
                && a.Offer.ValidFrom <= DateTime.UtcNow && a.Offer.ValidTo >= DateTime.UtcNow))
                && a.Offer.OfferCountries.Any(b => b.IsActive && b.CountryCode == countryCode))
                .Include(x => x.Offer).ThenInclude(y => y.Merchant).ThenInclude(z => z.MerchantImages);

            if (merchantId.HasValue)
            {
                offerListItemQuery = offerListItemQuery.Where(x => x.Offer.MerchantId == merchantId);
            }
            var offerListItems = await offerListItemQuery.OrderBy(x => x.DisplayOrder).Take(pageSize).ToListAsync();
            return offerListItems;
        }

        public async Task<db.PagedResult<db.OfferListItem>> GetPagedOffersForListItem(int listItemId, string countrycode, int page, int pageSize)
        {
            IQueryable<db.OfferListItem> offerQuery = _ctx.OfferListItem.AsNoTracking()
                .Where(x => x.OfferListId == listItemId && x.CountryCode == countrycode &&
                x.Offer.Status.Name == Constants.Status.Active && x.Offer.Status.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Include(x => x.Offer)
                .ThenInclude(x => x.Merchant);
            return await offerQuery.GetPaged(page, 250);
        }

        //Get Top 5 SliderOfferModel based on Keys.OfferHubList with optional category search
        public async Task<List<db.OfferListDataModel>> GetOfferListDataModels(string countryCode, List<int> categories, int pageSize, string listName)
        {
            IQueryable<db.OfferListItem> queryable = _ctx.OfferListItem.AsNoTracking()
                .Where(x => x.CountryCode == countryCode &&
                            (x.DisplayFrom == null ||
                             (x.DisplayFrom != null && x.DisplayFrom.Value.Date <= DateTime.UtcNow.Date)) &&
                            (x.DisplayTo == null ||
                             (x.DisplayTo != null && x.DisplayTo.Value.Date >= DateTime.UtcNow.Date)) &&
                            x.Offer.Status.Name == Constants.Status.Active && x.Offer.Status.IsActive &&
                            x.Offer.Status.Type == Constants.StatusType.Offer &&
                            (x.Offer.Validindefinately
                             || (x.Offer.ValidFrom != null && x.Offer.ValidTo != null
                                                           && x.Offer.ValidFrom <= DateTime.UtcNow &&
                                                           x.Offer.ValidTo >= DateTime.UtcNow))
                            && x.Offer.OfferCountries.Any(y => y.IsActive && y.CountryCode == countryCode)
                            && x.OfferList.ListName == listName)
                .Include(x => x.OfferList)
                .Include(x => x.Offer)
                .ThenInclude(y => y.Merchant)
                .ThenInclude(z => z.MerchantImages);
            if (categories != null && categories.Count > 0)
            {
                queryable = queryable.Where(b => b.Offer.OfferCategories.Any(i => categories.Contains(i.CategoryId)));
            }
            var offerListItems = await queryable.OrderBy(x => x.DisplayOrder).Take(pageSize).ToListAsync();
            if (offerListItems == null)
                return null;
            List<db.OfferListDataModel> sliderOffers = new List<db.OfferListDataModel>();
            offerListItems.ForEach(x => sliderOffers.Add(MapToOfferListDataModel(x)));
            return sliderOffers;
        }

        #endregion Reads

        #region Private Methods

        public async Task<List<db.OfferListDataModel>> GetLocalOfferListDataModels(string countryCode, int regionId)
        {
            IQueryable<db.OfferListItem> queryable = _ctx.OfferListItem.AsNoTracking()
                .Where(x => x.CountryCode == countryCode &&
                            (x.DisplayFrom == null || (x.DisplayFrom != null && x.DisplayFrom.Value.Date <= DateTime.UtcNow.Date)) &&
                            (x.DisplayTo == null ||
                            (x.DisplayTo != null && x.DisplayTo.Value.Date >= DateTime.UtcNow.Date)) &&
                            x.Offer.Status.Name == Constants.Status.Active && x.Offer.Status.IsActive &&
                            x.Offer.Status.Type == Constants.StatusType.Offer &&
                            (x.Offer.Validindefinately
                             || (x.Offer.ValidFrom != null && x.Offer.ValidTo != null
                                                           && x.Offer.ValidFrom <= DateTime.UtcNow &&
                                                           x.Offer.ValidTo >= DateTime.UtcNow)) && 
                            x.Offer.OfferCountries.Any(y => y.IsActive && y.CountryCode == countryCode) && 
                            x.OfferList.WhitelabelId == regionId && x.OfferList.WhiteLabelSettings.IsRegional
                            )
                .Include(x=>x.OfferList)
                .Include(x => x.Offer)
                .ThenInclude(y => y.Merchant)
                .ThenInclude(z => z.MerchantImages)
                //Will need these soon...
                //.Include(x => x.Offer.OfferCategories)
                //.ThenInclude(c=>c.Category)
                ;

            var offerListItems = await queryable.OrderBy(x => x.DisplayOrder).ToListAsync();
           
            if (offerListItems == null)
                return null;
            List<db.OfferListDataModel> sliderOffers = new List<db.OfferListDataModel>();
            offerListItems.ForEach(x => sliderOffers.Add(MapToOfferListDataModel(x)));
            return sliderOffers;
        }

        private db.OfferListDataModel MapToOfferListDataModel(db.OfferListItem model)
        {
            return new db.OfferListDataModel
            {
                OfferId = model.OfferId,
                MerchantId = model.Offer.MerchantId,
                MerchantName = model.Offer.Merchant.Name,
                OfferShortDescription = model.Offer.ShortDescription,
                OfferLongDescription = model.Offer.LongDescription,
                OfferText = model.Offer.Headline,
                Logo = model.Offer.Merchant?.MerchantImages?.Where(x => x.ImageType == (int)ImageType.Logo)?.OrderBy(x => x.DisplayOrder)?.FirstOrDefault()?.ImagePath,
                DisabledLogo = model.Offer.Merchant?.MerchantImages?.Where(x => x.ImageType == (int)ImageType.DisabledLogo)?.OrderBy(x => x.DisplayOrder)?.FirstOrDefault()?.ImagePath,
                FeatureImage = model.Offer.Merchant?.MerchantImages?.Where(x => x.ImageType == (int)ImageType.FeatureImage && x.ImagePath.Contains("__4"))?.FirstOrDefault()?.ImagePath,
                LargeImage = model.Offer.Merchant?.MerchantImages?.Where(x => x.ImageType == (int)ImageType.FeatureImage && x.ImagePath.Contains("__3"))?.FirstOrDefault()?.ImagePath,
                DisplayType = model.OfferList.DisplayType,
                offerTypeId = model.Offer.OfferTypeId
               //, OfferCategories = model.Offer.OfferCategories?.Where(x=>x.Category.ParentId==0).ToList()
            };
        }

        #endregion Private Methods
    }
}