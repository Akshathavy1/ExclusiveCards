using System;
using System.Collections.Generic;
using System.Linq;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using dto = ExclusiveCard.Services.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace ExclusiveCard.Website.Filters
{

    public class GlobalSetDefaultViewDataValuesAttribute : ActionFilterAttribute
    {
        #region Private Methods and constructor

        private readonly IMemoryCache _cache;
        private readonly IOptions<TypedAppSettings> _settings;
        private readonly IWhiteLabelService _whiteLabelService;
        private readonly IOfferTypeService _offerTypeService;
        private readonly Services.Interfaces.Public.ICategoryService _categoryService;
        private readonly Services.Interfaces.Admin.IMerchantService _merchantService;
        private readonly IWebsiteSocialMediaService _socialMediaService;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ITalkSportService _talkSportService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerSecurityQuestionService _customerSecurityQuestionService;


        public GlobalSetDefaultViewDataValuesAttribute(IMemoryCache cache,
            IOptions<TypedAppSettings> settings,
            IWhiteLabelService whiteLabelService,
            IOfferTypeService offerTypeService,
            Services.Interfaces.Public.ICategoryService categoryService,
            Services.Interfaces.Admin.IMerchantService merchantService,
            IWebsiteSocialMediaService socialMediaService,
            ICustomerAccountService customerAccountService,
            ITalkSportService talkSportService,
            IHttpContextAccessor httpContextAccessor,
            ICustomerSecurityQuestionService customerSecurityQuestionService)
        {
            _cache = cache;
            _settings = settings;
            _whiteLabelService = whiteLabelService;
            _offerTypeService = offerTypeService;
            _categoryService = categoryService;
            _merchantService = merchantService;
            _socialMediaService = socialMediaService;
            _customerAccountService = customerAccountService;
            _talkSportService = talkSportService;
            _httpContextAccessor = httpContextAccessor;
            _customerSecurityQuestionService = customerSecurityQuestionService;
        }

        #endregion

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            string currentUrl = $"{request.Scheme}://{request.Host.Value}";
            var user = _customerAccountService.GetUserAsync(context.HttpContext.User).Result;

            var isCookieExist = request.Cookies["siteClanCookie"];
            var siteClan =new dto.SiteClan();
            if (isCookieExist != null)
            {
                siteClan = JsonConvert.DeserializeObject<dto.SiteClan>(request.Cookies["siteClanCookie"]);
            }

            LayoutViewModel model = new LayoutViewModel
            {
                IsSignedIn = (user != null),
                Settings = _settings.Value,
                OfferTypes = _cache.Get<List<dto.OfferType>>(Data.Constants.Keys.LayoutOfferTypes),
                Categories = _cache.Get<List<dto.Category>>(Data.Constants.Keys.ParentCategories),
                Merchants = _cache.Get<string>(Data.Constants.Keys.MerchantList),
                WhiteLabel = _cache.Get<dto.WhiteLabelSettings>(string.Format(Data.Constants.Keys.WhiteLabel, currentUrl)),
                SocialMediaLinks = _cache.Get<List<dto.Public.WebsiteSocialMediaLink>>(string.Format(Data.Constants.Keys.SocialMediaLinks, currentUrl)),
                DiamondCost = _cache.Get<decimal>(Data.Constants.Keys.DiamondCost),
                ConsumerRights = request.Host.Value.Contains("Consumer-Rights", StringComparison.CurrentCultureIgnoreCase),
                SiteClan = siteClan,
                SponsorImages = _cache.Get<List<dto.SponsorImages>>(string.Format(Data.Constants.Keys.SponsorImages, currentUrl)),

            };

            if (model.DiamondCost == 0m)
            {
                var membershipPlan = _customerAccountService.GetDefaultDiamondPlan();
                if (membershipPlan != null)
                {
                    model.DiamondCost = membershipPlan.CustomerCardPrice;
                    _cache.Set(Data.Constants.Keys.DiamondCost, model.DiamondCost, DateTimeOffset.UtcNow.AddDays(1));
                }
            }

            if (model.OfferTypes == null || model.OfferTypes.Count == 0)
            {
                model.OfferTypes = _offerTypeService.GetAll().Result;
                _cache.Set(Data.Constants.Keys.LayoutOfferTypes, model.OfferTypes, DateTimeOffset.UtcNow.AddHours(1));
            }

            if (model.Categories == null || model.Categories.Count == 0)
            {
                var categories = _categoryService.GetAllparentcategories().Result;
                if (categories != null)
                {
                    model.Categories = categories;
                    _cache.Set(Data.Constants.Keys.ParentCategories, model.Categories,
                        DateTimeOffset.UtcNow.AddHours(1));
                }
                else
                {
                    model.Categories = new List<dto.Category>();
                }
            }



            if (model.Merchants == null)
            {
                List<dto.Merchant> merchants = _merchantService.GetAll().Result;
                if (merchants != null)
                {
                    List<string> merchantsList = merchants.Select(merchantNew => new string(merchantNew.Name)).ToList();



                    model.Merchants = JsonConvert.SerializeObject(merchantsList);
                    _cache.Set(Data.Constants.Keys.MerchantList, model.Merchants, DateTime.UtcNow.AddHours(1));
                }
            }


            if (model.WhiteLabel == null)
            {
                model.WhiteLabel = _whiteLabelService.GetSiteSettings(currentUrl);
                _cache.Set(string.Format(Data.Constants.Keys.WhiteLabel, currentUrl), model.WhiteLabel, DateTime.UtcNow.AddHours(6));
            }

            if (model.SponsorImages == null)
            {
                model.SponsorImages = _whiteLabelService.GetSponsorImagesById(model.WhiteLabel.Id).Result;
                _cache.Set(string.Format(Data.Constants.Keys.SponsorImages, currentUrl), model.SponsorImages, DateTime.UtcNow.AddHours(6));
            }


            if (model.SocialMediaLinks == null)
            {
                //model.SocialMediaLinks = _socialMediaService.GetAllAsync().Result;
                model.SocialMediaLinks = _socialMediaService.GetSocialMediaLinks(model.WhiteLabel.Id).Result;
                _cache.Set(string.Format(Data.Constants.Keys.SocialMediaLinks, currentUrl), model.SocialMediaLinks, DateTime.UtcNow.AddHours(1));
            }

            if (model.IsSignedIn)
            {
                var customerSummary = _cache.Get<dto.CustomerAccountSummary>(string.Format(Data.Constants.Keys.CustomerSummary, user.Id));
                if (customerSummary == null)
                {
                    customerSummary = _customerAccountService.GetAccountSummary(user.Id);
                    _cache.Set(string.Format(Data.Constants.Keys.CustomerSummary, user.Id), customerSummary, DateTime.UtcNow.AddMinutes(15));
                   
                }
                else
                {
                    var isExisted = _customerSecurityQuestionService.Get(customerSummary.CustomerId);
                    if (isExisted != null)
                    {
                        model.CustomerSecurity = true;
                    }
                    else { model.CustomerSecurity = false; }
                }

                model.EmailConfirmed = customerSummary.EmailConfirmed;
                if (model.WhiteLabel != null)
                    model.WhiteLabel.CardName = customerSummary.PlanName;
                model.CustomerName = customerSummary.CustomerName;
                model.CurrentValue = customerSummary.Balances?.CurrentValue;
                model.MembershipCardId = customerSummary.MembershipCardId;
                model.MembershipPlanId = customerSummary.MembershipPlanId;
                model.IsDiamondCustomer = customerSummary.IsDiamondCustomer;
                model.DiamondExpiry = customerSummary.CardExpiryDate;


                if (customerSummary.SiteClanId !=null)
                {
                     model.SiteClan = _talkSportService.GetSiteClanById(Convert.ToInt32(customerSummary.SiteClanId)).Result;
                     var json = JsonConvert.SerializeObject(model.SiteClan);
                     //context.HttpContext.Response.Cookies.Append("siteClanCookie", json);
                }

            }


            var control = context.Controller as Controller;
            if (control == null) return;
            control.ViewBag.LayoutModel = model;
            control.ViewBag.WhiteLabel = model.WhiteLabel;

            string country = "GB"; //TODO Config this
            if (context.ActionArguments.TryGetValue("country", out object value))
            {
                if (value != null)
                {
                    country = value.ToString();
                }
            }

            if (context.Controller is Controller controller)
            {
                controller.ViewData["Country"] =
                    country; //TODO Add country to some constants class and replace everywhere
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            string country = "GB"; //TODO Config this

            if (context.Controller is Controller controller)
            {
                //this is a guard if the value is set to null set the default value
                if (controller.ViewData["Country"] == null)
                {
                    controller.ViewData["Country"] = country; //TODO Add country to some constants class and replace everywhere
                }
            }
            //check if request is coming from mobile by looking into headers

            //var request = context.HttpContext.Request;
            //bool isMobile = false;
            //if (request.Headers.ContainsKey("IsMobile"))
            //{
            //    Boolean.TryParse(request.Headers["IsMobile"], out isMobile);
            //}
            //else if (request.Query["IsMobile"].Count > 0)
            //{
            //    Microsoft.Extensions.Primitives.StringValues queryVal;

            //    if (request.Query.TryGetValue("IsMobile", out queryVal))
            //    {
            //        Boolean.TryParse(queryVal[0], out isMobile);
            //    }
            //}
            //if (isMobile)
            //{
            //    bool cookieExists = request.Cookies["cookieconsent_status"] == null;
            //    if (cookieExists)
            //    {
            //        context.HttpContext.Response.Cookies.Append("cookieconsent_status", "dismiss");
            //    }
            //}

            base.OnActionExecuted(context);
        }
    }
}
