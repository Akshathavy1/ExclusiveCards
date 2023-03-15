using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("MerchantSocialMediaLink", Schema = "Exclusive")]
    public class MerchantSocialMediaLink
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        [ForeignKey("SocialMediaCompany")]
        public int SocialMediaCompanyId { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string SocialMediaURI { get; set; }

        public virtual Merchant Merchant { get; set; }

        public virtual SocialMediaCompany SocialMediaCompany { get; set; }
    }
}
