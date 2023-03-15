using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IMembershipCardService
    {
        #region Writes

        Task<MembershipCard> UpdatePhysicalCardDetailsAsync(int membershipCardId, bool physicalCardRequested, int physicalCardStatusId);

        Task<MembershipCard> DeleteAsync(MembershipCard membershipCard);

        #endregion

        #region Reads

        Task<List<MembershipCard>> GetMembershipCardsForCustomerAsync(int customerId, bool onlyValidCards);

        Task<List<MembershipCard>> GetCardsToSendOutAsync();

        #endregion
    }
}
