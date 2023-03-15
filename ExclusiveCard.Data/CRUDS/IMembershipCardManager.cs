using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMembershipCardManager
    {
        Task<MembershipCard> Add(MembershipCard membershipCard, string prefix, string digits, string suffix);
        Task<MembershipCard> Update(MembershipCard membershipCard);

        Task<MembershipCard> UpdatePhysicalCardDetailsAsync(int id, bool physicalCardRequested,
            int physicalCardStatus);

        Task<MembershipCard> DeleteAsync(MembershipCard membershipCard);
        Task<MembershipCard> Get(int id);
        Task<MembershipCard> GetLastRecord();
        Task<List<MembershipCard>> GetAll(int customerId, bool onlyValidCards = false);
        MembershipCard GetByMembershipCard(string cardNumber);
        Task<MembershipCard> GetByCustomerProviderId(string customerProviderId);
        Task<MembershipCard> GetByCustomerPlan(int customerId, int? planId);
        Task<List<MembershipCard>> GetCardsToSendOutAsync();
        Task<MembershipCard> GetByAspNetUserId(string aspnetUserId);
        MembershipCard GetPlanProviderByMembershipCardId(int id);
    }
}