using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class WhiteLabelSettings
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string URL { get; set; }
        public string Slug { get; set; }
        public string CompanyNumber { get; set; }
        public string CSSFile { get; set; }
        public string Logo { get; set; }
        public string ClaimsEmail { get; set; }
        public string HelpEmail { get; set; }
        public string MainEmail { get; set; }
        public string Address { get; set; }
        public string CardName { get; set; }
        public string PrivacyPolicy { get; set; }
        public string Terms { get; set; }
        public int? SiteType { get; set; }
        public string CharityName { get; set; }
        public string CharityUrl { get; set; }
        public int? SiteOwnerId { get; set; }
        public string RegistrationUrl { get; set; }

        public string NewsletterLogo { get; set; }
        public virtual SiteOwner SiteOwner { get; set; }
        public bool isRegional { get; set; }
        public int DefaultRegion { get; set; }

        public virtual ICollection<MarketingContactList> MarketingContactLists { get; set; }
        public virtual ICollection<MarketingCampaign> Campaigns { get; set; }

    }
}
