using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IOfferService
    {
        #region Writes

        Task<Models.DTOs.Offer> Add(Offer offer);

        Task<List<Models.DTOs.Offer>> AddBulkAsync(List<Offer> offers);

        Task<Models.DTOs.Offer> Update(Offer offer);

        Task<Models.DTOs.OfferListItem> UpdateOfferList(OfferListItem offerListItem);

        Task<Models.DTOs.OfferListItem> AddOfferListItem(OfferListItem offerList);

        Task<List<Models.DTOs.OfferListItem>> AddOfferListItemList(List<OfferListItem> offerList);

        Task<Models.DTOs.OfferListItem> DeleteOfferListItem(OfferListItem offerList, bool saveChanges = true);

        Task DeleteOfferListItemsById(int id);

        Task<bool> ReorderOffersOnSelectedPostition(int offerListId, string countryCode, int order, int increment);

        //Task ExecuteSPMappingSTOfferToOffer(int affiliateId, int offerImportId, int recordsToProcess);

        Task BulkUpdateAsync(List<Offer> offers);

        #endregion Writes

        #region Reads

        Task<Models.DTOs.Offer> Get(int id, bool includeMerchant = false, bool includeOfferType = false,
            bool includeStatus = false, bool includeAffiliate = false, bool includeCategories = false);

        Task<List<Models.DTOs.Offer>> GetAll(bool includeMerchant = false, bool includeOfferType = false, bool includeStatus = false);

        Task<List<Models.DTOs.Admin.OfferSummary>> Search(OfferSearchCriteria searchCriteria);

        Task<Models.DTOs.PagedResult<Models.DTOs.Admin.OfferSummary>> PagedSearchOffersandOfferList(OfferSearchCriteria searchCriteria);

        Task<Models.DTOs.PagedResult<Models.DTOs.Admin.OfferSummary>> PagedSearch(int? merchantId, int? affiliateId, int? typeId, int? statusId,
            string keyword, DateTime? validFrom, DateTime? validTo, int page, int pageSize, OffersSortOrder sortOrder);

        Task<Models.DTOs.PagedResult<Models.DTOs.Admin.OfferSummary>> GetAllPagedSearch(int page, int pageSize);

        Task<Models.DTOs.PagedResult<Models.DTOs.Admin.OfferSummary>> PagedSearchOfferList(int? merchantId, int? affiliateId, int? typeId, int? statusId, string keyword, string countryCode, DateTime? validFrom, DateTime? validTo, int page, int pageSize);

        Task<Models.DTOs.PagedResult<Models.DTOs.Admin.OfferSummary>> PagedSearchOfferListitems(int listItemId, string CountryCode, int page, int pageSize);

        Task<List<Models.DTOs.OfferList>> GetAllOfferList();

        Task<Models.DTOs.OfferListItem> GetByOfferIdandCountry(int offerId, int listId, string countryCode);

        Task<List<Models.DTOs.OfferListItem>> GetByListIdAndCountry(int listId, string countryCode);

        Task<Models.DTOs.OfferList> GetOfferListByName(string name);

        Task<List<Tuple<string, int>>> CheckIfOfferExistsForAffiliate(int affiliateId, List<string> affiliateReferences);

        Task<List<Models.DTOs.Offer>> GetByMerchant(int merchantId);

        Task<List<Models.DTOs.OfferListItem>> GetAllByOfferId(int offerId);

        Task<Models.DTOs.OfferMerchantBranch> AddOfferMerchantBranch(OfferMerchantBranch offerBranch);

        Task DeleteOfferBranch(int offerId);

        #endregion Reads
    }
}