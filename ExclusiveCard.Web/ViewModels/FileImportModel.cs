using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class FileImportModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("*Affiliate:")]
        public int AffiliateId { get; set; }
        public List<SelectListItem> AffiliateList { get; set; }
        public List<AffiliateToAffiliateFileMapping> Affiliates { get; set; }

        [Required]
        [DisplayName("*File Type:")]
        public int FileTypeId { get; set; }
        public List<SelectListItem> FileTypes { get; set; }

        [MaxLength(3)]
        [DisplayName("Country:")]
        public string ImportCountryCode { get; set; }

        public List<SelectListItem> ListofCountries { get; set; }

        [Required]
        [DisplayName("File:")]
        public string File { get; set; }
        public string ErrorFilePath { get; set; }

        public IFormFile FileSelected;

        public int TotalRecords { get; set; }

        public int Success { get; set; }

        public int ImportStatus { get; set; }

        //NEW enum name: Import.Processed
        public int Migrated { get; set; }

        public string CurrentStatus { get; set; }

        public int Failed { get; set; }

        public int Staged { get; set; }

        public int Duplicates { get; set; }

        public int Updates { get; set; }

        public FileImportModel()
        {
            AffiliateList = new List<SelectListItem>();
            Affiliates = new List<AffiliateToAffiliateFileMapping>();
            FileTypes = new List<SelectListItem>();
            ListofCountries = new List<SelectListItem>();
        }
    }
}
