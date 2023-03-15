using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IOfferCategoryManager
    {
        Task<OfferCategory> Add(OfferCategory offerCategory);
        Task<OfferCategory> Update(OfferCategory offerCategory);
        Task Delete(int offerId);
        Task<List<OfferCategory>> GetAll(int offerId);
    }
}