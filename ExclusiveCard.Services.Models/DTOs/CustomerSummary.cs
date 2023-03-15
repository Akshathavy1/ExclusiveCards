using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class CustomerSummary
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Username { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime? Dob { get; set; }
        public string Postcode { get; set; }
        public string CardNumber { get; set; }
    }
}
