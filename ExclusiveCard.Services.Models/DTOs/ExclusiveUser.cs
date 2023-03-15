using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class ExclusiveUser
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }


       
    }
}
