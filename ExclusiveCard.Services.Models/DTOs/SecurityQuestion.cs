using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class SecurityQuestion
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public bool IsActive { get; set; }

        public ICollection<CustomerSecurityQuestion> CustomerSecurityQuestions { get; set; }
    }
}
