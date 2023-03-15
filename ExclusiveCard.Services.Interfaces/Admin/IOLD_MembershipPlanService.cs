using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    [Obsolete("DO NOT write any new code or add references to this, use Exclusivecard.Services.Interfaces.IMembershipService instead")]
    public interface IOLD_MembershipPlanService
    {
        #region Writes

        Task<MembershipPlan> Add(MembershipPlan membershipPlan);
        Task<MembershipPlan> Update(MembershipPlan membershipPlan);
        Task<MembershipPlan> DeleteAsync(MembershipPlan membershipPlan);

        Task<MembershipPlanType> AddPlanTypeAsync(MembershipPlanType membershipPlanType);
        Task<MembershipPlanType> UpdatePlanTypeAsync(MembershipPlanType membershipPlan);
        Task<MembershipPlanType> DeletePlanTypeAsync(MembershipPlanType membershipPlan);


        #endregion

        #region Reads

        MembershipPlan GetDiamondPlan(int partnerId);
        Task<MembershipPlan> Get(int id, bool includePartner = false);

        //Get MembershipPlan by planTypeId, partnerId, duration, countryCode
        Task<MembershipPlan> GetMembershipPlan(int planTypeId, int? partnerId, int duration, string countryCode);

        Task<List<MembershipPlanPaymentProvider>> GetPaymentProvidersForMembershipPlanAsync(
            int membershipPlanId);

        Task<MembershipPlan> GetByDescriptionAsync(string description);
        Task<MembershipPlanType> GetByPlanTypeDescriptionAsync(string description);
        Task<MembershipPlanType> GetPlanTypeByIdAsync(int planTypeId);
        Task<List<MembershipPlanBenefits>> GetBenefitsByPlanIdAsync(int membershipPlanId);
        Task<TermsConditions> GetTermsConditionsByIdAsync(int tcId);
        string GetDescriptionByUserId(string aspNetUserId);
        
        Task<OrderSummary> GetOrderSummaryForDiamondPlan(int standardPlanId);
        Task<OrderSummary> GetOrderSummary(int membershipLevelId);

        MembershipPlan GetStandardPlan(int partnerId, int cardProviderId);
        MembershipPlan GetDiamondPlan(int partnerId, int cardProviderId);

        #endregion
    }
}
