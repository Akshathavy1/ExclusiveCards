﻿using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Website.Helpers;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using dto = ExclusiveCard.Services.Models.DTOs;
using pub = ExclusiveCard.Services.Interfaces.Public;

namespace ExclusiveCard.Website.Controllers
{
    //TODO:  Investigate whether this is a cut and paste job of MyAccountController 
    // If confirmed, will need to be rewritten properly, to replace this file with the existing 
    //  MyAccountController and logic and insert a "mobile layout" file  in the views instead.  
    // To leave as is will be a maintenance nightmare.

    public class MobileController : BaseController
    {
        #region Private members and constructor

        //for offerhub
        private readonly IOffersService _offersService;

        //for myAccount
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


        public MobileController(IOffersService offersService,IBankDetailService bankDetailService,
            ICashbackPayoutService cashbackPayoutService,
            ICustomerBankDetailService customerBankDetailService,
            Services.Interfaces.Admin.IPartnerService partnerService,
            IPartnerRewardWithdrawalService partnerRewardWithdrawalService,
            IUserService userService,
            IOptions<TypedAppSettings> settings,
            pub.ICustomerService customerService,
            IContactDetailService contactDetailService,
            ICustomerAccountService customerAccountService,
            IMemoryCache cache)
        {
            _offersService = offersService;
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
        }

        #endregion

        #region public actions
//        [Authorize(Roles = "User,PartnerAPI")]
        [HttpGet]
        [ActionName("OfferHub")]
        public async Task<IActionResult> OfferHub(string country)
        {
            OfferHubMainViewModel model = new OfferHubMainViewModel();

            try
            {
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                //Get OfferListItems data for slider
                //For slider use offer hub as list name and take only 5 records.
                var data = await _offersService.GetOfferListDataModels(countryCode, null, 10,                    Data.Constants.Keys.OfferHubList);
                //Map to View model
                await MapToOfferHubData(model.OfferHubs, data);
                foreach (var offer in model.OfferHubs)
                {
                    offer.UseFeatureImage = true;
                }
                //Get OfferListItems data for top cashback offers
                //For Top cashback Offers use Best Cashback Offers as list name and take only 8 records.
                var cashbackOffers = await _offersService.GetOfferListDataModels(countryCode, null,
                    80, Data.Constants.Keys.BestCashback);
                //Map to view model
                await MapToOfferHubData(model.BestCashbackOffers, cashbackOffers);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }

            return View(model);
            
        }
        [HttpGet]
        [ActionName("LocalOffer")]
        public async Task<IActionResult> LocalOffer(string country)
        {
            LocalOfferMainViewModel model = new LocalOfferMainViewModel();

            try
            {
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                //Get OfferListItems data for slider
                //For slider use offer hub as list name and take only 5 records.
                var data = await _offersService.GetOfferListDataModels(countryCode, null, 10, Data.Constants.Keys.OfferHubList);
                //Map to View model
                await MapToLocalOfferData(model.LocalOffer, data);
                foreach (var offer in model.LocalOffer)
                {
                    offer.UseFeatureImage = true;
                }
                //Get OfferListItems data for top cashback offers
                //For Top cashback Offers use Best Cashback Offers as list name and take only 8 records.
                var cashbackOffers = await _offersService.GetOfferListDataModels(countryCode, null,
                    80, Data.Constants.Keys.BestCashback);
                //Map to view model
                await MapToLocalOfferData(model.BestCashbackOffers, cashbackOffers);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }

            return View(model);

        }

        [Authorize(Roles = "User,PartnerAPI")]
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

                //Get Initial Account data for customer
                var summary = await GetCustomerAccountSummary();
                MapMyAccountDetails(model, summary);
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



                    //Get Transaction Logs
                    int pageSize = 100;
                    int.TryParse(_settings.Value.ReportPageSize.ToString(), out pageSize);
                    Enums.TransactionLogSortOrder sortOrder = GetSortOrder(request);
                    var data = await _partnerRewardWithdrawalService.GetTransactionLog(user.Id, (int)model.Transactions.CurrentPageNumber, pageSize,                        sortOrder);

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

        [Authorize(Roles = "User,PartnerAPI")]
        [HttpGet]
        [ActionName("Withdraw")]
        public async Task<IActionResult> Withdraw(string country, int membershipCardId, string membershipPlanType)
        {
            AccountViewModel account = new AccountViewModel();
            WithdrawViewModel model = new WithdrawViewModel();
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
                if (membershipPlanType == MembershipPlanTypeEnum.PartnerReward.ToString())
                {
                    var data = await _partnerRewardWithdrawalService.GetWithdrawalDataForRequest(membershipCardId);
                    if (data != null)
                    {
                        model.RequestExists = data.RequestExists;
                        model.CustomerId = data.CustomerId;
                        model.BankDetailId = data.BankDetailId ?? 0;
                        model.PartnerRewardId = data.PartnerRewardId ?? 0;
                        model.Name = data.Name;
                        model.AccountNumber = data.AccountNumber;
                        model.SortCode = data.SortCode;
                        model.AvailableFund = data.AvailableFund ?? 0m;
                    }
                }
                else
                {
                    var data = await _cashbackPayoutService.GetCashoutDataForRequest(user.Id);
                    if (data != null)
                    {
                        model.RequestExists = data.RequestExists;
                        model.CustomerId = data.CustomerId;
                        model.BankDetailId = data.BankDetailId ?? 0;
                        model.PartnerRewardId = data.PartnerRewardId ?? 0;
                        model.Name = data.Name;
                        model.AccountNumber = data.AccountNumber;
                        model.SortCode = data.SortCode;
                        model.AvailableFund = data.AvailableFund ?? 0m;
                    }
                }

                if (!string.IsNullOrEmpty(error))
                {
                    return View("Error");
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(
                    JsonResponse<string>.ErrorResponse("An error occurred while preparing for withdrawal request"));
            }
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_withdraw", model);

            account.Withdraw = model;
            ViewBag.ViewType = "Withdraw Funds";
            // return PartialView("_withdraw", model);
            return View("MyAccount", account);
        }
        [Authorize(Roles = "User,PartnerAPI")]
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
                        District = customer.ContactDetail?.District
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

            account.Settings = model;
            ViewBag.ViewType = "Settings";
            // return PartialView("_withdraw", model);
            return View("MyAccount", account);
        }

        [Authorize(Roles = "User,PartnerAPI")]
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

        [Authorize(Roles = "User,PartnerAPI")]
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

        [Authorize(Roles = "User,PartnerAPI")]
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



        #endregion

        #region Private methods

        private async Task MapToOfferHubData(List<OfferHubViewModel> destination, List<OfferListDataModel> source)
        {
            try
            {
                if (destination == null)
                {
                    destination = new List<OfferHubViewModel>();
                }

                await Task.WhenAll(source?.Select(async offer =>
                {
                    destination.Add(new OfferHubViewModel
                    {
                        MerchantId = offer.MerchantId,
                        MerchantName = offer.MerchantName,
                        OfferId = offer.OfferId,
                        OfferText = offer.OfferText,
                        OfferShortDescription = offer.OfferShortDescription,
                        OfferLongDescription = offer.OfferLongDescription,
                        Logo = offer.Logo,
                        DisabledLogo = offer.DisabledLogo,
                        FeatureImage = offer.FeatureImage,
                        LargeImage = offer.LargeImage,
                        UseFeatureImage = offer.UseFeatureImage
                    });

                    await Task.CompletedTask;
                }));
            }
            catch //(Exception e)
            {
               // Logger.Error(e);
            }
        }
        private async Task MapToLocalOfferData(List<LocalOfferViewModel> destination, List<OfferListDataModel> source)
        {
            try
            {
                if (destination == null)
                {
                    destination = new List<LocalOfferViewModel>();
                }

                await Task.WhenAll(source?.Select(async offer =>
                {
                    destination.Add(new LocalOfferViewModel
                    {
                        MerchantId = offer.MerchantId,
                        MerchantName = offer.MerchantName,
                        OfferId = offer.OfferId,
                        OfferText = offer.OfferText,
                        OfferShortDescription = offer.OfferShortDescription,
                        OfferLongDescription = offer.OfferLongDescription,
                        Logo = offer.Logo,
                        DisabledLogo = offer.DisabledLogo,
                        FeatureImage = offer.FeatureImage,
                        LargeImage = offer.LargeImage,
                        UseFeatureImage = offer.UseFeatureImage
                    });

                    await Task.CompletedTask;
                }));
            }
            catch (Exception e)
            {
                // Logger.Error(e);
            }
        }

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

        private void MapToViewModel(TransactionLogList model,  PagedResult<TransactionLog> data, string name)
        {
            if (data != null)
            {
                foreach (var result in data.Results)
                {
                    if (result.Status == Data.Constants.Status.UserPaid)
                    {
                        result.Status = "Paid";
                        result.Merchant = name;
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
                        Invested = result.Invested
                    });
                }

                model.CurrentPageNumber = data.CurrentPage;
                model.PagingViewModel.CurrentPage = data.CurrentPage;
                model.PagingViewModel.PageCount = data.PageCount;
                model.PagingViewModel.PageSize = data.PageSize;
                model.PagingViewModel.RowCount = data.RowCount;
            }
        }

        private void MapMyAccountDetails(AccountViewModel model, CustomerAccountSummary summary, int? page = null)
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
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        
        private async Task<CustomerAccountSummary> GetCustomerAccountSummary(string userId = null)
        {
            if (userId == null)
            {
                var user = await _customerAccountService.GetUserAsync(HttpContext.User);
                userId = user.Id;
            }

            // Get customer Account summary from Cache (this is set in that horrible global filter but we may as well use it)
            var summary = _cache.Get<CustomerAccountSummary>(string.Format(Data.Constants.Keys.CustomerSummary, userId));
            if (summary == null)
                summary = _customerAccountService.GetAccountSummary(userId);

            return summary;
        }

       

        #endregion

    }
}