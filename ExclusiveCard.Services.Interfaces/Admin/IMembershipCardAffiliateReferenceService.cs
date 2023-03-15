using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
   public interface IMembershipCardAffiliateReferenceService
    {
        #region Writes

        Task<MembershipCardAffiliateReference> Add(MembershipCardAffiliateReference membershipCard);
        Task<MembershipCardAffiliateReference> Update(MembershipCardAffiliateReference membershipCard);

        #endregion

        #region Reads

        Task<MembershipCardAffiliateReference> Get(int affiliateId, string reference);

        Task<List<MembershipCardAffiliateReference>> GetAll();

        #endregion
    }
}
