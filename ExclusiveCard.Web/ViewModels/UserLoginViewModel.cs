using System;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class UserLoginViewModel
    {
        public string AuthToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Role { get; set; }
        public string ErrorMessage { get; set; }
    }
}
