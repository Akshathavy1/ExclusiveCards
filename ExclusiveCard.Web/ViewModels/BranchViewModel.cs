using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class BranchViewModel
    {
        [DisplayName("ID:")]
        public int Id { get; set; }

        public int? ContactDetailsId { get; set; }

        public int MerchantId { get; set; }

        [MaxLength(128)]
        public string MerchantName { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(128)]
        [DisplayName("*Name:")]
        public string Name { get; set; }

        [DisplayName("Main Branch:")]
        public bool Mainbranch { get; set; }

        [MaxLength(128)]
        [DisplayName("Short Description:")]
        public string ShortDescription { get; set; }

        [MaxLength(512)]
        [DisplayName("Long Description:")]
        public string LongDescription { get; set; }

        [Range(0, short.MaxValue)]
        [DisplayName("Display Order:")]
        public short DisplayOrder { get; set; }

        [MaxLength(200)]
        public string Notes { get; set; }

        public List<SelectListItem> SelectCountry { get; set; }

        public ContactsViewModel ContactDetail { get; set; }

        public BranchViewModel()
        {
            ContactDetail = new ContactsViewModel();
            SelectCountry = new List<SelectListItem>();
        }
    }
}
