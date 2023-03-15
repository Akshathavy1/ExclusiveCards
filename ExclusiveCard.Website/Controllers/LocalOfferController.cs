using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Models;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using ExclusiveCard.Services.Public;

using dto=ExclusiveCard.Services.Models.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExclusiveCard.Website.Controllers
{
    public class LocalOfferController : BaseController
    {
        #region Private members and constructor

        private readonly IOffersService _offersService;
        private const string ThumbnailSuffix = "__1";
        private const string MediumSuffix = "__2";
        private const string LargeSuffix = "__3";
        private IWhiteLabelService _whiteLabelService;
        public LocalOfferController(IOffersService offersService,IWhiteLabelService whiteLabelService)
        {
            _offersService = offersService;
            _whiteLabelService = whiteLabelService;
        }

        #endregion

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(int id, string regionalName ,string country)
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
                //var white = _whiteLabelService.GetAll();
                var white = _whiteLabelService.GetRegionSites();
                foreach (var item in white)
                {
                    if (item.isRegional == true)
                    {
                        LocalOffer localOffer = new LocalOffer();
                        localOffer.id = item.Id;
                        localOffer.Name = item.Name;
                        model.whiteLableList.Add(localOffer);
                    }
                }

                if (id != 0 && model.whiteLableList!=null)
                {
                    //Site white label Id is a regional site
                    if(model.whiteLableList.Any(x => x.id == id))
                    {
                        model.WhiteLableName = model.whiteLableList.FirstOrDefault(x => x.id == id).Name;
                    }
                    else
                    {
                        //Find the default white label for the site
                        var thisSite = _whiteLabelService.GetSiteSettingsById(id);
                        model.WhiteLableName = model.whiteLableList.FirstOrDefault(x => x.id == thisSite.DefaultRegion).Name;
                        id = thisSite.DefaultRegion;

                        //model.WhiteLableName = model.whiteLableList.FirstOrDefault().Name;
                        //id = model.whiteLableList.FirstOrDefault().id;
                    }                    
                }

                if(model.whiteLableList != null)
                {
                    model.whiteLableList = model.whiteLableList.OrderBy(x => x.Name).ToList();
                }                

                var data = await _offersService.GetLocalOfferListDataModels(countryCode, id);
                //Map to View model
                await MapToLocalOfferData(model.LocalOffer, data.Where(x => x.DisplayType == ((int)DisplayType.featured)).ToList());
                foreach (var offer in model.LocalOffer)
                {
                    offer.UseFeatureImage = true;
                }

                //Map to view model
                await MapToLocalOfferData(model.BestCashbackOffers, data.Where(x => x.DisplayType == ((int)DisplayType.mainbody)).ToList());
                //Get Parent Categories
                model.Categories = _offersService.GetParentCategories();
                model.WhiteLabel = white.ToList();

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }

            return View(model);
        }

        #region Private methods

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
                        UseFeatureImage = offer.UseFeatureImage,
                        OfferTypeId=offer.OfferTypeId

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
