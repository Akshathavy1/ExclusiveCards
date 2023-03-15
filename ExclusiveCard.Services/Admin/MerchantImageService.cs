using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class MerchantImageService : IMerchantImageService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IMerchantImageManager _merchantImageManager;

        #endregion

        #region Contructor

        public MerchantImageService(IMapper mapper, IMerchantImageManager merchantImageManager)
        {
            _mapper = mapper;
            _merchantImageManager = merchantImageManager;
        }

        #endregion

        #region Writes

        //Add merchantImage
        public async Task<Models.DTOs.MerchantImage> Add(Models.DTOs.MerchantImage merchantImage)
        {
            MerchantImage req = MapToEntityModel(merchantImage);
            return MapToDto(await _merchantImageManager.Add(req));
        }

        //Update merchantImage
        public async Task<Models.DTOs.MerchantImage> Update(Models.DTOs.MerchantImage merchantImage)
        {
            MerchantImage req = MapToEntityModel(merchantImage);
            return MapToDto(
                await _merchantImageManager.Update(req));
        }

        //Delete merchantImage
        public async Task<int> Delete(int merchantId, short? displayOrder)
        {
          return await _merchantImageManager.Delete(merchantId, displayOrder);
        }

        //Delete merchantImage
        public async Task<int> DeleteByMerchantIdAndType(int merchantId, int type)
        {
            return await _merchantImageManager.DeleteByMerchantIdAndType(merchantId, type);
        }

        public async Task<string> DeleteByMerchantImagePath(string path)
        {
            return await _merchantImageManager.DeleteByMerchantImagePath(path);
        }

        #endregion

        #region Reads

        //Get merchant
        public Models.DTOs.MerchantImage Get(int merchantId)
        {
            return ManualMappings.MapMerchantImage(_merchantImageManager.Get(merchantId));
        }

        //Get all merchants
        public async Task<List<Models.DTOs.MerchantImage>> GetAll(int merchantId, string imageSize, short? displayOrder)
        {
            return ManualMappings.MapMerchantImages(await _merchantImageManager.GetAll(merchantId, imageSize, displayOrder));
        }

        public async Task<Models.DTOs.MerchantImage> GetByIdAsync(int id)
        {
            return ManualMappings.MapMerchantImage(await _merchantImageManager.GetByIdAsync(id));
        }

        #endregion

        #region Private Members

        private MerchantImage MapToEntityModel(Models.DTOs.MerchantImage dto)
        {
            if (dto == null)
                return null;

            return new MerchantImage
            {
                Id = dto.Id,
                MerchantId = dto.MerchantId,
                ImagePath = dto.ImagePath,
                DisplayOrder = dto.DisplayOrder,
                TimeStamp = dto.TimeStamp,
                ImageType = dto.ImageType
            };
        }

        private Models.DTOs.MerchantImage MapToDto(MerchantImage data)
        {
            if (data == null)
                return null;

            return new Models.DTOs.MerchantImage
            {
                Id = data.Id,
                MerchantId = data.MerchantId,
                ImagePath = data.ImagePath,
                DisplayOrder = data.DisplayOrder,
                TimeStamp = data.TimeStamp,
                ImageType = data.ImageType
            };
        }

        #endregion
    }
}
