using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("MembershipPlan",Schema = "Exclusive")]
    public class MembershipPlan
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Partner")]
        public int? PartnerId { get; set; }

        [ForeignKey("MembershipPlanType")]
        public int MembershipPlanTypeId { get; set; }

        public int NumberOfCards { get; set; }
        
        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }
        
        public decimal CustomerCardPrice { get; set; }
        
        public decimal PartnerCardPrice { get; set; }

        public Single CustomerCashbackPercentage { get; set; }

        public Single DeductionPercentage { get; set; }

        [MaxLength(500)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CurrencyCode { get; set; }

        public int Duration { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
        [ForeignKey("MembershipLevel")]
        public int? MembershipLevelId { get; set; }
        public bool PaidByEmployer { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal PaymentFee { get; set; }
        public int WhitelabelId { get; set; }
        public int? AgentCodeId { get; set; }

        [ForeignKey("CardPartner")]
        public int? CardProviderId { get; set; }

        public Single BenefactorPercentage { get; set; }

        [ForeignKey("SiteCategory")]
        public int? SiteCategoryId { get; set; }

        //[ForeignKey("SiteClan")]
        //public int SiteClanId { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual MembershipPlanType MembershipPlanType { get; set; }
        public virtual MembershipLevel MembershipLevel { get; set; }

        public virtual ICollection<MembershipCard> MembershipCards { get; set; }

        public virtual ICollection<MembershipRegistrationCode> MembershipRegistrationCodes { get; set; }
         
        public virtual ICollection<MembershipPlanPaymentProvider> MembershipPlanPaymentProvider { get; set; }

        public virtual ICollection<PayPalSubscription> PayPalSubscriptions { get; set; }
        public virtual ICollection<MembershipPlanBenefits> MembershipPlanBenefits { get; set; }
        public virtual Partner CardProvider { get; set; }

        public virtual WhiteLabelSettings WhiteLabel { get; set; }
        public virtual SiteCategory SiteCategory { get; set; }
        public virtual SiteClan SiteClan { get; set; }

    }
}
