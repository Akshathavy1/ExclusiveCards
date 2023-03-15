using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class MembershipRegistrationCodeManager : IMembershipRegistrationCodeManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public MembershipRegistrationCodeManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task<MembershipRegistrationCode> Add(MembershipRegistrationCode membershipRegistrationCode)
        {
            DbSet<MembershipRegistrationCode> membershipRegistrationCodes = _ctx.Set<MembershipRegistrationCode>();
            membershipRegistrationCodes.Add(membershipRegistrationCode);
            await _ctx.SaveChangesAsync();

            return membershipRegistrationCode;
        }

        public async Task<MembershipRegistrationCode> Update(MembershipRegistrationCode membershipRegistrationCode)
        {
            DbSet<MembershipRegistrationCode> membershipRegistrationCodes = _ctx.Set<MembershipRegistrationCode>();
            membershipRegistrationCodes.Update(membershipRegistrationCode);
            await _ctx.SaveChangesAsync();

            return membershipRegistrationCode;
        }

        public async Task<MembershipRegistrationCode> GetAsync(string code)
        {
            //1. Check the card is valid as in it is not deleted & is active && within date range
            //2. Check whether pending tokens are there within last one hour for given registration code
            //3. Number of cards given for the registration code should be less than the count allowed for the code
            //Check for Membership Plan valid from and valid to dates
            //If NumberOfCards = 0 then, u can create any number of cards
            return await _ctx.MembershipRegistrationCode.FirstOrDefaultAsync(x =>
                x != null && x.RegistartionCode == code && !x.IsDeleted && x.IsActive
                && DateTime.UtcNow >= x.ValidFrom && DateTime.UtcNow <= x.ValidTo && DateTime.UtcNow >= x.MembershipPlan.ValidFrom && DateTime.UtcNow <= x.MembershipPlan.ValidTo
                && (x.NumberOfCards == 0 || x.NumberOfCards > 0 && ((decimal.Add(x.MembershipPlan.MembershipCards.Count(y => y.MembershipPlanId == x.MembershipPlanId),
                        x.MembershipPendingTokens.Count(z => z.MembershipRegistrationCode.RegistartionCode == code
                                                             && z.DateCreated >= DateTime.UtcNow.AddHours(-1) && z.DateCreated <= DateTime.UtcNow))
                    < x.MembershipPlan.NumberOfCards) && decimal.Add(x.MembershipCards.Count, x.MembershipPendingTokens.Count(z =>
                    z.MembershipRegistrationCode.RegistartionCode == code && z.DateCreated >= DateTime.UtcNow.AddHours(-1) && z.DateCreated <= DateTime.UtcNow)) < x.NumberOfCards)));
        }

        public async Task<List<MembershipRegistrationCode>> GetAllAsync()
        {
            return await _ctx.MembershipRegistrationCode.ToListAsync();
        }
    }
}
