using Castle.Components.DictionaryAdapter;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Helpers;
using ExclusiveCard.Website.Models;
using ExclusiveCard.Website.Models.Api;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.Website.API.Controller
{
    [Route("api/")]
    public class DealsController : BaseController
    {
        private readonly IOffersService _offerService;
        private readonly ICustomerService _customerService;
        private readonly IMembershipCardService _membershipCardService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IMerchantService _MerchantService;


        public DealsController(IOffersService offerService, ICustomerService customerService,
            IMembershipCardService membershipCardService, ICategoryService categoryService, IMerchantService merchantService)
        {
            _offerService = offerService;
            _customerService = customerService;
            _membershipCardService = membershipCardService;
            _categoryService = categoryService;
            _MerchantService = merchantService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpPost]
        [Route("Deals")]
        public async Task<IActionResult> SearchDeals([FromBody] DealsSearchRequest dealSearchRequest)
        {
            List<int> categoryIds = new List<int>();
            List<DealsSearchResponse> dealsSearchResponses = null;
            
            //if (!Helpers.UserHelper.ValidateUser(dealSearchRequest.UserToken, dealSearchRequest.UserName))
            //{
            //    return Unauthorized();
            //}

            var parentCategory = _categoryService.Get(dealSearchRequest.Category);
            if (parentCategory != null)
            {
                var categories = _categoryService.GetAll().Where(x =>   x.IsActive == true && (x.Id == parentCategory.Id
                                                                       || x.ParentId == parentCategory.Id));

                categoryIds = categories.Select(x => x.Id).ToList();
            }

            var searchCriteria = new DTOs.OfferSearchCriteria
            {
                KeyWord = dealSearchRequest.Keyword,
                CurrentPage = dealSearchRequest.PageNumber,
                PageSize = dealSearchRequest.PageSize,
                OfferId = dealSearchRequest.DealId,
                Categories = categoryIds
            };

            var resp = await _offerService.PagedSearch(searchCriteria);
            DTOs.PagedResult<DTOs.Public.OfferSummary> offerResult = resp;

            if (offerResult != null)
            {
                dealsSearchResponses = Map(offerResult.Results.ToList());
            }

            return Ok(dealsSearchResponses);
        }

        [HttpPost]
        [Route("Deal")]
        public async Task<IActionResult> GetDeal([FromBody] GetDealRequest request)
        {
            try
            {
                CommonHelper.Initialize();
                GetDealResponse response = new GetDealResponse();
                var userLoggedIn = ValidateUser(request.UserToken, request.UserName);
                bool hasMerchantCard = false;
                string cardNumber = string.Empty;
                if (userLoggedIn)
                {
                    DTOs.Customer customer = _customerService.GetCustomerByUserName(request.UserName);
                    if (customer != null)
                    {
                        List<DTOs.MembershipCard> cards = await _membershipCardService.GetAll(customer.Id);
                        if (cards != null)
                        {
                            hasMerchantCard = cards.Any(x => x.IsActive);
                            cardNumber = cards
                                .FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.ValidTo > DateTime.UtcNow)?
                                .CardNumber;
                        }
                    }
                }
                if (request.DealId == null)
                {
                    return null;
                }

                DTOs.Offer offer = await _offerService.Get(request.DealId.Value, true, true, true, false, false);
                if (offer != null)
                {
                    response = new GetDealResponse
                    {
                        Id = offer.Id,
                        Summary = offer.LongDescription,
                        Title = offer.Headline ?? string.Empty,
                        DealTypeId = offer.OfferTypeId,
                        Content = new Content
                        {
                            id = offer.Id,
                            ActionTypeId = 2,
                            IsDeleted = false,
                            Texts = new List<Text>(),
                            Images = new List<Image>()
                        }
                    };
                    if (!string.IsNullOrEmpty(request.UserToken) && !string.IsNullOrEmpty(offer.LinkUrl))
                    {
                        string redirectURL = Request.Scheme + "://" + Request.Host.Value + Url.Content("~/offers/Redirect");
                        response.Content.ActionData = string.IsNullOrEmpty(cardNumber)
                            ? $"{redirectURL}?offerId={offer.Id}&token={request.UserToken}"
                            : $"{redirectURL}?offerId={offer.Id}&membershipCard={cardNumber}&token={request.UserToken}";
                    }
                    if (offer.Merchant != null)
                    {

                        if (!string.IsNullOrEmpty(offer.Merchant.ContactName))
                        {
                            response.Content.Texts.Add(new Text
                            {
                                TextName = "CompanyName",
                                TextLabel = "",
                                TextValue = offer.Merchant.ContactName
                            });
                        }

                        if (!string.IsNullOrEmpty(offer.Merchant.LongDescription))
                        {
                            response.Content.Texts.Add(new Text
                            {
                                TextName = "CompanyDetails",
                                TextLabel = "",
                                TextValue = offer.Merchant.LongDescription
                            });
                        }

                        if (!string.IsNullOrEmpty(offer.Merchant.ShortDescription))
                        {
                            response.Content.Texts.Add(new Text
                            {
                                TextName = "AboveCarousel",
                                TextLabel = "",
                                TextValue = offer.Merchant.ShortDescription
                            });
                        }
                    }
                    if (!string.IsNullOrEmpty(offer.Terms) && offer.Terms != "<p><br></p>")
                    {
                        response.Content.Texts.Add(new Text
                        {
                            TextName = "Terms",
                            TextLabel = "Terms",
                            TextValue = offer.Terms
                        });
                    }
                    else if (!string.IsNullOrEmpty(offer.Merchant.Terms))
                    {
                        response.Content.Texts.Add(new Text
                        {
                            TextName = "Terms",
                            TextLabel = "Terms",
                            TextValue = offer.Merchant.Terms
                        });
                    }

                    if (!string.IsNullOrEmpty(offer.Exclusions))
                    {
                        response.Content.Texts.Add(new Text
                        {
                            TextName = "Exclusions",
                            TextLabel = "Exclusions",
                            TextValue = offer.Exclusions
                        });
                    }
                    
                    if (hasMerchantCard && !string.IsNullOrEmpty(offer.Instructions))
                    {
                        response.Content.Texts.Add(new Text
                        {
                            TextName = "Instructions",
                            TextLabel = "Instructions",
                            TextValue = offer.Instructions
                        });
                    }

                    if (hasMerchantCard && !string.IsNullOrEmpty(offer.OfferCode))
                    {
                        response.Content.Texts.Add(new Text
                        {
                            TextName = "OfferCode",
                            TextLabel = "Code",
                            TextValue = offer.OfferCode
                        });
                    }

                    //Try to get a list of merchant images...
                    if (offer?.Merchant != null && offer.Merchant.MerchantImages == null)
                    {
                        DTOs.Merchant merch = _MerchantService.Get(offer.Merchant.Id,includeImage:true);
                        offer.Merchant.MerchantImages = merch.MerchantImages;
                    }

                    //TODO:Image width and height to be retreived from appsettings
                    if (offer?.Merchant != null && offer.Merchant.MerchantImages != null &&
                        offer.Merchant.MerchantImages.Any())
                    {
                        response.Content.Images = new List<Image>();
                        List<DTOs.MerchantImage> images = offer.Merchant.MerchantImages
                            .Where(x => x.ImagePath.EndsWith("__2")).ToList();
                        for (int i = 0; i < images.Count; i++)
                        {
                            
                            var image = new Website.Models.Api.Image
                            {
                                Id = images[i].Id,
                                ImageName = "Logo",
                                ImagePath = $"{Request.Scheme + "://" + Request.Host.Value + Url.Content("~/")}api/Image/?path={images[i].ImagePath}",
                                TimeStamp = DateTime.UtcNow,
                                ImageWidth = CommonHelper.AppSettings["Images:MediumWidth"],
                                ImageHeight = CommonHelper.AppSettings["Images:MediumHeight"],
                                IsDeleted = false
                            };
                            image.ImageName = i == 0 ? "Logo" : null;
                            response.Content.Images.Add(image);
                        }
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex} {ex.StackTrace}");
                return BadRequest("There is an error on server.");
            }
        }

        [HttpGet]
        [Route("Image")]
        public async Task<IActionResult> GetImage(string path)
        {
            try
            {
                CommonHelper.Initialize();

                //string container = "exclusivecard";
                AzureStorageProvider azureProvider = new AzureStorageProvider();
                if (!string.IsNullOrEmpty(path))
                {
                    byte[] imgData = await azureProvider.GetImage(CommonHelper.AppSettings["BlobConnectionString"].ToString(), CommonHelper.AppSettings["ContainerName"].ToString(), path);
                    if (imgData != null)
                    {
                        MemoryStream ms = new MemoryStream(imgData);
                        HttpResponseMessage response =
                            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(ms) };
                        response.Content.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue("image/Jpeg");
                        return File(imgData, "image/Jpeg");
                    }
                    return null;
                }
                HttpResponseMessage errorresponse = new HttpResponseMessage(HttpStatusCode.NoContent);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex} {ex.StackTrace}");
                return null;
            }
        }

        #region Private Methods

        private List<DealsSearchResponse> Map(List<DTOs.Public.OfferSummary> offers)
        {
            CommonHelper.Initialize();
            List<DealsSearchResponse> dealsSearchResponses = new EditableList<DealsSearchResponse>();
            foreach (DTOs.Public.OfferSummary offerSummary in offers)
            {
                if (offerSummary != null)
                {
                    DealsSearchResponse dealsSearchResponse = new DealsSearchResponse
                    {
                        Summary = offerSummary.OfferLongDescription,
                        Title = offerSummary.OfferHeadline,
                        ThumbnailPath = $"{Request.Scheme + "://" + Request.Host.Value + Url.Content("~/")}api/Image/?path={offerSummary.MerchantLogoPath}",
                        Id = offerSummary.OfferId
                        //Online = offerSummary.
                    };
                    dealsSearchResponses.Add(dealsSearchResponse);
                }
            }
            return dealsSearchResponses;
        }

        private bool ValidateUser(string token, string userId)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            try
            {
                LoginToken loginToken = LoginToken.FromEncryptedString(token);
                if (!string.Equals(userId, loginToken.UserId, StringComparison.CurrentCultureIgnoreCase)
                    || loginToken.ExpiresTimestamp < DateTime.UtcNow.Ticks)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        #endregion
    }
}
