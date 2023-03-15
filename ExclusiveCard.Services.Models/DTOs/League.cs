using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class League
    {
        
        public int Id { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int SiteCategoryId { get; set; }

        public virtual ICollection<SiteClan> SiteClan { get; set; }

        public virtual SiteCategory SiteCategory { get; set; }
    }
}
