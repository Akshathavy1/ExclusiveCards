using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface IRegistrationManager
    {
        /// <summary>
        /// Creates new a registration batch
        /// </summary>
        /// <param name="summary"></param>
        /// <returns>Has created</returns>
        Task<bool> CreateRegistrationBatchAsync(dto.RegistrationCodeSummary summary);

        /// <summary>
        /// Downloads a registration batch
        /// </summary>
        /// <param name="summary"></param>
        /// <returns>The downloaded byte array</returns>
        Task<byte[]> DownloadRegistrationsAsync(dto.RegistrationCodeSummary summary);

        /// <summary>
        /// Get all registration summaries
        /// </summary>
        /// <param name="membershipPlanId"></param>
        /// <returns>List of registration summaries</returns>
        List<dto.RegistrationCodeSummary> GetAllSummaries(int membershipPlanId);

        /// <summary>
        /// Get all registration codes
        /// </summary>
        /// <param name="registrationCodeSummaryId"></param>
        /// <returns>List of registration codes</returns>
        Task<List<dto.MembershipRegistrationCode>> GetAllRegistrationsAsync(int registrationCodeSummaryId);
    }
}