using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
   public class OfferImportFile
    {
        public int Id { get; set; }

        public int AffiliateFileId { get; set; }

        public DateTime DateImported { get; set; }

        public string FilePath { get; set; }

        public string ErrorFilePath { get; set; }

        public int ImportStatus { get; set; }

        public int TotalRecords { get; set; }

        public int Imported { get; set; }

        public int Failed { get; set; }

        public int Staged { get; set; }

        public string CountryCode { get; set; }
        
        public int Duplicates { get; set; }

        public int Updates { get; set; }

        public  ICollection<OfferImportAwin> OfferImportAwins { get; set; }

        public ICollection<OfferImportAwinDuplicate> OfferImportAwinDuplicates { get; set; }

        public Status Status { get; set; }

        public AffiliateFile AffiliateFile { get; set; }
    }
}
