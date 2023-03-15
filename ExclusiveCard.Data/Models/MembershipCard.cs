using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("MembershipCard", Schema = "Exclusive")]
    public class MembershipCard
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }

        [ForeignKey("MembershipPlan")]
        public int MembershipPlanId { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string CardNumber { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public DateTime? DateIssued { get; set; }

        [ForeignKey("MembershipStatus")]
        public int StatusId { get; set; }

        public bool PhysicalCardRequested {get; set;}

        public string CustomerPaymentProviderId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("PhyicalCardStatus")]
        public int? PhysicalCardStatusId { get; set; }

        [ForeignKey("MembershipRegistrationCode")]
        public int? RegistrationCode { get; set; }

        [ForeignKey("PartnerReward")]
        public int? PartnerRewardId { get; set; }

        [ForeignKey("TermsConditions")]
        public int? TermsConditionsId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual MembershipPlan MembershipPlan { get; set; }

        public virtual Status MembershipStatus { get; set; }

        public virtual Status PhysicalCardStatus { get; set; }

        public virtual MembershipRegistrationCode MembershipRegistrationCode { get; set; }
        public virtual PartnerRewards PartnerReward { get; set; }
        public virtual TermsConditions TermsConditions { get; set; }
        public virtual ICollection<MembershipCardAffiliateReference> MembershipCardAffiliateReferences { get; set; }

        public virtual ICollection<CashbackTransaction> CashbackTransactions { get; set; }

        public virtual ICollection<CashbackSummary> CashbackSummaries { get; set; }

        public virtual ICollection<ClickTracking> ClickTrackings { get; set; }

        public virtual ICollection<CustomerPayment> CustomerPayment { get; set; }
        public virtual ICollection<OfferRedemption> OfferRedemptions { get; set; }
    }
}
