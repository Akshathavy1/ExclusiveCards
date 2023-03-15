using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.StagingModels
{
    [Table("OfferImportFile", Schema ="Staging")]
   public class OfferImportFile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("FileType")]
        public int AffiliateFileId { get; set; }

        public DateTime DateImported { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string FilePath { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string ErrorFilePath { get; set; }

        [ForeignKey("Status")]
        public int ImportStatus { get; set; }

        public int TotalRecords { get; set; }

        public int Imported { get; set; }

        public int Failed { get; set; }

        public int Staged { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CountryCode { get; set; }

        public int Duplicates { get; set; } = 0;

        public int Updates { get; set; } = 0;

        public virtual ICollection<OfferImportAwin> OfferImportAwins { get; set; }

        public virtual ICollection<OfferImportAwinDuplicate> OfferImportAwinDuplicates { get; set; }

        public virtual Status Status { get; set; }

        public virtual AffiliateFile AffiliateFile { get; set; }
    }
}
