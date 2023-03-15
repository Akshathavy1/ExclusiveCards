using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IAffiliateMappingRuleService
    {
        #region Writes

        Task<AffiliateMappingRule> Add(AffiliateMappingRule affiliateMappingRule);

        Task<AffiliateMappingRule> Update(AffiliateMappingRule affiliateMappingRule);

        #endregion

        #region Reads

        Task<AffiliateMappingRule> Get(int id);
        Task<AffiliateMappingRule> GetByDesc(string desc, int? affiliateId = null);

        Task<List<AffiliateMappingRule>> GetAllMappingRules(string desc, int? affiliateId = null);

        #endregion
    }
}
