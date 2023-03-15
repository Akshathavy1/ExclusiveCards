using System;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
    public class OfferService : IOfferService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferManager _offerManager;
        private readonly IOfferListManager _manager;
        private readonly IOfferListItemManager _offerListItemManager;

        #endregion Private Members

        #region Constructor

        public OfferService(IMapper mapper, IOfferManager offerManager,
            IOfferListManager offerListManager, IOfferListItemManager offerListItemManager)
        {
            _mapper = mapper;
            _offerManager = offerManager;
            _manager = offerListManager;
            _offerListItemManager = offerListItemManager;
        }

        #endregion Constructor

        #region Writes

        //Add Offer
        public async Task<DTOs.Offer> Add(DTOs.Offer offer)
        {
            if (offer.SSOConfigId == 0)
            {
                offer.SSOConfigId = null;
                offer.ProductCode = null;
            }
            Offer req = _mapper.Map<Offer>(offer);
            return Map(await _offerManager.Add(req), false, false, false, false, false);
        }

        //Update Offer
        public async Task<DTOs.Offer> Update(DTOs.Offer offer)
        {
            if (offer.SSOConfigId == 0)
            {
                offer.SSOConfigId = null;
                offer.ProductCode = null;
            }
            Offer req = _mapper.Map<Offer>(offer);
            return Map(
                await _offerManager.Update(req), false, false, false, false, false);
        }

        //Add Offers
        public async Task<List<DTOs.Offer>> AddBulkAsync(List<DTOs.Offer> offers)
        {
            List<Offer> req = _mapper.Map<List<Offer>>(offers);
            return _mapper.Map<List<DTOs.Offer>>(
                await _offerManager.AddBulkAsync(req));
        }

        public async Task<DTOs.OfferListItem> UpdateOfferList(DTOs.OfferListItem offerListItem)
        {
            OfferListItem req = MapToOfferListItemData(offerListItem);
            return MapToOfferListItemDto(
                await _offerListItemManager.Update(req));
        }

        public async Task<DTOs.OfferListItem> AddOfferListItem(DTOs.OfferListItem offerList)
        {
            OfferListItem req = MapToOfferListItemData(offerList);
            return MapToOfferListItemDto(
               await _offerListItemManager.Add(req));
        }

        public async Task<List<DTOs.OfferListItem>> AddOfferListItemList(List<DTOs.OfferListItem> offerList)
        {
            List<OfferListItem> req = MapToOfferListItemsData(offerList);
            return MapToOfferListItemsDto(
               await _offerListItemManager.AddListOfferListItems(req));
        }

        //delete an offerlist item
        public async Task<DTOs.OfferListItem> DeleteOfferListItem(DTOs.OfferListItem offerList, bool saveChanges = true)
        {
            OfferListItem req = MapToOfferListItemData(offerList);
            return MapToOfferListItemDto(
               await _offerListItemManager.Delete(req, saveChanges));
        }

        //delete list of offelist items basing on offerlistId
        public async Task DeleteOfferListItemsById(int id)
        {
            await _offerListItemManager.DeleteByOfferListId(id);
        }

        //Reorder existing list of items for OfferList
        public async Task<bool> ReorderOffersOnSelectedPostition(int offerListId, string countryCode, int order, int increment)
        {
            return await _offerListItemManager.ReorderOffersOnSelectedPostition(offerListId, countryCode, order, increment);
        }

        //// Execute SP for Staging Offer map to exculsive offer
        //public async Task ExecuteSPMappingSTOfferToOffer(int affiliateId, int offerImportId, int recordsToProcess)
        //{
        //    await _offerManager.ExecuteSPMappingSTOfferToOffer(affiliateId, offerImportId, recordsToProcess);
        //}

        public async Task BulkUpdateAsync(List<DTOs.Offer> offers)
        {
            var req = _mapper.Map<List<Offer>>(offers);
            await _offerManager.BulkUpdate(req);
        }

        public async Task<DTOs.OfferMerchantBranch> AddOfferMerchantBranch(DTOs.OfferMerchantBranch offerBranch)
        {
            OfferMerchantBranch req = _mapper.Map<OfferMerchantBranch>(offerBranch);
            return _mapper.Map<DTOs.OfferMerchantBranch>(
                await _offerManager.AddOfferBranch(req));
        }

        #endregion Writes

        public async Task DeleteOfferBranch(int offerId)
        {
            await _offerManager.DeleteOfferBranch(offerId);
        }

        #region Reads

        //Get Offer
        public async Task<DTOs.Offer> Get(int id, bool includeMerchant = false, bool includeOfferType = false, bool includeStatus = false, bool includeAffiliate = false, bool includeCategories = false)
        {
            return Map(await _offerManager.Get(id, includeMerchant, includeOfferType, includeStatus, includeAffiliate, includeCategories), includeMerchant, includeOfferType, includeStatus, includeAffiliate, includeCategories);
        }

        //Get All Offer
        public async Task<List<DTOs.Offer>> GetAll(bool includeMerchant = false, bool includeOfferType = false, bool includeStatus = false)
        {
            return MapToList(await _offerManager.GetAll(includeMerchant, includeOfferType, includeStatus), includeMerchant, includeOfferType, includeStatus, false, false);
        }

        public async Task<List<DTOs.Admin.OfferSummary>> Search(DTOs.OfferSearchCriteria searchCriteria)
        {
            List<Offer> offersList = await _offerManager.GetSearchList(searchCriteria.MerchantId, searchCriteria.AffiliateId, searchCriteria.OfferType, searchCriteria.OfferStatus, searchCriteria.KeyWord, searchCriteria.ValidFrom, searchCriteria.ValidTo);
            return (from item in offersList
                    select new DTOs.Admin.OfferSummary()
                    {
                        OfferId = item.Id,
                        MerchantName = item.Merchant?.Name,
                        OfferShortDescription = item.ShortDescription,
                        SearchRanking = item.SearchRanking,
                        ValidFrom = item.ValidFrom?.ToString("dd/MM/yyyy"),
                        ValidTo = item.ValidTo?.ToString("dd/MM/yyyy")
                    }).ToList();
        }

        public async Task<DTOs.PagedResult<DTOs.Admin.OfferSummary>> PagedSearchOffersandOfferList(DTOs.OfferSearchCriteria searchCriteria)
        {
            return Map(await _offerManager.GetPagedSearch(searchCriteria.MerchantId, searchCriteria.AffiliateId,
                searchCriteria.OfferType, searchCriteria.OfferStatus, searchCriteria.KeyWord, searchCriteria.ValidFrom,
                searchCriteria.ValidTo, searchCriteria.CountryCode, searchCriteria.Categories, searchCriteria.MerchantName,
                searchCriteria.OfferTypes, searchCriteria.PageSize, searchCriteria.CurrentPage, searchCriteria.SortOrder));
        }

        public async Task<DTOs.PagedResult<DTOs.Admin.OfferSummary>> PagedSearch(int? merchantId, int? affiliateId, int? typeId, int? statusId, string keyword, DateTime? validFrom, DateTime? validTo, int page, int pageSize, OffersSortOrder sortOrder)
        {
            return Map(await _offerManager.GetPagedSearchList(merchantId, affiliateId, typeId, statusId, keyword, validFrom, validTo, page, pageSize, sortOrder));
        }

        public async Task<DTOs.PagedResult<DTOs.Admin.OfferSummary>> PagedSearchOfferList(int? merchantId, int? affiliateId, int? typeId, int? statusId, string keyword, string countryCode, DateTime? validFrom, DateTime? validTo, int page, int pageSize)
        {
            return Map(await _offerManager.GetPagedSearchOffersList(merchantId, affiliateId, typeId, statusId, keyword, countryCode, validFrom, validTo, page, pageSize));
        }

        public async Task<DTOs.PagedResult<DTOs.Admin.OfferSummary>> GetAllPagedSearch(int page, int pageSize)
        {
            return Map(await _offerManager.GetAllPaged(page, pageSize));
        }

        public async Task<List<DTOs.OfferList>> GetAllOfferList()
        {
            return MapToOfferLists(await _manager.GetAll());
        }

        public async Task<DTOs.PagedResult<DTOs.Admin.OfferSummary>> PagedSearchOfferListitems(int listItemId, string countryCode, int page, int pageSize)
        {
            return MapOfferListItem(await _offerListItemManager.GetPagedOffersForListItem(listItemId, countryCode, page, pageSize));
        }

        //GetByOfferIdandCountry
        public async Task<DTOs.OfferListItem> GetByOfferIdandCountry(int offerId, int listId, string countryCode)
        {
            return MapToOfferListItemDto(
               await _offerListItemManager.GetByOfferIdCountryandListId(offerId, listId, countryCode));
        }

        public async Task<List<DTOs.OfferListItem>> GetAllByOfferId(int offerId)
        {
            return MapToOfferListItemsDto(
              await _offerListItemManager.GetAllByOfferId(offerId));
        }

        public async Task<List<DTOs.OfferListItem>> GetByListIdAndCountry(int listId, string countryCode)
        {
            return MapToOfferListItemsDto(
               await _offerListItemManager.GetByCountryandListId(listId, countryCode));
        }

        //GetOfferListByName
        public async Task<DTOs.OfferList> GetOfferListByName(string name)
        {
            return MapToOfferListDto(await _manager.GetByName(name));
        }

        public async Task<List<Tuple<string, int>>> CheckIfOfferExistsForAffiliate(int affiliateId, List<string> affiliateReferences)
        {
            return await _offerManager.CheckIfOfferExistsForAffiliate(affiliateId, affiliateReferences);
        }

        public async Task<List<DTOs.Offer>> GetByMerchant(int merchantId)
        {
            return MapToList(await _offerManager.GetByMerchantId(merchantId), false, false, false, false, false);
        }

        #endregion Reads

        #region PrivateMethods

        private DTOs.PagedResult<DTOs.Admin.OfferSummary> Map(PagedResult<Offer> offerPagedResult)
        {
            DTOs.PagedResult<DTOs.Admin.OfferSummary> result = new DTOs.PagedResult<DTOs.Admin.OfferSummary>();
            foreach (Offer item in offerPagedResult.Results)
            {
                var offer = new DTOs.Admin.OfferSummary()
                {
                    OfferId = item.Id,
                    MerchantName = item.Merchant?.Name,
                    OfferShortDescription = item.ShortDescription,
                    OfferLongDescription = item.LongDescription,
                    SearchRanking = item.SearchRanking
                };

                if (item.ValidFrom.HasValue)
                {
                    offer.ValidFrom = TimeZoneInfo.ConvertTimeFromUtc(item.ValidFrom.Value, TimeZoneInfo.Local).ToString("dd/MM/yyyy");
                }
                if (item.ValidTo.HasValue)
                {
                    offer.ValidTo = TimeZoneInfo.ConvertTimeFromUtc(item.ValidTo.Value, TimeZoneInfo.Local)
                        .ToString("dd/MM/yyyy");
                }
                result.Results.Add(offer);
            }

            result.CurrentPage = offerPagedResult.CurrentPage;
            result.PageCount = offerPagedResult.PageCount;
            result.PageSize = offerPagedResult.PageSize;
            result.RowCount = offerPagedResult.RowCount;
            return result;
        }

        private DTOs.PagedResult<DTOs.Admin.OfferSummary> MapOfferListItem(PagedResult<ExclusiveCard.Data.Models.OfferListItem> offerPagedResult)
        {
            DTOs.PagedResult<DTOs.Admin.OfferSummary> result = new DTOs.PagedResult<DTOs.Admin.OfferSummary>();
            foreach (ExclusiveCard.Data.Models.OfferListItem item in offerPagedResult.Results)
            {
                result.Results.Add(new DTOs.Admin.OfferSummary()
                {
                    OfferId = item.Offer.Id,
                    MerchantName = item.Offer.Merchant?.Name,
                    OfferShortDescription = item.Offer.ShortDescription,
                    SearchRanking = item.Offer.SearchRanking,
                    ValidFrom = item.DisplayFrom?.ToString("yyyy-MM-dd"),
                    ValidTo = item.DisplayTo?.ToString("yyyy-MM-dd"),
                    DisplayOrder = item.DisplayOrder
                });
            }

            result.CurrentPage = offerPagedResult.CurrentPage;
            result.PageCount = offerPagedResult.PageCount;
            result.PageSize = offerPagedResult.PageSize;
            result.RowCount = offerPagedResult.RowCount;
            return result;
        }

        private List<DTOs.Offer> MapToList(List<Offer> offers, bool includeMerchant, bool includeOfferType, bool includeStatus, bool includeAffiliate, bool includeCategories)
        {
            if (offers == null || offers.Count == 0)
                return null;
            List<DTOs.Offer> list = new List<DTOs.Offer>();
            list.AddRange(offers.Select(offer => Map(offer, includeMerchant, includeOfferType, includeStatus, includeAffiliate, includeCategories)));
            return list;
        }

        private DTOs.Offer Map(Offer offer, bool includeMerchant, bool includeOfferType, bool includeStatus, bool includeAffiliate, bool includeCategories)
        {
            if (offer == null)
                return null;
            DTOs.Offer dtoOffer = new DTOs.Offer
            {
                Id = offer.Id,
                MerchantId = offer.MerchantId,
                AffiliateId = offer.AffiliateId,
                OfferTypeId = offer.OfferTypeId,
                StatusId = offer.StatusId,
                ValidFrom = offer.ValidFrom,
                ValidTo = offer.ValidTo,
                Validindefinately = offer.Validindefinately,
                ShortDescription = offer.ShortDescription,
                LongDescription = offer.LongDescription,
                Instructions = offer.Instructions,
                Terms = offer.Terms,
                Exclusions = offer.Exclusions,
                LinkUrl = offer.LinkUrl,
                OfferCode = offer.OfferCode,
                Reoccuring = offer.Reoccuring,
                SearchRanking = offer.SearchRanking,
                Datecreated = offer.Datecreated,
                Headline = offer.Headline,
                AffiliateReference = offer.AffiliateReference,
                DateUpdated = offer.DateUpdated,
                RedemptionAccountNumber = offer.RedemptionAccountNumber,
                RedemptionProductCode = offer.RedemptionProductCode,
                SSOConfigId = offer.SSOConfigId,
                ProductCode = offer.ProductCode
            };
            if (includeMerchant && offer.Merchant != null)
            {
                dtoOffer.Merchant = new DTOs.Merchant
                {
                    Id = offer.Merchant.Id,
                    Name = offer.Merchant.Name,
                    ContactDetailsId = offer.Merchant.ContactDetailsId,
                    ContactName = offer.Merchant.ContactName,
                    ShortDescription = offer.Merchant.ShortDescription,
                    LongDescription = offer.Merchant.LongDescription,
                    Terms = offer.Merchant.Terms,
                    WebsiteUrl = offer.Merchant.WebsiteUrl,
                    IsDeleted = offer.Merchant.IsDeleted
                };
                if (offer.Merchant.MerchantImages != null && offer.Merchant.MerchantImages.Count > 0)
                {
                    dtoOffer.Merchant.MerchantImages = new List<DTOs.MerchantImage>();
                    foreach (Data.Models.MerchantImage image in offer.Merchant.MerchantImages)
                    {
                        dtoOffer.Merchant.MerchantImages.Add(new DTOs.MerchantImage
                        {
                            Id = image.Id,
                            MerchantId = image.MerchantId,
                            ImagePath = image.ImagePath,
                            DisplayOrder = image.DisplayOrder,
                            ImageType = image.ImageType
                        });
                    }
                }
            }
            if (includeOfferType && offer.OfferType != null)
            {
                dtoOffer.OfferType = new DTOs.OfferType
                {
                    Id = offer.OfferType.Id,
                    Description = offer.OfferType.Description,
                    IsActive = offer.OfferType.IsActive,
                    SearchRanking = offer.OfferType.SearchRanking
                };
            }
            if (includeStatus && offer.Status != null)
            {
                dtoOffer.Status = new DTOs.Status
                {
                    Id = offer.Status.Id,
                    Name = offer.Status.Name,
                    Type = offer.Status.Type,
                    IsActive = offer.Status.IsActive
                };
            }
            if (includeAffiliate && offer.Affiliate != null)
            {
                dtoOffer.Affiliate = new DTOs.Affiliate
                {
                    Id = offer.Affiliate.Id,
                    Name = offer.Affiliate.Name
                };
            }

            if (includeCategories && offer.OfferCategories?.Count > 0)
            {
                dtoOffer.OfferCategories = new List<DTOs.OfferCategory>();
                foreach (var item in offer.OfferCategories)
                {
                    dtoOffer.OfferCategories.Add(new DTOs.OfferCategory
                    {
                        OfferId = item.OfferId,
                        CategoryId = item.CategoryId
                    });
                }
            }

            return dtoOffer;
        }

        private List<DTOs.OfferList> MapToOfferLists(List<Data.Models.OfferList> offerLists)
        {
            List<DTOs.OfferList> list = new List<DTOs.OfferList>();
            if (offerLists == null)
                return null;
            offerLists.ForEach(x => list.Add(MapToOfferList(x)));
            return list;
        }

        private DTOs.OfferList MapToOfferList(Data.Models.OfferList list)
        {
            if (list == null)
                return null;
            return new DTOs.OfferList
            {
                Id = list.Id,
                Description = list.Description,
                IsActive = list.IsActive,
                IncludeShowAllLink = list.IncludeShowAllLink,
                ListName = list.ListName,
                MaxSize = list.MaxSize,
                PermissionLevel = list.PermissionLevel,
                ShowAllLinkCaption = list.ShowAllLinkCaption
            };
        }

        private List<DTOs.OfferListItem> MapToOfferListItemsDto(List<OfferListItem> data)
        {
            if (data == null || data.Count == 0)
                return null;

            var list = new List<DTOs.OfferListItem>();

            list.AddRange(data.Select(MapToOfferListItemDto));

            return list;
        }

        private DTOs.OfferListItem MapToOfferListItemDto(OfferListItem data)
        {
            if (data == null)
                return null;

            var model = new DTOs.OfferListItem
            {
                OfferListId = data.OfferListId,
                OfferId = data.OfferId,
                ExcludedCountries = data.ExcludedCountries,
                CountryCode = data.CountryCode,
                DisplayOrder = data.DisplayOrder,
                DisplayFrom = data.DisplayFrom,
                DisplayTo = data.DisplayTo
            };

            if (data.OfferList != null)
            {
                model.OfferList = new DTOs.OfferList
                {
                    Id = data.OfferList.Id,
                    ListName = data.OfferList.ListName,
                    Description = data.OfferList.Description,
                    MaxSize = data.OfferList.MaxSize,
                    IsActive = data.OfferList.IsActive,
                    IncludeShowAllLink = data.OfferList.IncludeShowAllLink,
                    PermissionLevel = data.OfferList.PermissionLevel,
                    ShowAllLinkCaption = data.OfferList.ShowAllLinkCaption,
                };
            }

            if (data.Offer != null)
            {
                model.Offer = new DTOs.Offer
                {
                    Id = data.Offer.Id,
                    MerchantId = data.Offer.MerchantId,
                    AffiliateId = data.Offer.AffiliateId,
                    OfferTypeId = data.Offer.OfferTypeId,
                    StatusId = data.Offer.StatusId,
                    ValidFrom = data.Offer.ValidFrom,
                    ValidTo = data.Offer.ValidTo,
                    Validindefinately = data.Offer.Validindefinately,
                    ShortDescription = data.Offer.ShortDescription,
                    LongDescription = data.Offer.LongDescription,
                    Instructions = data.Offer.Instructions,
                    Terms = data.Offer.Terms,
                    Exclusions = data.Offer.Exclusions,
                    LinkUrl = data.Offer.LinkUrl,
                    OfferCode = data.Offer.OfferCode,
                    Reoccuring = data.Offer.Reoccuring,
                    SearchRanking = data.Offer.SearchRanking,
                    Datecreated = data.Offer.Datecreated,
                    Headline = data.Offer.Headline,
                    AffiliateReference = data.Offer.AffiliateReference,
                    DateUpdated = data.Offer.DateUpdated,
                    RedemptionAccountNumber = data.Offer.RedemptionAccountNumber,
                    RedemptionProductCode = data.Offer.RedemptionProductCode
                };
            }

            return model;
        }

        private List<OfferListItem> MapToOfferListItemsData(List<DTOs.OfferListItem> data)
        {
            if (data == null || data.Count == 0)
                return null;

            var list = new List<OfferListItem>();

            list.AddRange(data.Select(MapToOfferListItemData));

            return list;
        }

        private OfferListItem MapToOfferListItemData(DTOs.OfferListItem data)
        {
            if (data == null)
                return null;

            var model = new OfferListItem
            {
                OfferListId = data.OfferListId,
                OfferId = data.OfferId,
                ExcludedCountries = data.ExcludedCountries,
                CountryCode = data.CountryCode,
                DisplayOrder = data.DisplayOrder,
                DisplayFrom = data.DisplayFrom,
                DisplayTo = data.DisplayTo
            };

            if (data.OfferList != null)
            {
                model.OfferList = new Data.Models.OfferList
                {
                    Id = data.OfferList.Id,
                    ListName = data.OfferList.ListName,
                    Description = data.OfferList.Description,
                    MaxSize = data.OfferList.MaxSize,
                    IsActive = data.OfferList.IsActive,
                    IncludeShowAllLink = data.OfferList.IncludeShowAllLink,
                    PermissionLevel = data.OfferList.PermissionLevel,
                    ShowAllLinkCaption = data.OfferList.ShowAllLinkCaption,
                };
            }

            if (data.Offer != null)
            {
                model.Offer = new Offer
                {
                    Id = data.Offer.Id,
                    MerchantId = data.Offer.MerchantId,
                    AffiliateId = data.Offer.AffiliateId,
                    OfferTypeId = data.Offer.OfferTypeId,
                    StatusId = data.Offer.StatusId,
                    ValidFrom = data.Offer.ValidFrom,
                    ValidTo = data.Offer.ValidTo,
                    Validindefinately = data.Offer.Validindefinately,
                    ShortDescription = data.Offer.ShortDescription,
                    LongDescription = data.Offer.LongDescription,
                    Instructions = data.Offer.Instructions,
                    Terms = data.Offer.Terms,
                    Exclusions = data.Offer.Exclusions,
                    LinkUrl = data.Offer.LinkUrl,
                    OfferCode = data.Offer.OfferCode,
                    Reoccuring = data.Offer.Reoccuring,
                    SearchRanking = data.Offer.SearchRanking,
                    Datecreated = data.Offer.Datecreated,
                    Headline = data.Offer.Headline,
                    AffiliateReference = data.Offer.AffiliateReference,
                    DateUpdated = data.Offer.DateUpdated,
                    RedemptionAccountNumber = data.Offer.RedemptionAccountNumber,
                    RedemptionProductCode = data.Offer.RedemptionProductCode
                };
            }

            return model;
        }

        private DTOs.OfferList MapToOfferListDto(OfferList data)
        {
            if (data == null)
                return null;

            var dto = new DTOs.OfferList
            {
                Id = data.Id,
                ListName = data.ListName,
                Description = data.Description,
                MaxSize = data.MaxSize,
                IsActive = data.IsActive,
                IncludeShowAllLink = data.IncludeShowAllLink,
                ShowAllLinkCaption = data.ShowAllLinkCaption,
                PermissionLevel = data.PermissionLevel
            };

            if (data.OfferListItems != null && data.OfferListItems.Count > 0)
            {
                dto.OfferListItems = new List<DTOs.OfferListItem>();
                dto.OfferListItems = MapToOfferListItemsDto(data.OfferListItems.ToList());
            }

            return dto;
        }

        #endregion PrivateMethods
    }
}