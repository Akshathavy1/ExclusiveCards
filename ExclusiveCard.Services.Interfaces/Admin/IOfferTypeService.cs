using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.Services.Interfaces.Admin
{
   public interface IOfferTypeService
    {
        #region Writes

        Task<OfferType> Add(OfferType offerType);

        Task<OfferType> Update(OfferType offerType);

        #endregion

        #region Reads

        Task<OfferType> GetByName(string name);

        Task<List<OfferType>> GetAll();

        #endregion
    }
}
