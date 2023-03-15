using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("WhiteLabelSettings", Schema = "CMS")]
    public class WhiteLabelSettings
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string Name { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string DisplayName { get; set; }

        [MaxLength(100)]
        [DataType("varchar")]
        public string URL { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string Slug { get; set; }

        [MaxLength(10)]
        [DataType("varchar")]
        public string CompanyNumber { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string CSSFile { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string Logo { get; set; }

        [MaxLength(100)]
        [DataType("varchar")]
        public string ClaimsEmail { get; set; }

        [MaxLength(100)]
        [DataType("varchar")]
        public string HelpEmail { get; set; }

        [MaxLength(100)]
        [DataType("varchar")]
        public string MainEmail { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("varchar")]
        public string Address { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string CardName { get; set; }

        [MaxLength(512)]
        [DataType("varchar")]
        public string PrivacyPolicy { get; set; }

        [MaxLength(512)]
        [DataType("varchar")]
        public string Terms { get; set; }

        public int? SiteType { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string CharityName { get; set; }

        [MaxLength(512)]
        [DataType("varchar")]
        public string CharityUrl { get; set; }

        [MaxLength(255)]
        [DataType("varchar")]
        public string NewsletterLogo { get; set; }

        [ForeignKey("SiteOwner")]
        public int? SiteOwnerId { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("varchar")]
        public string RegistrationUrl { get; set; }

        public bool IsRegional { get; set; }

        public int DefaultRegion { get; set; }

        public virtual ICollection<MarketingCampaign> Campaigns { get; set; }

        public virtual ICollection<MarketingContactList> MarketingContactLists { get; set; }

        public virtual SiteOwner SiteOwner { get; set; }
    }
}