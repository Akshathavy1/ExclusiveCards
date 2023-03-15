using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IMembershipCardService
    {
        #region Writes

        Task<dto.MembershipCard> Add(dto.MembershipCard membershipCard, string prefix, string digits, string suffix);
        Task<dto.MembershipCard> Update(dto.MembershipCard membershipCard);

        #endregion

        #region Reads

        Task<dto.MembershipCard> Get(int id);

        Task<dto.MembershipCard> GetLastRecord();

        Task<List<dto.MembershipCard>> GetAll(int customerId);

        dto.MembershipCard GetByMembershipCard(string cardNumber);

        Task<dto.MembershipCard> GetByCustomerProviderId(string customerProviderId);

        Task<dto.MembershipCard> GetByCustomerPlan(int customerId, int? planId);

        Task<dto.MembershipCard> GetByAspNetUserId(string aspNetUserId);

        dto.MembershipCard GetActiveMembershipCard(string aspNetUserId);

        dto.MembershipCard GetDiamondMembershipCard(string aspNetUserId);

        dto.MembershipCard GetPlanProviderByMembershipCardId(int id);

        #endregion
    }
}
