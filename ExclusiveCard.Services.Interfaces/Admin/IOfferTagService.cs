using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IOfferTagService
    {
        #region Writes

        Task<OfferTag> Add(OfferTag offerTag);

        Task Delete(int offerId);

        #endregion

        #region Reads

        Task<List<OfferTag>> GetAll(int offerId);

        #endregion
    }
}
