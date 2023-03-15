using ExclusiveCard.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class OfferViewModel
    {
        [DisplayName("Id:")]
        public int Id { get; set; }

        [DisplayName("DateAdded:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }

        public int? RefMerchantId { get; set; }

        [Required(ErrorMessage = "Merchant is required")]
        [DisplayName("*Merchant:")]
        public int MerchantId { get; set; }

        public List<SelectListItem> ListofMerchants { get; set; }

        [DisplayName("3rd Party Site:")]
        public int SSOThirdpartySiteId { get; set; } = 0;

        public List<SelectListItem> ListofSSOThirdPartySites { get; set; }

        [DisplayName("Product Code:")]
        public string ProductCode { get; set; }

        //[Required(ErrorMessage = "Affiliate is required")]
        [DisplayName("Affiliate:")]
        public int? AffiliateId { get; set; }

        public List<SelectListItem> ListofAffiliate { get; set; }

        [Required(ErrorMessage = "OfferType is required")]
        [DisplayName("*OfferType:")]
        public int OfferTypeId { get; set; }

        public List<SelectListItem> ListofOfferType { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [DisplayName("*Status:")]
        public int StatusId { get; set; }

        public List<SelectListItem> ListofStatus { get; set; }

        [DisplayName("Search Ranking:")]
        public short SearchRanking { get; set; }

        public List<SelectListItem> ListofRanking { get; set; }

        [Required(ErrorMessage = "Reoccuring is required")]
        [DisplayName("*Reoccuring:")]
        public bool Reoccuring { get; set; }

        [Required(ErrorMessage = "ValidIndefintely is required")]
        [DisplayName("*Valid Indefintely:")]
        public bool ValidIndefintely { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [ValidFromDateCheck(ErrorMessage = "Date must be after 01 Jan 1900 and before or equal to current date")]
        [DisplayName("Valid From:")]
        public DateTime? ValidFrom { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [ValidToDateCheck(ErrorMessage = "Date must be after 01 Jan 1900 and before or equal to next year current date")]
        [DisplayName("Valid To:")]
        public DateTime? ValidTo { get; set; }

        [MaxLength(30)]
        [DisplayName("Headline:")]
        public string Headline { get; set; }

        [MaxLength(128, ErrorMessage = "Short Decription can have maximium of 128 Characters")]
        //[Required(ErrorMessage = "Short Description is required")]
        [DisplayName("Short Description:")]
        public string ShortDescription { get; set; }

        [MaxLength(512, ErrorMessage = "Long Decription can have maximium of 512 Characters")]
        //[Required(ErrorMessage = "Long Description is required")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Long Description:")]
        public string LongDescription { get; set; }

        [MaxLength(512)]
        //[Required(ErrorMessage = "Instructions is required")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Instructions:")]
        public string Instructions { get; set; }

        [MaxLength(512)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Terms:")]
        public string Terms { get; set; }

        [MaxLength(512)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Exclusions:")]
        public string Exclusions { get; set; }

        [MaxLength(1024)]
        [DisplayName("Link URL:")]
        public string LinkURL { get; set; }

        [MaxLength(128)]
        [DisplayName("Offer Code:")]
        public string OfferCode { get; set; }

        [MaxLength(50)]
        [DisplayName("Tags:")]
        public string Tags { get; set; }

        public List<CustomTagList> ListofTag { get; set; }

        public List<CustomTagList> TagLists { get; set; }

        [DisplayName("Country:")]
        public List<string> Countries { get; set; }

        [DisplayName("Merchant Branches:")]
        public List<string> Branches { get; set; }

        public List<CustomCountryList> ListofCountries { get; set; }

        [DisplayName("Categories:")]
        public List<string> Categories { get; set; }

        public List<CategoryTreeView> ListofCategory { get; set; }

        public string AffiliateReference { get; set; }

        [DisplayName("Redemption Account Number:")]
        [MaxLength(32)]
        public string RedemptionAccountNumber { get; set; }

        [DisplayName("Redemption Product Code:")]
        [MaxLength(32)]
        public string RedemptionProductCode { get; set; }

        [DisplayName("Merchant Branches:")]
        public int MerchantBranchId { get; set; }

        public List<SelectListItem> ListofBranches { get; set; }

        public OfferViewModel()
        {
            ListofMerchants = new List<SelectListItem>();
            ListofAffiliate = new List<SelectListItem>();
            ListofOfferType = new List<SelectListItem>();
            ListofStatus = new List<SelectListItem>();
            ListofRanking = new List<SelectListItem>();
            ListofTag = new List<CustomTagList>();
            ListofCountries = new List<CustomCountryList>();
            ListofCategory = new List<CategoryTreeView>();
            TagLists = new List<CustomTagList>();
            ListofBranches = new List<SelectListItem>();
        }
    }
}