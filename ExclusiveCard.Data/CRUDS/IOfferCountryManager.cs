using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferCountryManager
    {
        Task<OfferCountry> Add(OfferCountry offerCountry);
        Task<OfferCountry> Update(OfferCountry offerCountry);
        Task Delete(int offerId);
        Task<List<OfferCountry>> GetAll(int offerId);
    }
}