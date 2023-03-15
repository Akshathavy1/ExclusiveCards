using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Website.Models
{
    public class ManageUserViewModel
    {
        [DisplayName("Username:")]
        public string Username { get; set; }
        [DisplayName("Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public List<UserSummary> UserSummaries { get; set; }

        public ManageUserViewModel()
        {
            UserSummaries = new List<UserSummary>();
        }
    }
}
