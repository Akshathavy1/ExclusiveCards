using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class NewsletterViewModel
    {
        [DisplayName("Name:")]
        public string Name { get; set; }

        public int? NewsLetterId { get; set; }
        public int Id { get; set; }

        [DisplayName("Subject:")]
        public string Subject { get; set; }

        [DisplayName("Message:")]
        public string Message { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public string HeaderHtml { get; set; }
        public string FooterHtml { get; set; }
        public bool Enable { get; set; }
        public string Schedule { get; set; }
        public string EmailName { get; set; }
        public int OfferListId { get; set; }
        public int EmailTemplateId { get; set; }
        public int TemplateTypeId { get; set; }
        public string RegistrationCode { get; set; }
        public string TestEmailRecipient { get; set; }

        public List<SelectListItem> ListofNewsletterName { get; set; }
        public List<WhiteLabelSettings> WhiteLabelSettings { get; set; }
        public List<MarketingCampaign> MarketingCampaigns { get; set; }

    }
}
