using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Admin
{
    public class OfferCountryService : IOfferCountryService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferCountryManager _offerCountryManager;

        #endregion

        #region Constructor

        public OfferCountryService(IMapper mapper, IOfferCountryManager offerCountryManager)
        {
            _mapper = mapper;
            _offerCountryManager = offerCountryManager;
        }

        #endregion

        #region Writes

        //Add OfferCountry
        public async Task<Models.DTOs.OfferCountry> Add(DTOs.OfferCountry offerCountry)
        {
            OfferCountry req = _mapper.Map<OfferCountry>(offerCountry);
            return  _mapper.Map<Models.DTOs.OfferCountry>(
               await _offerCountryManager.Add(req));
        }

        //Update OfferCountry
        public async Task<Models.DTOs.OfferCountry> Update(DTOs.OfferCountry offerCountry)
        {
            OfferCountry req = _mapper.Map<OfferCountry>(offerCountry);
            return _mapper.Map<Models.DTOs.OfferCountry>(
               await _offerCountryManager.Update(req));
        }

        //Delete OfferCountry
        public async Task Delete(int offerId)
        {
           await _offerCountryManager.Delete(offerId);
        }

        #endregion

        #region Reads

        //Get All OfferCountry
        public async Task<List<Models.DTOs.OfferCountry>> GetAll(int offerId)
        {
            return MapToDtos(await _offerCountryManager.GetAll(offerId));
        }

        #endregion

        private List<Models.DTOs.OfferCountry> MapToDtos(List<OfferCountry> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<Models.DTOs.OfferCountry> dtos = new List<Models.DTOs.OfferCountry>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private Models.DTOs.OfferCountry MapToDto(OfferCountry data)
        {
            if (data == null)
                return null;
            return new Models.DTOs.OfferCountry
            {
                OfferId = data.OfferId,
                CountryCode = data.CountryCode
            };
        }
    }
}
