using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferTypeManager
    {
        Task<OfferType> Add(OfferType offerType);
        Task<OfferType> Update(OfferType offerType);
        Task<OfferType> Get(string offerTypeName);
        Task<List<OfferType>> GetAll();
    }
}