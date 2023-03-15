using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Category
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Code { get; set; }

        public int ParentId { get; set; }

        //public string UrlSlack { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        public string UrlSlug { get; set; }

        public ICollection<OfferCategory> OfferCategories { get; set; }
        public ICollection<CategoryFeatureDetail> CategoryFeatureDetails { get; set; }
    }
}
