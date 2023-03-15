using System;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
    public class OfferImportAwinDuplicate
    {
        public int Id { get; set; }

        public int OfferImportFileId { get; set; }
     
        public string PromotionId { get; set; }
       
        public string Type { get; set; }
       
        public string Code { get; set; }
       
        public string Description { get; set; }

        public DateTime Starts { get; set; }

        public DateTime Ends { get; set; }
      
        public string Categories { get; set; }
        
        public string Regions { get; set; }
       
        public string Terms { get; set; }
       
        public string DeeplinkTracking { get; set; }
       
        public string Deeplink { get; set; }
       
        public string CommissionGroups { get; set; }
      
        public string Commission { get; set; }
       
        public string Exclusive { get; set; }

        public DateTime DateAdded { get; set; }
       
        public string Title { get; set; }

        public virtual OfferImportFile OfferImportFile { get; set; }
    }
}
