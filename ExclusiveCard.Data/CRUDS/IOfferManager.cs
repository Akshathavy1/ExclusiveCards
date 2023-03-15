using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferManager
    {
        Task<Offer> Add(Offer offer);

        Task<List<Offer>> AddBulkAsync(List<Offer> offers);

        Task<Offer> Update(Offer offer);

        Task BulkUpdate(List<Offer> offer);

        //Task ExecuteSPMappingSTOfferToOffer(int affiliateTypeId, int offerImportId, int recordsToProcess);

        Task<Offer> Get(int id, bool includeMerchant, bool includeOfferType, bool includeStatus, bool includeAffiliate, bool includeCategories);

        Task<List<Offer>> GetAll(bool includeMerchant, bool includeOfferType, bool includeStatus);

        Task<PagedResult<Offer>> GetAllPaged(int page, int pageSize);

        Task<List<Offer>> GetSearchList(int? merchantId, int? affiliateId, int? typeId, int? statusId, string keyword,
            DateTime? validFrom, DateTime? validTo);

        Task<PagedResult<Offer>> GetPagedSearch(int? merchantId, int? affiliateId,
            int? typeId, int? statusId, string keyword, DateTime? validFrom,
            DateTime? validTo, string countryCode, List<int> categories, string merchantName,
            List<int> offerTypes, int pageSize, int currentPage, int sortOrder);

        Task<PagedResult<Offer>> GetPagedSearchList(int? merchantId, int? affiliateId, int? typeId, int? statusId,
            string keyword, DateTime? validFrom, DateTime? validTo, int page, int pageSize, OffersSortOrder sortOrder);

        Task<PagedResult<Offer>> GetPublicPagedResult(int? merchantId, int? offerType,
            string countryCode, DateTime? validFrom, DateTime? validTo,
            int? sortOrder, List<int> categories, string keyword, string merchantName, List<int> offerTypes,
            int pageSize, int currentPage, int? excludeMerchantId);

        Task<List<Offer>> GetPublicListofOffersAsync(int? merchantId, int? offerType,
            string countryCode, DateTime? validFrom, DateTime? validTo,
            int? sortOrder, List<int> categories, string keyword, string merchantName, List<int> offerTypes);

        Task<PagedResult<Offer>> GetPublicMainSearchResult(int? offerId, int? offerType, string countryCode,
            string searchTerm, int pageSize, int currentPage, int sortOrder, List<int> categories, List<int> offerTypes);

        Task<PagedResult<Offer>> GetPagedSearchOffersList(int? merchantId, int? affiliateId, int? typeId, int? statusId,
            string keyword, string countryCode, DateTime? validFrom, DateTime? validTo, int page, int pageSize);

        Task<List<Tuple<string, int>>> CheckIfOfferExistsForAffiliate(int affiliateId,
            List<string> affiliateReferences);

        Task<List<Offer>> GetByMerchantId(int merchantId);

        Task<OfferMerchantBranch> AddOfferBranch(OfferMerchantBranch offerBranch);

        Task DeleteOfferBranch(int offerId);
    }
}