using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("*Name:")]
        public string Name { get; set; }
        public string Code { get; set; }
        [DisplayName("Parent:")]
        public int ParentId { get; set; }
        public List<SelectListItem> Parents { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string UrlSlug { get; set; }
        public CategoryFeatureViewModel Feature { get; set; }

        public CategoryViewModel()
        {
            Parents = new List<SelectListItem>();
            Feature = new CategoryFeatureViewModel();
        }
    }
}
