using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ST = ExclusiveCard.Data.StagingModels;
using dto = ExclusiveCard.Services.Models.DTOs.StagingModels;
using ExclusiveCard.Services.Interfaces.Admin;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ExclusiveCard.Services.Admin
{
    [Obsolete("This is the old Offer File Import service. Has/about to be replaced by OfferImportService")]
    public class OfferImportFileService : IOfferImportFileService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferImportFileManager _offerImportFileManager;

        #endregion

        #region Constructor

        public OfferImportFileService(IMapper mapper, IOfferImportFileManager offerImportFileManager)
        {
            _mapper = mapper;
            _offerImportFileManager = offerImportFileManager;
        }

        #endregion

        #region Writes


        public async Task<Models.DTOs.StagingModels.OfferImportFile> Add(dto.OfferImportFile offerImportFile)
        {
            ST.OfferImportFile req = MapToData(offerImportFile);
            return MapToDto(
               await _offerImportFileManager.Add(req));
        }


        public async Task<Models.DTOs.StagingModels.OfferImportFile> Update(dto.OfferImportFile offerImportFile)
        {
            ST.OfferImportFile req = MapToData(offerImportFile);
            return MapToDto(
                await _offerImportFileManager.Update(req));
        }

        #endregion

        #region Reads

        public  Models.DTOs.StagingModels.OfferImportFile Get(int affiliateId, int fileTypeId, int status)
        {
            return MapToDto(
                 _offerImportFileManager.Get(affiliateId, fileTypeId, status));
        }

        public async Task<Models.DTOs.StagingModels.OfferImportFile> GetById(int id, int? status = 0)
        {
            return MapToDto(
                await _offerImportFileManager.GetById(id, status));
        }

        public async Task<List<Models.DTOs.StagingModels.OfferImportFile>> GetAllAsync(int? importStatus)
        {
            return MapToListDto(
                await _offerImportFileManager.GetAllAsync(importStatus));
        }

        #endregion

        private List<Models.DTOs.StagingModels.OfferImportFile> MapToListDto(List<ST.OfferImportFile> data)
        {
            List<Models.DTOs.StagingModels.OfferImportFile>
                list = new List<Models.DTOs.StagingModels.OfferImportFile>();

            list.AddRange(data.Select(MapToDto));

            return list;
        }

        private Models.DTOs.StagingModels.OfferImportFile MapToDto(ST.OfferImportFile data)
        {
            if (data == null)
                return null;

            var dto = new Models.DTOs.StagingModels.OfferImportFile
            {
                Id = data.Id,
                AffiliateFileId = data.AffiliateFileId,
                DateImported = data.DateImported,
                FilePath = data.FilePath,
                ErrorFilePath = data.ErrorFilePath,
                ImportStatus = data.ImportStatus,
                TotalRecords = data.TotalRecords,
                Imported = data.Imported,
                Failed = data.Failed,
                Staged = data.Staged,
                CountryCode = data.CountryCode,
                Duplicates = data.Duplicates,
                Updates = data.Updates
            };

            if (data.AffiliateFile != null)
            {
                dto.AffiliateFile = new Models.DTOs.AffiliateFile
                {
                    Id = data.AffiliateFile.Id,
                    AffiliateId = data.AffiliateFile.AffiliateId,
                    FileName = data.AffiliateFile.FileName,
                    Description = data.AffiliateFile.Description,
                    StagingTable = data.AffiliateFile.StagingTable,
                    AffiliateFileMappingId = data.AffiliateFile.AffiliateFileMappingId
                };
            }

            if (data.OfferImportAwins != null && data.OfferImportAwins.Count > 0)
            {
                dto.OfferImportAwins = new List<Models.DTOs.StagingModels.OfferImportAwin>();
                foreach (var import in data.OfferImportAwins)
                {
                    dto.OfferImportAwins.Add(MapToOfferImportAwinDto(import));
                }
            }

            return dto;
        }

        private Models.DTOs.StagingModels.OfferImportAwin MapToOfferImportAwinDto(ST.OfferImportAwin data)
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

        private ST.OfferImportFile MapToData(dto.OfferImportFile data)
        {
            if (data == null)
                return null;

            var dto = new ST.OfferImportFile
            {
                Id = data.Id,
                AffiliateFileId = data.AffiliateFileId,
                DateImported = data.DateImported,
                FilePath = data.FilePath,
                ErrorFilePath = data.ErrorFilePath,
                ImportStatus = data.ImportStatus,
                TotalRecords = data.TotalRecords,
                Imported = data.Imported,
                Failed = data.Failed,
                Staged = data.Staged,
                CountryCode = data.CountryCode,
                Duplicates = data.Duplicates,
                Updates = data.Updates
            };

            return dto;
        }
    }
}
