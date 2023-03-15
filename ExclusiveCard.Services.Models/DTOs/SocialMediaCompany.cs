using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class SocialMediaCompany
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        public ICollection<MerchantSocialMediaLink> MerchantSocialMediaLinks { get; set; }
    }
}
