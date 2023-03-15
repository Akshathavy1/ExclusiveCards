using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("WebsiteSocialMediaLink", Schema = "CMS")]
    public class WebsiteSocialMediaLink
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CountryCode { get; set; }

        [ForeignKey("SocialMediaCompany")]
        public int SocialMediaCompanyId { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string SocialMediaURI { get; set; }

        public int WhiteLabelSettingsId { get; set; }
    }
}
