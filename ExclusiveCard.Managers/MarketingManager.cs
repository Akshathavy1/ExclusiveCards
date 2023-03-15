using ExclusiveCard.Data.Repositories;
using dtos = ExclusiveCard.Services.Models.DTOs;
using db = ExclusiveCard.Data.Models;
using provider = ExclusiveCard.Providers.Marketing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Linq;
using AutoMapper;
using ExclusiveCard.Enums;
using System.Text.RegularExpressions;
using NCrontab;


namespace ExclusiveCard.Managers
{
    public class MarketingManager : IMarketingManager
    {
        private readonly provider.IMarketingProvider _marketingProvider;
        private readonly IRepository<db.WhiteLabelSettings> _whiteLabel;
        private readonly IRepository<db.MarketingContact> _marketingContact;
        private readonly IRepository<db.Customer> _customer;
        private readonly IRepository<db.MarketingCampaign> _marketingCampaign;
        private readonly IRepository<db.OfferListItem> _offerListItem;
        private readonly IRepository<db.MarketingContactList> _marketingContactList;
        private readonly IRepository<db.Newsletter> _newsletter;
        private readonly IRepository<db.EmailTemplate> _emailTemplate;

        private readonly IMembershipManager _membershipManager;
        private readonly IEmailManager _emailManager;

        private IMapper _mapper;
        private readonly ILogger _logger;

        public MarketingManager(provider.IMarketingProvider marketingProvider,
                                IRepository<db.WhiteLabelSettings> whiteLabel,
                                IRepository<db.MarketingContact> marketingContact,
                                IRepository<db.Customer> customer,
                                IRepository<db.MarketingCampaign> marketingCampaign,
                                IRepository<db.OfferListItem> offerListItem,
                                IRepository<db.MarketingContactList> marketingContactList,
                                IRepository<db.Newsletter> newsletter,
                                IRepository<db.EmailTemplate> emailTemplate,

                                IMembershipManager membershipManager,
                                IEmailManager emailManager,

                                IMapper mapper)
        {
            _marketingProvider = marketingProvider;
            _whiteLabel = whiteLabel;
            _marketingContact = marketingContact;
            _customer = customer;
            _marketingCampaign = marketingCampaign;
            _offerListItem = offerListItem;
            _marketingContactList = marketingContactList;
            _newsletter = newsletter;
            _emailManager = emailManager;

            _membershipManager = membershipManager;
            _emailTemplate = emailTemplate;

            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();
        }


        #region Needed by the controller

        public async Task<List<dtos.WhiteLabelSettings>> GetWhiteLabelsByNewsletter(int NewsletterId)
        {
            var dbResult = await _whiteLabel.FilterNoTrackAsync(x => x.Campaigns.Any(c => c.NewsletterId == NewsletterId));
            var result = _mapper.Map<List<dtos.WhiteLabelSettings>>(dbResult);
            return result;
        }
        public async Task<List<dtos.MarketingCampaign>> GetCampaignsByNewsletter(int newsletterId)
        {
            //var dbResult = await _marketingCampaign.FilterNoTrackAsync(x => x.NewsletterId == newsletterId);
            var dbResult = _marketingCampaign.Include(x=>x.WhiteLabelSettings).Where(x => x.NewsletterId == newsletterId);
            var result = _mapper.Map<List<dtos.MarketingCampaign>>(dbResult.ToList());

            return await Task.FromResult(result);
        }

        public async Task<dtos.Newsletter> Update(dtos.Newsletter letter)
        {
            var dbLetter = _mapper.Map<db.Newsletter>(letter);
            _newsletter.Update(dbLetter);
            _newsletter.SaveChanges();

            var result = _mapper.Map<dtos.Newsletter>(dbLetter);

            return await Task.FromResult(result);
        }

        public async Task<dtos.EmailTemplate> Update(dtos.EmailTemplate template)
        {
            var dbtemplate = _mapper.Map<db.EmailTemplate>(template);
            _emailTemplate.Update(dbtemplate);
            _emailTemplate.SaveChanges();

            var result = _mapper.Map<dtos.EmailTemplate>(dbtemplate);

            return await Task.FromResult(result);
        }

        public async Task<List<dtos.MarketingCampaign>> Update(List<dtos.MarketingCampaign> campaigns)
        {
            List<dtos.MarketingCampaign> results = new List<dtos.MarketingCampaign>();

            foreach (var campaign in campaigns)
            {
                var dbCampaign = _marketingCampaign.GetById(campaign.Id);
                if(dbCampaign != null && dbCampaign.Id > 0 && dbCampaign.Enabled != campaign.Enabled)
                {
                    dbCampaign.Enabled = campaign.Enabled;
                    _marketingCampaign.Update(dbCampaign);
                    _marketingCampaign.SaveChanges();
                }
                var result = _mapper.Map<dtos.MarketingCampaign>(dbCampaign);
                results.Add(result);
            }

            return await Task.FromResult(results);
        }

        public async Task<List<dtos.Newsletter>> GetAll()
        {
            //var dbNewsletters = _newsletter.FilterNoTrack(n => n.Enabled == true);
            var dbNewsletters = _newsletter.FilterNoTrack( x => x.Id > 0 );
            var result = _mapper.Map<List<dtos.Newsletter>>(dbNewsletters);

            return await Task.FromResult(result);
        }

        public  async Task<dtos.Newsletter> GetNewsLetterById(int id)
        {
            var dbNewsletter = _newsletter.GetById(id);
            var result = _mapper.Map<dtos.Newsletter>(dbNewsletter);

            return await Task.FromResult(result);
        }

        public  async Task<dtos.EmailTemplate> GetEmailTemplateById(int id)
        {
            var dbEmailTemplate = _emailTemplate.GetById(id);
            var result = _mapper.Map<dtos.EmailTemplate>(dbEmailTemplate);

            return await Task.FromResult(result);
        }

        public async Task<dtos.MarketingCampaign> GetMarketingCampaignById(int campaignId)
        {
            var dbCampaign = _marketingCampaign.Include( x => x.WhiteLabelSettings)
                                               .Where(w => w.Id == campaignId 
                                                      ).FirstOrDefault();

            var result = _mapper.Map<dtos.MarketingCampaign>(dbCampaign);

            return await Task.FromResult(result);
        }

        public async Task<provider.Models.MarketingEventEmailConfig> EmailForCampaign(int campaignId)
        {
            provider.Models.MarketingEventEmailConfig config = new provider.Models.MarketingEventEmailConfig();
            var marketingCampaign = _marketingCampaign.Include(
                                                                x => x.WhiteLabelSettings,
                                                                x => x.Newsletter,
                                                                x => x.Newsletter.EmailTemplate
                                                                ).Where(w => w.Id == campaignId).FirstOrDefault();
            List<string> htmlContent = new List<string>();
            string offerListHtml = string.Empty;
            string whiteLabelUrl = string.Empty;

            if (marketingCampaign != null && marketingCampaign.WhiteLabelSettings != null && marketingCampaign.Newsletter != null)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(marketingCampaign.WhiteLabelSettings.URL))
                    {
                        Uri uri = new Uri(marketingCampaign.WhiteLabelSettings.URL);
                        whiteLabelUrl = GetDomain(uri);

                        if (!whiteLabelUrl.EndsWith("/"))
                        {
                            whiteLabelUrl = whiteLabelUrl + "/";
                        }
                    }

                    var offers = await GetMarketingOfferListById(marketingCampaign.Newsletter.OfferListId);
                    if (offers != null && offers.Any())
                    {
                        offerListHtml = HtmlOfferList(offers, whiteLabelUrl);
                    }
                    htmlContent = _emailManager.CreateFromEmailTemplate(marketingCampaign.Newsletter.EmailTemplateId,
                        new
                        {
                            logo = $@"{whiteLabelUrl}_assets/images/whitelabels/{marketingCampaign.WhiteLabelSettings.Slug}/{marketingCampaign.WhiteLabelSettings.NewsletterLogo}",
                            whitelabelName = marketingCampaign.WhiteLabelSettings.Name,
                            offerList = offerListHtml,
                            url = whiteLabelUrl,
                        });

                    config.HtmlContent = htmlContent[(int)EmailTemplateSections.HTMLContent];
                    config.Subject = htmlContent[(int)EmailTemplateSections.Subject];
                    config.PlainContent = htmlContent[(int)EmailTemplateSections.BodyText];
                    config.SenderId = marketingCampaign.SenderId;
                    config.CustomUnsubscribeUrl = marketingCampaign.WhiteLabelSettings.URL;
                    //Send grid will generate plain text version if we don't provide
                    config.GeneratePlainContent = string.IsNullOrEmpty(htmlContent[(int)EmailTemplateSections.BodyText]);
                }
                catch(System.Exception ex)
                {
                    _logger.Error(ex, $"Unable to extract marketing email template for campaign {marketingCampaign.Id}");
                }
            }
            return config;
        }

        #endregion

        #region Updates against the provider

        public async Task<int> AddMissingContactLists()
        {
            int result = 0;
            string listExtension = "_ContactList";
#if DEBUG
            listExtension = "_ContactList_TEST";
#endif
            try
            {
                //Get a list of White labels from db (ignore deployment slot 1 and any that already have a contact list id)
                List<db.WhiteLabelSettings> dbWhiteLabels = _whiteLabel.FilterNoTrack(x => !x.DisplayName.Contains("slot") &&
                                                                                      !string.IsNullOrWhiteSpace(x.Name) &&
                                                                                      (x.MarketingContactLists == null || !x.MarketingContactLists.Any(m => m.ContactListName.EndsWith(listExtension)))
                                                                                     ).ToList();
                //Get all existing contact lists from sendgrid
                var providerContactLists = await _marketingProvider.GetLists();

                foreach (var whitelabel in dbWhiteLabels)
                {
                    try
                    {

                        var providerList = providerContactLists.marketingListResponse.FirstOrDefault(x => x.Name == $"{whitelabel.Name.Replace(" ", "")}{listExtension}");

                        if (providerList == null)
                        {
                            //create list on provider
                            provider.Models.MarketingList marketingList = new provider.Models.MarketingList();
                            marketingList.Name = $"{whitelabel.Name.Replace(" ", "")}{listExtension}";
                            providerList = await _marketingProvider.CreateLists(marketingList);
                            //create a list in the database
                            db.MarketingContactList dblist = new db.MarketingContactList()
                            {
                                WhiteLabelId = whitelabel.Id,
                                ContactListReference = providerList.Id,
                                ContactListName = providerList.Name
                            };
                            _marketingContactList.Create(dblist);
                            _marketingContactList.SaveChanges();

                            result++;
                        }

                    }
                    catch (System.Exception ex)
                    {
                        _logger.Error(ex, $"Unable to add missing contact list for {whitelabel.Name}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Add missing contact lists process failed");
            }
            return result;
        }

        public async Task<int> RemoveOptedOutContacts()
        {
            int result = 0;
            try
            {
                var dbremoveContacts = _customer.Include(c => c.ContactDetail)
                                                .Where( c => !c.MarketingNewsLetter &&
                                                             !string.IsNullOrWhiteSpace(c.ContactDetail.EmailAddress)
                                                      ).ToList();

                //Check the provider to see if any of these contacts exist
                var checkforThese = await _marketingProvider.SearchContactsByEmail(dbremoveContacts.Select(x => x.ContactDetail.EmailAddress).ToList());
                var providerContacts = _mapper.Map<List<provider.Models.MarketingContactId>>(checkforThese.SearchResult);

                if (providerContacts != null && providerContacts.Any())
                {
                    //Delete contacts from provider's system
                    bool success = await _marketingProvider.DeleteContact(providerContacts);
                    if (!success)
                        throw new System.Exception("Unable to remove contacts from provider's system");

                }

                try
                {
                    //Delete any contact records from database
                    var dbTidyUpContacts = _marketingContact.FilterNoTrack(x => x.Customer.MarketingNewsLetter == false &&
                                                                            !String.IsNullOrWhiteSpace(x.ContactReference)).ToList();
                    foreach (var dbContact in dbTidyUpContacts)
                    {
                        _marketingContact.Delete(dbContact);
                    }
                    _marketingContact.SaveChanges();
                }
                catch (System.Exception ex)
                {
                    _logger.Error(ex, $"Unable to delete marketing contacts from the database");
                }

                result = providerContacts.Count;

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Remove marketing contacts process failed");
            }
            return result;
        }
        public async Task<bool> RemoveMarketingContact(string email)
        {
            bool result = false;
            try
            {
                var checkforThese = await _marketingProvider.SearchContactsByEmail(new List<string>() { email });
                var providerContacts = _mapper.Map<List<provider.Models.MarketingContactId>>(checkforThese.SearchResult);

                if (providerContacts != null && providerContacts.Any())
                {
                    //Delete contacts from provider's system
                    bool success = await _marketingProvider.DeleteContact(providerContacts);
                    if (!success)
                        throw new System.Exception("Unable to remove contacts from provider's system");

                }

                //Delete marketing contact record from database
                if (providerContacts != null && providerContacts.Any())
                {
                    var dbTidyUpContact = _marketingContact.Filter(x => x.ContactReference == providerContacts[0].Id).FirstOrDefault();
                    if(dbTidyUpContact != null && dbTidyUpContact.Id > 0)
                    {
                        _marketingContact.Delete(dbTidyUpContact);
                    }
                    _marketingContact.SaveChanges();
                }
                result = true;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, $"Unable to delete marketing contact");
            }

            return result;
        }

        public async Task<int> AddOptedInContacts()
        {
            int result = 0;
            try
            {
                //Find all eligable customer records that don't seem to already have a record on the provider
                var dbCustomers = _customer.Include(c => c.ContactDetail)
                                             .Where(c => !c.IsDeleted && c.IsActive && c.MarketingNewsLetter &&
                                                   (c.SendGridContact == null || string.IsNullOrWhiteSpace(c.SendGridContact.ContactReference)) &&
                                                    c.MembershipCards.Count > 0 &&
                                                    c.MembershipCards.Any(m => m.IsActive &&
                                                                          !m.IsDeleted &&
                                                                          m.ValidTo >= DateTime.UtcNow &&
                                                                          m.MembershipStatus.Name == MembershipCardStatus.Active.ToString()
                                                                         ) &&
                                                                         !string.IsNullOrWhiteSpace(c.ContactDetail.EmailAddress)
                                                   ).ToList();

                if (dbCustomers != null && dbCustomers.Any())
                {
                    // Map our customer data into the provider contact data structure
                    var providerContactData = _mapper.Map<List<provider.Models.MarketingContact>>(dbCustomers);
                    provider.Models.SearchResponse providerContacts = await _marketingProvider.SearchContact(providerContactData);
                    if (providerContacts != null && providerContacts.SearchResult != null && providerContacts.SearchResult.Any())
                    {
                        //Step 1 - Update our records from the provider
                        AddExistingProviderContacts(dbCustomers, providerContacts.SearchResult);
                    }

                    //Step 2 - Add any missing customer records to the provider
                    var newCustomerData = new List<db.Customer>();
                    await Task.WhenAll(dbCustomers.Select(async (customer) =>
                    {
                        if (providerContacts.SearchResult.All(x => x.Email.ToLower() != customer.ContactDetail?.EmailAddress.ToLower()))
                        {
                            newCustomerData.Add(customer);
                        }
                        await Task.CompletedTask;
                    }));
                    Dictionary<string, List<provider.Models.MarketingContact>> sortedContacts = SortContacts(newCustomerData);
                    foreach (var contactListKey in sortedContacts.Keys)
                    {
                        provider.Models.CreateContactsRequest remainingRequest = new provider.Models.CreateContactsRequest
                        {
                            ContactListIds = new List<string> { contactListKey },
                            Contacts = sortedContacts[contactListKey].ToList()
                        };
                        var contactResponse = await _marketingProvider.CreateContact(remainingRequest);
                        if (contactResponse != null && contactResponse.errors.Count == 0)
                        {
                            result += remainingRequest.Contacts.Count;
                            // Just a job Id will return!
                            // we don't get the contact record id's back
                            // and there's no eta for when the batch job will be ran
                        }
                        else
                        {
                            var errorMessage = contactResponse.errors.Select(x => x.Message).FirstOrDefault();
                            _logger.Error("Error in CreateContact on Provider: {0}", errorMessage);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Add marketing contacts process failed");
            }

            return result;
        }

        public async Task<int> RemoveOptedOutCampaigns()
        {
            int result = 0;

            try
            {
                var dbRemoveCampaigns = _marketingCampaign.Filter(x => (!x.Enabled || !x.Newsletter.Enabled ) &&
                                                                        !string.IsNullOrWhiteSpace(x.CampaignReference));

                if (dbRemoveCampaigns != null && dbRemoveCampaigns.Any())
                {
                    foreach (var campaign in dbRemoveCampaigns)
                    {
                        //Delete existing event from provider
                        bool removed = await _marketingProvider.DeleteMarketingEvent(campaign.CampaignReference);
                        if (removed)
                        {
                            campaign.CampaignReference = null;
                            _marketingCampaign.Update(campaign);
                            _marketingCampaign.SaveChanges();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Remove opted out campaigns process failed");
            }

            return result;
        }

        public async Task<int> AddOptedInCampaigns()
        {
            int result = 0;
            try
            {
                var dbAddCampaigns = _marketingCampaign.Include(x => x.WhiteLabelSettings)
                                                       .Where(x => x.Enabled &&
                                                                   x.Newsletter.Enabled).ToList();


                if (dbAddCampaigns != null && dbAddCampaigns.Any())
                {
                    foreach (var campaign in dbAddCampaigns)
                    {

                        var newMarketingEvent = await GetMarketingEvent(campaign);
                        provider.Models.MarketingEventResponses marketingResponse = null;

                        //Update Process >>
                        if (!string.IsNullOrWhiteSpace(campaign.CampaignReference))
                        {
                            marketingResponse = await _marketingProvider.UpdateMarketingEvent(newMarketingEvent, campaign.CampaignReference);
                            if (marketingResponse.Status != "draft")
                            {
                                //Must have been sent already or something so need to delete the old event
                                await _marketingProvider.DeleteMarketingEvent(campaign.CampaignReference);
                            }
                        }
                        //Create Process >>
                        if (marketingResponse == null || marketingResponse.Status != "draft")
                        {
                            //Creating new event or the update failed
                            marketingResponse = await _marketingProvider.CreateMarketingEvent(newMarketingEvent);
                            if (marketingResponse != null && !string.IsNullOrWhiteSpace(marketingResponse.Id))
                            {
                                campaign.CampaignReference = marketingResponse.Id;
                                campaign.CampaignName = marketingResponse.Name;
                                _marketingCampaign.Update(campaign);
                                _marketingCampaign.SaveChanges();
                            }
                        }
                        //Schedule Process >>
                        if (marketingResponse != null && marketingResponse.Status == "draft")
                        {
                            await _marketingProvider.ScheduleMarketingEvent(newMarketingEvent, marketingResponse.Id);
                            result++;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Add opted in campaigns process failed");
            }
            return result;
        }

        #endregion

        #region private

        /// <summary>
        /// Should not be here but offermanager is currently CRUD
        /// </summary>
        async Task<IList<dtos.Admin.MarketingOfferSummary>> GetMarketingOfferListById(int offerListId)
        {
            var dbResults = _offerListItem.Include(x => x.Offer,
                                                    x => x.Offer.Merchant,
                                                    x => x.Offer.Merchant.MerchantImages
                                                  ).Where(x => x.OfferListId == offerListId &&
                                                          x.Offer.Status.Id == (int)OfferStatus.Active &&
                                                          x.Offer.Status.IsActive
                                                         ).OrderBy(x => x.DisplayOrder).ToList();

            IList<dtos.Admin.MarketingOfferSummary> dtoResults = _mapper.Map<IList<dtos.Admin.MarketingOfferSummary>>(dbResults);

            return await Task.FromResult(dtoResults);
        }

        /// <summary>
        /// Sorts new marketing customers into the correct contact lists
        /// </summary>
        /// <param name="newCustomerData"></param>
        /// <returns></returns>
        Dictionary<string, List<provider.Models.MarketingContact>> SortContacts(List<db.Customer> newCustomerData)
        {
            Dictionary<string, List<provider.Models.MarketingContact>> dctCustomers = new Dictionary<string, List<provider.Models.MarketingContact>>();
            try
            {
                var contactLists = _marketingContactList.FilterNoTrack(x => x.WhiteLabelId > 0);
                //var contactLists = _marketingContactList.Include(x=>x.WhiteLabelSettings).Where(x => x.WhiteLabelId > 0 && x.WhiteLabelSettings != null);

                foreach (var customer in newCustomerData)
                {
                    //find Original membership card
                    var membershipCard = _membershipManager.GetOriginalMembershipCard(customer.Id);

                    if (membershipCard != null) //<< Member might not have any active cards
                    {
                        //find membership plan
                        var membershipPlan = _membershipManager.GetMembershipPlan(membershipCard.MembershipPlanId);
                        //find whitelabel id (from plan)
                        var contactlist = contactLists.FirstOrDefault(x => x.WhiteLabelId == membershipPlan.WhitelabelId);
                        if (contactlist != null)
                        {
                            //find matching contact list for whitelabel
                            if (!dctCustomers.ContainsKey(contactlist.ContactListReference))
                            {
                                dctCustomers.Add(contactlist.ContactListReference, new List<provider.Models.MarketingContact>());
                            }
                            //add customer
                            var mc = _mapper.Map<provider.Models.MarketingContact>(customer);
                            dctCustomers[contactlist.ContactListReference].Add(mc);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "SortContacts process failed");
            }
            return dctCustomers;
        }

        void AddExistingProviderContacts(List<db.Customer> customers, List<provider.Models.SearchResult> searchResults)
        {
            if (searchResults?.Count > 0 && customers?.Count > 0)
            {
                foreach (var customer in customers)
                {
                    var result = searchResults.FirstOrDefault(x => x.Email.ToLower() == customer.ContactDetail.EmailAddress.ToLower());
                    if (result != null)
                    {
                        db.MarketingContact contact = _marketingContact.Get(x => x.ExclusiveCustomerId == customer.Id);
                        if (contact != null)
                        {
                            contact.ContactReference = result.Id;
                            _marketingContact.Update(contact);
                        }
                        else
                        {
                            contact = new db.MarketingContact
                            {
                                ExclusiveCustomerId = customer.Id,
                                ContactReference = result.Id
                            };
                            _marketingContact.Create(contact);
                        }
                        _marketingContact.SaveChanges();
                    }
                }
            }
        }

        #region Marketing Event (email)

        async Task<provider.Models.MarketingEvent> GetMarketingEvent(db.MarketingCampaign marketingCampaign)
        {
            provider.Models.MarketingEvent result = null;

            if (marketingCampaign != null && marketingCampaign.WhiteLabelId > 0)
            {
                db.MarketingContactList contactList = _marketingContactList.FilterNoTrack(x => x.WhiteLabelId == marketingCampaign.WhiteLabelId).FirstOrDefault();
                if (contactList != null)
                {
                    db.Newsletter newsletter = _newsletter.GetById(marketingCampaign.NewsletterId);
                    if (newsletter != null)
                    {
                        var emailconfig = await EmailForCampaign(marketingCampaign.Id);
                        result = new provider.Models.MarketingEvent()
                        {
                            SendAt = NextMarketingEvent(newsletter.Schedule).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            EmailConfig = emailconfig,
                            SendTo = new provider.Models.MarketingEventSendTo() { ListIds = new List<string>() { contactList.ContactListReference } },
                            Name = marketingCampaign.CampaignName
                        };
                    }
                }
            }

            return result;
        }

        DateTime NextMarketingEvent(string schedule)
        {
            DateTime result = new DateTime();

            if (!string.IsNullOrWhiteSpace(schedule))
            {
                if(schedule.EndsWith('7'))
                    //Sunday should be zero not 7 so try to fix it...
                    schedule = schedule.Substring(0, schedule.Length - 1) + "0";

                var scheduleDateTime = CrontabSchedule.TryParse(schedule);

                if (scheduleDateTime == null)
                    //Use default (sat 15:03) or log error?
                    scheduleDateTime = CrontabSchedule.Parse("3 15 * * 6");

                //Try to handle same day updates...
                result = scheduleDateTime.GetNextOccurrence(DateTime.UtcNow.AddDays(-1));
                if (result < DateTime.UtcNow)
                    result = scheduleDateTime.GetNextOccurrence(DateTime.UtcNow);
            }

            return result;
        }

        string GetDomain(Uri url)
        {
            if (url.HostNameType == UriHostNameType.Dns)
            {
                string scheme = url.Scheme;
                string host = url.Host;
                Regex tstex = new Regex(@"[A-Za-z0-9_%-]+\.(com|co.uk)", RegexOptions.IgnoreCase);
                Match match = tstex.Match(host);
                return $"{scheme}://{match}";
            }

            return url.ToString();
        }

        string HtmlOfferList(IList<dtos.Admin.MarketingOfferSummary> list, string baseWebsiteUrl)
        {
            string html = "";
            try
            {
                StringBuilder sb = new StringBuilder();
                string offerHtml = @"<tr style='padding: 0; text-align: left; vertical-align: top;'>
                    <td class='c_offer__image' valign='top' style='-moz-hyphens: auto; -webkit-hyphens: auto; border-collapse: collapse !important; color: #616161; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 0;padding-bottom: 30px; text-align: left; vertical-align: top; width: 160px; word-wrap: break-word;'><a href = '[url]Offers?country={8}&merchantId={6}&offerId={7}' style='color: #6b6b6b; display: block; text-decoration: none;'><img src = '{0}{1}' alt='{2}' style='-ms-interpolation-mode: bicubic; border: none; border-radius: 0; clear: both; display: block; height: 160px; line-height: 100%; margin: 0; max-width: 100%; outline: none; padding: 0; text-decoration: none; width: 160px;'></a></td>
                    <td class='c_offer__content' valign='top' style='-moz-hyphens: auto; -webkit-hyphens: auto; border-collapse: collapse !important; color: #616161; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 0; padding-left: 25px; padding-top: 15px; text-align: left; vertical-align: top; word-wrap: break-word;'>
                    <p class='c_offer__merchant' style='color: #6b6b6b; display: block; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 14.286px; font-style: normal; font-weight: 700; letter-spacing: normal; line-height: 120%; margin: 0 0 16px; margin-bottom: 5px; text-align: left;'>{3}</p>
                    <h2 class='c_offer__title' style='color: #2b2b2b; display: block; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 22.479px; font-style: normal; font-weight: 700; letter-spacing: normal; line-height: 120%; margin: 0; margin-bottom: 10px; text-align: left;'><a href='[url]Offers?country={8}&merchantId={6}&offerId={7}' style='color: #2b2b2b; text-decoration: none;'>{4}</a></h2>
                    <p class='c_offer__description' style='color: #616161; display: block; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 14.286px; font-style: normal; font-weight: 400; letter-spacing: normal; line-height: 170%; margin: 0; text-align: left;'>{5}</p>
                    </td></tr>";
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        sb.AppendFormat(offerHtml, baseWebsiteUrl, "Image/GetImage?path=" + item.ImagePath,
                         item.OfferShortDescription
                         , item.MerchantName
                         , item.Heading
                         , item.OfferLongDescription
                         , item.MerchantId
                         , item.OfferId
                         , item.CountryCode);
                    }
                }
                html = sb.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return html;
        }

        #endregion

        #endregion

    }
}
