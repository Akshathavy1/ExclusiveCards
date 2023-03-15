using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using dtos = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IMarketingService
    {
        #region needed by controller

        Task<dtos.EmailTemplate> GetEmailTemplateById(int emailTemplateId);
        Task<List<dtos.Newsletter>> GetNewsletters();
        Task<dtos.Newsletter> GetNewsLetter(int id);
        /// <summary>
        /// Returns an email example related to the <paramref name="campaignId"/>
        /// </summary>
        /// <param name="campaignId">The campaign to use for identifying the email template and white label parameters</param>
        /// <returns></returns>
        Task<dtos.Email> GetTestEmail(int campaignId);
        Task<string> SendEmail(dtos.Email email);
        Task<dtos.Newsletter> Update(dtos.Newsletter newsletter);
        Task<dtos.EmailTemplate> Update(dtos.EmailTemplate template);
        Task<List<dtos.MarketingCampaign>> Update(List<dtos.MarketingCampaign> campaigns);
        Task<List<dtos.WhiteLabelSettings>> GetWhiteLabelsByNewsletter(int newsletterId);
        Task<List<dtos.MarketingCampaign>> GetCampaignsByNewsletter(int newsletterId);
        #endregion

        #region Process Marketing Tasks

        /// <summary>
        /// Creates contact lists if needed (e.g. for new white labels)
        /// Adds new contacts to provider that have the marketing flag set
        /// Removes contacts from provider that no longer have the marketing flag set
        /// </summary>
        /// <returns></returns>
        Task<int> ManageMarketingContacts();

        /// <summary>
        /// Marketing events are the details needed to send an email campaign at a defined date/time.
        /// This process creates and deletes marketing events in line with the current campaign active flags
        /// </summary>
        /// <returns></returns>
        Task<int> ManageMarketingEvents();

        #endregion
    }

}
