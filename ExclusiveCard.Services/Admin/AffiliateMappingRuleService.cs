using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Services.Admin
{
   public class AffiliateMappingRuleService : IAffiliateMappingRuleService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly IAffiliateMappingRuleManager _affiliateMappingRuleManager;

        #endregion

        #region Constuctor

        public AffiliateMappingRuleService(IMapper mapper, IAffiliateMappingRuleManager affiliateMappingRuleManager)
        {
            _affiliateMappingRuleManager = affiliateMappingRuleManager;
            _mapper = mapper;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.AffiliateMappingRule> Add(Models.DTOs.AffiliateMappingRule affiliateMappingRule)
        {
            AffiliateMappingRule req = _mapper.Map<AffiliateMappingRule>(affiliateMappingRule);
            return MapToDto(
                await _affiliateMappingRuleManager.Add(req));
        }

        public async Task<Models.DTOs.AffiliateMappingRule> Update(Models.DTOs.AffiliateMappingRule affiliateMappingRule)
        {
            AffiliateMappingRule req = _mapper.Map<AffiliateMappingRule>(affiliateMappingRule);
            return MapToDto(
                await _affiliateMappingRuleManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.AffiliateMappingRule> Get(int id)
        {
            return MapToDto(await _affiliateMappingRuleManager.Get(id));
        }

        public async Task<Models.DTOs.AffiliateMappingRule> GetByDesc(string desc, int? affiliateId = null)
        {
            return MapToDto(await _affiliateMappingRuleManager.GetByDesc(desc, affiliateId));
        }

        public async Task<List<Models.DTOs.AffiliateMappingRule>> GetAllMappingRules(string desc,
            int? affilateId = null)
        {
            return MapToList(
                await _affiliateMappingRuleManager.GetAllMappingRules(desc, affilateId));
        }

        #endregion

        private List<Models.DTOs.AffiliateMappingRule> MapToList(List<AffiliateMappingRule> data)
        {
            if (data == null || data.Count == 0)
                return null;

            var dtoList = new List<Models.DTOs.AffiliateMappingRule>();

            dtoList.AddRange(data.Select(MapToDto));

            return dtoList;
        }

        private Models.DTOs.AffiliateMappingRule MapToDto(AffiliateMappingRule data)
        {
            if (data == null)
                return null;
            var dto = new Models.DTOs.AffiliateMappingRule
            {
                Id = data.Id,
                Description = data.Description,
                AffiliateId = data.AffiliateId,
                IsActive = data.IsActive
            };

            if (data.AffiliateMappings?.Count > 0)
            {
                dto.AffiliateMappings = new List<Models.DTOs.AffiliateMapping>();
                dto.AffiliateMappings = MapAffiliateMappingDtoList(data.AffiliateMappings.ToList());
            }

            return dto;
        }

        private List<Models.DTOs.AffiliateMapping> MapAffiliateMappingDtoList(List<AffiliateMapping> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<Models.DTOs.AffiliateMapping> mappings = new List<Models.DTOs.AffiliateMapping>();

            mappings.AddRange(data.Select(MapToAffiliateMappingDto));

            return mappings;
        }

        private Models.DTOs.AffiliateMapping MapToAffiliateMappingDto(AffiliateMapping data)
        {
            if (data == null)
                return null;

            return new Models.DTOs.AffiliateMapping
            {
                Id = data.Id,
                AffiliateMappingRuleId = data.AffiliateMappingRuleId,
                AffilateValue = data.AffilateValue,
                ExclusiveValue = data.ExclusiveValue
            };
        }
    }
}
