using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IAffiliateMappingRuleManager
    {
        Task<AffiliateMappingRule> Add(AffiliateMappingRule affiliateMappingRule);
        Task<AffiliateMappingRule> Update(AffiliateMappingRule affiliateMappingRule);
        Task<AffiliateMappingRule> Get(int id);
        Task<AffiliateMappingRule> GetByDesc(string desc, int? affiliateId = null);
        Task<List<AffiliateMappingRule>> GetAllMappingRules(string desc, int? affiliateId = null);
    }
}