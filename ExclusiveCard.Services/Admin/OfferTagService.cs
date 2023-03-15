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
    public class OfferTagService : IOfferTagService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferTagManager _offerTagManager;

        #endregion

        #region Contructor

        public OfferTagService(IMapper mapper, IOfferTagManager offerTagManager)
        {
            _mapper = mapper;
            _offerTagManager = offerTagManager;
        }

        #endregion

        #region Writes

        //Add OfferTag
        public async Task<DTOs.OfferTag> Add(DTOs.OfferTag offerTag)
        {
            OfferTag req = _mapper.Map<OfferTag>(offerTag);
            return _mapper.Map<DTOs.OfferTag>(
                await _offerTagManager.Add(req));
        }
      
        #endregion

        public async Task Delete(int offerId)
        {
           await _offerTagManager.Delete(offerId);
        }

        #region Reads

        //Get all offerTag
        public async Task<List<DTOs.OfferTag>> GetAll(int offerId)
        {
            return MapToDtos(await _offerTagManager.GetAll(offerId));
        }

        #endregion

        private List<DTOs.OfferTag> MapToDtos(List<OfferTag> data)
        {
            if (data == null || data.Count == 0)
                return null;
            List<DTOs.OfferTag> dtos = new List<DTOs.OfferTag>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private DTOs.OfferTag MapToDto(OfferTag data)
        {
            if (data == null)
                return null;
            var offerTag = new DTOs.OfferTag
            {
                OfferId = data.OfferId,
                TagId = data.TagId
            };

            if (data.Tag != null)
            {
                offerTag.Tag = new DTOs.Tag
                {
                    Id = data.Tag.Id,
                    Tags = data.Tag.Tags,
                    TagType = data.Tag.TagType,
                    IsActive = data.Tag.IsActive
                };
            }

            return offerTag;
        }
    }
}
