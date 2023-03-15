using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;
using ExclusiveCard.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs.StagingModels;
using Microsoft.Extensions.Options;
using dto = ExclusiveCard.Services.Models.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace ExclusiveCard.Website.Controllers
{
    [AllowAnonymous]
    public class PayPalController : BaseController
    {
        #region Private Members

        private readonly IUserService _userService;
        private readonly IOLD_MembershipPlanService _planService;
        private readonly IStagingCustomerRegistrationService _stagingCustomerRegistrationService;
        private readonly SignInManager<ExclusiveUser> _signInManager;
        private readonly IOptions<TypedAppSettings> _settings;
        private readonly Services.Interfaces.Public.IMembershipCardService _membershipCardService;
        private readonly IPaymentNotificationService _paymentNotificationService;
        private readonly IMapper _mapper;
        private readonly ICustomerAccountService _newUserAccountService;
        private readonly IMemoryCache _cache;

        #endregion

        #region Constructor

        public PayPalController(IUserService userService, IOLD_MembershipPlanService planService,
            IStagingCustomerRegistrationService stagingCustomerRegistrationService,
            SignInManager<ExclusiveUser> signInManager, IOptions<TypedAppSettings> settings,
            Services.Interfaces.Public.IMembershipCardService membershipCardService,
            IPaymentNotificationService paymentNotificationService, 
             IMapper mapper,
            ICustomerAccountService newUserAccountService,
            IMemoryCache cache)
        {
            _userService = userService;
            _planService = planService;
            _stagingCustomerRegistrationService = stagingCustomerRegistrationService;
            _signInManager = signInManager;
            _settings = settings;
            _membershipCardService = membershipCardService;
            _paymentNotificationService = paymentNotificationService;
            _mapper = mapper;
            _newUserAccountService = newUserAccountService;
            _cache = cache;
        }

        #endregion

        [HttpPost]
        [ActionName("PayPalRequest")]
        public async Task<IActionResult> PayPalRequest(CustomerViewModel model)
        {
            //Method to store customer details in http.session and return the new providerId Guid.
            try
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

                if (model.Dateofbirth > DateTime.UtcNow)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Date must be after 01 Jan 1900 and before or equal to current date"));
                }

                dto.MembershipPlan membershipPlan = await _planService.Get(model.MembershipPlanId);
                if (membershipPlan == null)
                {
                    return Json(JsonResponse<string>.ErrorResponse("No active membership plan found."));
                }
                Data.Models.ExclusiveUser userDetail = await _userService.FindByNameAsync(model.Email);
                if (userDetail != null)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Email address already exists."));
                }
                //HttpContext?.Session?.SetString(Keys.Keys.sessionAccountDetail, string.Empty);
                HttpContext?.Session?.SetString(Keys.Keys.sessionproviderId, string.Empty);
                var customerView = model.ToJson();
                //HttpContext?.Session?.SetString(Keys.Keys.sessionAccountDetail, customerView);
                Guid providerId = Guid.NewGuid();
                HttpContext?.Session?.SetString(Keys.Keys.sessionproviderId, providerId.ToString());
                //Save to StagingCustomerRegistration
                CustomerRegistration customerRegistration = new CustomerRegistration();
                await MapToSaveStagingCustomerRegistration(providerId, customerView,
                    customerRegistration);
                if (!string.IsNullOrEmpty(customerRegistration.CustomerPaymentId.ToString()))
                {
                    return Json(JsonResponse<Guid>.SuccessResponse(customerRegistration.CustomerPaymentId));
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while create account. Please try again."));
                //// Save MemberShipCard and Customer Payment record
                //MembershipCard membershipCard = new MembershipCard();
                //await MembershipCardHelper.PayPalRequestSaveToData(_cache, _planService, _cardService, membershipCard, _statusService, 
                //    _paymentProviderService, _customerPaymentService, null, model.Tick, providerId, countryCode);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while create account. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("AddOnPayPalRequest")]
        public async Task<IActionResult> AddOnPayPalRequest(AddOnCardViewModel model)
        {
            try
            {
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

                dto.MembershipPlan membershipPlan = await _planService.Get(model.MembershipPlanId);
                if (membershipPlan == null)
                {
                    return Json(JsonResponse<string>.ErrorResponse("No active membership plan found."));
                }

                HttpContext?.Session?.SetString(Keys.Keys.sessionproviderId, string.Empty);
                var customerView = model.ToJson();

                Guid providerId = Guid.NewGuid();
                HttpContext?.Session?.SetString(Keys.Keys.sessionproviderId, providerId.ToString());

                //Save to StagingCustomerRegistration
                CustomerRegistration customerRegistration = new CustomerRegistration();
                await MapToSaveStagingCustomerRegistration(providerId, customerView,
                    customerRegistration);
                if (!string.IsNullOrEmpty(customerRegistration.CustomerPaymentId.ToString()))
                {
                    return Json(JsonResponse<Guid>.SuccessResponse(customerRegistration.CustomerPaymentId));
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while creating payment request. Please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while creating payment request. Please try again."));
            }
        }

        //Paypal payment for MembershipCard/Add-on card/Diamond upgrade
        [HttpGet]
        [ActionName("Success")]
        public async Task<IActionResult> Success(string tx, string st, string amt, string cc, string cm, string item_number, string sig)
        {
            Logger.Debug($"Success returned from PayPal st: {st} cm: {cm}");

            //PayPal seems to return "Pending" now?
            if (st.ToLower() == "pending" || st.ToLower() == "completed")
            {
                try
                {
                    // attempt to remove cached CustomerAccountSummary for this user, so it will be updated with account details
                    bool success = Guid.TryParse(cm, out Guid paymentId);
                    if (success)
                    {
                        var data = await _stagingCustomerRegistrationService.GetByCustomerPaymentIdAsync(paymentId);
                        if (data != null)
                        {
                            _cache.Remove(string.Format(Data.Constants.Keys.CustomerSummary, data.AspNetUserId));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Unable to clear cache after successful payment [create account]  for custom data " + cm);
                }

                ViewBag.CustomProviderId = cm;

                if (st.ToLower() == "pending")
                {
                    return View("PendingPayment");
                }
                else
                {
                    return View("SuccessfulPayment");
                }
            }
            else
            {
                return View("UnsuccessfulPayment");
            }
        }

        //Paypal payment cancelled for MembershipCard/Add-on card/Diamond upgrade
        [HttpGet]
        [ActionName("Cancel")]
        public IActionResult Cancel()
        {
            //PayPal Cancel method redirect to CreateAccount page
            return View("UnsuccessfulPayment");
            //return RedirectToAction("CreateAccount", "Account");
        }

       
        [HttpGet]
        [ActionName("Payment")]
        public async Task<IActionResult> Payment(int customerPlanId, bool isDiamondUpgrade = false)
        {
            //this was just wrong!!!
            //It needs to at least pass in the user's current membership plan so you can pick up the correct details
            try
            {
                //1. Get OrderSummaryData for Diamond Level
                //Data.DTOs.OrderSummaryDataModel orderSummaryData = await _planService.GetOrderSummary((int)Enums.MembershipLevel.Diamond);
                var orderSummaryData = await _planService.GetOrderSummaryForDiamondPlan(customerPlanId);
                //2. Map To OrderSummaryViewModel
                var viewModel = new OrderSummaryViewModel
                {
                    MembershipPlanId = orderSummaryData.MembershipPlanId,
                    PayPalButtonAppAndCardRef = orderSummaryData.SubscriptionAppAndCardRef,
                    PayPalButtonAppRef = orderSummaryData.SubscriptionAppRef,
                    PayPalLink = _settings.Value.PayPalLink,
                    OrderDetails = new List<OrderDetailsViewModel>
                    {
                        new OrderDetailsViewModel
                        {
                            IsSelected = isDiamondUpgrade,
                            CardPrice = orderSummaryData.CardPrice,
                            OrderName = orderSummaryData.OrderName
                        }
                    }
                };
                //3.Currently we have diamond card upgrade only
                viewModel.TotalPrice = viewModel.OrderDetails.Where(x => x.IsSelected).Sum(x => x.CardPrice);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("PayPalRequestUpgradeAccount")]
        public async Task<IActionResult> PayPalRequestUpgradeAccount(PlanUpgradeViewModel model)
        {
            try
            {
                if (model.MembershipPlanId <= 0)
                    return Json(JsonResponse<string>.ErrorResponse("Error while upgrade account. Please try again."));
                var providerId = Guid.NewGuid();
                if (!_signInManager.IsSignedIn(User))
                    return Json(JsonResponse<string>.ErrorResponse("Error while upgrade account. Please try again."));
                var user = await _userService.GetUserAsync(User);
                if (user?.Id == null)
                    return Json(JsonResponse<string>.ErrorResponse("Error while upgrade account. Please try again."));
                var data = model.ToJson();
                var response = new CustomerRegistration();
                await MapToSaveStagingCustomerRegistration(providerId, data, response, user.Id);
                return !string.IsNullOrEmpty(response.CustomerPaymentId.ToString())
                    ? Json(JsonResponse<Guid>.SuccessResponse(response.CustomerPaymentId))
                    : Json(JsonResponse<string>.ErrorResponse("Error while upgrade account. Please try again."));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error while upgrade account. Please try again."));
            }
        }

        [HttpGet]
        [ActionName("BoostSuccess")]
        public async Task<IActionResult> BoostSuccess(string tx, string st, string amt, string cc, string cm, string item_number, string sig)
        {
            try
            {
                // attempt to remove cached CustomerAccountSummary for this user, so it will be updated with new investment total
                bool success = Guid.TryParse(cm, out Guid paymentId);
                if (success)
                {
                    var data = await _stagingCustomerRegistrationService.GetByCustomerPaymentIdAsync(paymentId);
                    if (data != null)
                    {
                        _cache.Remove(string.Format(Data.Constants.Keys.CustomerSummary, data.AspNetUserId));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Unable to clear cache after boost success for custom data " + cm);
            }


            ViewBag.CustomProviderId = cm;
            return View();
        }

        [HttpGet]
        [ActionName("BoostCancel")]
        public IActionResult BoostCancel()
        {
            return View();
        }

        #region Private Methods

        //Method for Save StagingCustomerRegistration
        private async Task MapToSaveStagingCustomerRegistration(Guid customerPaymentId, string customerData,
            CustomerRegistration customerRegistrationResponse, string aspNetUserId = null)
        {
            var customerRegistration = new CustomerRegistration
            {
                CustomerPaymentId = customerPaymentId,
                Data = customerData,
                StatusId = (int) Enums.CustomerCreation.New, //statuses?.FirstOrDefault(x => x.IsActive && x.Type == Data.Constants.StatusType.CustomerCreation && x.Name == Data.Constants.Status.New)?.Id,
                AspNetUserId = aspNetUserId
            };
            CustomerRegistration response =
                await _stagingCustomerRegistrationService.AddAsync(customerRegistration);
            if (response != null)
            {
                customerRegistrationResponse.CustomerPaymentId = response.CustomerPaymentId;
                customerRegistrationResponse.Data = response.Data;
                customerRegistrationResponse.StatusId = response.StatusId;
            }
        }


        #endregion
    }
}
