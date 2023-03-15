using System;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferManager : IOfferManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion Private Member

        #region Constructor

        public OfferManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion Constructor

        public async Task<Offer> Add(Offer offer)
        {
            DbSet<Offer> offers = _ctx.Set<Offer>();
            offers.Add(offer);
            await _ctx.SaveChangesAsync();
            return offer;
        }

        public async Task<List<Offer>> AddBulkAsync(List<Offer> offers)
        {
            DbSet<Offer> offerSet = _ctx.Set<Offer>();
            offerSet.AddRange(offers);
            await _ctx.SaveChangesAsync();
            return offers.Where(x => x.Id == 0).ToList();
        }

        public async Task<Offer> Update(Offer offer)
        {
            DbSet<Offer> offers = _ctx.Set<Offer>();
            offers.Update(offer);
            await _ctx.SaveChangesAsync();

            return offer;
        }

        public async Task BulkUpdate(List<Offer> offer)
        {
            DbSet<Offer> offers = _ctx.Set<Offer>();
            offers.UpdateRange(offer);
            await _ctx.SaveChangesAsync();
        }

        public async Task<OfferMerchantBranch> AddOfferBranch(OfferMerchantBranch offerBranch)
        {
            DbSet<OfferMerchantBranch> offerMerchantBranch = _ctx.Set<OfferMerchantBranch>();
            offerMerchantBranch.Add(offerBranch);
            await _ctx.SaveChangesAsync();
            return offerBranch;
        }

        public async Task DeleteOfferBranch(int offerId)
        {
            List<OfferMerchantBranch> offerBranches = await _ctx.OfferMerchantBranch.Where(x => x.OfferId == offerId).AsNoTracking().ToListAsync();
            DbSet<OfferMerchantBranch> offerBranch = _ctx.Set<OfferMerchantBranch>();
            offerBranch.RemoveRange(offerBranches);
            await _ctx.SaveChangesAsync();
        }

        //Get temp table values in executeSpMapping

        public async Task<Offer> Get(int id, bool includeMerchant, bool includeOfferType, bool includeStatus, bool includeAffiliate, bool includeCategories)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer;
            offerQuery = offerQuery.Include(x => x.OfferMerchantBranches);
            if (includeMerchant)
            {
                offerQuery = offerQuery.Include(x => x.Merchant)
                    .ThenInclude(x => x.MerchantImages);
            }

            if (includeCategories)
            {
                offerQuery = offerQuery.Include(x => x.OfferCategories);
            }

            if (includeOfferType)
            {
                offerQuery = offerQuery.Include(x => x.OfferType);
            }
            if (includeStatus)
            {
                offerQuery = offerQuery.Include(x => x.Status);
            }

            if (includeAffiliate)
            {
                offerQuery = offerQuery.Include(x => x.Affiliate);
            }

            var offer = await offerQuery.FirstOrDefaultAsync(x => x.Id == id);
            return offer;
        }

        public async Task<List<Offer>> GetAll(bool includeMerchant, bool includeOfferType, bool includeStatus)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.OfferCountries);
            if (includeMerchant)
            {
                offerQuery = offerQuery.Include(x => x.Merchant);
            }
            if (includeOfferType)
            {
                offerQuery = offerQuery.Include(x => x.OfferType);
            }
            if (includeStatus)
            {
                offerQuery = offerQuery.Include(x => x.Status);
            }
            var offers = await offerQuery.ToListAsync();
            return offers;
        }

        public async Task<PagedResult<Offer>> GetAllPaged(int page, int pageSize)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant);
            return await offerQuery.GetPaged(page, pageSize);
        }

        public async Task<List<Offer>> GetSearchList(int? merchantId, int? affiliateId, int? typeId, int? statusId, string keyword, DateTime? validFrom, DateTime? validTo)
        {
            PagedResult<Offer> pagedData = new PagedResult<Offer>();

            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant);

            if (!string.IsNullOrEmpty(merchantId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.MerchantId == merchantId);
            }
            if (!string.IsNullOrEmpty(affiliateId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.AffiliateId == affiliateId);
            }
            if (!string.IsNullOrEmpty(typeId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.OfferTypeId == typeId);
            }
            if (!string.IsNullOrEmpty(statusId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.StatusId == statusId);
            }
            if (validFrom.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.ValidFrom >= validFrom || x.Validindefinately);
            }
            if (validTo.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.ValidTo <= validTo || x.Validindefinately);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                offerQuery = offerQuery.Where(x => x.Exclusions.ToLower().Contains(keyword.ToLower()) || x.Instructions.ToLower().Contains(keyword.ToLower()) ||
                                                   x.Headline.ToLower().Contains(keyword.ToLower()) || x.ShortDescription.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.LongDescription.Trim().ToLower().Contains(keyword.ToLower()) || x.OfferCode.ToLower().Contains(keyword.ToLower()) ||
                                                   x.Terms.Trim().ToLower().Contains(keyword.ToLower()) || x.Merchant.Name.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.Merchant.ShortDescription.Trim().ToLower().Contains(keyword.ToLower()));
            }

            return await offerQuery.ToListAsync();
        }

        public async Task<PagedResult<Offer>> GetPagedSearch(int? merchantId, int? affiliateId,
            int? typeId, int? statusId, string keyword, DateTime? validFrom,
            DateTime? validTo, string countryCode, List<int> categories, string merchantName,
            List<int> offerTypes, int pageSize, int currentPage, int sortOrder)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant);

            if (merchantId.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.MerchantId == merchantId.Value);
            }
            if (affiliateId.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.AffiliateId == affiliateId.Value);
            }
            if (typeId.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.OfferTypeId == typeId.Value);
            }
            if (statusId.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.StatusId == statusId.Value);
            }
            if (validFrom.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.ValidFrom >= validFrom || x.Validindefinately);
            }
            if (validTo.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.ValidTo <= validTo || x.Validindefinately);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                offerQuery = offerQuery.Where(x => x.Exclusions.ToLower().Contains(keyword.ToLower()) || x.Instructions.ToLower().Contains(keyword.ToLower()) ||
                                                   x.Headline.ToLower().Contains(keyword.ToLower()) || x.ShortDescription.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.LongDescription.Trim().ToLower().Contains(keyword.ToLower()) || x.OfferCode.ToLower().Contains(keyword.ToLower()) ||
                                                   x.Merchant.Name.Trim().ToLower().Contains(keyword.ToLower()) || x.Terms.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.Merchant.ShortDescription.Trim().Contains(keyword.ToLower()));
            }
            if (!string.IsNullOrEmpty(countryCode))
            {
                offerQuery = offerQuery.Where(x =>
                    x.OfferCountries.Any(y => y.IsActive && y.CountryCode == countryCode));
            }
            if (categories != null && categories.Any())
            {
                offerQuery = offerQuery.Where(x => x.OfferCategories.Any(y => categories.Contains(y.CategoryId)));
            }
            if (!string.IsNullOrEmpty(merchantName))
            {
                offerQuery = offerQuery.Where(x => x.Merchant.Name == merchantName);
            }
            if (offerTypes != null && offerTypes.Any())
            {
                offerQuery = offerQuery.Where(x => offerTypes.Contains(x.OfferTypeId));
            }

            //Filter the list, don't show already mapped items and excluded Diamond Offer in OfferType
            //offerQuery = offerQuery.Where(x => x.OfferListItems.All(y => y.OfferId != x.Id) && x.OfferType.Description != Data.Constants.Keys.DiamondOfferType);
            offerQuery = offerQuery.Where(x => x.OfferType.Description != Data.Constants.Keys.DiamondOfferType);

            return await offerQuery.GetPaged(currentPage, pageSize); //US 291 -> partial view with pagination
        }

        public async Task<PagedResult<Offer>> GetPagedSearchList(int? merchantId, int? affiliateId, int? typeId, int? statusId, string keyword, DateTime? validFrom, DateTime? validTo, int page, int pageSize, OffersSortOrder sortOrder)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant);
            if (!string.IsNullOrEmpty(merchantId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.MerchantId == merchantId);
            }
            if (!string.IsNullOrEmpty(affiliateId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.AffiliateId == affiliateId);
            }
            if (!string.IsNullOrEmpty(typeId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.OfferTypeId == typeId);
            }
            if (!string.IsNullOrEmpty(statusId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.StatusId == statusId);
            }
            if (validFrom.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidFrom != null && x.ValidFrom.Value.Date >= validFrom.Value.Date) || x.Validindefinately);
            }
            if (validTo.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidTo != null && x.ValidTo.Value.Date <= validTo.Value.Date) || x.Validindefinately);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                offerQuery = offerQuery.Include(y => y.OfferTags).Where(x => x.Headline.ToLower().Contains(keyword.ToLower()) || x.ShortDescription.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.LongDescription.Trim().ToLower().Contains(keyword.ToLower()) || x.OfferCode.ToLower().Contains(keyword.ToLower()) ||
                                                   x.Terms.Trim().ToLower().Contains(keyword.ToLower()) || x.Merchant.Name.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.OfferTags.Any(i => i.Tag.Tags.ToLower().Contains(keyword.ToLower())) || x.Merchant.ShortDescription.Trim().ToLower().Contains(keyword.ToLower()));
            }
            if (sortOrder.Equals(OffersSortOrder.MerchantNameAsc))
            {
                offerQuery = offerQuery.OrderBy(x => x.Merchant.Name);
            }
            if (sortOrder.Equals(OffersSortOrder.MerchantNameDesc))
            {
                offerQuery = offerQuery.OrderByDescending(x => x.Merchant.Name);
            }

            if (sortOrder.Equals(OffersSortOrder.OfferShortDescriptionAsc))
            {
                offerQuery = offerQuery.OrderBy(x => x.Merchant.ShortDescription);
            }
            if (sortOrder.Equals(OffersSortOrder.OfferShortDescriptionDesc))
            {
                offerQuery = offerQuery.OrderByDescending(x => x.Merchant.ShortDescription);
            }

            if (sortOrder.Equals(OffersSortOrder.ValidFromAsc))
            {
                offerQuery = offerQuery.OrderBy(x => x.ValidFrom);
            }
            if (sortOrder.Equals(OffersSortOrder.ValidFromDesc))
            {
                offerQuery = offerQuery.OrderByDescending(x => x.ValidFrom);
            }

            if (sortOrder.Equals(OffersSortOrder.ValidToAsc))
            {
                offerQuery = offerQuery.OrderBy(x => x.ValidTo);
            }
            if (sortOrder.Equals(OffersSortOrder.ValidToDesc))
            {
                offerQuery = offerQuery.OrderByDescending(x => x.ValidTo);
            }

            return await offerQuery.GetPaged(page, pageSize);
        }

        //Method for DisplayOffer page in public website
        public async Task<PagedResult<Offer>> GetPublicPagedResult(int? merchantId, int? offerType,
            string countryCode, DateTime? validFrom, DateTime? validTo,
            int? sortOrder, List<int> categories, string keyword, string merchantName, List<int> offerTypes,
            int pageSize, int currentPage, int? excludeMerchantId)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant).ThenInclude(x => x.MerchantImages);
            if (!string.IsNullOrEmpty(merchantId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.MerchantId == merchantId);
            }
            //if (!string.IsNullOrEmpty(offerType.ToString()))
            //{
            //    offerQuery = offerQuery.Where(x => x.OfferTypeId == offerType);
            //}
            if (!string.IsNullOrEmpty(countryCode))
            {
                offerQuery = offerQuery.Include(x => x.OfferCountries).Where(x => x.OfferCountries.Any(i => i.CountryCode == countryCode));
            }
            if (validFrom.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidFrom != null && x.ValidFrom.Value.Date >= validFrom.Value.Date) || x.Validindefinately);
            }
            if (validTo.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidTo != null && x.ValidTo.Value.Date <= validTo.Value.Date) || x.Validindefinately);
            }
            if (categories != null && categories.Count > 0)
            {
                offerQuery = offerQuery.Include(x => x.OfferCategories).Where(x => x.OfferCategories.Any(i => categories.Contains(i.CategoryId)));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                offerQuery = offerQuery.Include(y => y.OfferTags).Where(x => x.ShortDescription.ToLower().Contains(keyword.ToLower())
                || x.Merchant.Name.ToLower().Contains(keyword.ToLower())
                || x.LongDescription.ToLower().Contains(keyword.ToLower())
                || x.OfferTags.Any(i => i.Tag.Tags.ToLower().Contains(keyword.ToLower()))
                || x.Merchant.ShortDescription.ToLower().Contains(keyword.ToLower()));
            }
            if (!string.IsNullOrEmpty(merchantName))
            {
                offerQuery = offerQuery.Where(x => x.Merchant.Name.ToLower().Contains(merchantName.ToLower()));
            }
            if (offerTypes != null && offerTypes.Count > 0)
            {
                offerQuery = offerQuery.Include(x => x.OfferType).Where(x => offerTypes.Contains(x.OfferType.Id));
            }
            if (pageSize == 0)
            {
                pageSize = 50;
            }
            if (currentPage == 0)
            {
                currentPage = 1;
            }
            if (excludeMerchantId.HasValue)
            {
                offerQuery = offerQuery.Where(x => x.MerchantId != excludeMerchantId);
            }

            offerQuery = offerQuery.Where(x =>
                x.Status.Name == Constants.Status.Active && x.Status.IsActive &&
                x.Status.Type == Constants.StatusType.Offer && (x.Validindefinately
                                                                || (x.ValidFrom != null && x.ValidTo != null &&
                                                                    x.ValidFrom <= DateTime.UtcNow &&
                                                                    x.ValidTo >= DateTime.UtcNow))).Include(x => x.OfferType);
            switch (sortOrder)
            {
                case 1:
                    offerQuery = offerQuery.OrderBy(x => x.OfferType.SearchRanking).ThenBy(x => x.ValidFrom);
                    break;

                case 2:
                    offerQuery = offerQuery.OrderBy(x => x.Merchant.Name).ThenBy(x => x.OfferType.SearchRanking)
                        .ThenBy(x => x.ValidFrom);
                    break;

                case 3:
                    offerQuery = offerQuery.OrderBy(x => x.ValidTo).ThenBy(x => x.OfferType.SearchRanking);
                    break;

                default:
                    offerQuery = offerQuery.OrderBy(x => x.OfferType.SearchRanking).ThenBy(x => x.ValidFrom);
                    break;
            }

            return await offerQuery.GetPaged(currentPage, pageSize);
        }

        //Method for GetOffer in Home Page of public website
        public async Task<List<Offer>> GetPublicListofOffersAsync(int? merchantId, int? offerType,
            string countryCode, DateTime? validFrom, DateTime? validTo,
            int? sortOrder, List<int> categories, string keyword, string merchantName, List<int> offerTypes)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant).ThenInclude(x => x.MerchantImages);
            if (!string.IsNullOrEmpty(merchantId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.MerchantId == merchantId);
            }
            if (!string.IsNullOrEmpty(offerType.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.OfferTypeId == offerType);
            }
            if (!string.IsNullOrEmpty(countryCode))
            {
                offerQuery = offerQuery.Include(x => x.OfferCountries).Where(x => x.OfferCountries.Any(i => i.CountryCode == countryCode));
            }
            if (validFrom.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidFrom != null && x.ValidFrom.Value.Date >= validFrom.Value.Date) || x.Validindefinately);
            }
            if (validTo.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidTo != null && x.ValidTo.Value.Date <= validTo.Value.Date) || x.Validindefinately);
            }
            if (categories != null && categories.Count > 0)
            {
                offerQuery = offerQuery.Include(x => x.OfferCategories).Where(x => x.OfferCategories.Any(i => categories.Contains(i.CategoryId)));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                offerQuery = offerQuery.Include(y => y.OfferTags).Where(x => x.ShortDescription.ToLower().Contains(keyword.ToLower())
                || x.Merchant.Name.ToLower().Contains(keyword.ToLower())
                || x.LongDescription.ToLower().Contains(keyword.ToLower())
                || x.OfferTags.Any(i => i.Tag.Tags.ToLower().Contains(keyword.ToLower()))
                || x.Merchant.ShortDescription.ToLower().Contains(keyword.ToLower()));
            }
            if (!string.IsNullOrEmpty(merchantName))
            {
                offerQuery = offerQuery.Where(x => x.Merchant.Name.ToLower().Contains(merchantName.ToLower()));
            }
            if (offerTypes != null && offerTypes.Count > 0)
            {
                offerQuery = offerQuery.Include(x => x.OfferType).Where(x => offerTypes.Contains(x.OfferType.Id));
            }

            if (sortOrder.HasValue && sortOrder > 0)
            {
                offerQuery = offerQuery.OrderBy(x => x.SearchRanking).ThenBy(x => x.ValidTo).ThenBy(x => x.Id);
            }

            return await offerQuery.Where(x =>
                    x.Status.Name == Constants.Status.Active && x.Status.IsActive && x.Status.Type == Constants.StatusType.Offer && (x.Validindefinately
                    || (x.ValidFrom != null && x.ValidTo != null && x.ValidFrom <= DateTime.UtcNow &&
                        x.ValidTo >= DateTime.UtcNow)))
                .Include(x => x.OfferType)
                .OrderBy(x => x.OfferTypeId).ToListAsync();
        }

        //Method for MainSearchTerms in public website
        public async Task<PagedResult<Offer>> GetPublicMainSearchResult(int? offerId, int? offerType, string countryCode,
            string searchTerm, int pageSize, int currentPage, int sortOrder, List<int> categories, List<int> offerTypes)
        {
            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant).ThenInclude(x => x.MerchantImages).Where
                                                                (
                                                                    x => !x.Merchant.IsDeleted
                                                                    && (x.Validindefinately
                                                                        || (
                                                                            x.ValidFrom.HasValue && x.ValidFrom <= DateTime.UtcNow
                                                                            &&
                                                                            x.ValidTo.HasValue && x.ValidTo >= DateTime.UtcNow
                                                                            )
                                                                        )
                                                                );

            if (offerId.HasValue && offerId.Value > 0)
            {
                //offerQuery = offerQuery.Where(x => x.Id == offerId.Value);
                return await offerQuery.Where(x => x.Id == offerId.Value).GetPaged(currentPage, pageSize);
            }

            if (!string.IsNullOrEmpty(offerType.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.OfferTypeId == offerType);
            }
            if (!string.IsNullOrEmpty(countryCode))
            {
                offerQuery = offerQuery.Include(x => x.OfferCountries).Where(x => x.OfferCountries.Any(i => i.CountryCode == countryCode));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                offerQuery = offerQuery.Where(x => x.OfferType.Description != Data.Constants.Keys.DiamondOffer
                 && (
                            x.Merchant.Name.ToLower().Contains(searchTerm.ToLower())
                            || x.Merchant.ShortDescription.ToLower().Contains(searchTerm.ToLower())
                            || x.Merchant.LongDescription.ToLower().Contains(searchTerm.ToLower())
                            || x.Headline.ToLower().Contains(searchTerm.ToLower())
                            || x.ShortDescription.ToLower().Contains(searchTerm.ToLower())
                            || x.LongDescription.ToLower().Contains(searchTerm.ToLower())
                            || x.OfferTags.Any(t => t.Tag.Tags.ToLower().Contains(searchTerm.ToLower()))
                     )
                );
            }

            if (categories != null && categories.Count > 0)
            {
                offerQuery = offerQuery.Include(x => x.OfferCategories).Where(x => x.OfferCategories.Any(i => categories.Contains(i.CategoryId)));
            }
            if (offerTypes != null && offerTypes.Count > 0)
            {
                offerQuery = offerQuery.Include(x => x.OfferType).Where(x => offerTypes.Contains(x.OfferType.Id));
            }

            offerQuery = offerQuery.Where(x =>
                    x.Status.Name == Constants.Status.Active && x.Status.IsActive &&
                    x.Status.Type == Constants.StatusType.Offer
                    //&& (x.Validindefinately || (x.ValidFrom != null && x.ValidTo != null &&
                    //                            x.ValidFrom <= DateTime.UtcNow &&
                    //                            x.ValidTo >= DateTime.UtcNow))
                    )
            .Include(x => x.OfferType);

            switch (sortOrder)
            {
                case 1:
                    return await offerQuery.OrderBy(x => x.OfferType.SearchRanking).ThenBy(x => x.ValidFrom)
                        .GetPaged(currentPage, pageSize);

                case 2:
                    return await offerQuery.OrderBy(x => x.Merchant.Name).ThenBy(x => x.OfferType.SearchRanking)
                        .ThenBy(x => x.ValidFrom).GetPaged(currentPage, pageSize);

                case 3:
                    return await offerQuery.OrderBy(x => x.ValidTo).ThenBy(x => x.OfferType.SearchRanking)
                        .GetPaged(currentPage, pageSize);

                default:
                    return await offerQuery.OrderBy(x => x.SearchRanking)
                        .ThenBy(x => x.OfferType.SearchRanking)
                        .ThenBy(x => x.ValidTo)
                        .ThenBy(x => x.ValidFrom)
                        .ThenBy(x => x.Datecreated).GetPaged(currentPage, pageSize);
            }
        }

        public async Task<PagedResult<Offer>> GetPagedSearchOffersList(int? merchantId, int? affiliateId, int? typeId, int? statusId, string keyword, string countryCode, DateTime? validFrom, DateTime? validTo, int page, int pageSize)
        {
            PagedResult<Offer> pagedData = new PagedResult<Offer>();

            IQueryable<Offer> offerQuery = _ctx.Offer.Include(x => x.Merchant);
            if (!string.IsNullOrEmpty(merchantId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.MerchantId == merchantId);
            }
            if (!string.IsNullOrEmpty(affiliateId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.AffiliateId == affiliateId);
            }
            if (!string.IsNullOrEmpty(typeId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.OfferTypeId == typeId);
            }
            if (!string.IsNullOrEmpty(statusId.ToString()))
            {
                offerQuery = offerQuery.Where(x => x.StatusId == statusId);
            }
            if (validFrom.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidFrom != null && x.ValidFrom.Value.Date >= validFrom.Value.Date) || x.Validindefinately);
            }
            if (validTo.HasValue)
            {
                offerQuery = offerQuery.Where(x => (x.ValidTo != null && x.ValidTo.Value.Date <= validTo.Value.Date) || x.Validindefinately);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                offerQuery = offerQuery.Where(x => x.Exclusions.ToLower().Contains(keyword.ToLower()) || x.Instructions.ToLower().Contains(keyword.ToLower()) ||
                                                   x.Headline.ToLower().Contains(keyword.ToLower()) || x.ShortDescription.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.LongDescription.Trim().ToLower().Contains(keyword.ToLower()) || x.OfferCode.ToLower().Contains(keyword.ToLower()) ||
                                                   x.Terms.Trim().ToLower().Contains(keyword.ToLower()) || x.Merchant.Name.Trim().ToLower().Contains(keyword.ToLower()) ||
                                                   x.Merchant.ShortDescription.Trim().ToLower().Contains(keyword.ToLower()));
            }
            if (!string.IsNullOrEmpty(countryCode))
            {
                offerQuery = offerQuery.Where(x =>
                    x.OfferCountries.Any(y => y.IsActive && y.CountryCode == countryCode));
            }

            return await offerQuery.GetPaged(page, pageSize);
        }

        public async Task<List<Tuple<string, int>>> CheckIfOfferExistsForAffiliate(int affiliateId,
            List<string> affiliateReferences)
        {
            List<Tuple<string, int>> references = new List<Tuple<string, int>>();
            DbSet<Offer> offerSet = _ctx.Set<Offer>();
            references.AddRange(await offerSet
                .Where(x => x.AffiliateId == affiliateId && affiliateReferences.Contains(x.AffiliateReference))
                .Select(x => new Tuple<string, int>(x.AffiliateReference, x.Id)).ToListAsync());

            return references;
        }

        public async Task<List<Offer>> GetByMerchantId(int merchantId)
        {
            return await _ctx.Offer.Where(x => x.MerchantId == merchantId).ToListAsync();
        }
    }
}