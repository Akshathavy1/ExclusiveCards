using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface IPlanManager
    {
        /// <summary>
        /// Creates a new plan record
        /// </summary>
        /// <param name="membershipPlan">the new plan's details</param>
        /// <returns>The newly created plan</returns>
        Task<dto.MembershipPlan> CreatePlanAsync(dto.MembershipPlan membershipPlan);

        /// <summary>
        /// Get all plan records based on card provider id
        /// </summary>
        /// <param name="cardProviderId">Card Provider Id</param>
        /// <returns>List of all plan records</returns>
        Task<List<dto.MembershipPlan>> GetAllPlansAsync(int cardProviderId);

        /// <summary>
        /// Get all plan records based on card provider id and white label id
        /// </summary>
        /// <param name="whiteLabelId">White Label Id </param>
        /// <param name="cardProviderId">Card Provider Id</param>
        /// <returns>List of all plan records</returns>
        Task<List<dto.MembershipPlan>> GetAllPlansAsync(int whiteLabelId, int cardProviderId);

        /// <summary>
        /// Get the plan based on Id
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>The plan based on Id</returns>
        dto.MembershipPlan GetPlanById(int planId);

        /// <summary>
        /// Updates an existing plan record
        /// </summary>
        /// <param name="membershipPlan">the new details</param>
        /// <returns>Has updated</returns>
        Task<bool> UpdatePlanAsync(dto.MembershipPlan membershipPlan);
    }
}