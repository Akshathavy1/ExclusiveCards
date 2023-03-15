using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.Website.Models
{
    public class ForgotPasswordViewModel
    {
        [DisplayName("Security Question:")]
        public string QuestionId { get; set; }
        public List<SelectListItem> ListofQuestions { get; set; }

        [DisplayName("Email:")]
        public string Email { get; set; }

        public ForgotPasswordViewModel()
        {
            ListofQuestions = new List<SelectListItem>();
           
        }
    }
}
