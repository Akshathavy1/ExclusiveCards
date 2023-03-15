using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class MaintainOfferListsModel
    {
        [DisplayName("List Name:")]
        public int OfferListItemId { get; set; }
        public List<SelectListItem> ListofOfferListItems { get; set; }


        [DisplayName("Country:")]
        public string CountryCode { get; set; }
        public List<SelectListItem> ListofCountries { get; set; }

        [DisplayName("Merchant:")]
        public int? MerchantId { get; set; }
        public List<SelectListItem> ListofMerchants { get; set; }

        [DisplayName("Affiliate:")]
        public int? AffiliateId { get; set; }
        public List<SelectListItem> ListofAffiliate { get; set; }

        [DisplayName("Keyword:")]
        public string Keyword { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Valid From:")]
        [ValidDateCheck(ErrorMessage = "Date must be after 01 Jan 1900 and before or equal to current date")]
        public DateTime? ValidFrom { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("To:")]
        [ValidDateCheck(ErrorMessage = "Date must be after 01 Jan 1900 and before or equal to current date")]
        public DateTime? ValidTo { get; set; }

        [DisplayName("Type:")]
        public int? OfferType { get; set; }
        public List<SelectListItem> ListofOfferType { get; set; }
        [DisplayName("Status:")]
        public int? OfferStatus { get; set; }
        public List<SelectListItem> ListofStatus { get; set; }

        public OfferListMaintainViewModel ListtOfOffers { get; set; }

        public OfferListMaintainViewModel ListtOfOffersListitems { get; set; }

        public int offerPage { get; set; }
        public int OfferListPage { get; set; }
        public bool Processing { get; set; }

        public MaintainOfferListsModel()
        {
            ListofOfferListItems = new List<SelectListItem>();
            ListofCountries = new List<SelectListItem>();
            ListofMerchants = new List<SelectListItem>();
            ListofAffiliate = new List<SelectListItem>();
            ListofOfferType = new List<SelectListItem>();
            ListofStatus = new List<SelectListItem>();
            ListtOfOffers = new OfferListMaintainViewModel();
            ListtOfOffersListitems = new OfferListMaintainViewModel();
        }
    }
}
