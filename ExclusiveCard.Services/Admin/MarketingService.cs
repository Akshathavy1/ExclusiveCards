using AutoMapper;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Managers;
using ExclusiveCard.Providers.Email;
using ExclusiveCard.Services.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using oldmanagers = ExclusiveCard.Data.CRUDS;
using Microsoft.Extensions.Configuration;
using ExclusiveCard.Providers.Marketing;
using provider = ExclusiveCard.Providers.Marketing;
using dtos = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.Services.Admin
{

    public class MarketingService : IMarketingService
    {

        #region Private members

        private readonly IMarketingManager _marketingManager;
        private readonly IEmailManager _emailManager;

        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        #endregion

        #region Constuctor

        public MarketingService( IMarketingManager marketingManager,
                                 IEmailManager emailManager,

                                 IMapper mapper 
                                )
        {
            _marketingManager = marketingManager;
            _emailManager = emailManager;

            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();

        }

        #endregion

        #region public 

        public async Task<dtos.EmailTemplate> Update(dtos.EmailTemplate template)
        {
            var result = await _marketingManager.Update(template);
            return result;
        }

        public async Task<dtos.Newsletter> Update(dtos.Newsletter newsletter)
        {
            var result = await _marketingManager.Update(newsletter);
            return result;
        }

        public async Task<List<dtos.MarketingCampaign>> Update(List<dtos.MarketingCampaign> campaigns)
        {
            var result = await _marketingManager.Update(campaigns);
            return result;
        }

        public async Task<dtos.EmailTemplate> GetEmailTemplateById(int emailTemplateId)
        {
            var result = await _marketingManager.GetEmailTemplateById(emailTemplateId);
            return result;
        }

        public async Task<dtos.Newsletter> GetNewsLetter(int id)
        {
            var result = await _marketingManager.GetNewsLetterById(id);
            return result;
        }

        public async Task<List<dtos.Newsletter>> GetNewsletters()
        {
            var result = await _marketingManager.GetAll();
            return result;
        }
        public async Task<List<dtos.WhiteLabelSettings>> GetWhiteLabelsByNewsletter(int newsletterId)
        {
            var result = await _marketingManager.GetWhiteLabelsByNewsletter(newsletterId);
            return result;
        }
        public async Task<List<dtos.MarketingCampaign>> GetCampaignsByNewsletter(int newsletterId)
        {
            var result = await _marketingManager.GetCampaignsByNewsletter(newsletterId);
            return result;
        }

        public async Task<dtos.Email> GetTestEmail(int campaignId)
        {
            dtos.MarketingCampaign campaign = await _marketingManager.GetMarketingCampaignById(campaignId);
            var email = await _marketingManager.EmailForCampaign(campaignId);
            var result = _mapper.Map<dtos.Email>(email);
            result.Subject += " Test Email";

            return result;
        }

        public async Task<string> SendEmail(dtos.Email email)
        {
            var result = await _emailManager.SendEmailAsync(email);
            return result;
        }

        public async Task<int> ManageMarketingContacts()
        {
            var newlists = await _marketingManager.AddMissingContactLists();

            var contactsAdded = await _marketingManager.AddOptedInContacts();

            var contactsRemoved = await _marketingManager.RemoveOptedOutContacts();

            return newlists + contactsAdded + contactsRemoved;
        }

        public async Task<int> ManageMarketingEvents()
        {
            var newEvents = await _marketingManager.AddOptedInCampaigns();
            var EventsRemoved = await _marketingManager.RemoveOptedOutCampaigns();

            return newEvents + EventsRemoved;
        }

        #endregion

    }
}
