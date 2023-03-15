using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IOfferCountryService
    {
        #region Writes

        Task<Models.DTOs.OfferCountry> Add(OfferCountry offerCountry);

        Task<Models.DTOs.OfferCountry> Update(OfferCountry offerCountry);

        Task Delete(int offerId);

        #endregion
        
        #region Reads

        Task<List<Models.DTOs.OfferCountry>> GetAll(int offerId);

        #endregion
    }
}
