using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.Common
{
    public static class Membership
    {
        public static async Task<dto.MembershipPlanType> CreatePlanType(dto.MembershipPlanType type)
        {
            return await ServiceHelper.Instance.MembershipPlanService.AddPlanTypeAsync(type);
        }

        public static async Task<dto.MembershipPlan> CreatePlan(int planTypeId, dto.MembershipPlan plan)
        {
            plan.MembershipPlanTypeId = planTypeId;
            return await ServiceHelper.Instance.MembershipPlanService.Add(plan);
        }

        public static async Task DeletePlanType(dto.MembershipPlanType type)
        {
            await ServiceHelper.Instance.MembershipPlanService.DeletePlanTypeAsync(type);
        }

        public static async Task DeletePlan(dto.MembershipPlan plan)
        {
            await ServiceHelper.Instance.MembershipPlanService.DeleteAsync(plan);
        }
    }
}
