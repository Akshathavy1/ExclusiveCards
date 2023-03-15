using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
   public class AffiliateMappingService : IAffiliateMappingService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly IAffiliateMappingManager _affiliateMappingManager;

        #endregion

        #region Constuctor

        public AffiliateMappingService(IMapper mapper, IAffiliateMappingManager affiliateMappingManager)
        {
            _affiliateMappingManager = affiliateMappingManager;
            _mapper = mapper;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.AffiliateMapping> Add(Models.DTOs.AffiliateMapping affiliateMapping)
        {
            AffiliateMapping req = _mapper.Map<AffiliateMapping>(affiliateMapping);
            return MapToDto(
                await _affiliateMappingManager.Add(req));
        }

        public async Task<Models.DTOs.AffiliateMapping> Update(Models.DTOs.AffiliateMapping affiliateMapping)
        {
            AffiliateMapping req = _mapper.Map<AffiliateMapping>(affiliateMapping);
            return MapToDto(
                await _affiliateMappingManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.AffiliateMapping> Get(int id)
        {
            return MapToDto(await _affiliateMappingManager.Get(id));
        }

        public async Task<Models.DTOs.AffiliateMapping> GetAffiliateMapping(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj)
        {
            return MapToDto(await _affiliateMappingManager.GetAffiliateMapping(matchTypeId, 
                affiliateMappingRuleId, affiliateValueObj));
        }

        public async Task<Models.DTOs.AffiliateMapping> GetByAffiliateValue(int affiliateMappingRuleId, string affiliateValue)
        {
            return MapToDto(await _affiliateMappingManager.GetByAffiliateValue(affiliateMappingRuleId, 
                affiliateValue));
        }

        public List<Type> GetEntityTypes()
        {
            return _affiliateMappingManager.GetEntityTypes();
        }

        public async Task<List<Models.DTOs.AffiliateMapping>> GetAffiliateMappingList(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj)
        {
            return MapToList(await _affiliateMappingManager.GetAffiliateMappingList(matchTypeId,
              affiliateMappingRuleId, affiliateValueObj));
        }

        public async Task<List<Models.DTOs.AffiliateMapping>> GetAll()
        {
            return MapToList(await _affiliateMappingManager.GetAll());
        }

        #endregion

        private List<Models.DTOs.AffiliateMapping> MapToList(List<AffiliateMapping> data)
        {
            if (data == null || data.Count == 0)
                return null;

            var dtoList = new List<Models.DTOs.AffiliateMapping>();

            dtoList.AddRange(data.Select(MapToDto));

            return dtoList;
        }

        private Models.DTOs.AffiliateMapping MapToDto(AffiliateMapping data)
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
