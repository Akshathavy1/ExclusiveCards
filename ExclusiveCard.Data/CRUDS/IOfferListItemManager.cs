using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferListItemManager
    {
        Task<OfferListItem> Add(OfferListItem offerListItem);

        Task<List<OfferListItem>> AddListOfferListItems(List<OfferListItem> offerListItem);

        Task<OfferListItem> Update(OfferListItem offerListItem);

        Task<OfferListItem> Delete(OfferListItem offerListItem, bool saveChanges = true);

        Task DeleteByOfferListId(int id);

        Task<bool> ReorderOffersOnSelectedPostition(int itemListId, string countryCode, int order, int increment);

        Task<List<OfferListItem>> GetAllByOfferId(int offerId);

        Task<OfferListItem> GetByOfferIdCountryandListId(int offerId, int itemListId, string countryCode);

        Task<List<OfferListItem>> GetByCountryandListId(int itemListId, string countryCode);

        Task<List<OfferListItem>> GetAll();

        Task<List<OfferListItem>> GetAllByOfferListId(int offerListId);

        Task<List<OfferListItem>> GetByOfferListIdForCountry(int offerListId, string countryCode, int pageSize,
            int? merchantId);

        Task<PagedResult<OfferListItem>> GetPagedOffersForListItem(int listItemId, string countrycode, int page,
            int pageSize);

        Task<List<OfferListDataModel>> GetOfferListDataModels(string countryCode, List<int> categories, int pageSize, string listName);

        //local offer
        Task<List<OfferListDataModel>> GetLocalOfferListDataModels(string countryCode, int regionId);
    }
}