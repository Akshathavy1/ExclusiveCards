using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class OfferHubMainViewModel
    {
        public int RefMerchantId { get; set; }
        [Required]
        [DisplayName("*Merchant")]
        public int MerchantId { get; set; }
        public List<SelectListItem> ListOfMerchants { get; set; }
        [Required]
        [DisplayName("*Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }

        public OfferHubList OfferHubList { get; set; }

        public OfferHubMainViewModel()
        {
            ListOfMerchants = new List<SelectListItem>();
            OfferHubList = new OfferHubList();
        }
    }
}
