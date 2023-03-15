using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
    public class AwinCSVFile
    {
        public string PromotionID { get; set; }
        public string Advertiser { get; set; }
        public int AdvertiserID { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Starts { get; set; }
        public string Ends { get; set; }
        public string Categories { get; set; }
        public string Regions { get; set; }
        public string Terms { get; set; }
        public string DeeplinkTracking { get; set; }
        public string Deeplink { get; set; }
        public string CommissionGroups { get; set; }
        public string Commission { get; set; }
        public string Exclusive { get; set; }
        public string DateAdded { get; set; }
        public string Title { get; set; }
    }
}
