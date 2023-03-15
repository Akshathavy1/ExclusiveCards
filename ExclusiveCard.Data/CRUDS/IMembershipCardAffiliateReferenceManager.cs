using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMembershipCardAffiliateReferenceManager
    {
        Task<MembershipCardAffiliateReference> Add(MembershipCardAffiliateReference membershipCardAffiliateReference);

        Task<MembershipCardAffiliateReference>
            Update(MembershipCardAffiliateReference membershipCardAffiliateReference);

        Task<MembershipCardAffiliateReference> Get(int affiliateId, string reference);
        Task<List<MembershipCardAffiliateReference>> GetAll();
    }
}