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
    public class SiteClanController : BaseController
    {
        #region Private Members
        private readonly IMemoryCache _cache;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ISiteClanService _siteClanService;
        public WhiteLabelSettings whiteLabelSettings;

        #endregion

        #region Constructor

        public SiteClanController(
            IMemoryCache cache,
            ICustomerAccountService customerAccountService,
            ISiteClanService siteClanService)
        {
            _cache = cache;
            _customerAccountService = customerAccountService;
            _siteClanService = siteClanService;            
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
                var leagues = await _siteClanService.GetLeagues(whiteLabelSettings.Id);
                if (leagues != null)
                {
                    var siteClans = leagues.FirstOrDefault().SiteClan;
                    var siteOwner = siteClans.FirstOrDefault().SiteOwner;
                    ViewBag.SiteOwner = siteOwner;
                    HttpContext?.Session?.SetObjectAsJson(Data.Constants.Keys.SiteOwner,
                            siteOwner);

                }
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
                    userRegistrationTempData = new UserRegistrationTempData();
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
                   
                    dto.SiteOwner siteOwnerData = HttpContext.Session.GetObjectFromJson<dto.SiteOwner>(Data.Constants.Keys.SiteOwner);
                    ViewBag.SiteOwner = siteOwnerData;
                }

                return View("Standard");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred."));
            }
        }


        public async Task<IActionResult> Confirmation()
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
                    var result = await _siteClanService.GetSiteClanById(userRegistrationTempData.SiteClanId.Value);
                    //siteClan.CharityName = result.Charity.CharityName;
                    siteClan.Charity = result.Charity;
                    siteClan.Description = result.Description;
                    siteClan.Id = userRegistrationTempData.SiteClanId.Value;
                    league.Description = userRegistrationTempData.LeagueDescription;
                    siteClan.League = league;

                    if (whiteLabel != null)
                    {
                        int membershipPlanTypeId = (int)Enums.MembershipPlanTypeEnum.BenefitRewards;
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

                        dto.SiteOwner siteOwnerData = HttpContext.Session.GetObjectFromJson<dto.SiteOwner>(Data.Constants.Keys.SiteOwner);
                        ViewBag.SiteOwner = siteOwnerData;
                    }
                }                
                //

                return View("Confirmation", siteClan);
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
                if (league != null)
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
        [ActionName("GetRegistrationCode")]
        public IActionResult GetRegistrationCode(bool isBenefitReward)
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
