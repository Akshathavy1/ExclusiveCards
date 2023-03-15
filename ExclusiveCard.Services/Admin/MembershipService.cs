using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Managers;
using ExclusiveCard.Enums;
using Microsoft.Extensions.Caching.Memory;
using ExclusiveCard.Data.Constants;
using System.Linq;
using System.IO;

namespace ExclusiveCard.Services.Admin
{
    public class MembershipService : IMembershipService
    {
        #region Private Members

        private readonly ILogger _logger;
        private readonly IWhiteLabelManager _WhiteLabelManager;
        private readonly IPartnerManager _partnerManager;
        private readonly IPlanManager _planManager;
        private readonly IAgentManager _agentManager;
        private readonly IRegistrationManager _registrationManager;
        private readonly IMemoryCache _cache;

        #endregion Private Members

        public MembershipService(IWhiteLabelManager whiteLabelManager,
                                IPartnerManager partnerManager,
                                IPlanManager planManager,
                                IAgentManager agentManager,
                                IRegistrationManager registrationManager,
                                IMemoryCache cache)
        {
            _logger = LogManager.GetLogger(GetType().FullName);
            _WhiteLabelManager = whiteLabelManager;
            _partnerManager = partnerManager;
            _planManager = planManager;
            _agentManager = agentManager;
            _registrationManager = registrationManager;
            _cache = cache;
        }

        #region White labels

        ///<see cref="IMembershipService.GetAllSitesAsync"/>
        public async Task<IList<WhiteLabelSettings>> GetAllSitesAsync()
        {
            IList<WhiteLabelSettings> whiteLabels = null;
            try
            {
                whiteLabels = _cache.Get<List<WhiteLabelSettings>>(Keys.AllWhiteLabelSitesList);
                if (whiteLabels == null)
                {
                    whiteLabels = await _WhiteLabelManager.GetAllAsync();
                    _cache.Set(Keys.AllWhiteLabelSitesList, whiteLabels);
                }
                return whiteLabels;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return whiteLabels;
        }

        #endregion White labels

        #region Card Providers

        /// <see cref="IMembershipService.GetAllCardProvidersAsync"/>
        public async Task<List<PartnerDto>> GetAllCardProvidersAsync()
        {
            List<PartnerDto> cardProviders = null;
            try
            {
                cardProviders = _cache.Get<List<PartnerDto>>(Keys.AllCardProvidersList);
                if (cardProviders == null)
                {
                    cardProviders = await _partnerManager.GetAllPartnersAsync(PartnerType.CardProvider);
                    _cache.Set(Keys.AllCardProvidersList, cardProviders);
                }
                return cardProviders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return cardProviders;
        }

        /// <see cref="IMembershipService.CreateCardProviderAsync(PartnerDto)"/>
        public async Task<PartnerDto> CreateCardProviderAsync(PartnerDto cardProvider)
        {
            PartnerDto partnerDto = null;
            try
            {
                partnerDto = await _partnerManager.CreatePartnerAsync(cardProvider);
                if (partnerDto != null)
                {
                    _cache.Remove(Keys.AllCardProvidersList);
                    return partnerDto;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return partnerDto;
        }

        /// <see cref="IMembershipService.UpdateCardProviderAsync(PartnerDto)"/>
        public async Task<bool> UpdateCardProviderAsync(PartnerDto cardProvider)
        {
            try
            {
                var hasUpdated = await _partnerManager.UpdatePartnerAsync(cardProvider);
                if (hasUpdated)
                {
                    _cache.Remove(Keys.AllCardProvidersList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        #endregion Card Providers

        #region Membership Plans

        /// <see cref="IMembershipService.GetMembershipPlanById(int)"/>
        public MembershipPlan GetMembershipPlanById(int planId)
        {
            MembershipPlan membershipPlan = null;
            try
            {
                membershipPlan = _planManager.GetPlanById(planId);
                if (membershipPlan != null)
                {
                    return membershipPlan;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return membershipPlan;
        }

        /// <see cref="IMembershipService.GetAllMembershipPlansAsync(int, int)"/>
        public async Task<List<MembershipPlan>> GetAllMembershipPlansAsync(int WhitelabelId, int cardProviderId)
        {
            List<MembershipPlan> membershipPlans = null;
            try
            {
                membershipPlans = await _planManager.GetAllPlansAsync(WhitelabelId, cardProviderId);
                return membershipPlans;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return membershipPlans;
        }

        /// <see cref="IMembershipService.CreateMembershipPlanAsync(MembershipPlan)"/>
        public async Task<MembershipPlan> CreateMembershipPlanAsync(MembershipPlan plan)
        {
            MembershipPlan membershipPlan = null;
            try
            {
                membershipPlan = await _planManager.CreatePlanAsync(plan);
                if (membershipPlan != null)
                {
                    return membershipPlan;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return membershipPlan;
        }

        /// <see cref="IMembershipService.UpdateMembershipPlanAsync(MembershipPlan)"/>
        public async Task<bool> UpdateMembershipPlanAsync(MembershipPlan plan)
        {
            try
            {
                var hasUpdated = await _planManager.UpdatePlanAsync(plan);
                if (hasUpdated)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        /// <see cref="IMembershipService.GetDefaultPlanAsync(MembershipPlanTypeEnum)"/>
        public async Task<MembershipPlan> GetDefaultPlanAsync(MembershipPlanTypeEnum planType, Enums.MembershipLevel planLevel = Enums.MembershipLevel.Diamond)
        {
            MembershipPlan planDto = null;
            try
            {
                var defaultPartnerDto = await _partnerManager.GetDefaultPartnerAsync();
                var listOfPlanDto = await _planManager.GetAllPlansAsync(defaultPartnerDto.Id);
                if (listOfPlanDto != null && listOfPlanDto.Any())
                {
                    listOfPlanDto = listOfPlanDto.Where(p => (p.MembershipLevelId == (int)planLevel)
                                    && (p.MembershipPlanTypeId == (int)planType) && !p.PaidByEmployer && !p.IsDeleted && p.IsActive
                                    && (p.ValidFrom <= DateTime.UtcNow) && (p.ValidTo >= DateTime.UtcNow)).ToList();
                    planDto = listOfPlanDto.FirstOrDefault();
                    return planDto;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return planDto;
        }

        #endregion Membership Plans

        #region Agents

        /// <see cref="IMembershipService.CreateAgentAsync(AgentCode)"/>
        public async Task<AgentCode> CreateAgentAsync(AgentCode agent)
        {
            AgentCode agentDto = null;
            try
            {
                agentDto = await _agentManager.CreateAgentAsync(agent);
                if (agentDto != null)
                {
                    return agentDto;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return agentDto;
        }

        public Task<AgentCode> GetAgentAsync(int membershipPlanId)
        {
            throw new NotImplementedException();
        }

        /// <see cref="IMembershipService.GetAllAgentsAsync"/>
        public async Task<List<AgentCode>> GetAllAgentsAsync()
        {
            List<AgentCode> agents = null;
            try
            {
                agents = await _agentManager.GetAllAgentsAsync();
                return agents;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return agents;
        }

        /// <see cref="IMembershipService.UpdateAgentAsync(AgentCode)"/>
        public async Task<bool> UpdateAgentAsync(AgentCode agent)
        {
            try
            {
                var hasUpdated = await _agentManager.UpdateAgentAsync(agent);
                if (hasUpdated)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        #endregion Agents

        #region Registration Codes

        /// <see cref="IMembershipService.CreateRegistrationBatchAsync(RegistrationCodeSummary)"/>
        public async Task<bool> CreateRegistrationBatchAsync(RegistrationCodeSummary summary)
        {
            try
            {
                var created = await _registrationManager.CreateRegistrationBatchAsync(summary);
                if (created)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        /// <see cref="IMembershipService.GetAllRegistrationCodesAsync(int)"/>
        public async Task<List<MembershipRegistrationCode>> GetAllRegistrationCodesAsync(int registrationCodeSummaryId)
        {
            List<MembershipRegistrationCode> registrationCodes = null;
            try
            {
                registrationCodes = await _registrationManager.GetAllRegistrationsAsync(registrationCodeSummaryId);
                return registrationCodes;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return registrationCodes;
        }

        /// <see cref="IMembershipService.GetAllSummaries(int)"/>
        public List<RegistrationCodeSummary> GetAllSummaries(int membershipPlanId)
        {
            List<RegistrationCodeSummary> registrationSummaries = null;
            try
            {
                registrationSummaries = _registrationManager.GetAllSummaries(membershipPlanId);
                return registrationSummaries;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return registrationSummaries;
        }

        /// <see cref="IMembershipService.DownloadRegistrationAsync(RegistrationCodeSummary)"/>
        public async Task<byte[]> DownloadRegistrationAsync(RegistrationCodeSummary summary)
        {
            try
            {
                return await _registrationManager.DownloadRegistrationsAsync(summary);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        public Task<RegistrationCodeSummary> CreateRegistrationCodeSummary(RegistrationCodeSummary summary)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRegistrationCodeSummary(RegistrationCodeSummary summary)
        {
            throw new NotImplementedException();
        }

        #endregion Registration Codes
    }
}