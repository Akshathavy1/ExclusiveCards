using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class RegistrationCodeSummary
    {
        public int Id { get; set; }

        public int MembershipPlanId { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public int NumberOfCodes { get; set; }

        public int NumberOfUses { get; set; }

        public string StoragePath { get; set; }

        public ICollection<MembershipRegistrationCode> MembershipRegistrationCodes { get; set; }

        public string BlobConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}