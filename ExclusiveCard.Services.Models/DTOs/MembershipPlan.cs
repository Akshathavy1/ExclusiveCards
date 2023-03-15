using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MembershipPlan
    {
        public int Id { get; set; }
        
        public int? PartnerId { get; set; }

        public int MembershipPlanTypeId { get; set; }

        public int NumberOfCards { get; set; }
        
        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }
        
        public decimal CustomerCardPrice { get; set; }

        public decimal PartnerCardPrice { get; set; }

        public Single CustomerCashbackPercentage { get; set; }

        public Single DeductionPercentage { get; set; }

        public string Description { get; set; }
        
        public string CurrencyCode { get; set; }

        public int Duration { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
        public int? MembershipLevelId { get; set; }
        public bool PaidByEmployer { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal PaymentFee { get; set; }
        public int? CardProviderId { get; set; }
        public int WhitelabelId { get; set; }
        public int? AgentCodeId { get; set; }

        public Single BenefactorPercentage { get; set; }

        public int? SiteCategoryId { get; set; }

        //public int SiteClanId { get; set; }

        public  PartnerDto Partner { get; set; }

        public MembershipPlanType MembershipPlanType { get; set; }
        public MembershipLevel MembershipLevel { get; set; }

        public ICollection<MembershipCard> MembershipCards { get; set; }

        public ICollection<MembershipRegistrationCode> MembershipRegistrationCodes { get; set; }

        public ICollection<MembershipPlanPaymentProvider> MembershipPlanPaymentProvider { get; set; }

        public ICollection<PayPalSubscription> PayPalSubscriptions { get; set; }
        public ICollection<MembershipPlanBenefits> MembershipPlanBenefits { get; set; }
        public PartnerDto CardProvider { get; set; }

        public virtual SiteCategory SiteCategory { get; set; }
        //public virtual SiteClan SiteClan { get; set; }
    }
}
