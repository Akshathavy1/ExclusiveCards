using System.Collections.Generic;
using ExclusiveCard.Managers;
using ExclusiveCard.Services.Admin;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Constants;
using ExclusiveCard.Enums;
using System;

namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    [TestFixture]
    public class MembershipServiceUnitTests
    {
        private IMembershipService _membershipService;
        private Mock<IWhiteLabelManager> _WhiteLabelManager;
        private Mock<IPartnerManager> _partnerManager;
        private Mock<IPlanManager> _planManager;
        private Mock<IAgentManager> _agentManager;
        private Mock<IRegistrationManager> _registrationManager;
        private IMemoryCache _cache;

        [OneTimeSetUp]
        public void Initialise()
        {
            _WhiteLabelManager = new Mock<IWhiteLabelManager>();
            _partnerManager = new Mock<IPartnerManager>();
            _planManager = new Mock<IPlanManager>();
            _agentManager = new Mock<IAgentManager>();
            _registrationManager = new Mock<IRegistrationManager>();
            _cache = Configuration.ServiceProvider.GetService<IMemoryCache>();
            _membershipService = new MembershipService(_WhiteLabelManager.Object, _partnerManager.Object, _planManager.Object, _agentManager.Object, _registrationManager.Object, _cache);
        }

        #region White Label Settings

        /// <summary>
        /// Test case for GetAllSites() of MembershipService without cache
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetAllSitesWithNoCacheAsync()
        {
            //Arrange
            var whiteLabelSites = GetWhiteLabelSettings();
            _WhiteLabelManager.Setup(x => x.GetAllAsync()).ReturnsAsync(whiteLabelSites);

            //Act
            var sites = await _membershipService.GetAllSitesAsync();

            //Assert
            Assert.IsNotNull(sites, "Sites is Null");
            Assert.IsTrue(sites.Count > 0, "No data returned for sites");
        }

        /// <summary>
        /// Test case for GetAllSites() of MembershipService from cache
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetAllSitesWithFromCacheAsync()
        {
            //Arrange
            var whiteLabelSites = GetWhiteLabelSettings();
            _WhiteLabelManager.Setup(x => x.GetAllAsync()).ReturnsAsync(whiteLabelSites);
            _cache.Set(Keys.AllWhiteLabelSitesList, GetWhiteLabelSettings(2));

            //Act
            var sites = await _membershipService.GetAllSitesAsync();

            //Assert
            Assert.IsNotNull(sites, "Sites is Null");
            Assert.IsTrue(sites.Count > 0, "No data returned for sites");
            Assert.IsTrue(sites.Count == 2, $"Expected 2 from cahce but returned {sites.Count}");
        }

        #endregion White Label Settings

        #region Card Providers

        /// <summary>
        /// Test case for GetAllCardProvidersAsync() of MembershipService without cache
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetAllCardProvidersWithNoCacheAsync()
        {
            //Arrange
            var cardProviders = GetCardProvidersDTO();
            _partnerManager.Setup(x => x.GetAllPartnersAsync(It.IsAny<PartnerType>())).ReturnsAsync(cardProviders);

            //Act
            var cards = await _membershipService.GetAllCardProvidersAsync();

            //Assert
            Assert.IsNotNull(cards, "card is Null");
            Assert.IsTrue(cards.Count > 0, "No data returned for cards");
        }

        /// <summary>
        /// Test case for GetAllCardProvidersAsync() of MembershipService from cache
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetAllCardProvidersWithFromCacheAsync()
        {
            //Arrange
            var cardProviders = GetCardProvidersDTO();
            _partnerManager.Setup(x => x.GetAllPartnersAsync(It.IsAny<PartnerType>())).ReturnsAsync(cardProviders);

            var cardProviderDTOs = GetCardProvidersDTO(2);
            _cache.Set(Keys.AllCardProvidersList, cardProviderDTOs);

            //Act
            var cards = await _membershipService.GetAllCardProvidersAsync();

            //Assert
            Assert.IsNotNull(cards, "Card is Null");
            Assert.IsTrue(cards.Count > 0, "No data returned for cards");
            Assert.IsTrue(cards.Count == 2, $"Expected 2 from cahce but returned {cards.Count}");
        }

        /// <summary>
        /// Test case for CreateCardProvider() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestCreateCardProviderAsync()
        {
            // Arrange
            var cardProviderDto = CreateCardProviderDto();
            _partnerManager.Setup(x => x.CreatePartnerAsync(cardProviderDto)).ReturnsAsync(cardProviderDto);

            // Act
            var cardCreated = await _membershipService.CreateCardProviderAsync(cardProviderDto);

            // Assert
            Assert.IsNotNull(cardCreated, "Card is not null");
        }

        /// <summary>
        /// Test case for UpdateCardProvider() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestUpdateCardProviderAsync()
        {
            // Arrange
            var cardProviderDto = CreateCardProviderDto();
            _partnerManager.Setup(x => x.UpdatePartnerAsync(cardProviderDto)).ReturnsAsync(true);

            // Act
            var updated = await _membershipService.UpdateCardProviderAsync(cardProviderDto);

            // Assert
            Assert.IsTrue(updated, "Card is updated");
        }

        #endregion Card Providers

        #region Membership Plans

        /// <summary>
        /// Test case for GetMembershipPlanByIdAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public void TestGetMembershipPlanByIdAsync()
        {
            // Arrange
            var membershipPlan = CreateMembershipPlanDto();
            _planManager.Setup(x => x.GetPlanById(It.IsAny<int>())).Returns(membershipPlan);

            // Act
            var plan = _membershipService.GetMembershipPlanById(membershipPlan.Id);

            // Assert
            Assert.IsNotNull(plan, "Plan is Null");
        }

        /// <summary>
        /// Test case for GetAllMembershipPlansAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetAllMembershipPlansAsync()
        {
            // Arrange
            var membershipPlans = GetMembershipPlansDTO();
            _planManager.Setup(x => x.GetAllPlansAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(membershipPlans);

            // Act
            var plans = await _membershipService.GetAllMembershipPlansAsync(WhitelabelId: 1, cardProviderId: 1);

            // Assert
            Assert.IsNotNull(plans, "Plans is Null");
            Assert.IsTrue(plans.Count > 0, "No data returned for Plans");
        }

        /// <summary>
        /// Test case for CreateMembershipPlanAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestCreateMembershipPlanAsync()
        {
            // Arrange
            var planDto = CreateMembershipPlanDto();
            _planManager.Setup(x => x.CreatePlanAsync(planDto)).ReturnsAsync(planDto);

            // Act
            var planCreated = await _membershipService.CreateMembershipPlanAsync(planDto);

            // Assert
            Assert.IsNotNull(planCreated, "Plan is not null");
        }

        /// <summary>
        /// Test case for UpdateMembershipPlanAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestUpdateMembershipPlanAsync()
        {
            // Arrange
            var planDto = CreateMembershipPlanDto();
            _planManager.Setup(x => x.UpdatePlanAsync(planDto)).ReturnsAsync(true);

            // Act
            var updated = await _membershipService.UpdateMembershipPlanAsync(planDto);

            // Assert
            Assert.IsTrue(updated, "Membership plan is updated");
        }

        /// <summary>
        /// Test case for GetDefaultPlanAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetDefaultPlanAsync()
        {
            // Arrange
            var partnerDto = CreateCardProviderDto();
            var plansList = GetMembershipPlansDTO();
            _partnerManager.Setup(x => x.GetDefaultPartnerAsync()).ReturnsAsync(partnerDto);
            _planManager.Setup(y => y.GetAllPlansAsync(partnerDto.Id)).ReturnsAsync(plansList);

            // Act
            var plan = await _membershipService.GetDefaultPlanAsync(MembershipPlanTypeEnum.PartnerReward);
            // Assert
            Assert.IsNotNull(plan, "Plan is not null");
        }

        #endregion Membership Plans

        #region Agents

        /// <summary>
        /// Test case for GetAllAgentsAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetAllAgentsAsync()
        {
            // Arrange
            var agentCodeDtos = GetAgentCodes();
            _agentManager.Setup(x => x.GetAllAgentsAsync()).ReturnsAsync(agentCodeDtos);

            // Act
            var agents = await _membershipService.GetAllAgentsAsync();

            // Assert
            Assert.IsNotNull(agents, "Agents is Null");
            Assert.IsTrue(agents.Count > 0, "No data returned for Agents");
        }

        /// <summary>
        /// Test case for CreateAgentAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestCreateAgentAsync()
        {
            // Arrange
            var agentDto = CreateAgentCodeDto();
            _agentManager.Setup(x => x.CreateAgentAsync(agentDto)).ReturnsAsync(agentDto);

            // Act
            var agentCreated = await _membershipService.CreateAgentAsync(agentDto);

            // Assert
            Assert.IsNotNull(agentCreated, "Agent is not null");
        }

        /// <summary>
        /// Test case for UpdateAgentAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestUpdateAgentAsync()
        {
            // Arrange
            var agentCodeDto = CreateAgentCodeDto();
            _agentManager.Setup(x => x.UpdateAgentAsync(agentCodeDto)).ReturnsAsync(true);

            // Act
            var updated = await _membershipService.UpdateAgentAsync(agentCodeDto);

            // Assert
            Assert.IsTrue(updated, "Agent is updated");
        }

        #endregion Agents

        #region Registration Codes

        /// <summary>
        /// Test case for GetAllSummaries() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public void TestGetAllSummaries()
        {
            // Arrange
            var registrationSummaries = GetRegistrationCodeSummaries();
            _registrationManager.Setup(x => x.GetAllSummaries(It.IsAny<int>())).Returns(registrationSummaries);

            // Act
            var summaries = _membershipService.GetAllSummaries(membershipPlanId: 1);

            // Assert
            Assert.IsNotNull(summaries, "Registration Summary is Null");
            Assert.IsTrue(summaries.Count > 0, "No data returned for Registration Summaries");
        }

        /// <summary>
        /// Test case for GetAllRegistrationCodesAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetAllRegistrationCodesAsync()
        {
            // Arrange
            var registrationCodes = GetMembershipRegistrationCodes();
            _registrationManager.Setup(x => x.GetAllRegistrationsAsync(It.IsAny<int>())).ReturnsAsync(registrationCodes);

            // Act
            var codes = await _membershipService.GetAllRegistrationCodesAsync(registrationCodeSummaryId: 1);

            // Assert
            Assert.IsNotNull(codes, "Registration Code is Null");
            Assert.IsTrue(codes.Count > 0, "No data returned for Registration Codes");
        }

        /// <summary>
        /// Test case for CreateRegistrationBatchAsync() of MembershipService
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestCreateRegistrationBatchAsync()
        {
            // Arrange
            var summary = CreateRegistrationCodeSummaryDto();
            _registrationManager.Setup(x => x.CreateRegistrationBatchAsync(summary));

            // Act
            var summaryCreated = await _membershipService.CreateRegistrationBatchAsync(summary);

            // Assert
            Assert.IsNotNull(summaryCreated, "Registration Summary is not null");
        }

        #endregion Registration Codes

        #region Private Methods

        /// <summary>
        /// Setting hard-code card providers list
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<PartnerDto> GetCardProvidersDTO(int count = 10)
        {
            List<PartnerDto> cardProviders = new List<PartnerDto>();
            for (int i = 0; i < count; i++)
            {
                cardProviders.Add(new PartnerDto
                {
                    Id = i,
                    Name = $"CardProvider-{i}",
                    Type = (int)PartnerType.CardProvider
                });
            }
            return cardProviders;
        }

        /// <summary>
        /// Setting hard-code white labels list
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<WhiteLabelSettings> GetWhiteLabelSettings(int count = 20)
        {
            List<WhiteLabelSettings> whiteLabelSettings = new List<WhiteLabelSettings>();
            for (int i = 0; i < count; i++)
            {
                whiteLabelSettings.Add(new WhiteLabelSettings
                {
                    Id = i,
                    Name = $"WhiteLabelSite-{i}"
                });
            }
            return whiteLabelSettings;
        }

        /// <summary>
        /// Creating a hard-coded card provider dto
        /// </summary>
        /// <returns></returns>

        private PartnerDto CreateCardProviderDto()
        {
            var card = new PartnerDto
            {
                Id = 1,
                Name = "CardProvider-1",
                Type = (int)PartnerType.CardProvider
            };
            return card;
        }

        /// <summary>
        /// Setting hard-code membership plans list
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<MembershipPlan> GetMembershipPlansDTO(int count = 10)
        {
            List<MembershipPlan> plans = new List<MembershipPlan>();
            for (int i = 0; i < count; i++)
            {
                plans.Add(new MembershipPlan
                {
                    Id = i,
                    MembershipLevelId = (int)Enums.MembershipLevel.Standard,
                    MembershipPlanTypeId = (int)Enums.MembershipPlanTypeEnum.PartnerReward,
                    Description = $"PlanDescription-{i}",
                    Duration = i,
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddDays(10),
                    NumberOfCards = i,
                    CustomerCardPrice = 0,
                    PartnerCardPrice = 0,
                    DeductionPercentage = 20,
                    CustomerCashbackPercentage = 80,
                    BenefactorPercentage = 0,
                    PaidByEmployer = false,
                    PartnerId = i,
                    CurrencyCode = "GBP",
                    MinimumValue = 1,
                    PaymentFee = 0,
                    WhitelabelId = i,
                    CardProviderId = i + 1,
                    IsActive = true,
                    IsDeleted = false,
                });
            }
            return plans;
        }

        /// <summary>
        /// Creating a hard-coded membership plan dto
        /// </summary>
        /// <returns></returns>
        private MembershipPlan CreateMembershipPlanDto()
        {
            var plan = new MembershipPlan
            {
                Id = 1,
                MembershipLevelId = (int)Enums.MembershipLevel.Diamond,
                MembershipPlanTypeId = (int)Enums.MembershipPlanTypeEnum.PartnerReward,
                Description = $"PlanDescription-{1}",
                Duration = 10,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(10),
                CustomerCardPrice = 0,
                PartnerCardPrice = 0,
                NumberOfCards = 2,
                DeductionPercentage = 1,
                CustomerCashbackPercentage = 2,
                BenefactorPercentage = 0,
                PaidByEmployer = true,
                PartnerId = 1,
                CurrencyCode = "GBP",
                MinimumValue = 1,
                PaymentFee = 0,
                WhitelabelId = 1,
                CardProviderId = 2,
                IsActive = true,
                IsDeleted = false,
            };
            return plan;
        }

        /// <summary>
        /// Setting hard-code agents list
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<AgentCode> GetAgentCodes(int count = 10)
        {
            List<AgentCode> agentCodes = new List<AgentCode>();
            for (int i = 0; i < count; i++)
            {
                agentCodes.Add(new AgentCode
                {
                    Id = i,
                    Name = $"AgentCode-{i}",
                    ReportCode = $"ReportCode-{i}",
                    Description = $"Description-{i}",
                    CommissionPercent = 50,
                    StartDate = DateTime.UtcNow,
                    EndDate = null,
                    IsDeleted = false
                });
            }
            return agentCodes;
        }

        /// <summary>
        /// Creating a hard-coded agent code dto
        /// </summary>
        /// <returns></returns>
        private AgentCode CreateAgentCodeDto()
        {
            var agentCode = new AgentCode
            {
                Id = 1,
                Name = "AgentCode-1",
                ReportCode = "ReportCode-1",
                Description = "Description-1",
                CommissionPercent = 50,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                IsDeleted = false
            };
            return agentCode;
        }

        /// <summary>
        /// Setting hard-code registration code summaries list
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<RegistrationCodeSummary> GetRegistrationCodeSummaries(int count = 10)
        {
            List<RegistrationCodeSummary> summaries = new List<RegistrationCodeSummary>();
            for (int i = 0; i < count; i++)
            {
                summaries.Add(new RegistrationCodeSummary
                {
                    Id = i,
                    MembershipPlanId = 2,
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddDays(1),
                    NumberOfCodes = 25,
                    StoragePath = $"StoragePath-{i}"
                });
            }
            return summaries;
        }

        /// <summary>
        /// Creating a hard-coded registration code summary dto
        /// </summary>
        /// <returns></returns>
        private RegistrationCodeSummary CreateRegistrationCodeSummaryDto()
        {
            var summaryDto = new RegistrationCodeSummary
            {
                Id = 1,
                MembershipPlanId = 2,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(1),
                NumberOfCodes = 25,
                StoragePath = null
            };
            return summaryDto;
        }

        /// <summary>
        /// Setting hard-code registration code summaries list
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<MembershipRegistrationCode> GetMembershipRegistrationCodes(int count = 10)
        {
            List<MembershipRegistrationCode> registrationCodes = new List<MembershipRegistrationCode>();
            for (int i = 0; i < count; i++)
            {
                registrationCodes.Add(new MembershipRegistrationCode
                {
                    Id = i,
                    EmailHostName = null,
                    IsActive = true,
                    IsDeleted = false,
                    MembershipPlanId = 1,
                    NumberOfCards = i,
                    RegistartionCode = $"RegistrationCode-{i}",
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddDays(1)
                });
            }
            return registrationCodes;
        }

        #endregion Private Methods
    }
}