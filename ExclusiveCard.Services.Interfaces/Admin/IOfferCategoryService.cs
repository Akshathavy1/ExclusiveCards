using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IOfferCategoryService
    {
        #region Writes

        Task<OfferCategory> Add(OfferCategory offerCategory);

        Task<OfferCategory> Update(OfferCategory offerCategory);

        Task Delete(int offerId);

        #endregion

        #region Reads

        Task<List<OfferCategory>> GetAll(int offerId);

        #endregion
    }
}
