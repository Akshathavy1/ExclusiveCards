using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class MembershipCardAffiliateReferenceManager : IMembershipCardAffiliateReferenceManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public MembershipCardAffiliateReferenceManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task<MembershipCardAffiliateReference> Add(MembershipCardAffiliateReference membershipCardAffiliateReference)
        {
            DbSet<MembershipCardAffiliateReference> membershipCardAffiliateReferences = _ctx.Set<MembershipCardAffiliateReference>();
            membershipCardAffiliateReferences.Add(membershipCardAffiliateReference);
            await _ctx.SaveChangesAsync();
            return membershipCardAffiliateReference;
        }

        public async Task<MembershipCardAffiliateReference> Update(MembershipCardAffiliateReference membershipCardAffiliateReference)
        {
            DbSet<MembershipCardAffiliateReference> membershipCardAffiliateReferences = _ctx.Set<MembershipCardAffiliateReference>();
            membershipCardAffiliateReferences.Update(membershipCardAffiliateReference);
            await _ctx.SaveChangesAsync();

            return membershipCardAffiliateReference;
        }

        public async Task<MembershipCardAffiliateReference> Get(int affiliateId, string reference)
        {
            return await _ctx.MembershipCardAffiliateReference
                    .Include(x => x.MembershipCard)
                    .ThenInclude(y => y.MembershipPlan)
                    .FirstOrDefaultAsync(x => x.AffiliateId == affiliateId && x.CardReference == reference);
        }

        public async Task<List<MembershipCardAffiliateReference>> GetAll()
        {
            return await _ctx.MembershipCardAffiliateReference
                .Include(x => x.MembershipCard)
                .ThenInclude(y => y.MembershipPlan)
                .ToListAsync();
        }
    }
}
