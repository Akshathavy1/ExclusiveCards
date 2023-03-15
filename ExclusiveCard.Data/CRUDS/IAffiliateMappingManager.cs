using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IAffiliateMappingManager
    {
        Task<AffiliateMapping> Add(AffiliateMapping affiliateMapping);
        Task<AffiliateMapping> Update(AffiliateMapping affiliateMapping);
        Task<AffiliateMapping> Get(int id);

        Task<AffiliateMapping> GetAffiliateMapping(AffiliateMatchTypes matchTypeId, int affiliateMappingRuleId,
            object affiliateValueObj);

        Task<AffiliateMapping> GetByAffiliateValue(int affiliateMappingRuleId, string affiliateValue);
        List<Type> GetEntityTypes();

        Task<List<AffiliateMapping>> GetAffiliateMappingList(AffiliateMatchTypes matchTypeId,
            int affiliateMappingRuleId, object affiliateValueObj);

        Task<List<AffiliateMapping>> GetAll();
    }
}