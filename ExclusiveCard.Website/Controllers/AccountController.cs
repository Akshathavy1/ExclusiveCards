using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Helpers;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using dto = ExclusiveCard.Services.Models.DTOs;
using ITagService = ExclusiveCard.Services.Interfaces.Public.ITagService;

namespace ExclusiveCard.Website.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        #region Private Members

        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly SignInManager<ExclusiveUser> _signInManager;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly IUserService _userService;
        private readonly Services.Interfaces.Public.ICustomerService _customerService;
        private readonly Services.Interfaces.Public.IMembershipCardService _cardService;
        private readonly IStatusServices _statusServices;
        private readonly IStagingCustomerRegistrationService _stagingCustomerRegistrationService;
        private readonly Managers.IEmailManager _emailManager;
        private readonly IOLD_MembershipRegistrationCodeService _registrationCodeService;
        private readonly Services.Interfaces.Admin.IPartnerService _partnerService;
        private readonly IConfiguration _settings;
        private readonly IOLD_MembershipPlanService _membershipPlanService;
        private readonly ISecurityQuestionService _securityQuestionService;
        private readonly Services.Interfaces.Public.IMembershipCardService _membershipCardService;
        private readonly ITagService _tagService;
        private readonly IWhiteLabelService _whiteLabelService;



        #endregion

        #region Constructor

        public AccountController(
            IMemoryCache cache,
            IMapper mapper,
            ICustomerAccountService customerAccountService,
            SignInManager<ExclusiveUser> signInManager,
            IUserService userService,
            Services.Interfaces.Public.ICustomerService customerService,
            Services.Interfaces.Public.IMembershipCardService cardService,
            IStatusServices statusServices,
            IStagingCustomerRegistrationService stagingCustomerRegistrationService,
            Managers.IEmailManager emailService,
            IOLD_MembershipRegistrationCodeService membershipRegistrationCodeService,
            Services.Interfaces.Admin.IPartnerService partnerService,
            IConfiguration settings,
            IOLD_MembershipPlanService membershipPlanService,
            ISecurityQuestionService securityQuestionService,
            Services.Interfaces.Public.IMembershipCardService membershipCardService, ITagService tagService, IWhiteLabelService whiteLabelService)
        {
            _cache = cache;
            _mapper = mapper;
            _signInManager = signInManager;
            _customerAccountService = customerAccountService;
            _userService = userService;
            _customerService = customerService;
            _cardService = cardService;
            _statusServices = statusServices;
            _stagingCustomerRegistrationService = stagingCustomerRegistrationService;
            _emailManager = emailService;
            _registrationCodeService = membershipRegistrationCodeService;
            _partnerService = partnerService;
            _settings = settings;
            _membershipPlanService = membershipPlanService;
            _securityQuestionService = securityQuestionService;
            _membershipCardService = membershipCardService;
            _tagService = tagService;
            _whiteLabelService = whiteLabelService;
        }

        #endregion

        [HttpGet]
        [ActionName("Index")]
        public IActionResult Index(string country, int? customerId)
        {
            //Method to Redirect to account signup page
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            if (customerId.HasValue)
            {
                ViewBag.Customer = customerId.Value;
            }
            else
            {
                ViewBag.Customer = null;
            }
            return View("Index");
        }

        [HttpGet]
        [ActionName("CreateAccount")]
        public async Task<IActionResult> CreateAccount()
        {
            CustomerViewModel model=new CustomerViewModel();
            
            //Method to Redirect to Create Account Page
            try
            {
                UserRegistrationTempData userRegistrationTempData =
                    HttpContext.Session.GetObjectFromJson<UserRegistrationTempData>(Data.Constants.Keys
                        .UserRegistrationTempData);
                
                string token = string.Empty;
                string country = string.Empty;
                if (userRegistrationTempData != null)
                {
                    int membershipPlanId = userRegistrationTempData.Token.MembershipPlanId;
                    token = userRegistrationTempData.Token.Token.ToString();
                    int? siteClanId = userRegistrationTempData.SiteClanId;
                    if (_signInManager.IsSignedIn(User))
                        return RedirectToAction("Index", "Home");
                    if (string.IsNullOrEmpty(country))
                    {
                        country = "GB";
                    }
                    if (!string.IsNullOrEmpty(country))
                    {
                        ViewData["Country"] = country;
                    }
                    if (membershipPlanId > 0)
                    {
                        model.MembershipPlanId = membershipPlanId;
                        model.CountryId = "GB"; 
                    }
                    if (siteClanId > 0)
                    {
                        model.SiteClanId = siteClanId;
                    }


                    Guid customerProviderId = Guid.Empty;
                    string customerProvider = HttpContext?.Session?.GetString(Keys.Keys.sessionproviderId);
                    if (!string.IsNullOrEmpty(customerProvider))
                    {
                        Guid.TryParse(customerProvider, out customerProviderId);
                        HttpContext?.Session?.SetString(Keys.Keys.sessionproviderId, string.Empty);
                        //List<dto.Status> statuses = await _statusServices.GetAll(Data.Constants.StatusType.CustomerCreation);
                        dto.StagingModels.CustomerRegistration customerRegistration =
                            await _stagingCustomerRegistrationService.GetByCustomerPaymentIdAsync(customerProviderId,
                                (int)CustomerCreation.New);
                        //statuses.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.New && x.Type == Data.Constants.StatusType.CustomerCreation).Id);
                        if (customerRegistration != null)
                        {
                            model = JsonConvert.DeserializeObject<CustomerViewModel>(customerRegistration.Data) ??
                                    new CustomerViewModel();
                        }
                    }
                }

                
                //string customerDetailjson = HttpContext?.Session?.GetString(Keys.Keys.sessionAccountDetail);
                //if (!string.IsNullOrEmpty(customerDetailjson))
                //{
                //    model = JsonConvert.DeserializeObject<CustomerViewModel>(customerDetailjson) ??
                //            new CustomerViewModel();
                //    HttpContext?.Session?.SetString(Keys.Keys.sessionAccountDetail, string.Empty);
                //    HttpContext?.Session?.SetString(Keys.Keys.sessionproviderId, string.Empty);
                //}
                await MapToCustomerView(model, token);

                //## Do we ever want to charge the customer in the create account view (It doesn't handle payments atm)? ##...
                model.CardCost = 0;

                return View("CreateAccount", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Terms")]
        public IActionResult Terms(string country)
        {
            //Methos to New Tab window of terms and condition
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }
            return View();
        }

        [HttpGet]
        [ActionName("FAQ")]
        public IActionResult FAQ(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }
            return View();
        }

        [HttpGet]
        [ActionName("ContactUs")]
        public IActionResult ContactUs(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }
            return View();
        }

        [HttpGet]
        [ActionName("AboutUs")]
        public IActionResult AboutUs(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }

            return View("AboutUs");
        }

        [HttpGet]
        [ActionName("Giftcards")]
        public IActionResult Giftcards(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }
            return View();
        }

        [HttpGet]
        [ActionName("Donations")]
        public IActionResult Donations(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }
            return View();
        }

        /// <summary>
        /// Account\Save is the key action to create a new user account on Signup
        /// It is called after the user has completed the account details form and has clicked continue
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save(CustomerViewModel model)
        {
            //Method to save account details
            try
            {
                // Remove address validation rules
                RemoveAddressValidationRules(model);

                // Validate the data first
                if (!ModelState.IsValid)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            sb.Append(error.ErrorMessage);
                            sb.Append("<br/>");
                        }
                    }
                    return Json(JsonResponse<string>.ErrorResponse(sb.ToString()));
                }
                else if (model.Dateofbirth > DateTime.Today.AddYears(-18) || model.Dateofbirth < DateTime.Parse("01 Jan 1900"))
                    return Json(JsonResponse<string>.ErrorResponse("You must be at least 18 years old and date of birth must be after 01 Jan 1900"));

                // if Model valid then lets create an account 
                var customerAccountDto = MapCustomerViewModelToDTO(model);
                var confirmUrl = Request.Scheme + "://" + Request.Host.Value + Url.Content("~/Account/Confirm");
                var userToken = _customerAccountService.CreateAccountFromPendingToken(customerAccountDto, confirmUrl, true);
                HttpContext.Session.Remove(Data.Constants.Keys.UserRegistrationTempData);

                await Task.CompletedTask;

                return Json(JsonResponse<object>.SuccessResponse(userToken));
            }

            //TODO:  Add explicit error handlers here to catch duplicate address etc
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while create account. Please try again."));
            }
            
        }


        /// <summary>
        /// This method is being called to validate the registration code provided by the user while Joining Exclusive, 
        /// Since it is a ajax call, we need to have this method in public access specifier
        /// </summary>
        /// <param name="code"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("RegisterValidation")]
        public IActionResult RegisterValidation(string code, int? customerId)
        {
            //Method to Validate registrationCode
            try
            {
                dto.UserToken userToken =  _customerAccountService.ValidateRegistrationCode(code);   
                if (userToken != null)
                {
                    var request = HttpContext.Request;
                    string currentUrl = $"{request.Scheme}://{request.Host.Value}";
                    var label =
                        _cache.Get<dto.WhiteLabelSettings>(string.Format(Data.Constants.Keys.WhiteLabel,
                            currentUrl));

                    var whiteLabel = _whiteLabelService.GetSiteSettingsById(userToken.WhitelabelId);
                    if (whiteLabel != null)
                    {
                        userToken.Slug = whiteLabel.Slug;

                        if (whiteLabel.Slug == "sport-rewards" && whiteLabel.Slug !=label.Slug)
                        {
                            return Json(JsonResponse<string>.ErrorResponse("The code entered is invalid. Please try again."));
                        }
                    }

                    UserRegistrationTempData userRegistrationTempData =
                        HttpContext.Session.GetObjectFromJson<UserRegistrationTempData>(Data.Constants.Keys
                            .UserRegistrationTempData);
                    if (userRegistrationTempData == null)
                    {
                        userRegistrationTempData = new UserRegistrationTempData();
                    }

                    userRegistrationTempData.Token = userToken;
                    HttpContext?.Session?.SetObjectAsJson(Data.Constants.Keys.UserRegistrationTempData,
                        userRegistrationTempData);

                    return Json(JsonResponse<object>.SuccessResponse(userToken));
                }
                //TODO:  Localisation of error messages
                return Json(JsonResponse<string>.ErrorResponse("We couldn't find an account matching the registration code you entered. Please check your registration code and try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("The code entered is invalid. Please try again."));
            }
        }


        [HttpGet]
        [ActionName("Register")]
        public IActionResult Register(string code)
        {
            //Method to Validate registrationCode
            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    string bytes = EncryptionHelper.Decrypt(code.Replace(' ', '+'));
                    dto.UserToken userToken = _customerAccountService.ValidateRegistrationCode(bytes);
                    if (userToken != null)
                    {
                        var request = HttpContext.Request;
                        string currentUrl = $"{request.Scheme}://{request.Host.Value}";

                        var whiteLabel =
                            _cache.Get<dto.WhiteLabelSettings>(string.Format(Data.Constants.Keys.WhiteLabel,
                                currentUrl));
                       
                        if (whiteLabel != null)
                        {
                            if (whiteLabel.Slug == "sport-rewards" && whiteLabel.Id == userToken.WhitelabelId)
                            {
                                return RedirectToAction("Index", "SportRewards");  
                            }
                            else if( whiteLabel.Id == userToken.WhitelabelId && whiteLabel.SiteOwnerId != null)
                            {
                                return RedirectToAction("Index", "SiteClan");
                            }
                            else
                            {
                                UserRegistrationTempData userRegistrationTempData =
                                    HttpContext.Session.GetObjectFromJson<UserRegistrationTempData>(Data.Constants.Keys
                                        .UserRegistrationTempData);
                                if (userRegistrationTempData == null)
                                {
                                    userRegistrationTempData = new UserRegistrationTempData();

                                    userRegistrationTempData.Token = userToken;
                                    HttpContext?.Session?.SetObjectAsJson(Data.Constants.Keys.UserRegistrationTempData,
                                        userRegistrationTempData);
                                }

                                return RedirectToAction("CreateAccount","Account");

                            }
                        }

                       
                    }

                }
                return View("Index");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Index");
            }
        }

        /// <summary>
        /// This returns the view to capture user credentials and Login will post those credentials for logging in the user
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("SignIn")]
        public IActionResult SignIn(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }

            return View("Login", new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            IActionResult result = null;

            //TODO:  Make use of the Return URL on Login
            //returnUrl = returnUrl ?? Url.Content("~/");

            try
            {

                var user = await _customerAccountService.Login(model.Username, model.LoginPassword);

                if (user?.Id > 0)
                {
                    //if (!string.IsNullOrEmpty(returnUrl))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    _cache.Remove(string.Format(Data.Constants.Keys.WhiteLabel,
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}"));
                    result = Json(JsonResponse<object>.SuccessResponse(user));
                }
                else if (user?.Id == -1)
                    result = Json(JsonResponse<string>.ErrorResponse(
                        "Login failed. Your user account has now been locked out. Please contact Exclusive support."));
                else
                    result = Json(JsonResponse<string>.ErrorResponse(
                        "We couldn't find an account matching the username and password you entered. Please check your username and password and try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                result = Json(JsonResponse<string>.ErrorResponse("Error while attempting to login. Please try again."));
            }

            return result;
        }

        [HttpGet]
        [ActionName("RecoverPassword")]
        public IActionResult RecoverPassword(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }

            return View("Recover", new LoginViewModel());
        }

        /// <summary>
        /// When clicking on Forgot password link, this method will be called to send the instructions to reset password
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ResetPasswordEmail")]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            try
            {
                var host = Request.Headers["Origin"].ToString();
                string result = await _userService.ResetUserPasswordAsync(username, host, _settings["NoReplyEmailAddress"]);
                return result == true.ToString()
                    ? Json(JsonResponse<string>.SuccessResponse("We have sent instructions to the email address you provided on how to change your password and recover your account."))
                    : Json(JsonResponse<string>.ErrorResponse(result));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while trying to send email."));
            }
        }

        /// <summary>
        /// This method gets call when the user clicks on the link provided to them in the email
        /// </summary>
        /// <param name="customerAuth"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string customerAuth, string token)
        {
            try
            {
                ViewData["Country"] = "GB";

                ResetPasswordModel model = new ResetPasswordModel
                {
                    Token = token.Replace(" ", "+")
                };
                
                var user = await _userService.FindByIdAsync(customerAuth);
                if (user != null)
                    model.Username = user.UserName;
                
                return View("ForgotPassword", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        [ActionName("SaveNewPassword")]
        public async Task<IActionResult> SaveNewPassword(string username, string token, string password)
        {
            try
            {
                token = token.Replace(" ", "+");
                var user = await _userService.FindByEmailAsync(username);
                var result = await _userService.ResetPasswordAsync(user, token, password);
                if (result.Succeeded)
                {
                    return Json(JsonResponse<bool>.SuccessResponse(true));
                }
                return Json(JsonResponse<string>.ErrorResponse("Error resetting password. Please try again later."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error resetting password. Please try again later."));
            }
        }

        [Authorize]
        [HttpGet]
        [ActionName("Logout")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            try
            {
                //Remove the customers details from the cache
                var user = _customerAccountService.GetUserAsync(HttpContext.User).Result;
                if(user != null)
                    _cache.Remove(string.Format(Data.Constants.Keys.CustomerSummary, user.Id));

                ViewData["MembershipCard"] = "";
                await _customerAccountService.Logout();

                _cache.Remove(string.Format(Data.Constants.Keys.WhiteLabel,$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}"));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while logout. Please try again."));
            }
        }


        [HttpGet]
        [ActionName("PrivacyPolicy")]
        public IActionResult PrivacyPolicy(string country)
        {
            //Methods to New Tab window of  Privacy Policy
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }
            return View("PrivacyPolicy");
        }

        [HttpGet]
        [ActionName("SelectPayment")]
        public async Task<IActionResult> SelectPayment(string country, int customerId, int membershipPlanId, string token = "")
        {
            try
            {
                //If customer Id = 0, throw error
                if (customerId == 0)
                {
                    ErrorViewModel err = new ErrorViewModel
                    {
                        RequestId = "Customer not found for creating Add-on card."
                    };
                    return View("Error", err);
                }
                if (string.IsNullOrEmpty(country))
                {
                    country = "GB";
                }
                if (!string.IsNullOrEmpty(country))
                {
                    ViewData["Country"] = country;
                }
                //initiate viewmodel
                AddOnCardViewModel model = new AddOnCardViewModel
                {
                    Id = customerId,
                    Token = token,
                    CountryId = country
                };
                if (membershipPlanId > 0)
                {
                    model.MembershipPlanId = membershipPlanId;
                }
                //Bind data to viewmodel
                await MapAddOnCard(model);
                return View("AddOnPayment", model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("CookiePolicy")]
        public IActionResult CookiePolicy(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }

            return View();
        }

        [HttpGet]
        [ActionName("ForConsumers")]
        public IActionResult ForConsumers(string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                ViewData["Country"] = country;
            }
            else
            {
                ViewData["Country"] = "GB";
            }

            return View();
        }

        /// <summary>
        /// This action is called when a user follows the link received in email to confirm their email address
        /// after they have first created their accounts
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="eToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Confirm")]
        public async Task<IActionResult> Confirm(string userData, string eToken)
        {
            try
            {
                bool response = false;
                if (string.IsNullOrEmpty(userData) || string.IsNullOrEmpty(eToken))
                {
                    ViewBag.Response = false;
                    return View();
                }

                eToken = eToken.Replace(" ", "+");

                var user = await _userService.FindByIdAsync(userData);
                var result = await _customerAccountService.ConfirmEmailTokenAsync(user, eToken);
                if (result.Succeeded)
                {
                   _cache.Set<dto.CustomerAccountSummary>(string.Format(Data.Constants.Keys.CustomerSummary, user.Id), null);
                    response = true;
                }
                else
                {
                    Logger.Error(result.Errors);
                }

                ViewBag.Response = response;

                return View();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("ResendConfirmation")]
        public async Task<IActionResult> ResendConfirmation()
        {
            try
            {
                //Get logged in user
                var user = await _userService.GetUserAsync(HttpContext.User);
                //Get customer
                var customer = await _customerService.Get(user.Id);
                //Generate email validation token
                var token = await _customerAccountService.GenerateEmailConfirmationTokenAsync(user);
                //Send email
                string confirmUrl = Request.Scheme + "://" + Request.Host.Value + Url.Content("~/Account/Confirm");
                await _emailManager.SendEmailAsync(user.Email,
                    (int)EmailTemplateType.AccountConfirmationEmail,
                    new
                    {
                        Name = $"{customer.Forename} {customer.Surname}",
                        url = $"{confirmUrl}?userData={user.Id}&eToken={token}"
                    });

                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while resending confirmation email."));
            }
        }

       
        public async Task<IActionResult> Unsubscribe(string email)
        {
            try
            {

                var user = await _userService.FindByEmailAsync(email);
                //Get customer
                var customer = await _customerService.Get(user.Id);
                customer.MarketingNewsLetter = false;
                var token = _customerAccountService.UpdateCustomerSettings(customer);
                if (token!=null)
                {
                    return View("SuccessUnsubscribed");
                }
                return View("Error");

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred."));
            }
        }

        

        #region Private Methods

        // TODO: Replace all manual mapping functions here with automapper
        private async Task MapToCustomerView(CustomerViewModel model, string token)
        {
            if (model.MembershipPlanId == 0)
            {
                int planId;
                int.TryParse(_settings["ActiveMembersipPlan"], out planId);
                model.MembershipPlanId = planId;
            }
            //Get MembershipPlan and Partner
            var plan = await _membershipPlanService.Get(model.MembershipPlanId);
            if (plan != null)
            {
                if (plan.Partner != null)
                {
                    model.PartnerLogo = plan.Partner.ImagePath;
                }
                else if (plan.PartnerId.HasValue)
                {
                    var partner = await _partnerService.GetByIdAsync(plan.PartnerId.Value);
                    model.PartnerLogo = partner?.ImagePath;
                }

                model.PlanDescription = plan.Description;
                model.PaidByEmployer = plan.PaidByEmployer;
                model.MinimumValue = plan.MinimumValue;
                model.PaymentFee = plan.PaymentFee;
                //get plan type and Get Terms and conditions
                dto.MembershipPlanType planType =
                    await _membershipPlanService.GetPlanTypeByIdAsync(plan.MembershipPlanTypeId);
                if (planType?.TermsConditionsId != null)
                {
                    model.TermsAndConditions =
                        await _membershipPlanService.GetTermsConditionsByIdAsync(
                            planType.TermsConditionsId.Value);
                }
            }

            //Get Membership plan benefits
            model.PlanBenefits =
                await _membershipPlanService.GetBenefitsByPlanIdAsync(model.MembershipPlanId);

            //Get payment provider details for membership Plan
            List<dto.MembershipPlanPaymentProvider> paymentproviders =
                await _membershipPlanService.GetPaymentProvidersForMembershipPlanAsync(
                    model.MembershipPlanId);

            if (paymentproviders != null && paymentproviders.Count > 0)
            {
                foreach (dto.MembershipPlanPaymentProvider provider in paymentproviders)
                {
                    if (provider.PaymentProvider?.Name == Enums.PaymentProvider.PayPal.ToString())
                    {
                        model.PaymentProvider.PayPal = true;
                        model.SubscribeAppRef = provider.SubscribeAppRef;
                        model.SubscribeAppAndCardRef = provider.SubscribeAppAndCardRef;
                        model.OneOfPaymentRef = provider.OneOffPaymentRef;
                    }
                    if (provider.PaymentProvider?.Name == Enums.PaymentProvider.Cashback.ToString())
                    {
                        model.PaymentProvider.Cashback = true;
                    }
                    model.PaymentProvider.MembershipCard = provider.MembershipPlan.CustomerCardPrice;
                    model.PaymentProvider.PlanName = provider.MembershipPlan.Description;
                    if (provider.MembershipPlan.Duration > 0)
                    {
                        switch (provider.MembershipPlan.Duration)
                        {
                            case 365:
                                model.PaymentProvider.PlanType = "One year full membership card at";
                                break;
                            case 30:
                                model.PaymentProvider.PlanType = "One month membership card at";
                                break;
                        }
                    }
                    model.CardCost = provider.MembershipPlan.CustomerCardPrice;
                }
            }

            //model.ListofQuestion = await MapToSecurityQuestions();
            model.ListofCountry = GetCountryList();
            model.DiamondUpgrade = true;
            //model.CountryId = "GB";
            if (!string.IsNullOrEmpty(token))
            {
                model.Token = token;
            }
        }

        private async Task<List<SelectListItem>> MapToSecurityQuestions()
        {
            List<dto.SecurityQuestion> response = await _securityQuestionService.GetAll();
            return response.Select(sequrity => new SelectListItem
            {
                Text = sequrity.Question,
                Value = Convert.ToString(sequrity.Id)
            }).ToList();
        }

        private List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> country = new List<SelectListItem>
            {
                new SelectListItem {Text = "CZ", Value = "CZ"},
                new SelectListItem {Text = "GB", Value = "GB"},
                new SelectListItem {Text = "PL", Value = "PL"},
                new SelectListItem {Text = "SC", Value = "SC"},
                new SelectListItem {Text = "SK", Value = "SK"}
            };

            return country;
        }

        private async Task MapAddOnCard(AddOnCardViewModel model)
        {
            try
            {
                //Check if membershipPlan Id > 0
                if (model.MembershipPlanId > 0)
                {
                    //Get payment provider details for membership Plan
                    List<dto.MembershipPlanPaymentProvider> paymentproviders =
                        await _membershipPlanService.GetPaymentProvidersForMembershipPlanAsync(
                            model.MembershipPlanId);

                    if (paymentproviders != null && paymentproviders.Count > 0)
                    {
                        foreach (dto.MembershipPlanPaymentProvider provider in paymentproviders)
                        {
                            if (provider.PaymentProvider?.Name == Enums.PaymentProvider.PayPal.ToString())
                            {
                                model.PaymentProvider.PayPal = true;
                                model.SubscribeAppRef = provider.SubscribeAppRef;
                                model.SubscribeAppAndCardRef = provider.SubscribeAppAndCardRef;
                            }
                            if (provider.PaymentProvider?.Name == Enums.PaymentProvider.Cashback.ToString())
                            {
                                model.PaymentProvider.Cashback = true;
                            }
                            model.PaymentProvider.MembershipCard = provider.MembershipPlan.CustomerCardPrice;
                            model.PaymentProvider.PlanName = provider.MembershipPlan.Description;
                            if (provider.MembershipPlan.Duration > 0)
                            {
                                switch (provider.MembershipPlan.Duration)
                                {
                                    case 365:
                                        model.PaymentProvider.PlanType = "One year full membership card at";
                                        break;
                                    case 30:
                                        model.PaymentProvider.PlanType = "One month membership card at";
                                        break;
                                }
                            }
                            model.CardCost = provider.MembershipPlan.CustomerCardPrice;
                        }
                    }
                }

                //Get last membershipCard and get the physical Card Requested status
                List<dto.MembershipCard> cards = await _membershipCardService.GetAll(model.Id);
                dto.MembershipCard card = cards?.FirstOrDefault();
                if (card == null)
                {
                    model.PhysicalCardRequested = false;
                }
                else if (card.PhysicalCardRequested)
                {
                    model.PhysicalCardRequested = card.PhysicalCardRequested;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private dto.CustomerAccountDto MapCustomerViewModelToDTO(CustomerViewModel model)
        {
            var contactDetail = new dto.ContactDetail()
            {
                EmailAddress = model.Email,
                //PostCode = model.Postcode
            };


            var customer = new dto.Customer()
            {
                ContactDetail = contactDetail,
                DateOfBirth = model.Dateofbirth,
                Forename = model.Forename,
                Surname = model.Surname,
                //Title = model.Title,
                MarketingNewsLetter = model.MarketingPreference,
                CustomerSecurityQuestions = new List<dto.CustomerSecurityQuestion>(),
                SiteClanId = model.SiteClanId > 0 ? model.SiteClanId : null
            };

            //var securityAnswer = new dto.CustomerSecurityQuestion()
            //{
            //    //SecurityQuestionId = model.QuestionId,
            //    //Answer = model.Answer
            //};
            //customer.CustomerSecurityQuestions.Add(securityAnswer);

            Guid.TryParse(model.Token, out Guid token);
            var pendingToken = new dto.MembershipPendingToken() { Token = token };

            var customerAccount = new dto.CustomerAccountDto()
            {
                Username = model.Email,
                Password = model.Password,
                TermsConditionsId = model.TermsAndConditions?.Id,
                CountryCode = "GB",  // TODO - Consider adding back country code support for other memberships
                Customer = customer,
                PendingMembershipToken = pendingToken
            };

            return customerAccount;
        }

        private void RemoveAddressValidationRules(CustomerViewModel model)
        {
            if (!model.Tick)
            {
                var keysToRemove = ModelState.Keys.Where(x => x.Contains("Addressone")).ToList();
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }
                keysToRemove = ModelState.Keys.Where(x => x.Contains("Town")).ToList();
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }
                keysToRemove = ModelState.Keys.Where(x => x.Contains("Country")).ToList();
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }
            }
        }


        #endregion
    }
}