using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferTagManager
    {
        Task<OfferTag> Add(OfferTag offerTag);
        Task Delete(int offerId);
        Task<List<OfferTag>> GetAll(int id);
    }
}