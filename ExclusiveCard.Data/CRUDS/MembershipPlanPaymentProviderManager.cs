using System.Collections.Generic;
using System.Linq;
using ExclusiveCard.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public class MembershipPlanPaymentProviderManager : IMembershipPlanPaymentProviderManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public MembershipPlanPaymentProviderManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        //Add MembershipPlanPaymentProvider
        public async Task<MembershipPlanPaymentProvider> AddAsync(MembershipPlanPaymentProvider membershipPlanPaymentProvider)
        {
            DbSet<MembershipPlanPaymentProvider> memberships = _ctx.Set<MembershipPlanPaymentProvider>();
            memberships.Add(membershipPlanPaymentProvider);
            await _ctx.SaveChangesAsync();
            return membershipPlanPaymentProvider;
        }

        //Update MembershipPlanPaymentProvider
        public async Task<MembershipPlanPaymentProvider> UpdateAsync(MembershipPlanPaymentProvider membershipPlanPaymentProvider)
        {
            DbSet<MembershipPlanPaymentProvider> memberships = _ctx.Set<MembershipPlanPaymentProvider>();
            memberships.Update(membershipPlanPaymentProvider);
            await _ctx.SaveChangesAsync();

            return membershipPlanPaymentProvider;
        }

        public async Task<List<MembershipPlanPaymentProvider>> Get(int membershipPlanId)
        {
            DbSet<MembershipPlanPaymentProvider> providers = _ctx.Set<MembershipPlanPaymentProvider>();
            return await providers
                .Include(x => x.MembershipPlan)
                .Include(x => x.PaymentProvider)
                .Where(x => x.MembershipPlanId == membershipPlanId).ToListAsync();
        }
    }
}
