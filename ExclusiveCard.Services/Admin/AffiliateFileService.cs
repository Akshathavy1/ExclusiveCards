using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Services.Admin
{
   public class AffiliateFileService : IAffiliateFileService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly IAffiliateFileManager _affiliateFileManager;

        #endregion

        #region Constuctor

        public AffiliateFileService(IMapper mapper, IAffiliateFileManager affiliateFileManager)
        {
            _affiliateFileManager = affiliateFileManager;
            _mapper = mapper;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.AffiliateFile> Add(Models.DTOs.AffiliateFile affiliateFile)
        {
            AffiliateFile req = MapToData(affiliateFile);
            return MapToDto(await _affiliateFileManager.Add(req));
        }

        public async Task<Models.DTOs.AffiliateFile> Update(Models.DTOs.AffiliateFile affiliateFile)
        {
            AffiliateFile req = MapToData(affiliateFile);
            return MapToDto(await _affiliateFileManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.AffiliateFile> Get(int id)
        {
            return MapToDto(await _affiliateFileManager.Get(id));
        }

        public async Task<List<Models.DTOs.AffiliateFile>> GetByAffiliateAsync(int id)
        {
            return MapToListDto(await _affiliateFileManager.GetByAffiliateAsync(id));
        }

        public async Task<List<Models.DTOs.AffiliateFile>> GetAll()
        {
            return MapToListDto(await _affiliateFileManager.GetAll());
        }

        #endregion

        #region Private Methods

        private List<Models.DTOs.AffiliateFile> MapToListDto(List<AffiliateFile> data)
        {
            if (data == null || data.Count == 0)
                return null;

            var dtoList = new List<Models.DTOs.AffiliateFile>();

            dtoList.AddRange(data.Select(MapToDto));

            return dtoList;
        }

        private Models.DTOs.AffiliateFile MapToDto(AffiliateFile data)
        {
            if (data == null)
                return null;
            var dto = new Models.DTOs.AffiliateFile
            {
                Id = data.Id,
                AffiliateId = data.AffiliateId,
                FileName = data.FileName,
                Description = data.Description,
                StagingTable = data.StagingTable,
                AffiliateFileMappingId = data.AffiliateFileMappingId
            };

            if (data.Affiliate != null)
            {
                dto.Affiliate = new Models.DTOs.Affiliate
                {
                    Id = data.Affiliate.Id,
                    Name = data.Affiliate.Name
                };
            }

            if (data.AffiliateFileMapping != null)
            {
                dto.AffiliateFileMapping = new Models.DTOs.AffiliateFileMapping
                {
                    Id = data.AffiliateFileMapping.Id,
                    AffiliateId = data.AffiliateFileMapping.AffiliateId,
                    Description = data.AffiliateFileMapping.Description,

                };
            }

            return dto;
        }

        private AffiliateFile MapToData(Models.DTOs.AffiliateFile req)
        {
            if (req == null)
                return null;
            var data = new AffiliateFile
            {
                Id = req.Id,
                AffiliateId = req.AffiliateId,
                FileName = req.FileName,
                Description = req.Description,
                StagingTable = req.StagingTable,
                AffiliateFileMappingId = req.AffiliateFileMappingId
            };

            if (req.Affiliate != null)
            {
                data.Affiliate = new Affiliate
                {
                    Id = data.Affiliate.Id,
                    Name = data.Affiliate.Name
                };
            }

            if (req.AffiliateFileMapping != null)
            {
                data.AffiliateFileMapping = new AffiliateFileMapping
                {
                    Id = data.AffiliateFileMapping.Id,
                    AffiliateId = data.AffiliateFileMapping.AffiliateId,
                    Description = data.AffiliateFileMapping.Description,

                };
            }

            return data;
        }

        #endregion
    }
}
