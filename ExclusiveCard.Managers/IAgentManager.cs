using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface IAgentManager
    {
        /// <summary>
        /// Creates a new agent record
        /// </summary>
        /// <param name="agentCode">the new agent's details</param>
        /// <returns>The newly created agent</returns>
        Task<dto.AgentCode> CreateAgentAsync(dto.AgentCode agentCode);

        /// <summary>
        /// Get all agent records
        /// </summary>
        /// <returns>List of all agent records</returns>
        Task<List<dto.AgentCode>> GetAllAgentsAsync();

        /// <summary>
        /// Updates an existing agent record
        /// </summary>
        /// <param name="agentCode">the new details</param>
        /// <returns>Has updated</returns>
        Task<bool> UpdateAgentAsync(dto.AgentCode agentCode);
    }
}