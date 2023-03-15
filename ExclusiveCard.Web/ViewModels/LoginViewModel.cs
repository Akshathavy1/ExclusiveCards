using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
