using ExclusiveCard.Services.Interfaces.Public;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Website.Controllers
{
    public class LifeStyleHubController : BaseController
    {
        #region Private members and constructor

        private readonly IOffersService _offersService;
        private const string ThumbnailSuffix = "__1";
        private const string MediumSuffix = "__2";
        private const string LargeSuffix = "__3";

        public LifeStyleHubController(IOffersService offersService)
        {
            _offersService = offersService;
        }

        #endregion

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string country)
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
                var data = await _offersService.GetOfferListDataModels(countryCode, null, 10,
                    Data.Constants.OfferLists.LifeStyleHub);
                //Map to View model
                await MapToOfferHubData(model.OfferHubs, data);
                foreach (var offer in model.OfferHubs)
                {
                    offer.UseFeatureImage = true;
                }
                //Get OfferListItems data for top cashback offers
                //For Top cashback Offers use Best Cashback Offers as list name and take only 8 records.
                var cashbackOffers = await _offersService.GetOfferListDataModels(countryCode, null,
                    80, Data.Constants.OfferLists.LifeStyleHubList);
                //Map to view model
                await MapToOfferHubData(model.BestCashbackOffers, cashbackOffers);
                //Get Parent Categories
                model.Categories = _offersService.GetParentCategories();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }

            return View(model);
        }

        #region Private methods

        private async Task MapToOfferHubData(List<OfferHubViewModel> destination, List<DTOs.OfferListDataModel> source)
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
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private async Task MapToLocalOfferData(List<LocalOfferViewModel> destination, List<DTOs.OfferListDataModel> source)
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
                Logger.Error(e);
            }
        }

        #endregion
    }
}