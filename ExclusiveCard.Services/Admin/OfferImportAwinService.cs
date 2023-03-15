using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ST = ExclusiveCard.Data.StagingModels;
using ExclusiveCard.Services.Interfaces.Admin;
using dto = ExclusiveCard.Services.Models.DTOs.StagingModels;

using System.Collections.Generic;
using System.Linq;

namespace ExclusiveCard.Services.Admin
{
   public class OfferImportAwinService : IOfferImportAwinService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferImportAwinManager _offerImportAwinManager;

        #endregion

        #region Constructor

        public OfferImportAwinService(IMapper mapper, IOfferImportAwinManager offerImportAwinManager)
        {
            _mapper = mapper;
            _offerImportAwinManager = offerImportAwinManager;
        }

        #endregion

        #region Writes


        public async Task<Models.DTOs.StagingModels.OfferImportAwin> Add(dto.OfferImportAwin offerImportAwin)
        {
            ST.OfferImportAwin req = MapToData(offerImportAwin);
            return MapToDto(
               await _offerImportAwinManager.Add(req));
        }


        public async Task<Models.DTOs.StagingModels.OfferImportAwin> Update(dto.OfferImportAwin offerImportAwin)
        {
            ST.OfferImportAwin req = MapToData(offerImportAwin);
            return MapToDto(
                await _offerImportAwinManager.Update(req));
        }

        public async Task Delete(List<dto.OfferImportAwin> awins)
        {
            var dataToBeDeleted = MapToList(awins);
            await _offerImportAwinManager.Delete(dataToBeDeleted);
        }

        public async Task<List<Models.DTOs.StagingModels.OfferImportAwin>> AddToAwinAsync(List<dto.OfferImportAwin> offerImportAwins)
        {
            List<ST.OfferImportAwin> req = MapToList(offerImportAwins);
            return MapToListDto(await _offerImportAwinManager.AddToAwinAsync(req));
        }

        #endregion

        #region Reads

        public async Task<List<Models.DTOs.StagingModels.OfferImportAwin>> GetAll(int? importfileId)
        {
            return MapToListDto(
                await _offerImportAwinManager.GetAllAsync(importfileId));
        }

        #endregion

        private List<ST.OfferImportAwin> MapToList(List<dto.OfferImportAwin> data)
        {
            List<ST.OfferImportAwin>
                list = new List<ST.OfferImportAwin>();

            list.AddRange(data.Select(MapToData));

            return list;
        }

        private List<Models.DTOs.StagingModels.OfferImportAwin> MapToListDto(List<ST.OfferImportAwin> data)
        {
            List<Models.DTOs.StagingModels.OfferImportAwin>
                list = new List<Models.DTOs.StagingModels.OfferImportAwin>();

            list.AddRange(data.Select(MapToDto));

            return list;
        }

        private Models.DTOs.StagingModels.OfferImportAwin MapToDto(ST.OfferImportAwin data)
        {
            if (data == null)
                return null;
            return new Models.DTOs.StagingModels.OfferImportAwin
            {
                Id = data.Id,
                OfferImportFileId = data.OfferImportFileId,
                PromotionId = data.PromotionId,
                Advertiser = data.Advertiser,
                AdvertiserId = data.AdvertiserId,
                Type = data.Type,
                Code = data.Code,
                Description = data.Description,
                Starts = data.Starts,
                Ends = data.Ends,
                Categories = data.Categories,
                Regions = data.Regions,
                Terms = data.Terms,
                DeeplinkTracking = data.DeeplinkTracking,
                Deeplink = data.Deeplink,
                CommissionGroups = data.CommissionGroups,
                Commission = data.Commission,
                Exclusive = data.Exclusive,
                DateAdded = data.DateAdded,
                Title = data.Title
            };
        }

        private ST.OfferImportAwin MapToData(dto.OfferImportAwin data)
        {
            if (data == null)
                return null;
            return new ST.OfferImportAwin
            {
                Id = data.Id,
                OfferImportFileId = data.OfferImportFileId,
                PromotionId = data.PromotionId,
                Advertiser = data.Advertiser,
                AdvertiserId = data.AdvertiserId,
                Type = data.Type,
                Code = data.Code,
                Description = data.Description,
                Starts = data.Starts,
                Ends = data.Ends,
                Categories = data.Categories,
                Regions = data.Regions,
                Terms = data.Terms,
                DeeplinkTracking = data.DeeplinkTracking,
                Deeplink = data.Deeplink,
                CommissionGroups = data.CommissionGroups,
                Commission = data.Commission,
                Exclusive = data.Exclusive,
                DateAdded = data.DateAdded,
                Title = data.Title
            };
        }
    }
}
