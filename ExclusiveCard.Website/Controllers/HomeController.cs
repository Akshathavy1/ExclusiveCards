using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Helpers;
using Microsoft.AspNetCore.Mvc;
using ExclusiveCard.Website.Models;
using dto = ExclusiveCard.Services.Models.DTOs.Public;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using ExclusiveCard.Data.Constants;
using Microsoft.AspNetCore.Hosting;

namespace ExclusiveCard.Website.Controllers
{
    public class HomeController : BaseController
    {
        #region Private Members

        private readonly IOffersService _offersService;
        private readonly SignInManager<ExclusiveCard.Data.Models.ExclusiveUser> _signInManager;
        private readonly IUserService _userService;
        private IHostingEnvironment _environment;

        #endregion

        public HomeController(IOffersService offersService, SignInManager<ExclusiveCard.Data.Models.ExclusiveUser> signInManager, IUserService userService, IHostingEnvironment environment)
        {
            _offersService = offersService;
            _signInManager = signInManager;
            _userService = userService;
            _environment = environment;
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string country)
        {
            try
            {
                OfferHubMainViewModel model = new OfferHubMainViewModel();

                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                ViewBag.Country = countryCode;

                //Pick up the images for the marquee
                var path = System.IO.Path.Combine(_environment.WebRootPath, @"images\PlaceHolder-Merchants");
                ViewBag.Images = System.IO.Directory.EnumerateFiles(path, "*.png").Union(System.IO.Directory.EnumerateFiles(path, "*.jpg"))
                                          .Select(fn => "/images/PlaceHolder-Merchants/" + System.IO.Path.GetFileName(fn));

                var homepageOffers = await _offersService.GetOfferListDataModels(countryCode, null,80, Data.Constants.OfferLists.HomepageOffers);

                await MapToOfferHubData(model.HomepageOffers, homepageOffers);

                return View("Index");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Investment")]
        public IActionResult Investment(string country)
        {
            try
            {
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                ViewBag.Country = countryCode;
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("GetForCountry")]
        public async Task<IActionResult> GetForCountry(string country)
        {
            string countryCode = "GB";
            if (!string.IsNullOrEmpty(country))
            {
                countryCode = country;
                ViewData["Country"] = country;
            }
            OffersViewModel model = new OffersViewModel();
            try
            {               
                await OffersHelper.GetAndMapOffers(countryCode, model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
            return View("Index", model);
        }

        [HttpGet]
        [ActionName("GetByListName")]
        public async Task<IActionResult> GetByListName(string country, string listName)
        {
            OffersViewModel model = new OffersViewModel();
            try
            {
                string countryCode = "GB";
                if (!string.IsNullOrEmpty(country))
                {
                    countryCode = country;
                    ViewData["Country"] = country;
                }
                List<dto.OfferSummary> offerSummary = await _offersService.GetOffersByListName(listName, countryCode);
                switch (listName)
                {
                    case Keys.Keys.CashBackOfferList:
                        return PartialView("_offerBrief", offerSummary);
                    case Keys.Keys.VoucherOfferList:
                        return PartialView("_offerBrief", offerSummary);
                    case Keys.Keys.SalesOfferList:
                        return PartialView("_offerBrief", offerSummary);
                    case Keys.Keys.HighStreetOfferList:
                        return PartialView("_highStreetBrief", OffersHelper.GetHighStreetDeals(offerSummary, null));
                    case Keys.Keys.EndingSoonList:
                        return PartialView("_dailyOffer", offerSummary);
                    case Keys.Keys.DailyOfferList:
                        return PartialView("_dailyOffer", offerSummary);
                    case Keys.Keys.RestaurantOfferList:
                        return PartialView("_highStreetBrief", OffersHelper.GetHighStreetDeals(offerSummary, null));
                }                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
            return View("Index", model);
        }

        private string GetCountryCode()
        {
            return HttpContext.Request.Headers["Accept-Language"].ToString()?.Split(",")[0]?.Substring(3);
        }

        private async Task MapToOfferHubData(List<OfferHubViewModel> destination, List<Services.Models.DTOs.OfferListDataModel> source)
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
                    ViewBag.OfferList = destination;
                    await Task.CompletedTask;
                }));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        private async Task MapToLocalOfferData(List<LocalOfferViewModel> destination, List<Services.Models.DTOs.OfferListDataModel> source)
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
                    ViewBag.OfferList = destination;
                    await Task.CompletedTask;
                }));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
