using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    /// <summary>
    /// CustomerAccountDto is the data transfer object that passes the info for creation of a new
    /// customer account  (i.e. Customer + membership card(s) ) from the controller to the 
    /// CustomerAccountService
    /// </summary>
    public class CustomerAccountDto
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Customer Customer { get; set; }

        public int? TermsConditionsId { get; set; }

        public MembershipPendingToken  PendingMembershipToken { get; set; }

        public string CountryCode { get; set; }

        public string RegistrationCode { get; set; }


    }
}
