using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Helpers;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using ExclusiveCard.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using dto = ExclusiveCard.Services.Models.DTOs;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using ExclusiveCard.Data.Context;
using serve = ExclusiveCard.Website.App.ServiceHelper;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace ExclusiveCard.Website.Controllers
{
    public class OffersController : BaseController
    {
        #region Private Members

        private readonly IMemoryCache _cache;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IMembershipCardService _cardService;
        private readonly SignInManager<ExclusiveUser> _signInManager;
        private readonly IOffersService _offersService;
        private readonly ICategoryService _categoryService;
        private readonly IOfferBranchServices _offerBranchServices;
        private readonly Services.Interfaces.Admin.IMerchantService _merchantService;
        private readonly Services.Interfaces.Admin.IMerchantBranchService _merchantBranchService;

        private readonly IOptions<TypedAppSettings> _settings;
        private readonly Services.Interfaces.Admin.IMembershipCardAffiliateReferenceService _membershipCardAffiliateReferenceService;
        private readonly Services.Interfaces.Admin.IAffiliateMappingRuleService _affiliateMappingRuleService;
        private readonly Managers.IEmailManager _emailManager;
        private readonly IClickTrackingService _clickTrackingService;
        private readonly Services.Interfaces.Admin.IContactDetailService _contactDetailService;
        private readonly Services.Interfaces.Admin.IStatusServices _statusServices;
        private readonly Services.Interfaces.Admin.IOfferTypeService _offerTypeService;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly IConfiguration _configuration;

        #endregion Private Members

        #region Constructor

        public OffersController(IMemoryCache cache, IUserService userService, ICustomerService customerService,
            IMembershipCardService cardService, SignInManager<ExclusiveUser> signInManager, IOffersService offersService, ICategoryService categoryService,
            Services.Interfaces.Admin.IMerchantService merchantService,
            Services.Interfaces.Admin.IMerchantBranchService merchantBranchService,
            IOptions<TypedAppSettings> settings, Services.Interfaces.Admin.IMembershipCardAffiliateReferenceService membershipCardAffiliateReferenceService,
            Services.Interfaces.Admin.IAffiliateMappingRuleService affiliateMappingRuleService,
            Managers.IEmailManager emailService, IClickTrackingService clickTrackingService, Services.Interfaces.Admin.IContactDetailService contactDetailService,
            Services.Interfaces.Admin.IStatusServices statusServices, Services.Interfaces.Admin.IOfferTypeService offerTypeService,
            IOfferBranchServices offerBranchServices, ICustomerAccountService customerAccountService, IConfiguration configuration)
        {
            _cache = cache;
            _userService = userService;
            _customerService = customerService;
            _cardService = cardService;
            _signInManager = signInManager;
            _offersService = offersService;
            _categoryService = categoryService;
            _merchantService = merchantService;
            _merchantBranchService = merchantBranchService;
            _settings = settings;
            _membershipCardAffiliateReferenceService = membershipCardAffiliateReferenceService;
            _affiliateMappingRuleService = affiliateMappingRuleService;
            _emailManager = emailService;
            _clickTrackingService = clickTrackingService;
            _contactDetailService = contactDetailService;
            _statusServices = statusServices;
            _offerTypeService = offerTypeService;
            _offerBranchServices = offerBranchServices;
            _customerAccountService = customerAccountService;
            _configuration = configuration;
        }

        #endregion Constructor

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string country, int merchantId, int? offerId)
        {
            MerchantOffersViewModel model = new MerchantOffersViewModel();
            try
            {
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                await GetMerchantOffersAndMapToPagedData(merchantId, offerId, countryCode, 1, model);
                ViewBag.Offer = offerId;
                ViewBag.Merchant = model.Merchant?.Name;

                ViewBag.logo = model.Logo;
                if (model.Merchant.ContactDetailsId != null)
                {
                    model.Merchant.ContactDetail = await _contactDetailService.Get((int)model.Merchant.ContactDetailsId);
                }
                List<dto.MerchantBranch> merchantBranches = new List<dto.MerchantBranch>();
                merchantBranches = await _merchantBranchService.GetAll(merchantId);
                foreach (var item in merchantBranches)
                {
                    if (item.ContactDetail == null)
                    {
                        item.ContactDetail = await _contactDetailService.Get((int)item.ContactDetailsId);
                    }
                }
                if (offerId != 0)
                {
                    var branch = await _offerBranchServices.GetofferBranch((int)offerId);

                    List<dto.MerchantBranch> offerMerchantBranches = new List<dto.MerchantBranch>();
                    foreach (var id in branch)
                    {
                        offerMerchantBranches.Add(merchantBranches.FirstOrDefault(x => x.Id == id));
                    }
                    if (offerMerchantBranches != null)
                    {
                        foreach (var id in offerMerchantBranches)
                        {
                            if (id.ContactDetail != null)
                            {
                                if (id.ContactDetail.Address1 != null || id.ContactDetail.Address2 != null || id.ContactDetail.Address3 != null)
                                {
                                    model.MerchantBranches.Add(id);
                                }
                            }
                        }
                    }
                }

                ViewBag.merchantBranchCount = model.MerchantBranches.Count();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
            return View("Index", model);
        }

        [HttpGet]
        [ActionName("GetpagedData")]
        public async Task<IActionResult> GetPagedData(string country, int merchantId, int currentPage, string type)
        {
            MerchantOffersViewModel model = new MerchantOffersViewModel();
            try
            {
                CommonHelper.Initialize();
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                switch (type)
                {
                    case Keys.Keys.CashbackType:
                        break;

                    case Keys.Keys.StandardType:
                        break;

                    case Keys.Keys.VoucherType:
                        break;

                    case Keys.Keys.RestaurantType:
                        break;

                    case Keys.Keys.HighStreet:
                        break;

                    case Keys.Keys.SalesType:
                        break;
                }
                await OffersHelper.GetMerchantOffersAndMapToPagedData(merchantId, countryCode, currentPage, model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
            return View("Index", model);
        }

        [HttpPost]
        [ActionName("Search")]
        public async Task<IActionResult> Search(string country, List<int> categories, int offerTypeId,
            string merchantName, string keywords, string offerTypeName, string listName, int pageNumber)

        {
            //Return offersDetail view with pagination
            PagedOffersViewModel viewModel;
            try
            {
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                dto.OfferSearchCriteria criteria = new dto.OfferSearchCriteria
                {
                    KeyWord = keywords,
                    Categories = categories,
                    CurrentPage = pageNumber,
                    OfferType = offerTypeId
                };
                if (!string.IsNullOrEmpty(listName))
                {
                    criteria.SortOrder = 1;
                }

                viewModel = await SearchAndMapOffer(countryCode, criteria);
                ViewData["OfferType"] = offerTypeName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error retreiving data. Please try again."));
            }
            return PartialView("_offersDetail", viewModel);
        }

        [HttpGet]
        [ActionName("DisplayOffer")]
        public async Task<IActionResult> DisplayOffer(string country, string merchant, string searchCategories,
            string searchOfferTypes, int parentCategoryId, string parentCategoryName, string offerSort, int pageSize,
            int pageNumber, string checkedOfferType = null)
        {
            #region Initialize

            //pageNumber to default
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }
            //pageSize to default
            if (pageSize == 0)
            {
                pageSize = _settings.Value.OfferCount;
            }
            //offerSort to default
            if (string.IsNullOrEmpty(offerSort))
            {
                offerSort = _settings.Value.OfferSort;
            }
            //searchOfferTypes to default
            if (string.IsNullOrEmpty(searchOfferTypes))
            {
                if (checkedOfferType == "Local Offer")
                {
                    searchOfferTypes = "Local Offer";
                }
                else
                {
                    searchOfferTypes = "Standard Cashback,Cashback,Voucher Code,Sales,Local Offer";
                }
            }

            #endregion Initialize

            //Return DisplayOffer view with all offer Types
            OffersDisplayViewModel model = new OffersDisplayViewModel();
            try
            {
                string[] arrayCategory = null;
                string[] arrayOfferType = null;
                if (!string.IsNullOrEmpty(searchCategories))
                {
                    arrayCategory = searchCategories.Split(",");
                }
                if (!string.IsNullOrEmpty(searchOfferTypes))
                {
                    arrayOfferType = searchOfferTypes.Split(',');
                }

                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }

                if (!string.IsNullOrEmpty(merchant))
                {
                    merchant = merchant.Trim();
                }

                model.MerchantName = merchant;
                model.OfferSort = offerSort;
                model.ParentCategoryId = parentCategoryId;
                model.ParentCategoryName = parentCategoryName;
                model = await MapToDisplayOffer(model, countryCode, merchant,
                    arrayCategory, arrayOfferType, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }

            return View("DisplayOffer", model);
        }

        [HttpGet]
        [ActionName("SearchByText")]
        public async Task<IActionResult> SearchByText(int pageNumber, string country, string mainSearchTerm,
            string searchCategories, string searchOfferTypes, string offerSort, int pageSize)
        
        {
            #region Initialize

            //pageNumber to default
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }
            //pageSize to default
            if (pageSize == 0)
            {
                pageSize = _settings.Value.OfferCount;
            }
            //offerSort to default
            if (string.IsNullOrEmpty(offerSort))
            {
                offerSort = _settings.Value.OfferSort;
            }
            //searchOfferTypes to default
            if (string.IsNullOrEmpty(searchOfferTypes))
            {
                searchOfferTypes = "Standard Cashback,Cashback,Voucher Code,Sales,Local Offer";
            }

            #endregion Initialize

            //Return DisplayOffer by main search text
            OffersDisplayViewModel model = new OffersDisplayViewModel();
            try
            {
                //Split into array for active set and get Ids for filter.
                string[] arrayCategory = null;
                string[] arrayOfferType = null;
                if (!string.IsNullOrEmpty(searchCategories))
                {
                    arrayCategory = searchCategories.Split(",");
                }
                if (!string.IsNullOrEmpty(searchOfferTypes))
                {
                    arrayOfferType = searchOfferTypes.Split(',');
                }

                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }

                if (!string.IsNullOrEmpty(mainSearchTerm))
                {
                    mainSearchTerm = mainSearchTerm.Trim().Replace("%26", "&");
                }

                model.MainSearchTerm = mainSearchTerm;
                model.OfferSort = offerSort;
                model = await MapToSearchDisplayOffer(model, countryCode, mainSearchTerm, arrayCategory,
                    arrayOfferType, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
            return View("SearchOffer", model);
        }

        [HttpPost]
        [ActionName("PagedSearchTerm")]
        public async Task<IActionResult> PagedSearchTerm(string country, int? offerTypeId, string offerTypeName, string keyword, int pageNumber)
        {
            //Return Paged search term
            PagedOffersViewModel viewModel;
            try
            {
                dto.OfferSearchCriteria criteria = new dto.OfferSearchCriteria
                {
                    KeyWord = keyword,
                    CurrentPage = pageNumber,
                    OfferType = offerTypeId
                };
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                viewModel = await OffersHelper.MainSearchAndMapOffer(countryCode, criteria);
                ViewData["OfferType"] = offerTypeName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error retreiving data. Please try again."));
            }
            return PartialView("_offersDetail", viewModel);
        }

        [HttpGet]
        [ActionName("Redirect")]
        public async Task<IActionResult> Redirect(int offerId, string membershipCard, string token = null)
        {
            RedirectViewModel redirect = new RedirectViewModel
            {
                OfferId = offerId,
                MembershipCardNumber = membershipCard,
                Token = token
            };

            dto.Offer offer = await _offersService.Get(offerId, true, false, false, false, false);
            redirect.MerchantName = offer?.Merchant?.Name;
            redirect.MerchantLogoPath = offer?.Merchant?.MerchantImages
                ?.FirstOrDefault(x => x.ImageType == (int)ImageType.Logo && x.ImagePath.Contains("__2"))?.ImagePath;            

            //SSO processing, must be diamond member...
            if (offer?.SSOConfigId != null)
            {
                var user = await _userService.GetUserAsync(HttpContext.User);
                if (user == null)
                //Mobile apps can call Redirect as an external browser link without user credentials (not logged in), so this can be null
                {
                    var noAccess = new OfferAccessDeniedViewModel()
                    {
                        OfferMerchant = offer?.Merchant?.Name ?? "Offer",
                        ShortDescription = offer?.ShortDescription ?? "",
                        IsLoggedIn = false,
                        IsDiamondMemberNeeded = false
                    };
                    return View("AccessDenied", noAccess);
                }

                var customer = _customerAccountService.GetCustomer(user.Id);
                var summary = _customerAccountService.GetAccountSummary(customer?.Id ?? -1);

                if (summary == null || !summary.IsDiamondCustomer)
                {
                    var noAccess = new OfferAccessDeniedViewModel()
                    {
                        OfferMerchant = offer?.Merchant?.Name ?? "Offer",
                        ShortDescription = offer?.ShortDescription ?? "",
                        IsLoggedIn = true,
                        IsDiamondMemberNeeded = true
                    };
                    return View("AccessDenied", noAccess);
                }
                else
                {
                    //Navigate to SSO 
                    var exclusiveUrl = _configuration["SAML2:ExclusiveSiteUrl"];
                    if (exclusiveUrl.ToLower().Contains(HttpContext.Request.Host.Value.ToLower()))
                    {
                        return RedirectToAction("SSORedirect", "SSO", new
                        {
                            userId = user.Id,
                            email = customer?.IdentityUser?.Email,
                            ssoConfigId = offer.SSOConfigId,
                            productCode = offer.ProductCode,
                            merchantName = $"{redirect.MerchantName} Offer"
                        });
                    }
                    var urlWithParameters = $"{exclusiveUrl}saml2?userId={user.Id}&email={customer?.IdentityUser?.Email}&ssoConfigId={offer.SSOConfigId}&productCode={offer.ProductCode}&merchantName={redirect.MerchantName}";
                    return Redirect(urlWithParameters);
                }
            }
            if (offer.LinkUrl.Contains("/LifeStyleHub"))
            {
                return RedirectToAction("Index", "LifeStyleHub");
            }
            var host = Request.Host;
            if (offer.LinkUrl.Contains(host.Value) || offer.LinkUrl.Substring(0, 1) == "/")
            {
                return RedirectToAction("Transfer", "Offers",
                    new { offerId = offerId, membershipCard = membershipCard, token = token, view = true });
            }

            return View("Redirect", redirect);
        }

        //[Authorize(Roles = "User")]
        [HttpGet]
        [ActionName("Transfer")]
        public async Task<IActionResult> Transfer(int offerId, string membershipCard, string token = null, bool view = false) //membershipCard is card number
        {
            try
            {
                Logger.Trace(string.Format("Redirect-Start:  Offer Id = {0}, membershipCardId= {1}, token = {2}", offerId.ToString(), membershipCard, token));
            }
            catch
            { }

            try
            {
                ExclusiveUser user = new ExclusiveUser();
                if (string.IsNullOrEmpty(membershipCard) && string.IsNullOrEmpty(token))
                {
                    try
                    {
                        if (HttpContext == null || HttpContext.User == null || HttpContext.User.Identity == null)
                            Logger.Trace(string.Format("Redirect-GetUser:  HttpContext / HttpContext.User / HttpContext.Indentiy is null"));
                        else
                            Logger.Trace(string.Format("Redirect-GetUser:  HttpContext.user = {0}", HttpContext.User.Identity.Name));
                    }
                    catch
                    { }

                    user = await _userService.GetUserAsync(HttpContext.User);

                    try
                    {
                        if (user == null)
                            Logger.Trace(string.Format("Redirect-After userService.GetUserAsync:  User is null"));
                        else
                            Logger.Trace(string.Format("Redirect-After userService.GetUserAsync:  user.Id={0}", user.Id.ToString()));
                    }
                    catch
                    { }
                }
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        Logger.Trace(string.Format("Redirect-TokenSupplied:  token = {0}", token));
                    }
                    catch
                    { }

                    var tokenDetails = _customerService.GetUserTokenByTokenValue(new Guid(token));
                    //login user with these details
                    user = await _userService.FindByIdAsync(tokenDetails.AspNetUserId);
                    await _signInManager.SignInAsync(user, false);

                    try
                    {
                        if (user == null)
                            Logger.Trace(string.Format("Redirect-AfterSignInFromToken:  User is null"));
                        else
                            Logger.Trace(string.Format("Redirect-AfterSignInFromToken:  user.Id={0}", user.Id.ToString()));
                    }
                    catch
                    { }
                }

                RedirectViewModel redirect = new RedirectViewModel
                {
                    OfferId = offerId
                    //MembershipCardId = !string.IsNullOrEmpty(membershipCardId)
                    //    ? membershipCardId
                    //    : CacheHelper.Get<string>(_cache, Keys.Keys.MembershipCardId, user?.Id)
                };

                if (!string.IsNullOrEmpty(membershipCard))
                {
                    redirect.MembershipCardNumber = membershipCard;
                }
                else if (!string.IsNullOrEmpty(user?.Id) && string.IsNullOrEmpty(CacheHelper.Get<string>(_cache, Keys.Keys.MembershipCardId, user.Id)))
                {
                    try
                    {
                        Logger.Trace(string.Format("Redirect-FindMembershipCard"));
                    }
                    catch
                    { }

                    dto.MembershipCard card = _cardService.GetActiveMembershipCard(user.Id);
                    redirect.MembershipCardId = card.Id;
                    redirect.MembershipCardNumber = card.CardNumber;
                    CacheHelper.Set(_cache, redirect.MembershipCardId, Keys.Keys.MembershipCardId, user.Id);

                    try
                    {
                        Logger.Trace(string.Format("Redirect-FindMembershipCard MembershipCardId = {0}", redirect.MembershipCardId));
                    }
                    catch
                    { }
                }
                else if (string.IsNullOrEmpty(user?.Id))
                {
                    Logger.Error("Could not find the logged-in user in Offer Redirection.");
                    return Json(
                        JsonResponse<string>.ErrorResponse("Could not find the logged in user. Please try again."));
                }
                else
                {
                    try
                    {
                        Logger.Trace(string.Format("Redirect-FindMembershipCard Card was in cache but no one actually chose to use it"));
                    }
                    catch
                    { }
                }

                await GetRedirectionViewModel(redirect);

                if (view)
                    return View("Redirect", redirect);
                return Json(JsonResponse<string>.SuccessResponse(redirect.DeepLinkUrl)); //View("Redirect", redirect);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while redirecting.")); //View("Error");
            }
        }

        [HttpGet]
        [ActionName("GetPagedBranchContact")]
        public async Task<IActionResult> GetPagedBranchContact(int merchantId, int currentPage)
        {
            PagedBranchContactViewModel pagebranch = new PagedBranchContactViewModel();
            await OffersHelper.MaptoGetPagedBranchContact(currentPage, merchantId, pagebranch);
            return PartialView("_merchantContactDetails", pagebranch);
        }

        [HttpGet]
        [ActionName("ShortSearch")]
        public async Task<IActionResult> ShortSearch(string country, string searchTerm)
        {
            try
            {
                //Check searchTerm not null and length > 0
                if (!string.IsNullOrEmpty(searchTerm) && searchTerm.Length >= 3)
                {
                    var data = await _offersService.GetOfferByKeyword(searchTerm);
                    /*var data = await _offersService.PagedMainSearch(new dto.OfferSearchCriteria
                    {
                        KeyWord = searchTerm,
                        CountryCode = country,
                        PageSize = 1,
                        CurrentPage = 1
                    });
                    var result = data.Results.FirstOrDefault();*/
                    return Json(JsonResponse<List<dto.Public.OfferSummary>>.SuccessResponse(data));
                }
                return Json(JsonResponse<dto.Public.OfferSummary>.SuccessResponse(null));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Json(JsonResponse<string>.ErrorResponse("Could not find any data"));
        }

        [HttpGet]
        [ActionName("DiamondBenefits")]
        public async Task<IActionResult> DiamondBenefits(string country)
        {
            try
            {
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }

                //Offer Types
                List<dto.OfferType> types = _cache.Get<List<dto.OfferType>>(Keys.Keys.OfferTypes);
                if (types == null || types.Count == 0)
                {
                    types = await _offersService.GetAllOfferType();
                    _cache.Set(types, Keys.Keys.OfferTypes);
                }

                if (types?.FirstOrDefault(x => x.Description == Data.Constants.Keys.DiamondOfferType && x.IsActive)?.Id == null)
                    throw new Exception("Can't find OfferTypes of Diamond Offer in Exclusive.OfferType table.");

                var diamondOffers = await _offersService.GetListofMerchantOffersByTypeAsync(
                    null,
                    types.FirstOrDefault(x => x.Description == Data.Constants.Keys.DiamondOfferType && x.IsActive)?.Id,
                    countryCode);

                return View("DiamondBenefits", diamondOffers);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("OfferRedemption")]
        public async Task<IActionResult> OfferRedemption(int offerId)
        {
            OfferRedemViewModel model = new OfferRedemViewModel();
            try
            {
                ExclusiveUser user = await _userService.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    return View("Error", new ErrorViewModel { RequestId = "User not found." });
                }

                var customer = await _customerService.Get(user.Id);
                if (customer?.ContactDetail != null)
                {
                    model = new OfferRedemViewModel
                    {
                        Id = customer.ContactDetail.Id,
                        Address1 = customer.ContactDetail.Address1,
                        Address2 = customer.ContactDetail.Address2,
                        Address3 = customer.ContactDetail.Address3,
                        Town = customer.ContactDetail.Town,
                        County = customer.ContactDetail.District,
                        Country = customer.ContactDetail.CountryCode,
                        Postcode = customer.ContactDetail.PostCode
                    };
                }

                //Check if redemption request exists
                var membershipCard = _cardService.GetActiveMembershipCard(user.Id);
                var redeemRequest = _offersService.GetOfferRedemption(membershipCard.Id, offerId);
                if (redeemRequest != null)
                    model.Redeemed = true;

                model.UserId = user?.Id;
                model.OfferId = offerId;
                model.Countries = GetCountryList();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error", new ErrorViewModel { RequestId = e.ToString() });
            }
            return View(model);
        }

        /// <summary>
        /// Redeems an offer, e.g claim a love2Shop card
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RedeemOffer")]
        public async Task<IActionResult> RedeemOffer(OfferRedemViewModel model)
        {
            try
            {
                //TODO:  Refactor Redeem Offer so that it calls an Offer serivce to do the redemption.
                //       The offer service should call the customer manager to fetch/update cust details, then call the
                //       offermanager to claim the offer.

                // It is completely unacceptable to have the offer controller updating customer records like this
                // It is an OFFER controller, it should only handle display logic for offers, not updating random data entities
                // This is a maintenance nightmare.  If we needed to upgrade the customer, who would think to look for code that
                // saved customer records in an OFFER controller, in a method named RedeemOffer.
                if (model != null)
                {
                    var currentDate = DateTime.UtcNow;
                    string month = currentDate.Month.ToString().Length == 1
                        ? $"0{currentDate.Month.ToString()}"
                        : currentDate.Month.ToString();
                    string day = currentDate.Day.ToString().Length == 1 ? $"0{currentDate.Day.ToString()}" : currentDate.Day.ToString();
                    var customerRef = string.Empty;
                    var result = false;
                    if (model.Id > 0)
                    {
                        var contact = await _contactDetailService.Get(model.Id);
                        contact.Address1 = model.Address1;
                        contact.Address2 = model.Address2;
                        contact.Address3 = model.Address3;
                        contact.District = model.County;
                        contact.Town = model.Town;
                        contact.PostCode = model.Postcode;
                        contact.CountryCode = model.Country;

                        await _contactDetailService.Update(contact);
                        result = true;
                    }
                    else
                    {
                        var customer = await _customerService.Get(model.UserId);
                        customer.ContactDetail = new dto.ContactDetail
                        {
                            Address1 = model.Address1,
                            Address2 = model.Address2,
                            Address3 = model.Address3,
                            Town = model.Town,
                            PostCode = model.Postcode,
                            District = model.County,
                            CountryCode = model.Country
                        };
                        await _customerService.Update(customer);

                        customerRef =
                            $"{currentDate.Year}{month}{day}{currentDate.Hour}{currentDate.Minute}{currentDate.Second}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}";

                        if (customer.ContactDetailId.HasValue)
                            result = true;
                    }

                    if (result)
                    {
                        if (string.IsNullOrEmpty(customerRef))
                        {
                            var customer = await _customerService.Get(model.UserId);
                            customerRef =
                                $"{currentDate.Year}{month}{day}{currentDate.Hour}{currentDate.Minute}{currentDate.Second}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}";
                        }

                        //Create Offer Redemption entry
                        dto.OfferRedemption redeem = new dto.OfferRedemption
                        {
                            MembershipCardId = model.MembershipCardId,
                            OfferId = model.OfferId,
                            CreatedDate = DateTime.UtcNow,
                            State = (int)OfferRedemptionStatus.Requested, //status.FirstOrDefault(x => x.Name == Data.Constants.Status.Requested).Id,
                            CustomerRef = customerRef
                        };

                        _offersService.AddRedemption(redeem);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while Redeeming the offer. Please try again."));
            }

            return Json(JsonResponse<bool>.SuccessResponse(true));
        }

        [HttpGet]
        [ActionName("RedeemSuccess")]
        public IActionResult RedeemSuccess()
        {
            return View();
        }

        [HttpGet]
        [ActionName("RedeemFail")]
        public IActionResult RedeemFail()
        {
            return View();
        }

        #region Private Members

        private async Task<OffersDisplayViewModel> MapToDisplayOffer(OffersDisplayViewModel offersDisplayView, string countryCode,
            string merchant, string[] searchCategory, string[] searchOfferTypes,
            int currentPage, int pageSize)
        {
            List<dto.Category> subCategories = _categoryService.GetByParentId(offersDisplayView.ParentCategoryId);
            foreach (var category in subCategories)
            {
                category.IsActive = searchCategory != null && searchCategory.Contains(category.UrlSlug);
            }

            if (searchCategory != null)
            {
                offersDisplayView.CategoryIds = subCategories.Where(x => x.IsActive && searchCategory.Contains(x.UrlSlug))
                    .Select(x => x.Id).ToList();
            }
            else
            {
                offersDisplayView.CategoryIds = subCategories.Select(x => x.Id).ToList();
            }

            // Add in the root category to search offers against that too.  Appears to have been missed off. ABW.
            if (offersDisplayView?.CategoryIds != null)
            {
                offersDisplayView.CategoryIds.Add(offersDisplayView.ParentCategoryId);
            }

            List<dto.OfferType> offerTypes = await _offersService.GetAllOfferType();
            foreach (dto.OfferType offerType in offerTypes)
            {
                offerType.IsActive = searchOfferTypes != null && searchOfferTypes.Contains(offerType.Description);
            }

            if (searchOfferTypes != null)
            {
                offersDisplayView.OfferTypeIds = offerTypes
                    .Where(x => x.IsActive && searchOfferTypes.Contains(x.Description)).Select(x => x.Id).ToList();
            }

            if (!string.IsNullOrEmpty(merchant))
            {
                offersDisplayView.MerchantName = merchant;
            }

            //List<dto.Merchant> merchants = await _merchantService.GetAll();
            //List<SelectListItem> ListofMerchantName = merchants.Select(merchantNew => new SelectListItem()
            //{
            //    Text = merchantNew.Name,
            //    Value = merchantNew.Id.ToString()
            //}).ToList();
            //offersDisplayView.ListOfMerchantName = ListofMerchantName;

            //List<dto.Tag> tags = await _tagService.GetAll();

            dto.OfferSearchCriteria criteria = new dto.OfferSearchCriteria
            {
                KeyWord = offersDisplayView.Keywords,
                Categories = offersDisplayView.CategoryIds,
                MerchantName = offersDisplayView.MerchantName,
                OfferTypes = offersDisplayView.OfferTypeIds,
                CountryCode = countryCode,
                PageSize = pageSize,
                CurrentPage = currentPage,
            };
            switch (offersDisplayView.OfferSort)
            {
                case "popularity":
                    criteria.SortOrder = 1;
                    break;

                case "merchant":
                    criteria.SortOrder = 2;
                    break;

                case "expiring":
                    criteria.SortOrder = 3;
                    break;

                default:
                    criteria.SortOrder = 1;
                    break;
            }

            offersDisplayView.Categories = subCategories;
            offersDisplayView.OfferTypes = offerTypes;
            //For slider use offer hub as list name and take only 5 records.
            offersDisplayView.OfferHubViewModels =
                MapToOfferHubViewModels(await _offersService.GetOfferListDataModels(countryCode,
                    offersDisplayView.CategoryIds, 5, Data.Constants.Keys.OfferHubList));
            offersDisplayView.PagedOffersView = MapToPagedOffersViewModel(await _offersService.PagedSearch(criteria));

            return offersDisplayView;
        }

        private List<OfferHubViewModel> MapToOfferHubViewModels(List<dto.OfferListDataModel> model)
        {
            List<OfferHubViewModel> offerHubViewModels = new List<OfferHubViewModel>();
            if (model == null)
                return offerHubViewModels;

            model.ForEach(x => offerHubViewModels.Add(new OfferHubViewModel
            {
                MerchantId = x.MerchantId,
                MerchantName = x.MerchantName,
                OfferId = x.OfferId,
                OfferText = x.OfferText,
                OfferShortDescription = x.OfferShortDescription,
                OfferLongDescription = x.OfferLongDescription,
                Logo = x.Logo,
                DisabledLogo = x.DisabledLogo,
                FeatureImage = x.FeatureImage,
                LargeImage = x.LargeImage
            }));
            return offerHubViewModels;
        }

        private async Task<OffersDisplayViewModel> MapToSearchDisplayOffer(OffersDisplayViewModel offersDisplayView,
            string countryCode, string searchTerm, string[] searchCategory,
            string[] searchOfferTypes, int currentPage, int pageSize)
        {
            List<dto.Category> categories = _categoryService.GetAllparentcategories().Result;
            foreach (dto.Category category in categories)
            {
                category.IsActive = searchCategory != null && searchCategory.Contains(category.UrlSlug);
            }

            if (searchCategory != null)
            {
                offersDisplayView.CategoryIds = categories.Where(x => x.IsActive && searchCategory.Contains(x.UrlSlug))
                    .Select(x => x.Id).ToList();
                offersDisplayView.CategoryIds = GetCategoryIdsWithSubCategory(offersDisplayView.CategoryIds);
            }
            List<dto.OfferType> offerTypes = await _offersService.GetAllOfferType();
            foreach (dto.OfferType offerType in offerTypes)
            {
                offerType.IsActive = searchOfferTypes != null && searchOfferTypes.Contains(offerType.Description);
            }

            if (searchOfferTypes != null)
            {
                offersDisplayView.OfferTypeIds = offerTypes
                    .Where(x => x.IsActive && searchOfferTypes.Contains(x.Description)).Select(x => x.Id).ToList();
            }

            offersDisplayView.Categories = categories;
            offersDisplayView.OfferTypes = offerTypes;

            // This was pulling a full list of merchants down from the server!!!
            // So I've moved populating ListOfMerchantName till after the data search and
            // just populated it with the merchants that are related to the offers...
            //List<dto.Merchant> merchants = await _merchantService.GetAll();
            //List<SelectListItem> listOfMerchantName = merchants.Select(merchantNew => new SelectListItem()
            //{
            //    Text = merchantNew.Name,
            //    Value = merchantNew.Id.ToString()
            //}).ToList();
            //offersDisplayView.ListOfMerchantName = listOfMerchantName;

            dto.OfferSearchCriteria offerSearchCriteria = new dto.OfferSearchCriteria
            {
                KeyWord = searchTerm,
                CountryCode = countryCode,
                PageSize = pageSize,
                CurrentPage = currentPage,
                Categories = offersDisplayView.CategoryIds,
                OfferTypes = offersDisplayView.OfferTypeIds
            };
            switch (offersDisplayView.OfferSort)
            {
                case "popularity":
                    offerSearchCriteria.SortOrder = 1;
                    break;

                case "merchant":
                    offerSearchCriteria.SortOrder = 2;
                    break;

                case "expiring":
                    offerSearchCriteria.SortOrder = 3;
                    break;

                default:
                    offerSearchCriteria.SortOrder = 1;
                    break;
            }

            //Pick up the search results
            var results = await _offersService.PagedMainSearch(offerSearchCriteria);
            //Populate the list of merchants with just a distinct that's related to the search results
            List<SelectListItem> listOfMerchantName = results.Results.Select(merchantNew => new SelectListItem()
            {
                Text = merchantNew.MerchantName,
                Value = merchantNew.MerchantId.ToString()
            }).GroupBy(g => g.Text).Select(grp => grp.First()).ToList();
            offersDisplayView.ListOfMerchantName = listOfMerchantName;

            offersDisplayView.PagedOffersView =
                MapToPagedOffersViewModel(results);

            return offersDisplayView;
        }

        private PagedOffersViewModel MapToPagedOffersViewModel(dto.PagedResult<dto.Public.OfferSummary> result)
        {
            return new PagedOffersViewModel
            {
                OfferSummaries = result.Results.OrderBy(x => x.Rank).ToList(),
                CurrentPageNumber = result.CurrentPage,
                PagingView = new PagingViewModel
                {
                    CurrentPage = result.CurrentPage,
                    PageCount = result.PageCount,
                    PageSize = result.PageSize,
                    RowCount = result.RowCount
                }
            };
        }

        //search text offer list - only parent categoryIds available
        private List<int> GetCategoryIdsWithSubCategory(List<int> categoryIds)
        {
            List<int> response = new List<int>();
            foreach (var id in categoryIds)
            {
                response.Add(id);
                List<dto.Category> categories = _categoryService.GetByParentId(id);
                categories.ForEach(x => response.Add(x.Id));
            }

            return response;
        }

        private async Task GetMerchantOffersAndMapToPagedData(int merchantId, int? offerId, string countryCode, int currentPage, MerchantOffersViewModel model)
        {
            try
            {
                model.Merchant = _merchantService.Get(merchantId, false, false, true, true);

                //Get Logo and featureImage
                if (model.Merchant?.MerchantImages?.Count > 0)
                {
                    model.Logo = model.Merchant.MerchantImages
                        .FirstOrDefault(x => x.ImageType == (int)ImageType.Logo && x.ImagePath.Contains(CommonHelper.MEDIUM_SUFFIX))
                        ?.ImagePath;
                    //#528 Try new size first
                    model.FeatureImage = model.Merchant.MerchantImages.FirstOrDefault(x =>
                        x.ImageType == (int)ImageType.FeatureImage && x.ImagePath.Contains(CommonHelper.FEATURE_SUFFIX))?.ImagePath;
                    if (string.IsNullOrWhiteSpace(model.FeatureImage))
                        model.FeatureImage = model.Merchant.MerchantImages.FirstOrDefault(x =>
                            x.ImageType == (int)ImageType.FeatureImage && x.ImagePath.Contains(CommonHelper.LARGE_SUFFIX))?.ImagePath;
                }

                //Get offer types
                List<dto.OfferType> types = _cache.Get<List<dto.OfferType>>(Keys.Keys.OfferTypes);
                if (types == null || types.Count == 0)
                {
                    types = await _offersService.GetAllOfferType();
                    _cache.Set(types, Keys.Keys.OfferTypes);
                }

                //search criteria
                var criteria = new dto.OfferSearchCriteria
                {
                    MerchantId = merchantId,
                    CountryCode = countryCode,
                    CurrentPage = 1,
                    PageSize = _settings.Value.OffersScreenCount,
                };

                criteria.OfferTypes.AddRange(types.Where(x => x.Description != Data.Constants.Keys.DiamondOfferType).Select(item => item.Id));

                //Get top 250 offers for the merchant
                var offers = await _offersService.PagedSearch(criteria);

                model.Cashback = offers.Results.OrderBy(x => x.Rank).ToList();

                // Get local offers
                model.LocalOffers = offers.Results.Where(x => x.OfferTypeDescription == Keys.Keys.LocalOffer).ToList();
                if (model.LocalOffers == null || !model.LocalOffers.Any())
                {
                    model.LocalOffers = await _offersService.GetListofMerchantOffersByTypeAsync(merchantId, types.FirstOrDefault(x => x.Description == Keys.Keys.LocalOffer)?.Id, countryCode);
                }

                //Get Standard offer  (Offertype = 5: StandardCashback)
                var standardCashback = model.Cashback.FirstOrDefault(x => x.OfferTypeDescription == Keys.Keys.StandardType);
                if (standardCashback == null || standardCashback.MerchantId == 0)
                {
                    // If not found in general paged search, look for standard cashback offer explicitly
                    var standardOffers = await _offersService.GetListofMerchantOffersByTypeAsync(
                        merchantId, types.FirstOrDefault(x => x.Description == Keys.Keys.StandardType)?.Id, countryCode);
                    standardCashback = standardOffers.FirstOrDefault();
                }

                // First check if there is a local offer to use as the featured offer
                if (model.LocalOffers != null && model.LocalOffers.Any())
                    model.FeaturedOffer = model.LocalOffers.FirstOrDefault();
                else  // If not, pick the first standard cashback offer
                    model.FeaturedOffer = standardCashback;

                // If a featured Offer was not the local one  found , then remove it from the main list of offers
                // Local offers are to be included in the main offer list as well as being in the featured offer section
                if (model.FeaturedOffer != null && model.FeaturedOffer.OfferTypeDescription != Keys.Keys.LocalOffer)
                {
                    //Delete it from model.Cashback
                    var index = model.Cashback.FindIndex(x => x.OfferId == model.FeaturedOffer.OfferId);
                    if (index > -1)
                    {
                        model.Cashback.RemoveAt(index);
                    }
                }

                //Get related offers
                dto.Offer selectedOffer = null;
                if (offerId.HasValue)
                    selectedOffer = await _offersService.Get(offerId.Value, false, false, false, false, true);

                var relatedOfferCriteria = new dto.OfferSearchCriteria
                {
                    //OfferType = selectedOffer?.OfferTypeId,
                    CountryCode = countryCode,
                    CurrentPage = 1,
                    PageSize = _settings.Value.RelatedOfferPageSize,
                    ExcludedMerchantId = merchantId
                };

                if (selectedOffer?.OfferCategories?.Count > 0)
                {
                    await Task.WhenAll(selectedOffer.OfferCategories.Select(async item =>
                    {
                        relatedOfferCriteria.Categories.Add(item.CategoryId);
                        await Task.CompletedTask;
                    }));
                }

                var relatedOffers = await _offersService.PagedSearch(relatedOfferCriteria);
                model.RelatedOffers = relatedOffers.Results.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// using the <paramref name="url"/>, this inserts <paramref name="pubRef"/> or amends the existing variable to <paramref name="pubRefValue"/> as part of the url path rather than as a parameter
        /// </summary>
        /// <param name="url">originating click url</param>
        /// <param name="pubRef">pubref variable name (refere)</param>
        /// <param name="pubRefValue">value for the pubref variable (model.MembershipCardReference)</param>
        /// <returns>url with the pubref variable details included in the url path</returns>
        private string IncludeInLink(string url, string pubRef, string pubRefValue)
        {
            //With Partnerise:
            //https://prf.hn/click/camref:XXXXX/pubref:MYSUBIDHERE/destination:https%3A%2F%2Fwww.coolblue.nl%2F
            //You have to use pubref in the link, not as a parameter.

            Regex rxFindDestination = new Regex("/destination:", RegexOptions.IgnoreCase);
            Regex rxFindPubRef = new Regex($@"(?<={pubRef}:).*?(?=[/$])", RegexOptions.IgnoreCase);

            string result = url;

            #region parameter validation

            if (string.IsNullOrWhiteSpace(url))
            {
                //audit error
                Logger.Trace("No url provided to IncludeInLink");
            }
            else if (string.IsNullOrWhiteSpace(pubRef))
            {
                //audit error
                Logger.Trace($"No pubRef variable provided to IncludeInLink ({url})");
            }
            else if (string.IsNullOrWhiteSpace(pubRefValue))
            {
                //audit error
                Logger.Trace($"No pubRef value provided to IncludeInLink ({url})");
            }

            #endregion parameter validation

            else
            {
                try
                {
                    Match pub = rxFindPubRef.Match(url);
                    Match dest = rxFindDestination.Match(url);
                    if (pub.Success)
                    {
                        //"pubref:" already exists, just replace text
                        result = url.Substring(0, pub.Index) + pubRefValue + url.Substring(pub.Index + pub.Length);
                    }
                    else if (dest.Success)
                    {
                        //"/destination:" found so insert before
                        result = url.Substring(0, dest.Index) + $"/{pubRef}:{pubRefValue}" + url.Substring(dest.Index);
                    }
                    else
                    {
                        //add before any parameters (?)
                        if (url.Contains('?'))
                        {
                            result = url.Substring(0, url.IndexOf('?') - 1) + $"/{pubRef}:{pubRefValue}" + url.Substring(url.IndexOf('?'));
                        }
                        else //or to end of line
                        {
                            result = $"{url}/{pubRef}:{pubRefValue}";
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return result;
        }

        private async Task GetRedirectionViewModel(RedirectViewModel model)
        {
            dto.Offer offer = await _offersService.Get(model.OfferId, true, true, false, true, false);
            if (offer != null)
            {
                model.MerchantId = offer.MerchantId;
                model.MerchantName = offer.Merchant?.Name;
                model.DeepLinkUrl = offer.LinkUrl;
                model.MerchantLogoPath = offer.Merchant?.MerchantImages?.FirstOrDefault()?.ImagePath;

                //Parse deeplinkUrl
                if (!string.IsNullOrEmpty(offer.LinkUrl) && !offer.LinkUrl.Contains("OfferRedemption"))
                {
                    Uri unparsed;
                    try
                    {
                        unparsed = new Uri(offer.LinkUrl);
                    }
                    catch
                    {
                        offer.LinkUrl = $"http://{offer.LinkUrl}";
                        model.DeepLinkUrl = offer.LinkUrl;
                        unparsed = new Uri(offer.LinkUrl);
                    }
                    NameValueCollection queryParams = HttpUtility.ParseQueryString(unparsed.Query);
                    queryParams.Remove("clickref");
                    UriBuilder uriBuilder = new UriBuilder();
                    uriBuilder.Query = queryParams.ToString();
                    uriBuilder.Scheme = "";
                    uriBuilder.Host = "";
                    uriBuilder.Path = model.DeepLinkUrl.Split('?')[0];
                    model.DeepLinkUrl = uriBuilder.ToString();
                }

                if (model.DeepLinkUrl.Contains("OfferRedemption"))
                {
                    model.DeepLinkUrl = model.DeepLinkUrl.Replace("{{offer}}", model.OfferId.ToString());
                }

                model.RedirectAllowed = true;
                //check if the offer type is cashback or standard cashback and add clicktracking to it. ignore for other offer types
                if (offer.AffiliateId.HasValue)
                {
                    model.AffiliateId = offer.AffiliateId.Value;

                    if (!string.IsNullOrEmpty(model.MembershipCardNumber))
                    {
                        try
                        {
                            Logger.Trace(string.Format("GetRedirectionViewModel model.MembershipCard = {0}", model.MembershipCardId));
                        }
                        catch
                        { }

                        dto.MembershipCard cardDetails = await _cardService.Get(model.MembershipCardId);
                        model.MembershipCardReference =
                            cardDetails?.MembershipCardAffiliateReferences.FirstOrDefault(x => x.AffiliateId == model.AffiliateId)?.CardReference;
                        if (cardDetails != null && cardDetails.IsActive)
                        {
                            model.RedirectAllowed = true;
                        }
                        else
                        {
                            model.RedirectAllowed = false;
                        }
                        //Check if ref is null && create reference
                        if (string.IsNullOrEmpty(model.MembershipCardReference))
                        {
                            try
                            {
                                Logger.Trace(string.Format("GetRedirectionViewModel-CreateRef: affiliateid = {0}, MembershipCardId = {1}", model.AffiliateId.ToString(), cardDetails.Id.ToString()));
                            }
                            catch
                            { }

                            dto.MembershipCardAffiliateReference affiliateRef = new dto.MembershipCardAffiliateReference
                            {
                                AffiliateId = model.AffiliateId,
                                MembershipCardId = cardDetails.Id,
                                CardReference = GenerateReference(30)
                            };

                            await _membershipCardAffiliateReferenceService.Add(affiliateRef);
                            model.MembershipCardReference = affiliateRef.CardReference;

                            try
                            {
                                Logger.Trace(string.Format("GetRedirectionViewModel-CreateRef: CardReference = {0}", affiliateRef.CardReference));
                            }
                            catch
                            { }
                        }

                        if (!string.IsNullOrEmpty(model.DeepLinkUrl) && cardDetails != null)
                        {
                            //dto.AffiliateMappingRule rules = await serve.Instance.AffiliateMappingRuleService.GetByDesc("Affiliate Membership card alias", model.AffiliateId);
                            dto.AffiliateMappingRule rules = await _affiliateMappingRuleService.GetByDesc(Keys.Keys.AffiliateMembershipcardalias, model.AffiliateId);
                            string refere = rules?.AffiliateMappings?.FirstOrDefault()?.AffilateValue;

                            if (refere == null && (offer.OfferType?.Description == Keys.Keys.CashbackType ||
                                                   offer.OfferType?.Description == Keys.Keys.StandardType))
                            {
                                dto.Email email = new dto.Email
                                {
                                    Subject = $"Redirect URL for {offer?.Affiliate?.Name}, affiliate value missing of Affiliate Mapping Rule Id: {rules.Id}",
                                    BodyHtml = $"Dear Admin,<br/><p><Could not find {Keys.Keys.AffiliateMembershipcardalias} of {offer?.Affiliate?.Name} for Affiliate Mapping Rule Id : {rules.Id}.></p>",
                                    BodyPlainText = $"Dear Admin, Could not find {Keys.Keys.AffiliateMembershipcardalias} of {offer?.Affiliate?.Name} for Affiliate Mapping Rule Id : {rules.Id}.></p>",
                                    EmailTo = new List<string> { _settings.Value.AdminEmail }
                                };
                                var res = await _emailManager.SendEmailAsync(email);
                                if (res != true.ToString())
                                {
                                    Logger.Error(
                                        $"Redirect URL for {offer?.Affiliate?.Name}, affiliate value missing of Affiliate Mapping Rule Id: {rules.Id}");
                                }
                            }
                            //Parse deeplinkUrl
                            if (!string.IsNullOrEmpty(offer.LinkUrl) && !offer.LinkUrl.Contains("OfferRedemption"))
                            {
                                Uri unparsed = new Uri(offer.LinkUrl);
                                NameValueCollection queryParams = HttpUtility.ParseQueryString(unparsed.Query);
                                queryParams.Remove("clickref");

                                if (string.Equals(offer?.Affiliate?.Name, Data.Constants.AffiliateKeys.Partnerize,
                                    StringComparison.OrdinalIgnoreCase) && refere != null)
                                {
                                    //Need to embed parameter into url for Partnerize
                                    try
                                    {
                                        Logger.Trace(string.Format("GetRedirectionViewModel-PartnerizeQueryParams: model.MembershipCardReference = {0}", model.MembershipCardReference));
                                    }
                                    catch
                                    { }
                                    UriBuilder uriBuilder = new UriBuilder();
                                    uriBuilder.Query = queryParams.ToString();
                                    uriBuilder.Scheme = "";
                                    uriBuilder.Host = "";
                                    uriBuilder.Path = IncludeInLink(model.DeepLinkUrl.Split('?')[0], refere, model.MembershipCardReference);
                                    model.DeepLinkUrl = uriBuilder.ToString();
                                }
                                else
                                {
                                    if (string.Equals(offer?.Affiliate?.Name, Data.Constants.AffiliateKeys.Webgains,
                                    StringComparison.OrdinalIgnoreCase) && refere != null)
                                    {
                                        try
                                        {
                                            Logger.Trace(string.Format("GetRedirectionViewModel-WebGainsQueryParams: model.MembershipCardReference = {0}", model.MembershipCardReference));
                                        }
                                        catch
                                        { }

                                        var wgTar = queryParams["wgtarget"];

                                        if (!string.IsNullOrEmpty(wgTar))
                                        {
                                            queryParams.Remove("wgtarget");
                                            queryParams.Add(refere, model.MembershipCardReference);
                                            queryParams.Add("wgtarget", wgTar);
                                        }
                                        else
                                        {
                                            queryParams.Add(refere, model.MembershipCardReference);
                                        }
                                    }
                                    else if (refere != null)
                                    {
                                        try
                                        {
                                            Logger.Trace(string.Format("GetRedirectionViewModel-QueryParams: model.MembershipCardReference = {0}", model.MembershipCardReference));
                                        }
                                        catch
                                        { }

                                        //If there's an existing subId and its empty use that
                                        var paramContent = queryParams.GetValues(refere);
                                        if (paramContent != null && string.IsNullOrWhiteSpace(paramContent[0]))
                                            queryParams.Set(refere, model.MembershipCardReference);
                                        else
                                            queryParams.Add(refere, model.MembershipCardReference);
                                    }

                                    UriBuilder uriBuilder = new UriBuilder();
                                    uriBuilder.Query = queryParams.ToString();
                                    uriBuilder.Scheme = "";
                                    uriBuilder.Host = "";
                                    uriBuilder.Path = model.DeepLinkUrl.Split('?')[0];
                                    model.DeepLinkUrl = uriBuilder.ToString();
                                }
                            }

                            dto.ClickTracking trans = new dto.ClickTracking
                            {
                                AffiliateId = model.AffiliateId,
                                MembershipCardId = cardDetails.Id,
                                DeeplinkURL = model.DeepLinkUrl,
                                OfferId = model.OfferId,
                                DateTime = DateTime.UtcNow
                            };

                            await _clickTrackingService.Add(trans);
                            model.DeepLinkUrl = trans.DeeplinkURL;

                            try
                            {
                                Logger.Trace(string.Format("GetRedirectionViewModel-Deeplink: URL = {0}", model.DeepLinkUrl));
                            }
                            catch
                            { }
                        }
                    }
                }
            }
        }

        private string GenerateReference(int length)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(length);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> country = new List<SelectListItem>
            {
                new SelectListItem {Text = "Czech Republic", Value = "CZ"},
                new SelectListItem {Text = "United Kingdom", Value = "GB"},
                new SelectListItem {Text = "Poland", Value = "PL"},
                new SelectListItem {Text = "Seychelles", Value = "SC"},
                new SelectListItem {Text = "Slovakia", Value = "SK"}
            };

            return country;
        }

        //Search for offer summaries based on criteria
        private async Task<PagedOffersViewModel> SearchAndMapOffer(string countryCode,
            dto.OfferSearchCriteria criteria)
        {
            PagedOffersViewModel model = await MapToOffersDetailView(countryCode,
                criteria.CurrentPage, criteria);
            //List<dto.Public.OfferSummary> offerSummary = new List<dto.Public.OfferSummary>();
            //offerSummary = offersService.Search(offerSearchCriteria);
            return model;
        }

        //offersDetail view for pagination
        private async Task<PagedOffersViewModel> MapToOffersDetailView(string countryCode,
            int currentPage, dto.OfferSearchCriteria criteria)
        {
            PagedOffersViewModel viewModel = MapToPagedOffersViewModel(await _offersService.PagedSearch(new dto.OfferSearchCriteria
            {
                MerchantId = criteria.MerchantId,
                AffiliateId = criteria.AffiliateId,
                Categories = criteria.Categories,
                OfferStatus = criteria.OfferStatus,
                KeyWord = criteria.KeyWord,
                ValidFrom = criteria.ValidFrom,
                ValidTo = criteria.ValidTo,
                CountryCode = countryCode,
                OfferType = criteria.OfferType,
                OfferTypes = criteria.OfferTypes,
                MerchantName = criteria.MerchantName,
                PageSize = _settings.Value.PageSize,
                CurrentPage = currentPage,
                SortOrder = criteria.SortOrder
            }));
            return viewModel;
        }

        #endregion Private Members
    }
}