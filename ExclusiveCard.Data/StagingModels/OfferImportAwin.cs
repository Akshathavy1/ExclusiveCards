using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.StagingModels
{
    [Table("OfferImportAwin", Schema ="Staging")]
    public class OfferImportAwin
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OfferImportFile")]
        public int OfferImportFileId { get; set; }

        [MaxLength(255)]
        [DataType("nvarchar")]
        public string PromotionId { get; set; }


        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Advertiser { get; set; }

        public int AdvertiserId { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Type { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Code { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        public DateTime Starts { get; set; }

        public DateTime Ends { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Categories { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Regions { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Terms { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string DeeplinkTracking { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Deeplink { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string CommissionGroups { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Commission { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Exclusive { get; set; }

        public DateTime DateAdded { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string Title { get; set; }

        public virtual OfferImportFile OfferImportFile { get; set; }
    }
}
