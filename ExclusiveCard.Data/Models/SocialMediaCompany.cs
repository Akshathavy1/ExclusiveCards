using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("SocialMediaCompany", Schema = "Exclusive")]
    public class SocialMediaCompany
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public virtual ICollection<MerchantSocialMediaLink> MerchantSocialMediaLinks { get; set; }
    }
}
