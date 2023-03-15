using ExclusiveCard.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IAffiliateMappingService
    {
        #region Writes

        Task<AffiliateMapping> Add(AffiliateMapping affiliateMapping);

        Task<AffiliateMapping> Update(AffiliateMapping affiliateMapping);

        #endregion

        #region Reads

        Task<AffiliateMapping> Get(int id);

        Task<AffiliateMapping> GetAffiliateMapping(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj);

        Task<AffiliateMapping> GetByAffiliateValue(int affiliateMappingRuleId, string affiliateValue);

        List<Type> GetEntityTypes();

        Task<List<AffiliateMapping>> GetAffiliateMappingList(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId, object affiliateValueObj);

        Task<List<AffiliateMapping>> GetAll();

        #endregion
    }
}
