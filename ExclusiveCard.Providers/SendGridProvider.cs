using SendGrid;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;
using System.Net;
using ExclusiveCard.Providers.Marketing.Models;
using NLog;
using Newtonsoft.Json;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using RestSharp;
using System.Linq;
using ExclusiveCard.Providers.Marketing;
using ExclusiveCard.Providers.Email;

namespace ExclusiveCard.Providers
{
    public class SendGridProvider : IEmailProvider, IMarketingProvider
    {
        #region Private fields and constructor

        private readonly IConfiguration _configuration;
        private string _sendGridApiKey;
        private string _sendGridMarketingApiKey;
        private Logger _logger;

        private const short TO = 0;
        private const short CC = 1;
        private const short BCC = 2;

        public SendGridProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _sendGridApiKey = _configuration["SendGridAPIKey"];
            _sendGridMarketingApiKey = _configuration["SendGridMarketingAPIKey"];

            _logger = LogManager.GetLogger(GetType().FullName);
        }

        #endregion

        #region Public methods

        #region IEmailProvider

        public async Task<string> SendEmailAsync(dto.Email email)
        {
            string emailsent = false.ToString();

            var client = GetEmailClient();
            Response response = null;

            var msg = CreateEmailMsg(email.Subject, email.BodyPlainText, email.BodyHtml, email.EmailFrom, email.EmailFromName);

            foreach (var toMail in email.EmailTo)
            {
                AddRecipients(msg, toMail, TO);
            }

            if (email.EmailCC != null && email.EmailCC.Count > 0)
            {
                foreach (var mail in email.EmailCC)
                {
                    AddRecipients(msg, mail, CC);
                }
            }

            if (email.EmailBCC != null && email.EmailBCC.Count > 0)
            {
                foreach (var bccMail in email.EmailBCC)
                {
                    AddRecipients(msg, bccMail, BCC);
                }
            }

            response = await client.RequestAsync(method: SendGridClient.Method.POST, requestBody: msg.Serialize(), urlPath: "mail/send");
            var message = response.Body.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.Accepted)
                emailsent = true.ToString();
            else
            {
                _logger.Error(
                    $"Unable to send email to {email.EmailTo} response code= {response.StatusCode + await response.Body.ReadAsStringAsync()}");
                emailsent = "Could not send email.";
            }

            return emailsent;

        }

        #endregion

        #region IMarketingProvider

        public async Task<CustomFieldResponse> CreateCustomFields(CustomFieldRequestModel requestModel)
        {
            Task<string> message;
            CustomFieldResponse customFieldResponse = new CustomFieldResponse();
            try
            {
                Response response = null;
                var client = GetSendGridMarketingClient();
                var data = JsonConvert.SerializeObject(requestModel);

                response = await client.RequestAsync(method: SendGridClient.Method.POST, urlPath: "marketing/field_definitions", requestBody: data);

                var statusCode = response.StatusCode;
                message = response.Body.ReadAsStringAsync();

                customFieldResponse = JsonConvert.DeserializeObject<CustomFieldResponse>(message.Result);
            }
            catch (Exception ex)
            {
                var ErrorList = new List<Error>
                {
                    new Error
                    {
                        Message = ex.Message.ToString()
                    }
                };
                customFieldResponse.errors = ErrorList;
            }
            return customFieldResponse;
        }

        public async Task<CustomFieldResponseList> GetCustomFields()
        {
            Task<string> message ;
            CustomFieldResponseList customFieldResponseList = new CustomFieldResponseList();
            try
            {
                Response response = null;
                var client = GetSendGridMarketingClient();

                response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "marketing/field_definitions");

                var statusCode = response.StatusCode;
                message = response.Body.ReadAsStringAsync();

                customFieldResponseList = JsonConvert.DeserializeObject<CustomFieldResponseList>(message.Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                customFieldResponseList = null;
            }
            return customFieldResponseList;
        }

        public async Task<string> GetSenders()
        {
            string responseData = "";

            var client = GetSendGridMarketingClient();
            Response response = null;

            response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "marketing/senders");

            var statusCode = response.StatusCode;
            var message = response.Body.ReadAsStringAsync().Result;
            var headers = response.Headers.ToString();

            if (message != null && message != "")
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<object>(message);
                }
                catch (Exception ex)
                { var exmsg = ex.Message.ToString(); }
            }

            if (statusCode == HttpStatusCode.OK)
                responseData = "success";
            else
                responseData = "failure";

            return responseData;
        }


        public async Task<MarketingListResponse> CreateLists(MarketingList list)
        {
            Task<string> message;
            var marketingListResponse = new MarketingListResponse();
            try
            {
                Response response = null;
                var data = JsonConvert.SerializeObject(list);

                var client = GetSendGridMarketingClient();

                response = await client.RequestAsync(method: SendGridClient.Method.POST, urlPath: "marketing/lists", requestBody: data);

                var statusCode = response.StatusCode;
                message = response.Body.ReadAsStringAsync();

                marketingListResponse = JsonConvert.DeserializeObject<MarketingListResponse>(message.Result);
            }
            catch (Exception ex)
            {
                var ErrorList = new List<Error>
                {
                    new Error
                    {
                        Message =  ex.Message.ToString()
                    }
                };
                marketingListResponse.errors = ErrorList;
            }
            return marketingListResponse;
        }

        public async Task<MarketingListResponseBulk> GetLists(int pageSize = 100)
        {
            Task<string> message;
            var marketingListRespobseBulk = new MarketingListResponseBulk();
            try
            {
                Response response = null;
                var client = GetSendGridMarketingClient();

                response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "marketing/lists?page_size=" + pageSize);

                var statusCode = response.StatusCode;
                message = response.Body.ReadAsStringAsync();

                marketingListRespobseBulk = JsonConvert.DeserializeObject<MarketingListResponseBulk>(message.Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                marketingListRespobseBulk = null;
            }
            return marketingListRespobseBulk;
        }

        public async Task<MarketingListResponse> GetListById(string id)
        {
            Task<string> message;
            var marketingListResponse = new MarketingListResponse();
            try
            {
                Response response = null;
                var client = GetSendGridMarketingClient();

                response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "marketing/lists/" + id);

                var statusCode = response.StatusCode;
                message = response.Body.ReadAsStringAsync();

                marketingListResponse = JsonConvert.DeserializeObject<MarketingListResponse>(message.Result);
            }
            catch (Exception ex)
            {
                var ErrorList = new List<Error>
                {
                    new Error
                    {
                        Message =  ex.Message.ToString()
                    }
                };
                marketingListResponse.errors = ErrorList;
            }
            return marketingListResponse;
        }

        public async Task<ContactResponse> CreateContact(CreateContactsRequest request)
        {
            //Once created respond with True or False based on Response code 202 -- Success
            Task<string> message;
            ContactResponse contactResponse = new ContactResponse();
            try
            {
                Response response = null;
                var client = GetSendGridMarketingClient();
                var data = JsonConvert.SerializeObject(request);

                response = await client.RequestAsync(method: SendGridClient.Method.PUT, urlPath: "marketing/contacts", requestBody: data);
                message = response.Body.ReadAsStringAsync();
                var statusCode = response.StatusCode;

                contactResponse = JsonConvert.DeserializeObject<ContactResponse>(message.Result);
            }
            catch (Exception ex)
            {
                var ErrorList = new List<Error>
                {
                    new Error
                    {
                        Message =  ex.Message.ToString()
                    }
                };
                contactResponse.errors = ErrorList;
            }
            return contactResponse;
        }

        public async Task<SearchResponse> SearchContact(List<MarketingContact> request)
        {
            SearchResponse searchResponse = new SearchResponse();
            Task<string> message;
            try
            {
                Response response = null;
                var client = GetSendGridMarketingClient();
                StringBuilder sb = new StringBuilder();
                SearchContact search = new SearchContact();

                sb.Append("email IN (");
                sb.Append(string.Join(",", request.Select(x => string.Format("'{0}'", x.Email.ToLower()))));
                sb.Append(")");

                search.Query = sb.ToString();
                var data = JsonConvert.SerializeObject(search);

                response = await client.RequestAsync(method: SendGridClient.Method.POST, urlPath: "marketing/contacts/search", requestBody: data);
                message = response.Body.ReadAsStringAsync();

                var statusCode = response.StatusCode;

                searchResponse = JsonConvert.DeserializeObject<SearchResponse>(message.Result);
            }
            catch (Exception ex)
            {
                var ErrorList = new List<Error>
                {
                    new Error
                    {
                        Message = ex.Message.ToString()
                    }
                };
                searchResponse.errors = ErrorList;
            }
            return searchResponse;
        }

        public async Task<SearchResponse> SearchContactsByEmail(List<string> emailAddreses)
        {
            SearchResponse searchResponse = new SearchResponse();
            Task<string> message;
            try
            {
                Response response = null;
                var client = GetSendGridMarketingClient();
                StringBuilder sb = new StringBuilder();
                SearchContact search = new SearchContact();

                sb.Append("email IN (");
                sb.Append(string.Join(",", emailAddreses.Select(x => string.Format("'{0}'", x.ToLower()))));
                sb.Append(")");

                search.Query = sb.ToString();
                var data = JsonConvert.SerializeObject(search);

                response = await client.RequestAsync(method: SendGridClient.Method.POST, urlPath: "marketing/contacts/search", requestBody: data);
                message = response.Body.ReadAsStringAsync();

                var statusCode = response.StatusCode;

                searchResponse = JsonConvert.DeserializeObject<SearchResponse>(message.Result);
            }
            catch (Exception ex)
            {
                var ErrorList = new List<Error>
                {
                    new Error
                    {
                        Message = ex.Message.ToString()
                    }
                };
                searchResponse.errors = ErrorList;
            }
            return searchResponse;
        }

        // API - List of contacts to delete
        // Expects a list of SendGrid contact id's to be deleted
        public async Task<bool> DeleteContact(List<MarketingContactId> request)
        {
            Task<string> message;
            Response response = null;
            try
            {
                var client = GetSendGridMarketingClient();

                var ids = string.Join(",", request.Where(w => !string.IsNullOrWhiteSpace(w.Id)).Select(x => x.Id));
                var deleteContactQuery = new DeleteContactQuery { Ids = ids };
                var data = JsonConvert.SerializeObject(deleteContactQuery);

                response = await client.RequestAsync(method: SendGridClient.Method.DELETE, urlPath: "marketing/contacts", queryParams: data);

                message = response.Body.ReadAsStringAsync();
                var statusCode = response.StatusCode;
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        // API - Single Contact to Delete
        public async Task<bool> DeleteContact(MarketingContactId request)
        {
            Task<string> message;
            Response response = null;
            try
            {
                var client = GetSendGridMarketingClient();

                var ids = request.Id;
                var deleteContactQuery = new DeleteContactQuery { Ids = ids };
                var data = JsonConvert.SerializeObject(deleteContactQuery);

                response = await client.RequestAsync(method: SendGridClient.Method.DELETE, urlPath: "marketing/contacts", queryParams: data);

                message = response.Body.ReadAsStringAsync();
                var statusCode = response.StatusCode;
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        #endregion

        #endregion

        #region Private methods

        #region IEmailProvider

        private SendGridMessage CreateEmailMsg(string subject, string bodyPlainText, string bodyHtml, string fromEmail, string fromName)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = subject,
                PlainTextContent = bodyPlainText,
                HtmlContent = bodyHtml
            };

            return msg;
        }

        private void AddRecipients(SendGridMessage msg, string recipients, short field)
        {
            if (!string.IsNullOrWhiteSpace(recipients))
            {
                string[] recipientArray = recipients.Split(';');

                foreach (var recipient in recipientArray)
                {
                    var emailAddress = new EmailAddress(recipient);
                    switch (field)
                    {
                        case TO:
                            msg.AddTo(emailAddress);
                            break;

                        case CC:
                            msg.AddCc(emailAddress);
                            break;

                        case BCC:
                            msg.AddBcc(emailAddress);
                            break;
                    }

                }
            }


        }

        private SendGridClient GetEmailClient()
        {
            var client = new SendGridClient(_sendGridApiKey);
            return client;
        }

        #endregion

        #region IMarketingProvider


        //TODO - Confirm if CreateCampaign calls are redundant.
        public async Task<CampaignResponse> CreateCampaign(CreateCampaign campaign)
        {
       
            CampaignResponse res = new CampaignResponse();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            var json = JsonConvert.SerializeObject(campaign);
            var data = json.ToString();
            var response = await client.RequestAsync(method: SendGridClient.Method.POST, urlPath: "campaigns", requestBody: data);
            message = response.Body.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<CampaignResponse>(message.Result);
            }
            else
            {
                _logger.Error(
                    $"Unable create campaign and response code= {response.StatusCode}");
                res = null;
            }

            return res;

        }
        //TODO - Confirm if UpdateCampaign calls are redundant.
        public async Task<CampaignResponse> UpdateCampaign(CreateCampaign request)
        {
            CampaignResponse res = new CampaignResponse();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            var json = JsonConvert.SerializeObject(request);
            var data = json.ToString();
            var response = await client.RequestAsync(method: SendGridClient.Method.PATCH, urlPath: "campaigns/"+ request.id, requestBody: data);
            message = response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<CampaignResponse>(message.Result);
            }
            else
            {
                _logger.Error(
                    $"Unable update campaign and response code= {response.StatusCode}");
                res = null;
            }

            return res;
        }

        public async Task<ScheduleResponse> CreateCampaignSchedules(CampaignSchedule campaign,int? campaignId)
        {
            ScheduleResponse res = new ScheduleResponse();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            var json = JsonConvert.SerializeObject(campaign);
            var data = json.ToString();
            var response = await client.RequestAsync(method: SendGridClient.Method.POST, urlPath: "campaigns/" + campaignId + "/schedules", requestBody: data);
            message = response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<ScheduleResponse>(message.Result);
            }
            else
            {
                _logger.Error(
                    $"Unable create campaign Schedules and response code= {response.StatusCode}");
                res = null;
            }
            return res;
        }

        public async Task<MarketingEventResponses> CreateMarketingEvent(MarketingEvent request)
        {
            MarketingEventResponses res = new MarketingEventResponses();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            var json = JsonConvert.SerializeObject(request);
            var data = json.ToString();
            var response = await client.RequestAsync(method: SendGridClient.Method.POST, urlPath: "marketing/singlesends", requestBody: data);
            message = response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.Created)
            {
                res = JsonConvert.DeserializeObject<MarketingEventResponses>(message.Result);
            }
            else
            {
                _logger.Error(
                    $"Unable create Single Send and response code= {response.StatusCode}, {message.Result}");
                res = null;
            }
            return res;
        }

        public async Task<MarketingEventResponses> UpdateMarketingEvent(MarketingEvent request,string senderId)
        {
            MarketingEventResponses res = new MarketingEventResponses();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            var json = JsonConvert.SerializeObject(request);
            var data = json.ToString();
            var response = await client.RequestAsync(method: SendGridClient.Method.PATCH, urlPath: "marketing/singlesends/"+ senderId, requestBody: data);
            message = response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<MarketingEventResponses>(message.Result);
            }
            else
            {
                _logger.Error(
                    $"Unable create Single Send and response code= {response.StatusCode}");
                res = JsonConvert.DeserializeObject<MarketingEventResponses>(message.Result); 
            }
            return res;
        }


        public async Task<MarketingEventResponses> ScheduleMarketingEvent(MarketingEvent request, string senderId)
        {
            MarketingEventResponses res = new MarketingEventResponses();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            var json = JsonConvert.SerializeObject(request);
            var data = json.ToString();
            var response = await client.RequestAsync(method: SendGridClient.Method.PUT, urlPath: "marketing/singlesends/" + senderId + "/schedule", requestBody: data);
            message = response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<MarketingEventResponses>(message.Result);
            }
            else
            {
                _logger.Error(
                    $"Unable create schedule for Single Send and response code= {response.StatusCode}");
                res = null;
            }
            return res;
        }

        public async Task<bool> DeleteMarketingEvent(string senderId)
        {
            bool result = false;
            MarketingEventResponses res = new MarketingEventResponses();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            //var json = JsonConvert.SerializeObject(request);
            //var data = json.ToString();
            var response = await client.RequestAsync(method: SendGridClient.Method.DELETE, urlPath: "marketing/singlesends/" + senderId);
            message = response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                result = true;
            }
            else
            {
                _logger.Error(
                    $"Unable delete Single Send and response code= {response.StatusCode}");
                result = false;
            }
            return result;
        }

        public async Task<int> GetWhiteLabelContactsCount(string id)
        {
            int count =0;
            ContactsCount res = new ContactsCount();
            Task<string> message;
            var client = GetSendGridMarketingClient();
            var response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "marketing/lists/" + id + "/contacts/count");
            message = response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<ContactsCount>(message.Result);
                if (res !=null)
                {
                    count = res.Count;
                }
                else
                {
                    count = 0;
                }
            }
            else
            {
                count = 0;
            }
            return count;
        }

        private SendGridClient GetSendGridMarketingClient()
        {
            var client = new SendGridClient(_sendGridMarketingApiKey);
            return client;
        }

        #endregion

        #endregion
    }
}
