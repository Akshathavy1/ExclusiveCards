using ExclusiveCard.Services.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExclusiveCard.Website.Models
{
    public class OffersDisplayViewModel
    {
        
        public List<int> CategoryIds { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }
        public List<int> OfferTypeIds { get; set; }
        public List<OfferType> OfferTypes { get; set; }
        [DisplayName("Keywords")]
        public string Keywords { get; set; }
        [DisplayName("Merchant Name")]
        public string MerchantName { get; set; }
        // public OffersResultViewModel OffersResultView { get; set; }
        public PagedOffersViewModel  PagedOffersView { get; set; }
        public List<SelectListItem> ListOfMerchantName { get; set; }

        public string MainSearchTerm { get; set; }
        public string OfferSort { get; set; }
        public int ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public List<OfferHubViewModel> OfferHubViewModels { get; set; }

        public OffersDisplayViewModel()
        {
            CategoryIds = new List<int>();
            Categories = new List<Category>();
            Tags = new List<Tag>();
            OfferTypeIds = new List<int>();
            OfferTypes = new List<OfferType>();
            PagedOffersView = new PagedOffersViewModel();
            ListOfMerchantName = new List<SelectListItem>();
            
            OfferHubViewModels = new List<OfferHubViewModel>();
        }
    }
}
