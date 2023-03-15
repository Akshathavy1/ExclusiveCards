using dtos = ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using provider = ExclusiveCard.Providers.Marketing;

namespace ExclusiveCard.Managers
{
    public interface IMarketingManager
    {
        /// <summary>
        /// Get all newsletters (enabled flag is ignored)
        /// </summary>
        /// <returns></returns>
        Task<List<dtos.Newsletter>> GetAll();
        Task<dtos.Newsletter> GetNewsLetterById(int id);
        Task<dtos.EmailTemplate> GetEmailTemplateById(int id);

        Task<dtos.Newsletter> Update(dtos.Newsletter letter);
        Task<dtos.EmailTemplate> Update(dtos.EmailTemplate template);

        /// <summary>
        /// Updates JUST the enabled flag for the marketing campaigns provided
        /// </summary>
        /// <param name="campaigns">list of marketing campaigns including the new enabled flag setting</param>
        /// <returns>updated results</returns>
        Task<List<dtos.MarketingCampaign>> Update(List<dtos.MarketingCampaign> campaigns);

        #region New Stuff

        Task<List<dtos.WhiteLabelSettings>> GetWhiteLabelsByNewsletter(int NewsletterId);
        Task<List<dtos.MarketingCampaign>> GetCampaignsByNewsletter(int newsletterId);

        /// <summary>
        /// returns the matching active campaign
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        Task<dtos.MarketingCampaign> GetMarketingCampaignById(int campaignId);

        /// <summary>
        /// Returns the marketing email configuration related to the <paramref name="campaignId"/>
        /// </summary>
        /// <param name="campaignId">The campaign to use for identifying the email template and white label parameters</param>
        /// <returns></returns>
        Task<provider.Models.MarketingEventEmailConfig> EmailForCampaign(int campaignId);

        /// <summary>
        /// Ensures that each white label has a contact list on the marketing provider
        /// Adds any missing contact lists to the provider and our database
        /// A contact list related to a white label needs to exist before you can target
        /// customers with whitelabel related marketing
        /// </summary>
        /// <returns>Number of new lists added</returns>
        Task<int> AddMissingContactLists();

        /// <summary>
        /// Removes contacts from the provider and database marketing tables where the customer marketing flag is turned off.
        /// </summary>
        /// <returns>Number of contacts removed</returns>
        Task<int> RemoveOptedOutContacts();

        /// <summary>
        /// Removes a contact from the provider and database marketing tables where the email matches
        /// </summary>
        /// <param name="email">email to be removed from marketing provider system</param>
        /// <returns></returns>
        Task<bool> RemoveMarketingContact(string email);

        /// <summary>
        /// Adds contacts to the provider and database where the customer marketing flag is turned on
        /// </summary>
        /// <returns>Number of contacts added</returns>
        Task<int> AddOptedInContacts();

        /// <summary>
        /// Removes marketing events from the provider where the campaingn is turned off in the database.
        /// </summary>
        /// <returns>Number of marketing events removed</returns>
        Task<int> RemoveOptedOutCampaigns();

        /// <summary>
        /// Adds marketing events to the provider where the campaign is turned on in the database
        /// </summary>
        /// <returns>Number of marketing events added</returns>
        Task<int> AddOptedInCampaigns();

        #endregion
    }
}
