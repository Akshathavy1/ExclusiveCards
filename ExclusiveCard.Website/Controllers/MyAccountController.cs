using System;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Helpers;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using dto = ExclusiveCard.Services.Models.DTOs;
using pub = ExclusiveCard.Services.Interfaces.Public;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ExclusiveCard.Website.Controllers
{
    [Authorize]
    public class MyAccountController : BaseController
    {
        #region Private Members

        private readonly IBankDetailService _bankDetailService;
        private readonly ICashbackPayoutService _cashbackPayoutService;
        private readonly ICustomerBankDetailService _customerBankDetailService;
        private readonly Services.Interfaces.Admin.IPartnerService _partnerService;
        private readonly IPartnerRewardWithdrawalService _partnerRewardWithdrawalService;
        private readonly IUserService _userService;
        private readonly IOptions<TypedAppSettings> _settings;
        private readonly IContactDetailService _contactDetailService;
        private readonly IMemoryCache _cache;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ISecurityQuestionService _securityQuestionService;
        private readonly ICustomerSecurityQuestionService _customerSecurityQuestionService;

        #endregion

        #region Constructor

        public MyAccountController(IBankDetailService bankDetailService,
            ICashbackPayoutService cashbackPayoutService,
            ICustomerBankDetailService customerBankDetailService,
            Services.Interfaces.Admin.IPartnerService partnerService,
            IPartnerRewardWithdrawalService partnerRewardWithdrawalService,
            IUserService userService,
            IOptions<TypedAppSettings> settings,
            pub.ICustomerService customerService,
            IContactDetailService contactDetailService,
            ICustomerAccountService customerAccountService,
            IMemoryCache cache, ISecurityQuestionService securityQuestionService, ICustomerSecurityQuestionService customerSecurityQuestionService)
        {
            _bankDetailService = bankDetailService;
            _cashbackPayoutService = cashbackPayoutService;
            _customerBankDetailService = customerBankDetailService;
            _partnerService = partnerService;
            _partnerRewardWithdrawalService = partnerRewardWithdrawalService;
            _userService = userService;
            _settings = settings;
            _contactDetailService = contactDetailService;
            _customerAccountService = customerAccountService;
            _cache = cache;
            _securityQuestionService = securityQuestionService;
            _customerSecurityQuestionService = customerSecurityQuestionService;
        }

        #endregion

        #region public actions

        [ActionName("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View("Error");
        }

        //[Authorize(Roles = "User")]
        [HttpGet]
        [ActionName("MyAccount")]
        public async Task<IActionResult> MyAccount(string country, int? page, string sortField,
            string sortDirection, bool diamondCardRequest = false)
        {
            AccountViewModel model = new AccountViewModel();
            try
            {
                if (!string.IsNullOrEmpty(country))
                {
                    ViewData["Country"] = country;
                }
                else
                {
                    ViewData["Country"] = "GB";
                }

                var user = await _customerAccountService.GetUserAsync(HttpContext.User);
                if (!string.IsNullOrEmpty(user?.Id))
                {

                    // Initialise TransactionLog Object
                    TransactionLogRequest request = new TransactionLogRequest
                    {
                        TransactionSortField = TransactionLogSortField.Date,
                        SortDirection = "asc"
                    };

                    if (!string.IsNullOrEmpty(sortField))
                    {
                        if (sortField == TransactionLogSortField.Merchant.ToString())
                        {
                            request.TransactionSortField = TransactionLogSortField.Merchant;
                        }
                        else if (sortField == TransactionLogSortField.Value.ToString())
                        {
                            request.TransactionSortField = TransactionLogSortField.Value;
                        }
                        else if (sortField == TransactionLogSortField.Status.ToString())
                        {
                            request.TransactionSortField = TransactionLogSortField.Status;
                        }
                        else if (sortField == TransactionLogSortField.Invested.ToString())
                        {
                            request.TransactionSortField = TransactionLogSortField.Invested;
                        }
                    }

                    if (!string.IsNullOrEmpty(sortDirection) && sortDirection == "desc")
                    {
                        request.SortDirection = sortDirection;
                    }
                    model.Transactions = new TransactionLogList(request);
                    model.Withdrawals = new TransactionLogList(request);

                    if (page.HasValue)
                    {
                        model.Transactions.CurrentPageNumber = page;
                        model.Transactions.PagingViewModel.CurrentPage = page.Value;
                        model.Withdrawals.CurrentPageNumber = page;
                        model.Withdrawals.PagingViewModel.CurrentPage = page.Value;
                    }
                    else
                    {
                        model.Transactions.CurrentPageNumber = 1;
                        model.Transactions.PagingViewModel.CurrentPage = 1;
                        model.Withdrawals.CurrentPageNumber = 1;
                        model.Withdrawals.PagingViewModel.CurrentPage = 1;
                    }

                    // End initialise transactionLog


                    //Get Initial Account data for customer
                    var summary = await  GetCustomerAccountSummary();
                    MapMyAccountDetails(model, summary, page);

                    //Get Transaction Logs
                    int pageSize = 100;
                    int.TryParse(_settings.Value.ReportPageSize.ToString(), out pageSize);
                    TransactionLogSortOrder sortOrder = GetSortOrder(request);
                    var data = await _partnerRewardWithdrawalService.GetTransactionLog(user.Id, (int)model.Transactions.CurrentPageNumber, pageSize,
                        sortOrder);

                    MapToViewModel(model.Transactions, data, model.CustomerName);

                    var withdraws = await _partnerRewardWithdrawalService.GetWithdrawalLog(user.Id,
                        (int)model.Withdrawals.CurrentPageNumber, pageSize);
                    MapToViewModel(model.Withdrawals, withdraws, model.CustomerName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }

            ViewBag.ViewType = "Account Overview";

            if (diamondCardRequest)
                ViewBag.ViewType = "Account Package";
            ViewData["isDiamondReq"] = diamondCardRequest;
            return View("MyAccount", model);
        }

        [HttpGet]
        [ActionName("Withdraw")]
        public async Task<IActionResult> Withdraw(string country, int membershipCardId, string membershipPlanType)
        {
            AccountViewModel account = new AccountViewModel();

            try
            {
                if (!string.IsNullOrEmpty(country))
                {
                    ViewData["Country"] = country;
                }
                else
                {
                    ViewData["Country"] = "GB";
                }

                //Get Initial Account data for customer
                var summary = await GetCustomerAccountSummary();
                MapMyAccountDetails(account, summary);

                var user = await _customerAccountService.GetUserAsync(HttpContext.User);

                var error = string.Empty;

                WithdrawViewModel model = new WithdrawViewModel();

                //if (membershipPlanType == MembershipPlanTypeEnum.PartnerReward.ToString() || summary.PlanType == MembershipPlanTypeEnum.PartnerReward.ToString())
                //{
                //    var data = await _partnerRewardWithdrawalService.GetWithdrawalDataForRequest(summary.MembershipCardId);

                //    if (data != null)
                //    {

                //        model.RequestExists = data.RequestExists;
                //        model.CustomerId = data.CustomerId;
                //        model.BankDetailId = data.BankDetailId ?? 0;
                //        model.PartnerRewardId = data.PartnerRewardId ?? 0;
                //        model.Name = data.Name;
                //        model.AccountNumber = data.AccountNumber;
                //        model.SortCode = data.SortCode;
                //        model.AvailableFund = data.AvailableFund ?? 0m;
                //        model.MembershipPlanType = summary.PlanType;
                //        var response = await _partnerRewardWithdrawalService.GetByBankDetailIdAsync(model.BankDetailId);
                //        if (response != null)
                //        {
                //            model.Status = Enum.GetName(typeof(WithdrawalStatus), response.StatusId);
                //        }

                //    }
                //}
                //else
                //{

                    var data = await _cashbackPayoutService.GetCashoutDataForRequest(user.Id);

                    if (data != null)
                    {
                        var response = await _cashbackPayoutService.GetByCustomerPartnerCurrency(data.CustomerId, "GBP");
                        model.RequestExists = data.RequestExists;
                        model.CustomerId = data.CustomerId;
                        model.BankDetailId = data.BankDetailId ?? 0;
                        model.PartnerRewardId = data.PartnerRewardId ?? 0;
                        model.Name = data.Name;
                        model.AccountNumber = data.AccountNumber;
                        model.SortCode = data.SortCode;
                        model.AvailableFund = data.AvailableFund ?? 0m;
                        model.MembershipPlanType = summary.PlanType;
                        if (response != null)
                        {
                            //model.AvailableFund = data.AvailableFund-response.Amount ?? 0m;
                            model.Status = Enum.GetName(typeof(Cashback), response.StatusId);
                        }


                    //}
                }

                if (!string.IsNullOrEmpty(error))
                {
                    return View("Error");
                }

                if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_withdraw", model);

                account.Withdraw = model;
                ViewBag.ViewType = "Withdraw Funds";
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(
                    JsonResponse<string>.ErrorResponse("An error occurred while preparing for withdrawal request"));
            }

            return View("MyAccount", account);
        }

        [HttpPost]
        [ActionName("RequestWithdrawal")]
        public async Task<IActionResult> RequestWithdrawal(WithdrawViewModel model)
        {
            try
            {
                if (model == null)
                    return Json(JsonResponse<string>.ErrorResponse("Withdrawal request is empty."));

                //validate logged in user password
                var currentUser = await _userService.GetUserAsync(HttpContext.User);
                if (currentUser != null)
                {
                    bool checkPassword = await _userService.CheckPasswordAsync(currentUser, model.Password);
                    if (!checkPassword)
                    {
                        return Json(JsonResponse<string>.ErrorResponse("Password is wrong. Please try again with correct password."));
                    }
                }

                bool isNew = false;
                //check for existing customer bank detail
                dto.CustomerBankDetail custBankDetails = _customerAccountService.GetCustomerBankDetail(model.CustomerId, model.BankDetailId);
                if (custBankDetails == null || custBankDetails.CustomerId < 1 || custBankDetails.BankDetailsId < 1)
                {
                    isNew = true;
                    custBankDetails = new dto.CustomerBankDetail() { CustomerId = model.CustomerId, IsActive = true, IsDeleted = false };
                }
                //Check if existing BankDetail matches
                dto.BankDetail bankDetail = _customerAccountService.GetBankDetail(custBankDetails.BankDetailsId);
                if (bankDetail != null && bankDetail.Id > 0 &&
                    (bankDetail.AccountName != model.Name ||
                    bankDetail.AccountNumber != model.AccountNumber ||
                    bankDetail.SortCode != model.SortCode))
                {
                    //Disable existing bank detail
                    bankDetail.IsDeleted = true;
                    _customerAccountService.UpdateBankDetail(bankDetail);

                    //Disable existing customer bank detail
                    custBankDetails.IsActive = false;
                    custBankDetails.IsDeleted = true;
                    _customerAccountService.UpdateCustomerBankDetail(custBankDetails);

                    isNew = true;
                    custBankDetails = new dto.CustomerBankDetail() { CustomerId = model.CustomerId, IsActive = true, IsDeleted = false };

                }
                if (isNew)
                {
                    //Create bankdetail
                    bankDetail = new dto.BankDetail
                    {
                        AccountName = model.Name,
                        AccountNumber = model.AccountNumber,
                        SortCode = model.SortCode,
                        IsDeleted = false
                    };
                    bankDetail = _customerAccountService.CreateBankDetail(bankDetail);

                    custBankDetails.BankDetailsId = bankDetail.Id;
                    custBankDetails.IsActive = true;
                    custBankDetails.IsDeleted = false;
                    _customerAccountService.CreateCustomerBankDetail(custBankDetails);
                }

                string error = string.Empty;

                dto.CashbackPayout withdrawal = new dto.CashbackPayout
                {
                    CustomerId = model.CustomerId,
                    StatusId = (int)Enums.Cashback.Requested,
                    Amount = model.WithdrawAmount,
                    CurrencyCode = "GBP",
                    BankDetailId = custBankDetails.BankDetailsId,
                    RequestedDate = DateTime.UtcNow,
                    ChangedDate = DateTime.UtcNow
                };
                var response = await _cashbackPayoutService.Add(withdrawal);
                if (response.Id == 0)
                {
                    error += "Failed to create withdrawal request.";
                }


                if (!string.IsNullOrEmpty(error))
                {
                    return Json(JsonResponse<string>.ErrorResponse(error));
                }
                _cache.Remove(string.Format(Data.Constants.Keys.MyAccount, currentUser.Id));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("An error occurred while processing withdrawal request. Please try again."));
            }
            return Json(JsonResponse<bool>.SuccessResponse(true));
        }

        [HttpGet]
        [ActionName("Transactions")]
        public async Task<IActionResult> Transactions(int? page, string sortField,
            string sortDirection, string membershipPlanType)
        {
            TransactionLogList model;
            try
            {
                

                var user = await _customerAccountService.GetUserAsync(HttpContext.User);

                TransactionLogRequest request = new TransactionLogRequest
                {
                    TransactionSortField = TransactionLogSortField.Date,
                    SortDirection = "asc"
                };

                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == TransactionLogSortField.Merchant.ToString())
                    {
                        request.TransactionSortField = TransactionLogSortField.Merchant;
                    }
                    else if (sortField == TransactionLogSortField.Value.ToString())
                    {
                        request.TransactionSortField = TransactionLogSortField.Value;
                    }
                    else if (sortField == TransactionLogSortField.Status.ToString())
                    {
                        request.TransactionSortField = TransactionLogSortField.Status;
                    }
                    else if (sortField == TransactionLogSortField.Invested.ToString())
                    {
                        request.TransactionSortField = TransactionLogSortField.Invested;
                    }
                }

                if (!string.IsNullOrEmpty(sortDirection) && sortDirection == "desc")
                {
                    request.SortDirection = sortDirection;
                }
                model = new TransactionLogList(request);

                if (page.HasValue)
                {
                    model.CurrentPageNumber = page;
                    model.PagingViewModel.CurrentPage = page.Value;
                }
                else
                {
                    model.CurrentPageNumber = 1;
                    model.PagingViewModel.CurrentPage = 1;
                }

                int pageSize = 50;
                int.TryParse(_settings.Value.PageSize.ToString(), out pageSize);
                Enums.TransactionLogSortOrder sortOrder = GetSortOrder(request);
                var data = await _partnerRewardWithdrawalService.GetTransactionLog(user.Id, (int)model.CurrentPageNumber, pageSize,
                    sortOrder);

                //Map to ViewModel
                MapToViewModel(model, data, null);

                ViewData["Type"] = membershipPlanType;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(
                    JsonResponse<string>.ErrorResponse("An error occurred while preparing for withdrawal request"));
            }

            return PartialView("_transactionLog", model);
        }

        [HttpGet]
        [ActionName("Account")]
        public IActionResult Account(int membershipCardId)
        {

            try
            {
                //if (!string.IsNullOrEmpty(error))
                //{
                //    return View("Error");
                //}

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(
                    JsonResponse<string>.ErrorResponse("An error occurred while preparing for withdrawal request"));
            }

            return PartialView("_settings");
        }

        [HttpGet]
        [ActionName("Help")]
        public IActionResult Help()
        {

            try
            {
                //if (!string.IsNullOrEmpty(error))
                //{
                //    return View("Error");
                //}

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(
                    JsonResponse<string>.ErrorResponse("An error occurred while preparing for withdrawal request"));
            }

            return PartialView("_support");
        }

        [HttpGet]
        [ActionName("Security")]
        public IActionResult Security()
        {
            return PartialView("_security");
        }

        [HttpGet]
        [ActionName("Package")]
        public async Task<IActionResult> Package(string country)
        {
            AccountViewModel account = new AccountViewModel();
            LayoutViewModel model = new LayoutViewModel();
            try
            {
                if (!string.IsNullOrEmpty(country))
                {
                    ViewData["Country"] = country;
                }
                else
                {
                    ViewData["Country"] = "GB";
                }

                //Get Initial Account data for customer
                var summary = await GetCustomerAccountSummary();
                MapMyAccountDetails(account, summary);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_preferences", model);

            account.Preferences = model;
            ViewBag.ViewType = "Account Package";
            return View("MyAccount", account);
        }

        [HttpGet]
        [ActionName("Settings")]
        public async Task<IActionResult> Settings(string country)
        {
            AccountViewModel account = new AccountViewModel();
            SettingsViewModel model = new SettingsViewModel();
            try
            {
                if (!string.IsNullOrEmpty(country))
                {
                    ViewData["Country"] = country;
                }
                else
                {
                    ViewData["Country"] = "GB";
                }

                //Get Initial Account data for customer
                var summary = await GetCustomerAccountSummary();
                MapMyAccountDetails(account, summary);

                var user = await _customerAccountService.GetUserAsync(HttpContext.User);
                var customer = _customerAccountService.GetCustomer(user.Id);
                if (customer != null)
                {
                    model = new SettingsViewModel
                    {
                        CustomerId = customer.Id,
                        ContactDetailId = customer.ContactDetailId,
                        Forename = customer.Forename,
                        Surname = customer.Surname,
                        DateOfBirth = customer.DateOfBirth,
                        Email = customer.ContactDetail?.EmailAddress,
                        Postcode = customer.ContactDetail?.PostCode,
                        NationalInsuranceNumber = customer.NINumber,
                        Address1 = customer.ContactDetail?.Address1,
                        Address2 = customer.ContactDetail?.Address2,
                        Address3 = customer.ContactDetail?.Address3,
                        Town = customer.ContactDetail?.Town,
                        District = customer.ContactDetail?.District,
                        MarketingNewsLetter = customer.MarketingNewsLetter
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            // return PartialView("_settings", model);
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_settings", model);

            model.ListofQuestion =await MapToSecurityQuestions();
            account.Settings = model;
            ViewBag.ViewType = "Settings";
            // return PartialView("_withdraw", model);
            return View("MyAccount", account);
        }

        [HttpGet]
        [ActionName("Deposit")]
        public async Task<IActionResult> Deposit(string country)
        {
            AccountViewModel model = new AccountViewModel();
            try
            {
                if (!string.IsNullOrEmpty(country))
                {
                    ViewData["Country"] = country;
                }
                else
                {
                    ViewData["Country"] = "GB";
                }

                var summary = await GetCustomerAccountSummary();
                
                //Get Initial Account data for customer
                MapMyAccountDetails(model, summary);

                //Get PaymentRef for MembershipCard plan
                var plan = _customerAccountService.GetMembershipPlan(summary.MembershipPlanId);
                var provider = plan.MembershipPlanPaymentProvider.FirstOrDefault(x => x.PaymentProviderId == (int)Enums.PaymentProvider.PayPal);
                if (provider != null)
                {
                    model.MembershipPlanId = provider.MembershipPlanId;
                    model.CustomerPaymentProviderId = Guid.NewGuid();
                    model.OneOffPaymentRef = provider.OneOffPaymentRef;
                    model.PayPalLink = _settings.Value.PayPalLink;
                }

                //Get withdraw request data
                if (model.MembershipPlanType == MembershipPlanTypeEnum.PartnerReward.ToString())
                {
                    var data = await _partnerRewardWithdrawalService.GetWithdrawalDataForRequest(model.MembershipCardId);
                    if (data != null)
                    {
                        model.Deposit = new WithdrawViewModel
                        {
                            RequestExists = data.RequestExists,
                            CustomerId = data.CustomerId,
                            BankDetailId = data.BankDetailId ?? 0,
                            PartnerRewardId = data.PartnerRewardId ?? 0,
                            Name = data.Name,
                            AccountNumber = data.AccountNumber,
                            SortCode = data.SortCode,
                            AvailableFund = data.AvailableFund ?? 0m
                        };
                    }
                }
                
                ViewBag.ViewType = "Account Boost";
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }

            return View("MyAccount", model);
        }

        [HttpGet]
        [ActionName("Dashboard")]
        public async Task<IActionResult> Dashboard(string country)
        {
            AccountViewModel model = new AccountViewModel();
            try
            {
                if (!string.IsNullOrEmpty(country))
                {
                    ViewData["Country"] = country;
                }
                else
                {
                    ViewData["Country"] = "GB";
                }

                //Get Initial Account data for customer
                var summary = await GetCustomerAccountSummary();
                MapMyAccountDetails(model, summary);

                // Get the reward partner (not the card provider this time)
                var partner = await _partnerService.GetByIdAsync(summary.RewardPartnerId);
                model.TamDashboard = new TamViewModel
                {
                    Username = summary.RewardKey,
                    Password = summary.RewardPassword,
                    Url = partner?.ManagementURL
                };
                
                
                ViewBag.ViewType = "TAM Dashboard";
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_dashboard", model.TamDashboard);

            return View("MyAccount", model);
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> Update(SettingsViewModel model)
        {
            try
            {
                if (model.CustomerId > 0)
                {
                    var contactDetail = new dto.ContactDetail()
                    {
                        EmailAddress = model.Email,
                        PostCode = model.Postcode,
                        Address1 = model.Address1,
                        Address2 = model.Address2,
                        Address3 = model.Address3,
                        District = model.District,
                        Town = model.Town
                    };

                    var customer = new dto.Customer()
                    {
                        Forename = model.Forename,
                        Surname = model.Surname,
                        NINumber = model.NationalInsuranceNumber,
                        Id = model.CustomerId,
                        ContactDetail = contactDetail,
                        MarketingNewsLetter = model.MarketingNewsLetter
                    };

                    _customerAccountService.UpdateCustomerSettings(customer);

                    // Clear customer summary cache, so it will be refetched with new data
                    var user = await _customerAccountService.GetUserAsync(HttpContext.User);
                    _cache.Remove(string.Format(Data.Constants.Keys.CustomerSummary, user.Id));
                }
                else
                {
                    return Json(
                        JsonResponse<string>.ErrorResponse(
                            "Customer not found."));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(
                    JsonResponse<string>.ErrorResponse(
                        "Error occurred while updating personal details. Please try again."));
            }

            return Json(JsonResponse<bool>.SuccessResponse(true));
        }

        [HttpPost]
        [ActionName("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(SettingsViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.CurrentPassword))
                {
                    var user = await _userService.GetUserAsync(HttpContext.User);
                    if (user != null)
                    {
                        var checkPassword = await _userService.CheckPasswordAsync(user, model.CurrentPassword);
                        if (checkPassword)
                        {
                            var passwordUpdated = await _userService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                            if (!passwordUpdated)
                                return Json(JsonResponse<string>.ErrorResponse("Changing password failed. Please try again."));
                        }
                        else
                        {
                            return Json(JsonResponse<string>.ErrorResponse("Current password is incorrect. Please try again."));
                        }
                    }
                    else
                    {
                        return Json(JsonResponse<string>.ErrorResponse("User not found."));
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Existing password is required."));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error updating new password. Please try again."));
            }
            return Json(JsonResponse<bool>.SuccessResponse(true));
        }

        [HttpPost]
        [ActionName("Investment")]
        public async Task<IActionResult> Investment(string customerPaymentId)
        {
            try
            {
                Logger.Debug("Investment method called on MyAccountController");

                // PLEASE TELL ME THIS WASN'T CREDITING CUSTOMER ACCOUNTS FROM A CLIENT SIDE JAVA SCRIPT FUNCTION 
                // FOR F**!S SAKE, 
                // Lets give the users a call to print their own money.  Genius.

                //var result = await _accountService.CreateInvestment(customerPaymentId, _settings.Value.AdminEmail);
                //bool success = false;
                //bool.TryParse(result, out success);
                //if(success)

                await Task.CompletedTask;

                return Json(JsonResponse<bool>.SuccessResponse(true));
                //return Json(JsonResponse<string>.ErrorResponse("Error creating investment."));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error creating investment."));
            }
        }


        [HttpPost]
        [ActionName("UpdateSecurity")]
        public async Task<IActionResult> UpdateSecurity(SettingsViewModel model)
        {
            try
            {
                var customerAccountDto = MapCustomerSecurityViewModelToDTO(model);
                if (model.CustomerId>0)
                {
                    var isExisted =  _customerSecurityQuestionService.Get(model.CustomerId);
                    if (isExisted!=null) {
                        if (isExisted.CustomerId > 0 && isExisted.SecurityQuestionId > 0)
                        {
                            await _customerSecurityQuestionService.Update(customerAccountDto);
                        }                        
                    }
                    else
                    {
                        await _customerSecurityQuestionService.Add(customerAccountDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error adding security. Please try again."));
            }
            return Json(JsonResponse<bool>.SuccessResponse(true));
        }

        #endregion

        #region Private Methods

        private Enums.TransactionLogSortOrder GetSortOrder(TransactionLogRequest req)
        {
            Enums.TransactionLogSortOrder sortOrder;

            switch (req.TransactionSortField)
            {
                case TransactionLogSortField.Date:
                    sortOrder = req.SortDirection == "asc" ? Enums.TransactionLogSortOrder.DateAsc : Enums.TransactionLogSortOrder.DateDesc;
                    break;

                case TransactionLogSortField.Merchant:
                    sortOrder = req.SortDirection == "asc" ? Enums.TransactionLogSortOrder.MerchantAsc : Enums.TransactionLogSortOrder.MerchantDesc;
                    break;

                case TransactionLogSortField.Value:
                    sortOrder = req.SortDirection == "asc" ? Enums.TransactionLogSortOrder.ValueAsc : Enums.TransactionLogSortOrder.ValueDesc;
                    break;

                case TransactionLogSortField.Status:
                    sortOrder = req.SortDirection == "asc" ? Enums.TransactionLogSortOrder.StatusAsc : Enums.TransactionLogSortOrder.StatusDesc;
                    break;

                case TransactionLogSortField.Invested:
                    sortOrder = req.SortDirection == "asc" ? Enums.TransactionLogSortOrder.InvestedAsc : Enums.TransactionLogSortOrder.InvestedDesc;
                    break;

                default:
                    sortOrder = Enums.TransactionLogSortOrder.DateAsc;
                    break;
            }

            return sortOrder;
        }

        private void MapToViewModel(TransactionLogList model, dto.PagedResult<dto.TransactionLog> data, string name)
        {
            if (data != null)
            {

                foreach (var result in data.Results)
                {
                    if (result.Status == Data.Constants.Status.UserPaid)
                    {
                        result.Status = "Paid";
                        //result.Merchant = name;
                        result.Merchant = result.Summary;
                        result.Value = result.PurchaseAmount;
                    }

                    if (result.Status == Data.Constants.Status.FileCreated ||
                        result.Status == Data.Constants.Status.Sent || result.Status == Data.Constants.Status.Confirmed)
                    {
                        result.Status = Data.Constants.Status.Processing;
                    }

                    model.Transactions.Add(new TransactionViewModel()
                    {
                        Date = result.Date,
                        Merchant = result.Merchant,
                        Value = result.Value,
                        Status = result.Status,
                        Invested = result.Invested,
                        Donated = result.Donated,
                    });
                    
                }

                model.CurrentPageNumber = data.CurrentPage;
                model.PagingViewModel.CurrentPage = data.CurrentPage;
                model.PagingViewModel.PageCount = data.PageCount;
                model.PagingViewModel.PageSize = data.PageSize;
                model.PagingViewModel.RowCount = data.RowCount;
            }
        }
        
        private void  MapMyAccountDetails(AccountViewModel model, dto.CustomerAccountSummary summary, int? page = null)
        {
            try
            {
                model.TamDashboard = null;
                model.Withdraw = null;
                
                if (summary != null)
                {
                    model.MembershipCardId = summary.MembershipCardId;
                    model.CustomerName = summary.CustomerName;
                    model.MembershipCardNumber = summary.CardNumber;
                    model.Pending = summary.Balances.PendingAmount;
                    model.Confirmed = summary.Balances.ConfirmedAmount;
                    model.Received = summary.Balances.ReceivedAmount;
                    model.Invested = summary.Balances.Invested;
                    model.Withdrawn = summary.Balances.Withdrawn;  
                    model.Balance = summary.Balances.CurrentValue;
                    model.ExpiryDate = string.Format(new DateFormat(), "{0}", summary.CardExpiryDate);
                    model.MembershipPlanType = summary.PlanType;
                    model.NiNumber = summary.NiNumber;
                    model.PartnerPassword = summary.RewardPassword;
                    model.DonatedAmount = summary.Balances.DonatedAmount;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private async Task<dto.CustomerAccountSummary> GetCustomerAccountSummary(string userId = null)
        {
            if (userId == null)
            {
                var user = await _customerAccountService.GetUserAsync(HttpContext.User);
                userId = user.Id;
            }

            // Get customer Account summary from Cache (this is set in that horrible global filter but we may as well use it)
            //var summary = _cache.Get<dto.CustomerAccountSummary>(string.Format(Data.Constants.Keys.CustomerSummary, userId));
            //if(summary == null)
            var summary = _customerAccountService.GetAccountSummary(userId);

            return summary;
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


        private dto.CustomerSecurityQuestion MapCustomerSecurityViewModelToDTO(SettingsViewModel model)
        {       
            var securityAnswer = new dto.CustomerSecurityQuestion()
            {
                SecurityQuestionId = model.QuestionId,
                Answer = model.Answer,
                CustomerId=model.CustomerId
                
            }; 
            return securityAnswer;
        }

        #endregion
    }
}
