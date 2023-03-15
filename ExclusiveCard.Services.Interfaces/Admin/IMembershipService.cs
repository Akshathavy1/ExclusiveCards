using ExclusiveCard.Enums;
using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    /// <summary>
    /// New service to manage membership plans, card providers, agents and registration codes
    /// </summary>
    public interface IMembershipService
    {
        #region White Labels

        /// <summary>
        /// Returns a list of all active white label websites
        /// </summary>
        Task<IList<WhiteLabelSettings>> GetAllSitesAsync();

        #endregion White Labels

        #region Card Providers

        /// <summary>
        /// Returns a list of all active card provider, partner records
        /// </summary>
        Task<List<PartnerDto>> GetAllCardProvidersAsync();

        /// <summary>
        /// Creates a new card provider, partner record
        /// </summary>
        /// <param name="cardProvider">the new card provider's details</param>
        /// <returns>The newly created card provider</returns>
        Task<PartnerDto> CreateCardProviderAsync(PartnerDto cardProvider);

        /// <summary>
        /// Updates an existing card provider, partner record
        /// </summary>
        /// <param name="cardProvider">the new details</param>
        /// <returns></returns>
        Task<bool> UpdateCardProviderAsync(PartnerDto cardProvider);

        #endregion Card Providers

        #region Membership Plans

        /// <summary>
        /// Get the plan associated with the agent
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>The associated agent plan</returns>
        MembershipPlan GetMembershipPlanById(int planId);

        /// <summary>
        /// Returns a list of all active membership plans associated with the given white label and card provider
        /// </summary>
        /// <param name="WhitelabelId"></param>
        /// <param name="cardProviderId"></param>
        /// <returns>List of membership plans</returns>
        Task<List<MembershipPlan>> GetAllMembershipPlansAsync(int WhitelabelId, int cardProviderId);

        /// <summary>
        /// Creates a new membership plan record
        /// </summary>
        /// <param name="plan">The new details</param>
        /// <returns>The newly created membership plan</returns>
        Task<MembershipPlan> CreateMembershipPlanAsync(MembershipPlan plan);

        /// <summary>
        /// Updates an existing membership plan
        /// </summary>
        /// <param name="plan"></param>
        /// <returns>Has updated<</returns>
        Task<bool> UpdateMembershipPlanAsync(MembershipPlan plan);

        /// <summary>
        /// Get the default plan to identify the current default diamond upgrade customer purchase cost (PayPal buttons etc)
        /// </summary>
        /// <param name="planType"></param>
        /// <returns>The default plan</returns>
        Task<MembershipPlan> GetDefaultPlanAsync(MembershipPlanTypeEnum planType, Enums.MembershipLevel planLevel = Enums.MembershipLevel.Diamond);

        #endregion Membership Plans

        #region Agents

        /// <summary>
        /// Returns a list of all active agent codes
        /// </summary>
        /// <returns>List of all active agent codes</returns>
        Task<List<AgentCode>> GetAllAgentsAsync();

        /// <summary>
        /// Returns the agent associated with the given membership plan
        /// </summary>
        /// <returns></returns>
        Task<AgentCode> GetAgentAsync(int membershipPlanId);

        /// <summary>
        /// Creates a new Agent code record
        /// </summary>
        /// <param name="agent"></param>
        /// <returns>The newly created agent</returns>
        Task<AgentCode> CreateAgentAsync(AgentCode agent);

        /// <summary>
        /// Updates an existing agent code
        /// </summary>
        /// <param name="agent"></param>
        /// <returns>Has updated</returns>
        Task<bool> UpdateAgentAsync(AgentCode agent);

        #endregion Agents

        #region Registration Codes

        /// <summary>
        /// Returns all existing regstration code summary dto's related to the given membership plan id.
        /// This is used by the controller to populate the registation code grid
        /// </summary>
        /// <param name="membershipPlanId"></param>
        /// <returns>List of all registration summaries associated with the given plan</returns>
        List<RegistrationCodeSummary> GetAllSummaries(int membershipPlanId);

        /// <summary>
        /// Returns all existing regstration codes linked to the given registration code summary id
        /// This is used to identify a batch of registration codes prior to creating a csv file
        /// </summary>
        /// <param name="registrationCodeSummaryId"></param>
        /// <returns>List of all registration codes</returns>
        Task<List<MembershipRegistrationCode>> GetAllRegistrationCodesAsync(int registrationCodeSummaryId);

        /// <summary>
        /// Creates a new regstration code summary and the number of membership registration codes described in the RegistrationCodeSummary object
        /// </summary>
        /// <param name="summary"></param>
        /// <returns>Has created</returns>
        Task<bool> CreateRegistrationBatchAsync(RegistrationCodeSummary summary);

        /// <summary>
        /// Download a csv file of registration codes and return as byte array
        /// </summary>
        /// <param name="summary"></param>
        /// <returns>Downloaded stream in byte array form</returns>
        Task<byte[]> DownloadRegistrationAsync(RegistrationCodeSummary summary);

        /// <summary>
        /// Creates a new regstration code summary and the number of membership registration codes described in the RegistrationCodeSummary object
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        Task<RegistrationCodeSummary> CreateRegistrationCodeSummary(RegistrationCodeSummary summary);

        /// <summary>
        /// Updates an existing RegistrationCodeSummary, creating extra membership registration codes if required
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        Task UpdateRegistrationCodeSummary(RegistrationCodeSummary summary);

        #endregion Registration Codes
    }
}