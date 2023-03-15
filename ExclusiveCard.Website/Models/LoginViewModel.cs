using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Website.Models
{
    public class LoginViewModel
    {
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "Email is required")]
        public string Username { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}
