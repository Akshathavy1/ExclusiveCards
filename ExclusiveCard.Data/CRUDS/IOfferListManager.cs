using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferListManager
    {
        Task<OfferList> Add(OfferList offerList);
        Task<OfferList> Update(OfferList offerList);
        Task<OfferList> Get(string listName, string countryCode);
        Task<List<OfferList>> GetAll(string countryCode);
        Task<List<OfferList>> GetAll();
        Task<OfferList> GetByName(string listName);
    }
}