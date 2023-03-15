namespace ExclusiveCard.Services.Models.DTOs.Public
{
    public class WebsiteSocialMediaLink
    {
        public int Id { get; set; }
        
        public string CountryCode { get; set; }
       
        public int SocialMediaCompanyId { get; set; }
       
        public string SocialMediaURI { get; set; }

        public int WhiteLabelSettingsId { get; set; }
    }
}
