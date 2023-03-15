using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ITagService = ExclusiveCard.Services.Interfaces.Public.ITagService;
using ExclusiveCard.Website.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

using Microsoft.Extensions.Caching.Memory;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Website.Helpers;
using Microsoft.AspNetCore.Http;
using Renci.SshNet;

namespace ExclusiveCard.Website.Controllers
{
    public class SportRewardsController : BaseController
    {
        #region Private Members
        private readonly ITagService _tagService;
        private readonly IMemoryCache _cache;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ITalkSportService _talkSportService;
        public WhiteLabelSettings whiteLabelSettings;

        #endregion

        #region Constructor

        public SportRewardsController(ITagService tagService, 
            IMemoryCache cache, 
            ICustomerAccountService customerAccountService,
            ITalkSportService talkSportService)
        {
            _tagService = tagService;
            _cache = cache;
            _customerAccountService = customerAccountService;
             _talkSportService= talkSportService;           
        }

        #endregion
        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            
            try
            {
                var request = HttpContext.Request;
                string currentUrl = $"{request.Scheme}://{request.Host.Value}";
                whiteLabelSettings =
                        _cache.Get<dto.WhiteLabelSettings>(string.Format(Data.Constants.Keys.WhiteLabel,
                            currentUrl));
                var leagues = await _talkSportService.GetLeagues(whiteLabelSettings.Id);
                return View("Index", leagues);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred."));
            }
        }

        [HttpGet]
        [ActionName("Standard")]
        public IActionResult Standard()
        {
            try
            {
                var request = HttpContext.Request;
                string currentUrl = $"{request.Scheme}://{request.Host.Value}";
                UserRegistrationTempData userRegistrationTempData =
                    HttpContext.Session.GetObjectFromJson<UserRegistrationTempData>(Data.Constants.Keys
                        .UserRegistrationTempData);
                if (userRegistrationTempData == null)
                {
                    userRegistrationTempData=new UserRegistrationTempData();
                }

                var whiteLabel =
                    _cache.Get<dto.WhiteLabelSettings>(string.Format(Data.Constants.Keys.WhiteLabel,
                        currentUrl));

                    if (whiteLabel != null)
                    {
                        int membershipPlanTypeId = (int)Enums.MembershipPlanTypeEnum.PartnerReward;
                        var plan = _customerAccountService.GetTalkSportRegistrationCode(whiteLabel.Id,
                            membershipPlanTypeId);
                        if (plan != null)
                        {
                            string code = plan.MembershipRegistrationCodes.FirstOrDefault()?.RegistartionCode;
                            dto.UserToken userToken = _customerAccountService.ValidateRegistrationCode(code);
                            userRegistrationTempData.Token = userToken;
                            HttpContext?.Session?.SetObjectAsJson(Data.Constants.Keys.UserRegistrationTempData,
                                userRegistrationTempData);
                            ViewBag.userToken = userToken;
                        }
                    }
                
                return View("Standard");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred."));
            }
        }

        
      
        
        public async Task<IActionResult> Club()
        {
            try
            {
                SiteClan siteClan = new SiteClan();
                League league = new League();

                UserRegistrationTempData userRegistrationTempData =
                    HttpContext.Session.GetObjectFromJson<UserRegistrationTempData>(Data.Constants.Keys
                        .UserRegistrationTempData);


                var request = HttpContext.Request;
                string currentUrl = $"{request.Scheme}://{request.Host.Value}";

                var whiteLabel =
                    _cache.Get<dto.WhiteLabelSettings>(string.Format(Data.Constants.Keys.WhiteLabel,
                        currentUrl));

                if (userRegistrationTempData.SiteClanId.HasValue && userRegistrationTempData.SiteClanId.Value > 0)
                {
                    var result = await _talkSportService.GetSiteClanById(userRegistrationTempData.SiteClanId.Value);
                    //siteClan.CharityName = result.Charity.CharityName;
                    siteClan.Charity = result.Charity;
                    siteClan.Description = result.Description;
                    siteClan.Id = userRegistrationTempData.SiteClanId.Value;
                    league.Description = userRegistrationTempData.LeagueDescription;
                    siteClan.League = league;

                    if (whiteLabel != null)
                    {
                        int membershipPlanTypeId = (int) Enums.MembershipPlanTypeEnum.BenefitRewards;
                        var plan = _customerAccountService.GetTalkSportRegistrationCode(whiteLabel.Id,
                            membershipPlanTypeId);
                        if (plan != null)
                        {
                            string code = plan.MembershipRegistrationCodes.FirstOrDefault()?.RegistartionCode;
                            dto.UserToken userToken = _customerAccountService.ValidateRegistrationCode(code);
                            userRegistrationTempData.Token = userToken;
                            HttpContext?.Session?.SetObjectAsJson(Data.Constants.Keys.UserRegistrationTempData,
                                userRegistrationTempData);
                            ViewBag.userToken = userToken;
                        }
                    }
                }
                
                //
                
                return View("Club", siteClan);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                var keys = string.Join(", ", HttpContext.Session.Keys.ToArray());
                return Json(JsonResponse<string>.ErrorResponse($"Error occurred. SessionKeys Present={keys} \r\n {e.ToString()}."));
            }
        }
        
        [HttpPost]
        public IActionResult GetClub(League league)
        {
            try
            {
                UserRegistrationTempData userRegistrationTempData = new UserRegistrationTempData();
                if (league !=null)
                {
                    if (league.Id > 0 && !string.IsNullOrEmpty(league.Description))
                    {
                        userRegistrationTempData.SiteClanId = league.Id;
                        userRegistrationTempData.LeagueDescription = league.Description;
                        HttpContext.Session.SetObjectAsJson(Data.Constants.Keys.UserRegistrationTempData,
                            userRegistrationTempData);

                        return Json(JsonResponse<bool>.SuccessResponse(true));
                    }
                }
                return Json(JsonResponse<string>.ErrorResponse($"Please select a league. Id= {league?.Id} Description={league?.Description}"));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse($"Error occurred in GetClub {e.ToString()}."));
            }
        }


        [HttpGet]
        [ActionName("GetTalkSportRegistrationCode")]
        public IActionResult GetTalkSportRegistrationCode(bool isBenefitReward)
        {
            try
            {
                var request = HttpContext.Request;
                string currentUrl = $"{request.Scheme}://{request.Host.Value}";

                var whiteLabel =
                    _cache.Get<dto.WhiteLabelSettings>(string.Format(Data.Constants.Keys.WhiteLabel,
                        currentUrl));

                if (whiteLabel != null)
                {
                    int membershipPlanTypeId = isBenefitReward ? (int)Enums.MembershipPlanTypeEnum.BenefitRewards : (int)Enums.MembershipPlanTypeEnum.PartnerReward;
                    var plan = _customerAccountService.GetTalkSportRegistrationCode(whiteLabel.Id, membershipPlanTypeId);
                    if (plan != null)
                    {
                        string code = plan.MembershipRegistrationCodes.FirstOrDefault()?.RegistartionCode;
                        dto.UserToken userToken = _customerAccountService.ValidateRegistrationCode(code);
                        return Json(JsonResponse<object>.SuccessResponse(userToken));
                    }
                }
                return Json(JsonResponse<string>.ErrorResponse("We couldn't find an account matching the registration code you entered. Please check your registration code and try again."));


            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred."));
            }
        }
    }
}
