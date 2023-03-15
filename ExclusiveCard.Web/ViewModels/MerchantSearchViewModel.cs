using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class MerchantSearchViewModel
    {
        //[Required(ErrorMessage = "SearchText is required")]
        [DisplayName("Search")]
        public string SearchText { get; set; }

        public List<SelectListItem> ListofMerchants { get; set; }

        public MerchantsList MerchantsList { get; set; }

        public MerchantSearchViewModel()
        {
            ListofMerchants  = new List<SelectListItem>();    
        }
    }
}
