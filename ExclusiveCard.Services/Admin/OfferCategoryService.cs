using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Interfaces.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Admin
{
   public class OfferCategoryService : IOfferCategoryService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferCategoryManager _offerCategoryManager;

        #endregion

        #region Constructor

        public OfferCategoryService(IMapper mapper, IOfferCategoryManager offerCategoryManager)
        {
            _mapper = mapper;
            _offerCategoryManager = offerCategoryManager;
        }

        #endregion

        #region Writes

        //Add OfferCategory
        public async Task<DTOs.OfferCategory> Add(DTOs.OfferCategory offerCategory)
        {
            OfferCategory req = _mapper.Map<OfferCategory>(offerCategory);
            return _mapper.Map<DTOs.OfferCategory>( 
                await _offerCategoryManager.Add(req));
        }

        //Update OfferCategory
        public async Task<DTOs.OfferCategory> Update(DTOs.OfferCategory offerCategory)
        {
            OfferCategory req = _mapper.Map<OfferCategory>(offerCategory);
            return _mapper.Map<DTOs.OfferCategory>(
                await _offerCategoryManager.Update(req));
        }

        //Delete OfferCategory
        public async Task Delete(int offerId)
        {
           await _offerCategoryManager.Delete(offerId);
        }
        #endregion

        #region Reads

        //Get All OfferCountry
        public async Task<List<DTOs.OfferCategory>> GetAll(int offerId)
        {
            return MapToDtos(
                await _offerCategoryManager.GetAll(offerId));
        }

        #endregion

        private List<DTOs.OfferCategory> MapToDtos(List<OfferCategory> data)
        {
            if (data == null || data.Count == 0)
                return null;
            List<DTOs.OfferCategory> dtos = new List<DTOs.OfferCategory>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private DTOs.OfferCategory MapToDto(OfferCategory data)
        {
            if (data == null)
                return null;
            return new DTOs.OfferCategory
            {
                OfferId = data.OfferId,
                CategoryId = data.CategoryId
            };
        }
    }
}
