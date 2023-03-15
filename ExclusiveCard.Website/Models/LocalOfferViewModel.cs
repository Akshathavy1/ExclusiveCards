﻿using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Website.Models
{
    public class LocalOfferViewModel
    {
        public int MerchantId { get; set; }
        public string DisabledLogo { get; set; }
        public string Logo { get; set; }
        public string FeatureImage { get; set; }
        public string LargeImage { get; set; }
        public bool UseFeatureImage { get; set; }
        public string OfferText { get; set; }
        public string MerchantName { get; set; }
        public int OfferId { get; set; }
        public string OfferShortDescription { get; set; }
        public string OfferLongDescription { get; set; }
        public int DisplayType { get; set; }
        public int OfferTypeId { get; set; }
        //public List<OfferCategory> OfferCategories { get; set; }

    }
}