using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class PartnerDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int? ContactDetailId { get; set; }
        
        public int? BankDetailsId { get; set; }

        public bool IsDeleted { get; set; }
        public int Type { get; set; }
        public string ImagePath { get; set; }
        public string ManagementURL { get; set; }
        public string AspNetUserId { get; set; }

        public string MembershipCardPrefix { get; set; }

        public ContactDetail ContactDetail { get; set; }

        public BankDetail BankDetail { get; set; }

        public ICollection<MembershipPlan> MembershipPlans { get; set; }

        public ICollection<CashbackTransaction> CashbackTransactions { get; set; }

        public ICollection<CashbackSummary> CashbackSummaries { get; set; }
        public ICollection<Files> Files { get; set; }
        public ICollection<PartnerRewards> PartnerRewards { get; set; }
        public ICollection<MembershipPlan> MembershipCardProviders { get; set; }
    }
}
