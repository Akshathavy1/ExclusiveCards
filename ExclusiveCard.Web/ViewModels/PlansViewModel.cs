using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class PlansViewModel
    {
        #region White Labels

        [DisplayName("Select White Label")]
        public int WhiteLabelId { get; set; }

        public List<SelectListItem> WhiteLabels { get; set; }

        #endregion White Labels

        #region Card Providers

        [DisplayName("Select Card Provider")]
        public int CardProviderId { get; set; }

        public List<SelectListItem> CardProviders { get; set; }

        [DisplayName("Name:")]
        public string CardProviderName { get; set; }

        #endregion Card Providers

        #region Membership Plans

        public WhiteLabelSettingMembershipPlan WhiteLabelSettingMembershipPlan { get; set; }

        #endregion Membership Plans

        #region Agents

        public WhiteLabelSettingsAgents WhiteLabelSettingsAgents { get; set; }

        #endregion Agents

        #region Registration Codes

        public RegistrationCodesSummary RegistrationCodesSummary { get; set; }

        #endregion Registration Codes
    }
}