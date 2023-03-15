using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.Models
{
    public class LayoutViewModel
    {
        public bool IsSignedIn { get; set; }
        public bool IsDiamondCustomer { get; set; }
        public decimal? CurrentValue { get; set; }
        public string DiamondStatus { get; set; }
        public DateTime DiamondExpiry { get; set; }
       
        public int MembershipCardId { get; set; }
        public int MembershipPlanId { get; set; }
        public bool EmailConfirmed { get; set; }
        public string CustomerName { get; set; }
        public bool CustomerSecurity { get; set; }



        // Website settings
        public dto.WhiteLabelSettings WhiteLabel { get; set; }
        public TypedAppSettings Settings { get; set; }
        public List<dto.OfferType> OfferTypes { get; set; }
        public List<dto.Category> Categories { get; set; }
        //public List<SelectListItem> Merchants { get; set; }
        public string Merchants { get; set; }
        public List<dto.Public.WebsiteSocialMediaLink> SocialMediaLinks { get; set; }
        public dto.SiteClan SiteClan { get; set; } 
        public List<dto.SponsorImages> SponsorImages { get; set; } 

        public decimal DiamondCost { get; set; }
        public bool ConsumerRights { get; set; }


        public LayoutViewModel()
        {
            IsDiamondCustomer = false;
            WhiteLabel = new dto.WhiteLabelSettings();
            SiteClan = new dto.SiteClan();
            Settings = new TypedAppSettings();
            OfferTypes = new List<dto.OfferType>();
            Categories = new List<dto.Category>();
            SocialMediaLinks = new List<dto.Public.WebsiteSocialMediaLink>();
            SponsorImages = new List<dto.SponsorImages>();
        }
    }
}
