using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CategoryFeatureViewModel
    {
        public int CategoryId { get; set; }
        [DisplayName("Featured Merchant:")]
        public int? FeatureMerchantId { get; set; }
        [DisplayName("Selected Image:")]
        public MerchantImageViewModel SelectedImage { get; set; }
        [DisplayName("Unselected Image:")]
        public MerchantImageViewModel UnselectedImage { get; set; }
        [DisplayName("Country:")]
        public string CountryCode { get; set; }
        public List<SelectListItem> Merchants { get; set; }
        public List<SelectListItem> Countries { get; set; }

        public CategoryFeatureViewModel()
        {
            Merchants = new List<SelectListItem>();
            Countries = new List<SelectListItem>();
            SelectedImage = new MerchantImageViewModel();
            UnselectedImage = new MerchantImageViewModel();
        }
    }
}
