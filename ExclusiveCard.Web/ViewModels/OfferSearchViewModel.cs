using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class OfferSearchViewModel
    {
        [DisplayName("Merchant:")]
        public int? MerchantId { get; set; }
        public List<SelectListItem> ListofMerchants { get; set; }
        [DisplayName("Affiliate:")]
        public int? AffiliateId { get; set; }
        public List<SelectListItem> ListofAffiliate { get; set; }
        [DisplayName("Keyword:")]
        public string Keyword { get; set; }
        [DataType(DataType.Date),DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Valid From:")]
        public DateTime? ValidFrom { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("To:")]
        public DateTime? ValidTo { get; set; }
        [DisplayName("Type:")]
        public int? OfferType { get; set; }
        public List<SelectListItem> ListofOfferType { get; set; }
        [DisplayName("Status:")]
        public int? OfferStatus { get; set; }
        public List<SelectListItem> ListofStatus { get; set; }
        public string ShortDescription { get; set; }

        public OfferListViewModel OffersList { get; set; }

        public int PageNumber { get; set; }

        public OfferSearchViewModel()
        {
            ListofMerchants = new List<SelectListItem>();
            ListofAffiliate = new List<SelectListItem>();
            ListofOfferType = new List<SelectListItem>();
            ListofStatus = new List<SelectListItem>();
        }
    }
}
