using ExclusiveCard.Providers.Marketing.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Providers.Marketing
{
    public interface IMarketingProvider
    {
        Task<ContactResponse> CreateContact(CreateContactsRequest request);

        Task<CustomFieldResponse> CreateCustomFields(CustomFieldRequestModel requestModel);

        Task<CustomFieldResponseList> GetCustomFields();

        Task<string> GetSenders();

        Task<MarketingListResponse> CreateLists(MarketingList list);

        Task<MarketingListResponseBulk> GetLists(int pageSize = 100);

        Task<MarketingListResponse> GetListById(string id);

        Task<SearchResponse> SearchContact(List<MarketingContact> name);

        /// <summary>
        /// Search all contacts on the provider for matching email addresses
        /// </summary>
        /// <param name="emailAddreses"></param>
        /// <returns></returns>
        Task<SearchResponse> SearchContactsByEmail(List<string> emailAddreses);

        Task<bool> DeleteContact(List<MarketingContactId> request);

        Task<bool> DeleteContact(MarketingContactId request);
        Task<CampaignResponse> CreateCampaign(CreateCampaign request);
        Task<CampaignResponse> UpdateCampaign(CreateCampaign request);
        Task<ScheduleResponse> CreateCampaignSchedules(CampaignSchedule campaign, int? campaignId);

        //single send
        Task<MarketingEventResponses> CreateMarketingEvent(MarketingEvent request);
        Task<MarketingEventResponses> UpdateMarketingEvent(MarketingEvent request, string senderId);
        Task<MarketingEventResponses> ScheduleMarketingEvent(MarketingEvent request, string senderId);
        Task<bool> DeleteMarketingEvent(string senderId);
        Task<int> GetWhiteLabelContactsCount(string id);

        #region new stuff


        #endregion
    }
}
