using AutoMapper;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using entities = ExclusiveCard.Data.Models;
using IPublic = ExclusiveCard.Services.Interfaces.Public;


namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class CustomerController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;
        private readonly IStatusServices _statusService;
        private readonly IUserService _userService;
        private readonly IMembershipCardService _membershipCardService;
        private readonly IOLD_MembershipPlanService _membershipPlanService;
        private readonly IOLD_MembershipRegistrationCodeService _membershipRegistrationCodeService;
        private readonly IPublic.IMembershipCardService _pMembershipCardService;
        private readonly IOptions<TypedAppSettings> _settings;
        
        public CustomerController(IMapper mapper,
            ICustomerService customerService,
            IStatusServices statusService,
            IUserService userService,
            IMembershipCardService membershipCardService,
            IOLD_MembershipPlanService membershipPlanService,
            IOLD_MembershipRegistrationCodeService membershipRegistrationCodeService,
            IPublic.IMembershipCardService pMembershipCardService,
            IOptions<TypedAppSettings> settings)
        {
            _mapper = mapper;
            _customerService = customerService;
            _statusService = statusService;
            _userService = userService;
            _membershipCardService = membershipCardService;
            _membershipPlanService = membershipPlanService;
            _membershipRegistrationCodeService = membershipRegistrationCodeService;
            _pMembershipCardService = pMembershipCardService;
            _settings = settings;
        }
        
        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                CustomerSearchViewModel customerSearchView = new CustomerSearchViewModel();
                string customerSearchjson = HttpContext?.Session?.GetString(Keys.Keys.SessionCustomerSearch);
                CustomerSearchViewModel model = null;
                if (!string.IsNullOrEmpty(customerSearchjson))
                {
                    model =
                        JsonConvert.DeserializeObject<CustomerSearchViewModel>(customerSearchjson) ??
                        null;
                    HttpContext?.Session?.SetString(Keys.Keys.SessionCustomerSearch, string.Empty);
                }

                await MapToCustomerSearchViewModel(customerSearchView, model);

                return View("Search", customerSearchView);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Edit")]
        public  async Task<IActionResult> Edit(int id, string type = "Edit")
        {
            try
            {
                CustomerDetailsViewModel customerModel = new CustomerDetailsViewModel();

                if (id > 0)
                {
                    DTOs.Customer customer = _customerService.GetDetails(id);

                    if (customer != null && !string.IsNullOrEmpty(customer.AspNetUserId))
                    {
                        //List<DTOs.Status> status = await _statusService.GetAll(StatusType.MembershipCard);
                        entities.ExclusiveUser user = await _userService.FindByIdAsync(customer.AspNetUserId);
                        if (user != null)
                        {
                            customerModel = new CustomerDetailsViewModel
                            {
                                Id = customer.Id,
                                Username = user.UserName,
                                Forename = customer.Forename,
                                Surname = customer.Surname,
                                Dob = customer.DateOfBirth,
                                Dateadd = customer.DateAdded,
                                MarketingNewsLetter = customer.MarketingNewsLetter,
                                MarketingThirdParty = customer.MarketingThirdParty,
                                Address1 = customer.ContactDetail.Address1,
                                Address2 = customer.ContactDetail.Address2,
                                Address3 = customer.ContactDetail.Address3,
                                Town = customer.ContactDetail.Town,
                                District = customer.ContactDetail.District,
                                Phone = customer.ContactDetail.MobilePhone,
                                EmailAddress = customer.ContactDetail.EmailAddress,
                                PostCode = customer.ContactDetail.PostCode,
                                CountryCode = customer.ContactDetail.CountryCode,
                                CountryItems = GetCountryList()
                            };

                            if (customer.MembershipCards != null && customer.MembershipCards.Count > 0)
                            {
                                customerModel.ExpiredCardExists =
                                    customer.MembershipCards.Any(
                                        x => x.StatusId == (int) MembershipCardStatus.Expired || x.StatusId == (int) MembershipCardStatus.Cancelled);//x => x.StatusId == status.FirstOrDefault(y => y.Name == Status.Expired).Id || x.StatusId == status.FirstOrDefault(y => y.Name == Status.Cancelled).Id);

                                List<DTOs.MembershipCard> cards = customer.MembershipCards.Where(x =>
                                    x.StatusId == (int) MembershipCardStatus.Pending ||
                                    x.StatusId == (int) MembershipCardStatus.Active).ToList(); //status.FirstOrDefault(y => y.Name == Status.Pending).Id || x.StatusId == status.FirstOrDefault(y => y.Name == Status.Active).Id)?.ToList();
                                if (cards != null && cards.Any())
                                {
                                    customerModel.MultipleCardExists = cards.ToList().Count > 1;
                                }
                            }
                        }
                        //Membership Cards for the customer
                        customerModel.MembershipCardList = await BindToGetAllCards(id);

                    }

                    ViewBag.Name = type == "Edit" ? TypeOfRequest.Edit.ToString() : TypeOfRequest.View.ToString();
                }
                return View("Edit", customerModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Cards")]
        public async Task<IActionResult> Cards()
        {
            List<CustomerDetailsMembershipCardsViewModel> model = new List<CustomerDetailsMembershipCardsViewModel>();
            
            List<DTOs.MembershipCard> membershipCards = await _membershipCardService.GetCardsToSendOutAsync();
            if (membershipCards != null && membershipCards.Count > 0)
            {
                foreach (DTOs.MembershipCard card in membershipCards)
                {
                    DTOs.MembershipPlan plan = await _membershipPlanService.Get(card.MembershipPlanId);
                    entities.ExclusiveUser user = await _userService.FindByIdAsync(card.Customer?.AspNetUserId);
                    CustomerDetailsMembershipCardsViewModel viewModel = new CustomerDetailsMembershipCardsViewModel
                    {
                        Id = card.CustomerId.Value,//Binding customer Id
                        Username = user?.UserName,
                        DateIssued = card.DateIssued,
                        CardNumber = card.CardNumber,
                        ExpiryDate = card.ValidTo,
                        CardRequest = card.PhysicalCardRequested,
                        PlanName = plan?.Description,
                        CardRequestedDate = card.ValidFrom
                    };
                    if (card.PhysicalCardStatusId.HasValue)
                    {
                        viewModel.PhysicalCardStatus = Enum.GetName(typeof(PhysicalCardStatus), card.PhysicalCardStatusId.Value);// status.FirstOrDefault(x => x.Id == card.PhysicalCardStatusId.Value)?.Name;
                    }


                    viewModel.CardStatusList = Enum.GetValues(typeof(PhysicalCardStatus)).Cast<PhysicalCardStatus>().Select(v => new SelectListItem
                    {
                        Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                        Value = ((int)v).ToString()
                    }).ToList();
                    
                    model.Add(viewModel);
                }
            }
            return View("SendOutCards", model);
        }

        [HttpGet]
        [ActionName("GetCards")]
        public async Task<IActionResult> GetCards(int customerId, bool onlyValidCards)
        {
            try
            {
                List<CustomerDetailsMembershipCardsViewModel> model = await BindToGetAllCards(customerId, onlyValidCards);
                var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    return PartialView("_membershipCardList", model);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(
                    JsonResponse<string>.ErrorResponse("Some error occurred while trying to get membership cards."));
            }
            return View("Error");
        }

        [HttpPost]
        [ActionName("Search")]
        public async Task<IActionResult> Search(CustomerSearchViewModel searchCriteria)
        {
            try
            {
                HttpContext?.Session?.SetString(Keys.Keys.SessionCustomerSearch, string.Empty);
                CustomerSearchViewModel customerSearchView = new CustomerSearchViewModel();
                await MapToCustomerSearchViewModel(customerSearchView, searchCriteria);
                //var customerSearch = JsonConvert.SerializeObject(customerSearchView);
                //HttpContext?.Session?.SetString(Keys.Keys.SessionCustomerSearch, customerSearch);
                return PartialView("_customerListPartial", customerSearchView);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save(CustomerDetailsViewModel customer)
        {
            try
            {
                DTOs.Customer response = _customerService.GetDetails(customer.Id);
                entities.ExclusiveUser user = await _userService.FindByIdAsync(response.AspNetUserId);

                if (!string.Equals(customer.Username, user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    var userExist = await _userService.FindByNameAsync(customer.Username);
                    if (userExist != null)
                    {
                        return Json(JsonResponse<string>.ErrorResponse(
                            "Provided username already exists for other user. Please try different username."));
                    }
                }

                Services.Models.DTOs.Customer request = MapToReq(response);

                request.Id = customer.Id;
                request.AspNetUserId = response.AspNetUserId;
                request.Forename = customer.Forename;
                request.Surname = customer.Surname;
                request.DateOfBirth = customer.Dob;
                request.DateAdded = customer.Dateadd.Value;
                request.MarketingNewsLetter = customer.MarketingNewsLetter;
                request.MarketingThirdParty = customer.MarketingThirdParty;
                

                //if username differs, update the username
                if (!string.Equals(customer.Username, user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    var user1 = await _userService.FindByIdAsync(response.AspNetUserId);
                    user1.UserName = customer.Username;
                    user1.Email = customer.Username;
                    if (user1.SecurityStamp == null)
                    {
                        user1.SecurityStamp = GenerateReference(32);
                    }
                    var suc = await _userService.UpdateUserAsync(user1);
                    customer.EmailAddress = customer.Username;
                }

                if (request.Id != 0)
                {
                    if (!string.IsNullOrEmpty(customer.Address1) || !string.IsNullOrEmpty(customer.Address2) ||
                        !string.IsNullOrEmpty(customer.Address3) || !string.IsNullOrEmpty(customer.Phone) ||
                        !string.IsNullOrEmpty(customer.EmailAddress) || !string.IsNullOrEmpty(customer.Town) ||
                        !string.IsNullOrEmpty(customer.District) || !string.IsNullOrEmpty(customer.Phone))
                    {
                        if (request.ContactDetail == null)
                        {

                            request.ContactDetail = new DTOs.ContactDetail
                            {
                                Address1 = customer.Address1,
                                Address2 = customer.Address2,
                                Address3 = customer.Address3,
                                Town = customer.Town,
                                District = customer.District,
                                PostCode = customer.PostCode,
                                CountryCode = customer.CountryCode,
                                MobilePhone = customer.Phone,
                                EmailAddress = customer.EmailAddress
                            };
                        }
                        else
                        {
                            request.ContactDetail.Address1 = customer.Address1;
                            request.ContactDetail.Address2 = customer.Address2;
                            request.ContactDetail.Address3 = customer.Address3;
                            request.ContactDetail.Town = customer.Town;
                            request.ContactDetail.District = customer.District;
                            request.ContactDetail.PostCode = customer.PostCode;
                            request.ContactDetail.CountryCode = customer.CountryCode;
                            request.ContactDetail.MobilePhone = customer.Phone;
                            request.ContactDetail.EmailAddress = customer.EmailAddress;
                        }
                    }
                    else
                    {
                        //customer.ContactDetailId = null;
                    }

                    response = await _customerService.Update(request);
                }
                else
                {

                }
                return Json(JsonResponse<int>.SuccessResponse(response.Id));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error saving customer details"));
            }
        }

        [HttpPost]
        [ActionName("UpdatePhysicalStatus")]
        public async Task<IActionResult> UpdatePhysicalStatus(int cardId, int physicalStatus)
        {
            try
            {
                if (cardId > 0 && physicalStatus > 0)
                {
                    DTOs.MembershipCard membershipCard = await _pMembershipCardService.Get(cardId);
                    if (membershipCard != null)
                    {
                        membershipCard.PhysicalCardStatusId = physicalStatus;
                        await _pMembershipCardService.Update(membershipCard);
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Could not find membership card or physical card status to update."));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while updating physical card status"));
            }
            return Json(JsonResponse<bool>.SuccessResponse(true));
        }

        [HttpPost]
        [ActionName("EnableDisableCard")]
        public async Task<IActionResult> EnableDisableCard(int customerId, int cardId, bool activate, bool validCard)
        {
            try
            {
                if (cardId > 0)
                {
                    int? statusId = activate
                        ? (int) MembershipCardStatus.Active //status.FirstOrDefault(x => x.Name == Status.Active)?.Id
                        : (int) MembershipCardStatus.Cancelled; //status.FirstOrDefault(x => x.Name == Status.Cancelled)?.Id;

                    DTOs.MembershipCard membershipCard = await _pMembershipCardService.Get(cardId);
                    if (membershipCard != null)
                    {
                        membershipCard.StatusId = statusId.Value;
                        await _pMembershipCardService.Update(membershipCard);
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Could not find membership card to enable/disable."));
                }

                List<CustomerDetailsMembershipCardsViewModel> model = await BindToGetAllCards(customerId, validCard);
                var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    return PartialView("_membershipCardList", model);
                }
                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while enabling/disabling card."));
            }
        }

        #region Private Methods

        public async Task MapToCustomerSearchViewModel(CustomerSearchViewModel customerSearchView,
            CustomerSearchViewModel model)
        {
            customerSearchView.CustomStatusList = Enum.GetValues(typeof(MembershipCardStatus)).Cast<MembershipCardStatus>().Select(v => new SelectListItem
            {
                Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                Value = ((int)v).ToString()
            }).ToList();

            DTOs.PagedResult<DTOs.CustomerSummary> paging = new DTOs.PagedResult<DTOs.CustomerSummary>();
            if (model != null)
            {
                paging = _customerService.GetPagedSearch(model.Username,
                    model.Forename, model.Surname,
                    model.CardNumber, model.Postcode, model.CardStatus, model.RegistrationCode, model.Dob,
                    model.CardDateIssued, model.PageNumber, Convert.ToInt32(_settings.Value.PageSize));
                customerSearchView.Username = model.Username;
                customerSearchView.Forename = model.Forename;
                customerSearchView.Surname = model.Surname;
                customerSearchView.CardNumber = model.CardNumber;
                customerSearchView.Postcode = model.Postcode;
                customerSearchView.CardStatus = model.CardStatus;
                customerSearchView.RegistrationCode = model.RegistrationCode;
                customerSearchView.Dob = model.Dob;
                customerSearchView.CardDateIssued = model.CardDateIssued;
                customerSearchView.PageNumber = model.PageNumber;
            }
            else
            {
                paging = _customerService.GetAllPagedSearch(1, Convert.ToInt32(_settings.Value.PageSize));
                customerSearchView.PageNumber = 1;
            }
            customerSearchView.ListofCustomerSummary = paging.Results.ToList();
            customerSearchView.PagingModel.CurrentPage = paging.CurrentPage;
            customerSearchView.PagingModel.PageCount = paging.PageCount;
            customerSearchView.PagingModel.PageSize = paging.PageSize;
            customerSearchView.PagingModel.RowCount = paging.RowCount;

            await Task.CompletedTask;
        }

        // TODO:  Rewrite CustomerController.BindtoGetAllCards.
        private async Task<List<CustomerDetailsMembershipCardsViewModel>> BindToGetAllCards(int customerId, bool onlyValidCards = true)
        {
            // Is this for real?  
            // Get all registation codes just to return a view model with the membership plan including their code
            // Replace this tripe with a single service all that returns a simple list of customer membership cards
            try
            {
                List<CustomerDetailsMembershipCardsViewModel> models =
                    new List<CustomerDetailsMembershipCardsViewModel>();
                List<DTOs.MembershipRegistrationCode> registrationCodes =
                    await _membershipRegistrationCodeService.GetAllAsync();
                List<DTOs.MembershipCard> membershipCards = await
                    _membershipCardService.GetMembershipCardsForCustomerAsync(customerId, onlyValidCards);

                foreach (DTOs.MembershipCard card in membershipCards)
                {
                    DTOs.MembershipPlan membershipPlan = await _membershipPlanService.Get(card.MembershipPlanId);
                    CustomerDetailsMembershipCardsViewModel model = new CustomerDetailsMembershipCardsViewModel
                    {
                        Id = card.Id,
                        PlanName = membershipPlan?.Description,
                        CardNumber = card.CardNumber,
                        DateIssued = card.DateIssued,
                        ExpiryDate = card.ValidTo,
                        Status = Enum.GetName(typeof(MembershipCardStatus), card.StatusId),//status?.FirstOrDefault(x => x.Id == card.StatusId)?.Name,
                        RegistrationCode = registrationCodes?.FirstOrDefault(x => x.Id == card.RegistrationCode)
                            ?.RegistartionCode,
                        CardRequest = card.PhysicalCardRequested
                    };
                    if (card.PhysicalCardStatusId.HasValue)
                    {
                        model.CardStatus = card.PhysicalCardStatusId.Value;
                    }

                    model.CardStatusList = Enum.GetValues(typeof(PhysicalCardStatus)).Cast<PhysicalCardStatus>().Select(v => new SelectListItem
                    {
                        Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                        Value = ((int)v).ToString()
                    }).ToList();
                    
                    models.Add(model);
                }
                return models;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
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

        private Services.Models.DTOs.Customer MapToReq(DTOs.Customer model)
        {
            if (model == null)
                return null;

            var req = new Services.Models.DTOs.Customer
            {
                Id = model.Id,
                AspNetUserId = model.AspNetUserId,
                ContactDetailId = model.ContactDetailId,
                Title = model.Title,
                Forename = model.Forename,
                Surname = model.Surname,
                DateOfBirth = model.DateOfBirth,
                IsActive = model.IsActive,
                IsDeleted = model.IsDeleted,
                DateAdded = model.DateAdded,
                MarketingNewsLetter = model.MarketingNewsLetter,
                MarketingThirdParty = model.MarketingThirdParty,
                NINumber = model.NINumber,
                ContactDetail = new DTOs.ContactDetail()
            };

            if (model.ContactDetail != null)
            {
                req.ContactDetail = new DTOs.ContactDetail
                {
                    Id = model.ContactDetail.Id,
                    Address1 = model.ContactDetail.Address1,
                    Address2 = model.ContactDetail.Address2,
                    Address3 = model.ContactDetail.Address3,
                    Town = model.ContactDetail.Town,
                    District = model.ContactDetail.District,
                    PostCode = model.ContactDetail.PostCode,
                    CountryCode = model.ContactDetail.CountryCode,
                    Latitude = model.ContactDetail.Latitude,
                    Longitude = model.ContactDetail.Longitude,
                    LandlinePhone = model.ContactDetail.LandlinePhone,
                    MobilePhone = model.ContactDetail.MobilePhone,
                    EmailAddress = model.ContactDetail.EmailAddress,
                    IsDeleted = model.ContactDetail.IsDeleted
                };
            }

            return req;
        }

        #endregion
    }
}