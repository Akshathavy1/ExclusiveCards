using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Offer", Schema = "Exclusive")]
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        [ForeignKey("Affiliate")]
        public int? AffiliateId { get; set; }

        [ForeignKey("OfferType")]
        public int OfferTypeId { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        [ForeignKey("SSOConfiguration")]
        public int? SSOConfigId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool Validindefinately { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string ShortDescription { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string LongDescription { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Instructions { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Terms { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Exclusions { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string LinkUrl { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string OfferCode { get; set; }

        public bool Reoccuring { get; set; }

        public short SearchRanking { get; set; }

        public DateTime Datecreated { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Headline { get; set; }

        [MaxLength(255)]
        [DataType("nvarchar")]
        public string AffiliateReference { get; set; }

        public DateTime? DateUpdated { get; set; }

        [MaxLength(32)]
        [DataType("nvarchar")]
        public string RedemptionAccountNumber { get; set; }

        [MaxLength(32)]
        [DataType("nvarchar")]
        public string RedemptionProductCode { get; set; }

        [DataType("nvarchar")]
        [MaxLength(Int32.MaxValue)]
        public string ProductCode { get; set; }

        public virtual Merchant Merchant { get; set; }

        public virtual OfferType OfferType { get; set; }

        public virtual Status Status { get; set; }

        public virtual Affiliate Affiliate { get; set; }

        public virtual SSOConfiguration SSOConfiguration { get; set; }

        public virtual ICollection<OfferMerchantBranch> OfferMerchantBranches { get; set; }

        public virtual ICollection<OfferCountry> OfferCountries { get; set; }

        public virtual ICollection<OfferCategory> OfferCategories { get; set; }

        public virtual ICollection<OfferTag> OfferTags { get; set; }

        public virtual ICollection<OfferListItem> OfferListItems { get; set; }

        public virtual ICollection<ClickTracking> ClickTrackings { get; set; }

        public virtual ICollection<OfferRedemption> OfferRedemptions { get; set; }
    }
}