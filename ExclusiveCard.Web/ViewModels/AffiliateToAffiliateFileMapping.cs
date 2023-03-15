using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class AffiliateToAffiliateFileMapping
    {
        public int AffiliateId { get; set; }
        public string AffiliateName { get; set; }

        public List<SelectListItem> FileTypes { get; set; }

        public AffiliateToAffiliateFileMapping()
        {
            FileTypes = new List<SelectListItem>();
        }
    }
}
