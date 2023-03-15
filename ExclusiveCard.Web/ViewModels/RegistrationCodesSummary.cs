using System.Collections.Generic;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class RegistrationCodesSummary
    {
        public int Id { get; set; }

        public List<dto.MembershipRegistrationCode> ListOfRegistrationCodes { get; set; }

        public int MembershipPlanId { get; set; }

        public string ValidFrom { get; set; }

        public string ValidTo { get; set; }

        public int NumberOfCodes { get; set; }

        public string StoragePath { get; set; }
    }
}