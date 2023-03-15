using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class LocalOfferMainViewModel
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

        public LocalOfferList LocalOfferList { get; set; }

        public LocalOfferMainViewModel()
        {
            ListOfMerchants = new List<SelectListItem>();
            LocalOfferList = new LocalOfferList();
        }
    }
}
