using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipCard
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        public int MembershipPlanId { get; set; }

        public string CardNumber { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public DateTime? DateIssued { get; set; }

        public int StatusId { get; set; }

        public string AgentCode { get; set; }

        public bool PhysicalCardRequested { get; set; }

        public string CustomerPaymentProviderId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int? PhysicalCardStatusId { get; set; }

        public int? RegistrationCode { get; set; }
        public int? PartnerRewardId { get; set; }
        public int? TermsConditionsId { get; set; }

        public Customer Customer { get; set; }

        public MembershipPlan MembershipPlan { get; set; }
        
        public Status MembershipStatus { get; set; }

        public Status PhysicalCardStatus { get; set; }

        public MembershipRegistrationCode MembershipRegistrationCode { get; set; }
        public PartnerRewards PartnerReward { get; set; }

        public TermsConditions TermsConditions { get; set; }

        public ICollection<MembershipCardAffiliateReference> MembershipCardAffiliateReferences { get; set; }

        public ICollection<CashbackTransaction> CashbackTransactions { get; set; }

        public ICollection<CashbackSummary> CashbackSummaries { get; set; }

        public ICollection<ClickTracking> ClickTrackings { get; set; }

        public ICollection<CustomerPayment> CustomerPayment { get; set; }
        public ICollection<OfferRedemption> OfferRedemptions { get; set; }
    }
}
