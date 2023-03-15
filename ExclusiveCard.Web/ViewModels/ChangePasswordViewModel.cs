using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class ChangePasswordViewModel
    {
        [DisplayName("*Old Password:")]
        [Required(ErrorMessage = "Old Password is required")]
        public string OldPassword { get; set; }

        [DisplayName("*New Password:")]
        [Required(ErrorMessage = "New Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,48}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string NewPassword { get; set; }

        [DisplayName("*Confirm Password:")]
        [Required(ErrorMessage = "Confirm Password is required")]
        public string ConfirmPassword { get; set; }

        public string Username { get; set; }
    }
}
