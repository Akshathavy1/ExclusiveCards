using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Merchant", Schema = "Exclusive")]
    public  class Merchant
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [ForeignKey("ContactDetail")]
        public int?  ContactDetailsId { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string ContactName { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string ShortDescription { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string LongDescription { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Terms { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string WebsiteUrl { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool FeatureImageOfferText { get; set; }
        [MaxLength(7)]
        [DataType("nvarchar")]
        public string BrandColour { get; set; }

        public virtual ContactDetail ContactDetail { get; set; }

        public virtual ICollection<MerchantSocialMediaLink> MerchantSocialMediaLinks { get; set; }

        public virtual ICollection<MerchantBranch> MerchantBranches { get; set; }

        public virtual ICollection<MerchantImage> MerchantImages { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }

        public virtual ICollection<CashbackTransaction> CashbackTransactions { get; set; }
        public virtual ICollection<CategoryFeatureDetail> CategoryFeatureDetails { get; set; }
    }
}
