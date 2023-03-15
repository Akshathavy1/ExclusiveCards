using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class SocialMediaItem
    {
        public int SocialMediaCompanyId { get; set; }

        public string Name { get; set; }
        [MaxLength(512)]
        public string URI { get; set; }
    }
}
