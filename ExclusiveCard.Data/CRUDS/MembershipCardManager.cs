using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class MembershipCardManager : IMembershipCardManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public MembershipCardManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        public async Task<MembershipCard> Add(MembershipCard membershipCard, string prefix, string digits, string suffix)
        {
            StringBuilder newCardNumber = new StringBuilder();
            int i = 1;//starting card number 00001 with country code reset for e.g : EX0000001GB , EX0000001SC
                      //using transaction scope to avoid duplicate cardNumber issued.
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //checking if membershipCard.CardNumber is null or empty.
                    if (string.IsNullOrEmpty(membershipCard.CardNumber))
                    {
                        //Get lastMembershipCard record for increment by 1 and store it to variable.
                        var lastMembershipCard = await _ctx.MembershipCard.OrderByDescending(x => x.CardNumber)
                            .FirstOrDefaultAsync(x => x.CardNumber.Contains(suffix));
                        if (!string.IsNullOrEmpty(prefix))
                        {
                            newCardNumber.Append(prefix);
                        }

                        if (lastMembershipCard != null)
                        {
                            var oldCardNumber =
                                lastMembershipCard.CardNumber.Substring(2, lastMembershipCard.CardNumber.Length - 4);
                            i = Convert.ToInt32(oldCardNumber) + 1;
                            newCardNumber.Append(i.ToString(digits));
                        }
                        else
                        {
                            newCardNumber.Append(i.ToString(digits));
                        }

                        if (!string.IsNullOrEmpty(suffix))
                        {
                            newCardNumber.Append(suffix);
                        }
                        membershipCard.CardNumber = newCardNumber.ToString();
                    }
                    DbSet<MembershipCard> membershipCards = _ctx.Set<MembershipCard>();
                    var entry = _ctx.Entry(membershipCard);
                    if (entry.State == EntityState.Detached)
                    {
                        _ctx.Set<MembershipCard>().Attach(membershipCard);
                    }
                    membershipCards.Add(membershipCard);
                    await _ctx.SaveChangesAsync();
                    entry.State = EntityState.Detached;
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
                finally
                {
                    scope.Complete();
                }
            }

            return membershipCard;
        }

        public async Task<MembershipCard> Update(MembershipCard membershipCard)
        {
            DbSet<MembershipCard> membershipCards = _ctx.Set<MembershipCard>();
            var entry = _ctx.Entry(membershipCard);
            if (entry.State == EntityState.Detached)
            {
                _ctx.Set<MembershipCard>().Attach(membershipCard);
            }
            membershipCards.Update(membershipCard);
            await _ctx.SaveChangesAsync();
            entry.State = EntityState.Detached;

            return membershipCard;
        }

        public async Task<MembershipCard> UpdatePhysicalCardDetailsAsync(int id, bool physicalCardRequested,
            int physicalCardStatus)
        {
            MembershipCard existingMembershipCard;
            existingMembershipCard = await _ctx.MembershipCard.FirstOrDefaultAsync(x => x.Id == id);
            if (existingMembershipCard != null)
            {
                var entry = _ctx.Entry(existingMembershipCard);
                if (entry.State == EntityState.Detached)
                {
                    _ctx.Set<MembershipCard>().Attach(existingMembershipCard);
                }

                existingMembershipCard.PhysicalCardRequested = physicalCardRequested;
                existingMembershipCard.PhysicalCardStatusId = physicalCardStatus;
                _ctx.MembershipCard.Update(existingMembershipCard);
                await _ctx.SaveChangesAsync();
                entry.State = EntityState.Detached;
            }
            return existingMembershipCard;
        }

        public async Task<MembershipCard> DeleteAsync(MembershipCard membershipCard)
        {
            DbSet<MembershipCard> membershipCards = _ctx.Set<MembershipCard>();
            var entry = _ctx.Entry(membershipCard);
            if (entry.State == EntityState.Detached)
            {
                _ctx.Set<MembershipCard>().Attach(membershipCard);
            }
            membershipCards.Remove(membershipCard);
            await _ctx.SaveChangesAsync();
            entry.State = EntityState.Detached;

            return membershipCard;
        }

        public async Task<MembershipCard> Get(int id)
        {
            return await _ctx.MembershipCard.Include(x => x.PartnerReward)
                .Include(x => x.MembershipCardAffiliateReferences)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<MembershipCard> GetLastRecord()
        {
            return await _ctx.MembershipCard.AsNoTracking()
                .OrderByDescending(x => x.CardNumber).FirstOrDefaultAsync();
        }

        public async Task<List<MembershipCard>> GetAll(int customerId, bool onlyValidCards = false)
        {
            var query = _ctx.MembershipCard
                .Where(x => x.CustomerId == customerId);

            if (onlyValidCards)
            {
                var status = await _ctx.Status
                    .Where(y => y.IsActive && y.Type == Constants.StatusType.MembershipCard).ToListAsync();

                query = query.Where(x =>
                    x.StatusId == status.FirstOrDefault(z => z.Name == Constants.Status.Active).Id ||
                     x.StatusId == status.FirstOrDefault(z => z.Name == Constants.Status.Pending).Id);
            }

            return await query.AsNoTracking().OrderByDescending(x => x.DateIssued)
                .ToListAsync();
        }

        public MembershipCard GetByMembershipCard(string cardNumber)
        {
            return _ctx.MembershipCard.AsNoTracking()
                .Include(x => x.MembershipCardAffiliateReferences)
                .FirstOrDefault(x => x.CardNumber == cardNumber);
        }

        public MembershipCard GetPlanProviderByMembershipCardId(int id)
        {
            return _ctx.MembershipCard.Include(x => x.MembershipPlan).ThenInclude(y => y.MembershipPlanPaymentProvider)
                .AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<MembershipCard> GetByCustomerProviderId(string customerProviderId)
        {
            return await _ctx.MembershipCard.AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsActive && x.CustomerPaymentProviderId == customerProviderId);
        }

        public async Task<MembershipCard> GetByCustomerPlan(int customerId, int? planId)
        {
            MembershipCard membershipCard;
            IQueryable<MembershipCard> membershipQuery = _ctx.MembershipCard.OrderByDescending(x => x.ValidTo);

            if (planId.HasValue)
            {
                membershipQuery = membershipQuery.Where(x =>
                         x.CustomerId == customerId && x.MembershipPlanId == planId && x.IsActive && !x.IsDeleted);
            }
            else
            {
                membershipQuery = membershipQuery.Where(x =>
                        x.CustomerId == customerId && x.IsActive && !x.IsDeleted);
            }

            membershipCard = await membershipQuery.AsNoTracking().FirstOrDefaultAsync(x =>
                x.MembershipStatus.Type == Constants.StatusType.MembershipCard &&
                (x.MembershipStatus.Name == Constants.Status.Active ||
                 x.MembershipStatus.Name == Constants.Status.Pending));
            return membershipCard;
        }

        public async Task<List<MembershipCard>> GetCardsToSendOutAsync()
        {
            List<Status> status = await _ctx.Status
                .Where(y => y.IsActive && y.Type == Constants.StatusType.PhysicalCardStatus).ToListAsync();

            IQueryable<MembershipCard> query = _ctx.MembershipCard.Include(x => x.Customer)
                .Where(x => x.PhysicalCardRequested && x.PhysicalCardStatusId != null
                && (x.PhysicalCardStatusId == status.FirstOrDefault(y => y.Name == Constants.Status.Active).Id
                || x.PhysicalCardStatusId == status.FirstOrDefault(y => y.Name == Constants.Status.Requested).Id
                    || x.PhysicalCardStatusId == status.FirstOrDefault(y => y.Name == Constants.Status.TempCardIssued).Id));

            return await query.AsNoTracking().OrderBy(x => x.DateIssued).ToListAsync();
        }

        public async Task<MembershipCard> GetByAspNetUserId(string aspnetUserId)
        {
            try
            {
                //var customer = await _ctx.Customer.SingleOrDefaultAsync(x => x.IsActive && x.AspNetUserId == aspnetUserId);
                //if (customer == null)
                //    return null;

                var query = _ctx.MembershipCard.Include(x => x.MembershipPlan).ThenInclude(x => x.MembershipLevel)
                    .Where(x => x.IsActive && x.Customer.AspNetUserId == aspnetUserId);
                //var status = await _ctx.Status
                //    .Where(y => y.IsActive && y.Type == Constants.StatusType.MembershipCard).ToListAsync();
                query = query.Where(x => x.MembershipStatus.Name == Constants.Status.Active && x.MembershipStatus.Type == Constants.StatusType.MembershipCard);
                return await query.AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return null;
        }
    }
}
