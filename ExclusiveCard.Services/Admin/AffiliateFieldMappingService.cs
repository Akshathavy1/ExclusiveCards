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
   public class AffiliateFieldMappingService: IAffiliateFieldMappingService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly IAffiliateFieldMappingManager _affiliateFieldMappingManager;

        #endregion

        #region Constuctor

        public AffiliateFieldMappingService(IMapper mapper, IAffiliateFieldMappingManager affiliateFieldMappingManager)
        {
            _affiliateFieldMappingManager = affiliateFieldMappingManager;
            _mapper = mapper;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.AffiliateFieldMapping> Add(Models.DTOs.AffiliateFieldMapping affiliateFieldMapping)
        {
            AffiliateFieldMapping req = MapToData(affiliateFieldMapping);
            return MapToDto(
                await _affiliateFieldMappingManager.Add(req));
        }

        public async Task<Models.DTOs.AffiliateFieldMapping> Update(Models.DTOs.AffiliateFieldMapping affiliateFieldMapping)
        {
            AffiliateFieldMapping req = MapToData(affiliateFieldMapping);
            return MapToDto(
                await _affiliateFieldMappingManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.AffiliateFieldMapping> Get(int id)
        {
            return MapToDto(await _affiliateFieldMappingManager.Get(id));
        }

        public async Task<List<Models.DTOs.AffiliateFieldMapping>> GetAll(int affiliateFileMappingid)
        {
            return MapToDtoList(await _affiliateFieldMappingManager.GetAll(affiliateFileMappingid));
        }

        #endregion

        #region Private Methods

        private List<Models.DTOs.AffiliateFieldMapping> MapToDtoList(List<AffiliateFieldMapping> data)
        {
            if (data == null || data.Count == 0)
                return null;

            var dtoList = new List<Models.DTOs.AffiliateFieldMapping>();

            dtoList.AddRange(data.Select(MapToDto));

            return dtoList;
        }

        private Models.DTOs.AffiliateFieldMapping MapToDto(AffiliateFieldMapping data)
        {
            if (data == null)
                return null;

            var dto = new Models.DTOs.AffiliateFieldMapping
            {
                Id = data.Id,
                AffiliateFileMappingId = data.AffiliateFileMappingId,
                AffiliateFieldName = data.AffiliateFieldName,
                ExclusiveTable = data.ExclusiveTable,
                ExclusiveFieldName = data.ExclusiveFieldName,
                IsList = data.IsList,
                Delimiter = data.Delimiter,
                AffiliateMappingRuleId = data.AffiliateMappingRuleId,
                AffiliateTransformId = data.AffiliateTransformId,
                AffiliateMatchTypeId = data.AffiliateMatchTypeId
            };

            if (data.AffiliateMappingRule != null)
            {
                dto.AffiliateMappingRule = new Models.DTOs.AffiliateMappingRule
                {
                    Id = data.AffiliateMappingRule.Id,
                    Description = data.AffiliateMappingRule.Description,
                    AffiliateId = data.AffiliateMappingRule.AffiliateId,
                    IsActive = data.AffiliateMappingRule.IsActive
                };
            }
            if (data.AffiliateFileMapping != null)
            {
                dto.AffiliateFileMapping = new Models.DTOs.AffiliateFileMapping
                {
                    Id = data.AffiliateFileMapping.Id,
                    AffiliateId = data.AffiliateFileMapping.AffiliateId,
                    Description = data.AffiliateFileMapping.Description
                };
            }

            return dto;
        }

        private AffiliateFieldMapping MapToData(Models.DTOs.AffiliateFieldMapping req)
        {
            if (req == null)
                return null;

            var data = new AffiliateFieldMapping
            {
                Id = req.Id,
                AffiliateFileMappingId = req.AffiliateFileMappingId,
                AffiliateFieldName = req.AffiliateFieldName,
                ExclusiveTable = req.ExclusiveTable,
                ExclusiveFieldName = req.ExclusiveFieldName,
                IsList = req.IsList,
                Delimiter = req.Delimiter,
                AffiliateMappingRuleId = req.AffiliateMappingRuleId,
                AffiliateTransformId = req.AffiliateTransformId,
                AffiliateMatchTypeId = req.AffiliateMatchTypeId
            };

            if (req.AffiliateMappingRule != null)
            {
                data.AffiliateMappingRule = new AffiliateMappingRule
                {
                    Id = req.AffiliateMappingRule.Id,
                    Description = req.AffiliateMappingRule.Description,
                    AffiliateId = req.AffiliateMappingRule.AffiliateId,
                    IsActive = req.AffiliateMappingRule.IsActive
                };
            }
            if (req.AffiliateFileMapping != null)
            {
                data.AffiliateFileMapping = new AffiliateFileMapping
                {
                    Id = req.AffiliateFileMapping.Id,
                    AffiliateId = req.AffiliateFileMapping.AffiliateId,
                    Description = req.AffiliateFileMapping.Description
                };
            }

            return data;
        }

        #endregion
    }
}
