using System;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
    public class CustomerRegistration
    {
        public Guid CustomerPaymentId { get; set; }
        public string Data { get; set; }
        public int StatusId { get; set; }
        public string AspNetUserId { get; set; }
        public Status CustomerStatus { get; set; }
    }
}
