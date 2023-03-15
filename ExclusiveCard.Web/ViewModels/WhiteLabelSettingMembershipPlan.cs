using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class WhiteLabelSettingMembershipPlan
    {
        #region Membership Plans

        [DisplayName("Select Membership Plans")]
        public int Id { get; set; }

        public List<SelectListItem> MembershipPlans { get; set; }

        [Required]
        [DisplayName("Membership Type")]
        public int MembershipLevelId { get; set; }

        public List<SelectListItem> ListOfMembershipTypes { get; set; } = new List<SelectListItem>();

        [Required]
        [DisplayName("Plan Type")]
        public int MembershipPlanTypeId { get; set; }

        public List<SelectListItem> ListOfPlanTypes { get; set; } = new List<SelectListItem>();

        [Required]
        [MaxLength(500)]
        [DisplayName("Plan Name")]
        public string Description { get; set; }

        [Required]
        [Range(7, 36500)]
        [DisplayName("Membership Length")]
        public int Duration { get; set; }

        [Required]
        [DisplayName("From:")]
        public string ValidFrom { get; set; }

        [Required]
        [DisplayName("To:")]
        public string ValidTo { get; set; }

        [Required]
        [Range(1, 1000000000)]
        [DisplayName("Maximum Number of Memberships")]
        public int NumberOfCards { get; set; }

        [Required]
        [DisplayName("Paid By Card Provider")]
        public bool PaidByEmployer { get; set; }

        public decimal PartnerCardPrice { get; set; }

        [DisplayName("Exclusive Share")]
        public int DeductionPercentage { get; set; }

        [DisplayName("Customer Share")]
        public int CustomerCashbackPercentage { get; set; }

        [DisplayName("Beneficiary Share")]
        public int BenefactorPercentage { get; set; }

        public int? PartnerId { get; set; }

        public string CurrencyCode { get; set; }

        public decimal MinimumValue { get; set; }

        public decimal PaymentFee { get; set; }

        public int? AgentCodeId { get; set; }

        public int? SiteCategoryId { get; set; }

        #endregion Membership Plans
    }
}