using ExclusiveCard.Services.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IOffersService
    {
        #region Writes

        Task<Models.DTOs.OfferList> Add(OfferList offerList);
        Task<Models.DTOs.OfferList> Update(OfferList offerList);
        Models.DTOs.OfferRedemption AddRedemption(Models.DTOs.OfferRedemption redeem);
        Models.DTOs.OfferRedemption UpdateRedemption(Models.DTOs.OfferRedemption redeem);
        Task<bool> RevertPickedOfferRedemption(int fileId);

        #endregion

        Task<string> CreateLove2ShopRequestFile(string adminEmail, string connectionString, string containerName, string blobFolder);

        #region Reads

        Task<Models.DTOs.Offer> Get(int id, bool includeMerchant, bool includeOfferTypes, bool includeStatus, bool includeAffiliate, bool includeCategories);

        Task<List<Models.DTOs.Public.OfferSummary>> GetOffersByListName(string listName, string countryCode);

        Task<List<Models.DTOs.OfferList>> GetOffersForCountry(string countryCode);

        Task<byte[]> GetImage(string blobConnectionString, string containerName, string path);

        Task<List<Models.DTOs.Public.OfferSummary>> Search(OfferSearchCriteria criteria);

        Task<PagedResult<Models.DTOs.Public.OfferSummary>> PagedSearch(OfferSearchCriteria criteria);

        Task<List<Models.DTOs.OfferList>> GetAllOfferLists();

        Task<List<Models.DTOs.Public.OfferSummary>> GetByOfferListCountry(int offerListId, string countryCode, int pageSize, int? merchantId, bool biggerImage = false);

        Task<PagedResult<Models.DTOs.Public.OfferSummary>> GetMerchantOffersByType(int merchantId, int offerType, string countryCode, int pageSize, int currentPage);

        Task<List<Models.DTOs.Public.OfferSummary>> GetListofMerchantOffersByTypeAsync(int? merchantId, int? offerType,
            string countryCode);

        Task<List<Models.DTOs.OfferType>> GetAllOfferType();

        Task<PagedResult<Models.DTOs.Public.OfferSummary>> PagedMainSearch(OfferSearchCriteria criteria);

        Task<Models.DTOs.OfferType> GetOfferType(string offerTypeName);
        List<Models.DTOs.Category> GetParentCategories();

        Task<List<OfferListDataModel>> GetOfferListDataModels(string countryCode, List<int> categories, int pageSize, string listName);
        Task<List<OfferListDataModel>> GetLocalOfferListDataModels(string countryCode, int RegionalId);
        Task<List<Models.DTOs.OfferRedemption>> GetOfferRedemptions(int? statusId);
        Task<List<RedemptionDataModel>> GetRedemptionRequestAsync(string blobFolder);
        Models.DTOs.OfferRedemption GetOfferRedemption(int membershipCardId, int offerId);
        Task<List<Models.DTOs.Public.OfferSummary>> GetOfferByKeyword(string keyword);
        //Task<List<Models.DTOs.Public.OfferSummary>> GetOfferByKeyword(string keyword);

        #endregion
    }
}