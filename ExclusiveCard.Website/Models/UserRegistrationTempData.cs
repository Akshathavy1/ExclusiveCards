 using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.Models
{
    public class UserRegistrationTempData
    {
        public int? SiteClanId { get; set; }
        public UserToken Token { get; set; }
        public string LeagueDescription { get; set; }
    }
}
