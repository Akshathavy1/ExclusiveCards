using AutoMapper;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.Helpers;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class PlansController : BaseController
    {
        private readonly IMembershipService _membershipService;
        private readonly IOptions<TypedAppSettings> _settings;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PlansController(IMembershipService membershipService,
                               IOptions<TypedAppSettings> settings,
                               IMapper mapper)
        {
            _membershipService = membershipService;
            _settings = settings;
            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                PlansViewModel plans = new PlansViewModel();
                plans.WhiteLabels = await MapToWhiteLabelsAsync();
                plans.CardProviders = await MapToCardProvidersAsync();
                plans.WhiteLabelSettingMembershipPlan = new WhiteLabelSettingMembershipPlan();
                plans.WhiteLabelSettingsAgents = new WhiteLabelSettingsAgents();
                plans.RegistrationCodesSummary = new RegistrationCodesSummary();
                return View("Plans", plans);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCardProvider(PlansViewModel plansViewModel)
        {
            try
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(plansViewModel.CardProviderName))
                {
                    if (plansViewModel.CardProviderId != 0)
                    {
                        dto.PartnerDto updatePartner = new dto.PartnerDto()
                        {
                            Id = plansViewModel.CardProviderId,
                            Name = plansViewModel.CardProviderName,
                            Type = (int)PartnerType.CardProvider
                        };
                        var result = await _membershipService.UpdateCardProviderAsync(updatePartner);
                        if (result)
                        {
                            return Json(JsonResponse<int>.SuccessResponse(plansViewModel.CardProviderId));
                        }
                    }
                    else
                    {
                        dto.PartnerDto createPartner = new dto.PartnerDto()
                        {
                            Name = plansViewModel.CardProviderName,
                            Type = (int)PartnerType.CardProvider
                        };
                        var result = await _membershipService.CreateCardProviderAsync(createPartner);
                        if (result != null && result.Id > 0)
                        {
                            return Json(JsonResponse<int>.SuccessResponse(result.Id));
                        }
                        else
                        {
                            return Json(JsonResponse<int>.ErrorResponse("Error while creating card provider."));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<int>.ErrorResponse("Error while creating card provider."));
        }

        [HttpPost]
        public async Task<IActionResult> SaveMembershipPlans(WhiteLabelSettingMembershipPlan plan, int whiteLabelId, int cardProviderId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.MembershipPlan planDto = _mapper.Map<dto.MembershipPlan>(plan);
                    planDto = await ConfigurePlanDTOAsync(planDto, whiteLabelId, cardProviderId);

                    if (!GetSumOfCashbackSharing(planDto))
                    {
                        return Json(JsonResponse<int>.ErrorResponse("The total sum in cash sharing must be 100."));
                    }

                    if (planDto.Id != 0)
                    {
                        var result = await _membershipService.UpdateMembershipPlanAsync(planDto);
                        if (result)
                        {
                            return Json(JsonResponse<int>.SuccessResponse(plan.Id));
                        }
                    }
                    else
                    {
                        var result = await _membershipService.CreateMembershipPlanAsync(planDto);
                        if (result != null && result.Id > 0)
                        {
                            return Json(JsonResponse<int>.SuccessResponse(result.Id));
                        }
                        else
                        {
                            return Json(JsonResponse<int>.ErrorResponse("Error while creating membership plan."));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<int>.ErrorResponse("Error while creating membership plan."));
        }

        [HttpPost]
        public async Task<IActionResult> SaveAgent(WhiteLabelSettingsAgents agent, int membershipPlanId)
        {
            string errorMessage = "Error while creating/updating agent.";
            try
            {
                if (ModelState.IsValid)
                {
                    if (agent.Id != 0)
                    {
                        dto.AgentCode updateAgent = _mapper.Map<dto.AgentCode>(agent);
                        var result = await _membershipService.UpdateAgentAsync(updateAgent);
                        var dtoPlan = _membershipService.GetMembershipPlanById(membershipPlanId);
                        if (result && dtoPlan != null)
                        {
                            dtoPlan.AgentCodeId = agent.Id;
                            var updated = await _membershipService.UpdateMembershipPlanAsync(dtoPlan);
                            if (updated)
                            {
                                return Json(JsonResponse<int>.SuccessResponse(agent.Id));
                            }
                        }
                        else
                        {
                            errorMessage = "Error while updating agent.";
                        }
                    }
                    else
                    {
                        dto.AgentCode createAgent = _mapper.Map<dto.AgentCode>(agent);
                        var result = await _membershipService.CreateAgentAsync(createAgent);
                        var dtoPlan = _membershipService.GetMembershipPlanById(membershipPlanId);
                        if (result != null && result.Id > 0 && dtoPlan != null)
                        {
                            dtoPlan.AgentCodeId = result.Id;
                            var updated = await _membershipService.UpdateMembershipPlanAsync(dtoPlan);
                            if (updated)
                            {
                                return Json(JsonResponse<int>.SuccessResponse(result.Id));
                            }
                        }
                        else
                        {
                            errorMessage = "Error while creating agent.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<int>.ErrorResponse(errorMessage));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAgent(int membershipPlanId)
        {
            string errorMessage = "Error while clearing Agent.";
            try
            {
                var dtoPlan = _membershipService.GetMembershipPlanById(membershipPlanId);
                dtoPlan.AgentCodeId = null;
                var updated = await _membershipService.UpdateMembershipPlanAsync(dtoPlan);
                if (updated)
                {
                    return Json(JsonResponse<int>.SuccessResponse(0));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<int>.ErrorResponse(errorMessage));
        }

        [HttpPost]
        public async Task<IActionResult> SaveRegistrationCode(RegistrationCodesSummary registrationCodeSummary, bool isUnique)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool created = false;
                    dto.RegistrationCodeSummary summary = _mapper.Map<dto.RegistrationCodeSummary>(registrationCodeSummary);
                    summary.BlobConnectionString = _settings.Value.BlobConnectionString;
                    summary.ContainerName = _settings.Value.FilesContainerName;
                    if (isUnique)
                    {
                        summary.NumberOfUses = 1;
                        created = await _membershipService.CreateRegistrationBatchAsync(summary);
                    }
                    else
                    {
                        summary.NumberOfUses = summary.NumberOfCodes;
                        summary.NumberOfCodes = 1;
                        created = await _membershipService.CreateRegistrationBatchAsync(summary);
                    }

                    if (created)
                    {
                        return Json(JsonResponse<int>.SuccessResponse(summary.Id));
                    }
                    else
                    {
                        return Json(JsonResponse<int>.ErrorResponse("Error while creating registration code summary."));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<int>.ErrorResponse("Error while creating registration code summary."));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadRegistrationCode(int registrationSummaryId, int membershipPlanId, string storagePath)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var summary = new dto.RegistrationCodeSummary
                    {
                        Id = registrationSummaryId,
                        MembershipPlanId = membershipPlanId,
                        BlobConnectionString = _settings.Value.BlobConnectionString,
                        ContainerName = _settings.Value.FilesContainerName,
                        StoragePath = storagePath
                    };
                    var downloadedStream = await _membershipService.DownloadRegistrationAsync(summary);
                    if (downloadedStream != null)
                    {
                        var fileName = $"RegistrationCodes_{summary.MembershipPlanId}_{summary.Id}.csv";
                        return File(downloadedStream, "application/octet-stream", fileName);
                    }
                    else
                    {
                        return Json(JsonResponse<int>.ErrorResponse("Error while downloading the registration code summary."));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<int>.ErrorResponse("Error while downloading the registration code summary."));
        }

        [HttpPost]
        public async Task<IActionResult> GenerateRegistrationCodeURL(int registrationCodeSummaryId, int whiteLabelId)
        {
            string url = null;
            try
            {
                var codes = await _membershipService.GetAllRegistrationCodesAsync(registrationCodeSummaryId);
                var registrationCode = codes.FirstOrDefault();
                var encryptedCode = EncryptionHelper.Encrypt(registrationCode.RegistartionCode);
                var response = await _membershipService.GetAllSitesAsync();
                if (response != null && response.Any())
                {
                    var whiteLabelDto = response.FirstOrDefault(x => x.Id == whiteLabelId);
                    url = $"{whiteLabelDto.URL}/Account/Register?code={encryptedCode}";
                    return Json(JsonResponse<string>.SuccessResponse(url));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<string>.ErrorResponse(url));
        }

        [HttpPost]
        public async Task<IActionResult> CopyRegistrationCode(int registrationCodeSummaryId)
        {
            string registrationCode = null;
            try
            {
                var codes = await _membershipService.GetAllRegistrationCodesAsync(registrationCodeSummaryId);
                registrationCode = codes.FirstOrDefault().RegistartionCode;
                return Json(JsonResponse<string>.SuccessResponse(registrationCode));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return Json(JsonResponse<string>.ErrorResponse(registrationCode));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCardProviders(int? cardProviderId)
        {
            PlansViewModel plans = new PlansViewModel();
            plans.CardProviders = await MapToCardProvidersAsync(cardProviderId);
            if (cardProviderId.HasValue)
            {
                plans.CardProviderId = cardProviderId.Value;
                plans.CardProviderName = "";
            }
            return PartialView("_whiteLabelSettingsCardProviders", plans);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembershipPlans(int whiteLabelId, int cardProviderId, int? membershipPlanId)
        {
            WhiteLabelSettingMembershipPlan plan = new WhiteLabelSettingMembershipPlan();
            plan = await MapToMembershipPlansAsync(whiteLabelId, cardProviderId, plan, membershipPlanId);
            return PartialView("_whiteLabelSettingsMembershipPlan", plan);
        }

        [HttpGet]
        public async Task<IActionResult> CheckHasStandardPlan(int whiteLabelId, int cardProviderId)
        {
            var hasStandardPlan = false;
            var response = await _membershipService.GetAllMembershipPlansAsync(whiteLabelId, cardProviderId);
            if (response != null && response.Any())
            {
                var listOfStandardPlans = response.Where(x => x.MembershipLevelId == (int)Enums.MembershipLevel.Standard).ToList();
                if (listOfStandardPlans != null && listOfStandardPlans.Any())
                {
                    hasStandardPlan = true;
                }
            }
            return Json(JsonResponse<bool>.SuccessResponse(hasStandardPlan));
        }

        [HttpGet]
        public async Task<IActionResult> GetAgents(int? agentId)
        {
            WhiteLabelSettingsAgents agent = new WhiteLabelSettingsAgents();
            agent = await MapToAgentsAsync(agent, agentId);
            return PartialView("_whiteLabelSettingsAgents", agent);
        }

        [HttpGet]
        public IActionResult GetRegistrationCodes(int membershipPlanId)
        {
            List<RegistrationCodesSummary> registrationCode = new List<RegistrationCodesSummary>();
            registrationCode = MapToRegistrationCodeSummary(registrationCode, membershipPlanId);
            return PartialView("_whiteLabelSettingsRegistrationCodes", registrationCode);
        }

        #region Private Methods

        /// <summary>
        /// Map retrieved white labels from cache or db to dropdown
        /// </summary>
        /// <returns></returns>
        private async Task<List<SelectListItem>> MapToWhiteLabelsAsync()
        {
            var response = await _membershipService.GetAllSitesAsync();
            response = response.OrderBy(x => x.Name).ToList();
            if (response != null && response.Any())
            {
                return response.Select(plan => new SelectListItem()
                {
                    Text = plan.Name,
                    Value = plan.Id.ToString()
                }).ToList();
            }
            return new List<SelectListItem>();
        }

        /// <summary>
        /// Map retrieved card providers from cache or db to dropdown
        /// </summary>
        /// <param name="cardProviderId"></param>
        /// <returns></returns>
        private async Task<List<SelectListItem>> MapToCardProvidersAsync(int? cardProviderId = null)
        {
            var response = await _membershipService.GetAllCardProvidersAsync();
            response = response.OrderBy(x => x.Name).ToList();
            if (response != null && response.Any())
            {
                var dropdowns = response.Select(cardProvider => new SelectListItem()
                {
                    Text = cardProvider.Name,
                    Value = cardProvider.Id.ToString(),
                    Selected = cardProviderId.HasValue && cardProvider.Id == cardProviderId.Value
                }).ToList();
                return dropdowns;
            }
            return new List<SelectListItem>();
        }

        /// <summary>
        /// Map retrieved membership plans from db to dropdown
        /// </summary>
        /// <param name="whiteLabelId"></param>
        /// <param name="cardProviderId"></param>
        /// <param name="membershipPlan"></param>
        /// <param name="membershipPlanId"></param>
        /// <returns></returns>
        private async Task<WhiteLabelSettingMembershipPlan> MapToMembershipPlansAsync(int whiteLabelId, int cardProviderId,
            WhiteLabelSettingMembershipPlan membershipPlan, int? membershipPlanId = null)
        {
            var response = await _membershipService.GetAllMembershipPlansAsync(whiteLabelId, cardProviderId);
            response = response.OrderBy(x => x.Description).ToList();
            if (response != null && response.Any())
            {
                var selectedMembershipPlan = response.FirstOrDefault(x => x.Id == membershipPlanId);
                if (selectedMembershipPlan != null)
                {
                    membershipPlan = _mapper.Map<WhiteLabelSettingMembershipPlan>(selectedMembershipPlan);
                    if (membershipPlan.Id > 0 &&
                        membershipPlan.ListOfPlanTypes.FirstOrDefault(x => x.Value == membershipPlan.Id.ToString()) != null)
                    {
                        membershipPlan.ListOfPlanTypes.FirstOrDefault(x => x.Value == membershipPlan.Id.ToString()).Selected = true;
                    }

                    if (membershipPlan.MembershipLevelId > 0 &&
                        membershipPlan.ListOfMembershipTypes.FirstOrDefault(x => x.Value == membershipPlan.MembershipLevelId.ToString()) != null)
                    {
                        membershipPlan.ListOfMembershipTypes.FirstOrDefault(x => x.Value == membershipPlan.MembershipLevelId.ToString()).Selected = true;
                    }
                }

                membershipPlan.MembershipPlans = response.Select(plan => new SelectListItem()
                {
                    Text = plan.Description,
                    Value = plan.Id.ToString(),
                    Selected = membershipPlanId.HasValue && plan.Id == membershipPlanId.Value
                }).ToList();
                membershipPlan.ListOfMembershipTypes = Enum.GetValues(typeof(Enums.MembershipLevel)).Cast<Enums.MembershipLevel>()
                .Select(membershipType => new SelectListItem
                {
                    Value = ((int)membershipType).ToString(),
                    Text = membershipType.ToString(),
                    Selected = membershipType.ToString() == "Diamond"
                }).OrderBy(x => x.Text).ToList();
                membershipPlan.ListOfPlanTypes = Enum.GetValues(typeof(Enums.MembershipPlanTypeEnum)).Cast<Enums.MembershipPlanTypeEnum>()
                .Select(planType => new SelectListItem
                {
                    Value = ((int)planType).ToString(),
                    Text = planType.ToString(),
                    Selected = planType.ToString() == "Partner Rewards"
                }).OrderBy(x => x.Text).ToList();

                return membershipPlan;
            }
            else
            {
                membershipPlan.ListOfMembershipTypes = Enum.GetValues(typeof(Enums.MembershipLevel)).Cast<Enums.MembershipLevel>()
                .Select(membershipType => new SelectListItem
                {
                    Value = ((int)membershipType).ToString(),
                    Text = membershipType.ToString(),
                    Selected = membershipType.ToString() == "Diamond"
                }).ToList();
                membershipPlan.ListOfPlanTypes = Enum.GetValues(typeof(Enums.MembershipPlanTypeEnum)).Cast<Enums.MembershipPlanTypeEnum>()
                .Select(planType => new SelectListItem
                {
                    Value = ((int)planType).ToString(),
                    Text = planType.ToString(),
                    Selected = planType.ToString() == "Partner Rewards"
                }).ToList(); 

                membershipPlan.MembershipPlans = new List<SelectListItem>();
                return membershipPlan;
            }
        }

        /// <summary>
        /// Map retrieved agents from db to dropdown
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        private async Task<WhiteLabelSettingsAgents> MapToAgentsAsync(WhiteLabelSettingsAgents agent, int? agentId)
        {
            var response = await _membershipService.GetAllAgentsAsync();
            response = response.OrderBy(x => x.Description).ToList();
            if (response != null && response.Any())
            {
                var selectedAgent = response.FirstOrDefault(x => x.Id == agentId);
                if (selectedAgent != null)
                {
                    agent = _mapper.Map<WhiteLabelSettingsAgents>(selectedAgent);
                }
                agent.Agents = response.Select(agent => new SelectListItem()
                {
                    Text = agent.Description,
                    Value = agent.Id.ToString(),
                    Selected = agentId.HasValue && agent.Id == agentId.Value
                }).ToList();
                return agent;
            }
            else
            {
                agent.Agents = new List<SelectListItem>();
                return agent;
            }
        }

        /// <summary>
        /// Map retrieved registration code summaries
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>List of RegistrationCodesSummary model</returns>
        private List<RegistrationCodesSummary> MapToRegistrationCodeSummary(List<RegistrationCodesSummary> registrationCode, int membershipPlanId)
        {
            // retrieving list if registration summaries associated with the selected plan.
            var response = _membershipService.GetAllSummaries(membershipPlanId);
            if (response != null && response.Any())
            {
                registrationCode = _mapper.Map<List<RegistrationCodesSummary>>(response);
                return registrationCode;
            }
            else
            {
                return registrationCode;
            }
        }
        
        private async Task<dto.MembershipPlan> ConfigurePlanDTOAsync(dto.MembershipPlan planDto, int whiteLabelId, int cardProviderId)
        {
            planDto.WhitelabelId = whiteLabelId;
            planDto.CardProviderId = cardProviderId;
            planDto.CurrencyCode = "GBP";
            planDto.IsActive = true;
            planDto.IsDeleted = false;            

            if (!planDto.PaidByEmployer)
            {
                planDto.PartnerCardPrice = 0;
                if (planDto.MembershipLevelId == (int)Enums.MembershipLevel.Diamond)
                {
                    var planType = (Enums.MembershipPlanTypeEnum)planDto.MembershipPlanTypeId;
                    var defaultPlan = await _membershipService.GetDefaultPlanAsync(planType, MembershipLevel.Diamond);
                    planDto.CustomerCardPrice = defaultPlan.CustomerCardPrice;
                    planDto.PartnerCardPrice = 0;
                }
                else
                {
                    planDto.CustomerCardPrice = 0;
                    planDto.PartnerCardPrice = 0;
                }
            }
            else
            {
                planDto.PartnerCardPrice = planDto.PartnerCardPrice;
                planDto.CustomerCardPrice = 0;
            }

            if (planDto.MembershipPlanTypeId == (int)Enums.MembershipPlanTypeEnum.BenefitRewards)
            {
                planDto.BenefactorPercentage = planDto.BenefactorPercentage;
            }
            else
            {
                planDto.BenefactorPercentage = 0;
            }
            return planDto;
        }

        private bool GetSumOfCashbackSharing(dto.MembershipPlan planDto)
        {
            bool result = false;
            if ((Enums.MembershipPlanTypeEnum)planDto.MembershipPlanTypeId == Enums.MembershipPlanTypeEnum.BenefitRewards)
            {
                result = (planDto.DeductionPercentage + planDto.CustomerCashbackPercentage + planDto.BenefactorPercentage) == 100;
            }
            else
            {
                result = (planDto.DeductionPercentage + planDto.CustomerCashbackPercentage) == 100;
            }
            return result;
        }        

        #endregion Private Methods
    }
}