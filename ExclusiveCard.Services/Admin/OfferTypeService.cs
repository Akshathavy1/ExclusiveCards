using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using DTOs = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.Services.Admin
{
    public class OfferTypeService : IOfferTypeService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferTypeManager _offerTypeManager;

        #endregion

        #region Contructor

        public OfferTypeService(IMapper mapper, IOfferTypeManager offerTypeManager)
        {
            _mapper = mapper;
            _offerTypeManager = offerTypeManager;
        }

        #endregion

        #region Writes

        //Add OfferType
        public async Task<DTOs.OfferType> Add(DTOs.OfferType offerType)
        {
            OfferType req = _mapper.Map<OfferType>(offerType);
            return _mapper.Map<DTOs.OfferType>(
                await _offerTypeManager.Add(req));
        }

        //Update OfferType
        public async Task<DTOs.OfferType> Update(DTOs.OfferType offerType)
        {
            OfferType req = _mapper.Map<OfferType>(offerType);
            return _mapper.Map<DTOs.OfferType>(
                await _offerTypeManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<DTOs.OfferType> GetByName(string name)
        {
            return _mapper.Map<DTOs.OfferType>(await _offerTypeManager.Get(name));

        }

        //Get all offerType
        public async Task<List<DTOs.OfferType>> GetAll()
        {
            return MapToList(await _offerTypeManager.GetAll());
        }

        #endregion

        #region Private Members

        private List<DTOs.OfferType> MapToList(List<Data.Models.OfferType> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<DTOs.OfferType> models = new List<DTOs.OfferType>();

            models.AddRange(data.Select(MapToDto));

            return models;
        }

        private DTOs.OfferType MapToDto(Data.Models.OfferType data)
        {
            if (data == null)
                return null;
            var model = new DTOs.OfferType()
            {
                Id = data.Id,
               Description = data.Description,
                IsActive = data.IsActive,
                ActionTextLocalisationId = data.ActionTextLocalisationId,
                TitleLocalisationId = data.TitleLocalisationId,
                SearchRanking = data.SearchRanking
            };

            return model;
        }

        #endregion

    }
}
