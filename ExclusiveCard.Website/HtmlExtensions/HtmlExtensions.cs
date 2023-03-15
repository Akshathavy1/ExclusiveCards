using System;
using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.HtmlExtensions
{
    public static class HtmlExtensions
    {
        public static IHtmlContent GetWhiteLabel(this IHtmlHelper htmlHelper, string sourceString, dto.WhiteLabelSettings settings)
        {
            //Get locale from context
            //call service to get string for this key and locale
            if (settings != null)
            {
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.URL) + "}}", settings.URL);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Slug) + "}}", settings.Slug);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Logo) + "}}", settings.Logo);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Name) + "}}", settings.Name);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Address) + "}}", settings.Address);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CSSFile) + "}}", settings.CSSFile);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.HelpEmail) + "}}", settings.HelpEmail);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.MainEmail) + "}}", settings.MainEmail);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.ClaimsEmail) + "}}", settings.ClaimsEmail);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.DisplayName) + "}}", settings.DisplayName);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CompanyNumber) + "}}", settings.CompanyNumber);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CardName) + "}}", settings.CardName);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.PrivacyPolicy) + "}}", settings.PrivacyPolicy);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Terms) + "}}", settings.Terms);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CharityName) + "}}", settings.CharityName);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CharityUrl) + "}}", settings.CharityUrl);
            }

            return new HtmlString(sourceString);
        }


        public static IHtmlContent GetSiteClan(this IHtmlHelper htmlHelper, string sourceString, dto.SiteClan siteClan, dto.WhiteLabelSettings settings)
        {
            //Get locale from context
            //call service to get string for this key and locale
            if (siteClan != null && settings!=null)
            {
                sourceString = sourceString.Replace("{{" + nameof(dto.SiteClan.Description) + "}}", siteClan.Description);
                sourceString = sourceString.Replace("{{" + nameof(dto.SiteClan.Charity.CharityName) + "}}", siteClan.Charity.CharityName);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Name) + "}}", settings.Name);
            }
            return new HtmlString(sourceString);
        }

        public static IHtmlContent GetDiamondCost(this IHtmlHelper htmlHelper, string sourceString, decimal diamondCost)
        {
            sourceString = sourceString.Replace("{{DiamondCost}}", Convert.ToString(diamondCost, CultureInfo.InvariantCulture));
            return new HtmlString(sourceString);
        }
    }
}
