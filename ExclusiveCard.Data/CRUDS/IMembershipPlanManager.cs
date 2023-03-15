using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMembershipPlanManager
    {
        Task<MembershipPlan> Add(MembershipPlan membershipPlan);
        Task<MembershipPlan> Update(MembershipPlan membershipPlan);
        Task<MembershipPlan> DeleteAsync(MembershipPlan membershipPlan);

        Task<MembershipPlanType> AddPlanTypeAsync(MembershipPlanType membershipPlanType);
        Task<MembershipPlanType> UpdatePlanTypeAsync(MembershipPlanType membershipPlanType);
        Task<MembershipPlanType> DeletePlanTypeAsync(MembershipPlanType membershipPlanType);

        MembershipPlan GetDiamondPlan(int partnerId);
        Task<MembershipPlan> Get(int id, bool includePartner = false);
        Task<MembershipPlan> GetMembershipPlan(int planTypeId, int? partnerId, int duration, string countryCode);
        Task<MembershipPlan> GetByDescriptionAsync(string description);
        Task<MembershipPlanType> GetByPlanTypeDescriptionAsync(string description);
        Task<MembershipPlanType> GetPlanTypeByIdAsync(int planTypeId);
        Task<List<MembershipPlanBenefits>> GetBenefitsByPlanIdAsync(int membershipPlanId);
        Task<TermsConditions> GetTermsConditionsByIdAsync(int tcId);
        string GetDescriptionByAspNetUserId(string aspNetUserId);

        Task<OrderSummaryDataModel> GetOrderSummaryDetails(int MembershipPlanId);
        Task<OrderSummaryDataModel> GetOrderDetails(int membershipLevelId);

        MembershipPlan GetStandardPlan(int partnerId, int cardProviderId);
        MembershipPlan GetDiamondPlan(int partnerId, int cardProviderId);
    }
}