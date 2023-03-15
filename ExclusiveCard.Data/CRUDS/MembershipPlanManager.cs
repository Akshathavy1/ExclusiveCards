using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class MembershipPlanManager : IMembershipPlanManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public MembershipPlanManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        public async Task<MembershipPlan> Add(MembershipPlan membershipPlan)
        {
            DbSet<MembershipPlan> membershipPlans = _ctx.Set<MembershipPlan>();
            membershipPlans.Add(membershipPlan);
            await _ctx.SaveChangesAsync();

            return membershipPlan;
        }

        public async Task<MembershipPlan> Update(MembershipPlan membershipPlan)
        {
            DbSet<MembershipPlan> membershipPlans = _ctx.Set<MembershipPlan>();
            membershipPlans.Update(membershipPlan);
            await _ctx.SaveChangesAsync();

            return membershipPlan;
        }

        public async Task<MembershipPlan> DeleteAsync(MembershipPlan membershipPlan)
        {
            DbSet<MembershipPlan> membershipPlans = _ctx.Set<MembershipPlan>();
            membershipPlans.Remove(membershipPlan);
            await _ctx.SaveChangesAsync();

            return membershipPlan;
        }



        public async Task<MembershipPlanType> AddPlanTypeAsync(MembershipPlanType membershipPlanType)
        {
            DbSet<MembershipPlanType> membershipPlans = _ctx.Set<MembershipPlanType>();
            membershipPlans.Add(membershipPlanType);
            await _ctx.SaveChangesAsync();

            return membershipPlanType;
        }

        public async Task<MembershipPlanType> UpdatePlanTypeAsync(MembershipPlanType membershipPlanType)
        {
            DbSet<MembershipPlanType> membershipPlans = _ctx.Set<MembershipPlanType>();
            membershipPlans.Update(membershipPlanType);
            await _ctx.SaveChangesAsync();

            return membershipPlanType;
        }

        public async Task<MembershipPlanType> DeletePlanTypeAsync(MembershipPlanType membershipPlanType)
        {
            DbSet<MembershipPlanType> membershipPlans = _ctx.Set<MembershipPlanType>();
            membershipPlans.Remove(membershipPlanType);
            await _ctx.SaveChangesAsync();
            
            return membershipPlanType;
        }



        public MembershipPlan GetDiamondPlan(int partnerId)
        {
            //This is rubbish it doesn't take into account the membership plan type
            return _ctx.MembershipPlan.AsNoTracking()
                .Include(x => x.MembershipPlanType)
                .OrderByDescending(x => x.MembershipLevel.Level).FirstOrDefault(x => x.IsActive && x.PartnerId == partnerId);
        }

        public async Task<MembershipPlan> Get(int id, bool includePartner = false)
        {
            IQueryable<MembershipPlan> membershipPlanQuery = _ctx.MembershipPlan;
            if (includePartner)
            {
                membershipPlanQuery = membershipPlanQuery.Include(x => x.Partner).Include(x => x.MembershipLevel);
            }
            var membershipPlan = await membershipPlanQuery.FirstOrDefaultAsync(x => x.Id == id);

            return membershipPlan;
        }

        public async Task<MembershipPlan> GetMembershipPlan(int planTypeId, int? partnerId, int duration, string countryCode)
        {
            IQueryable<MembershipPlan> membershipPlanQuery = _ctx.MembershipPlan
                .Include(x => x.MembershipLevel)
                .Include(x => x.Partner);
            if (partnerId.HasValue && partnerId > 0)
            {
                membershipPlanQuery = membershipPlanQuery.Where(x => x.PartnerId == partnerId);
            }
            var membershipPlan = await membershipPlanQuery.FirstOrDefaultAsync(x => !x.IsDeleted && x.MembershipPlanTypeId == planTypeId
                                                                                                            && x.Duration == duration && x.IsActive && x.ValidFrom <= DateTime.UtcNow && x.ValidTo >= DateTime.UtcNow);

            return membershipPlan;
        }

        public async Task<MembershipPlan> GetByDescriptionAsync(string description)
        {
            return await _ctx.MembershipPlan.Include(x => x.MembershipLevel).FirstOrDefaultAsync(x => x.Description == description);
        }

        public async Task<MembershipPlanType> GetByPlanTypeDescriptionAsync(string description)
        {
            return await _ctx.MembershipPlanType.FirstOrDefaultAsync(x => x.Description == description);
        }

        public async Task<MembershipPlanType> GetPlanTypeByIdAsync(int planTypeId)
        {
            try
            {
                return await _ctx.MembershipPlanType.FirstOrDefaultAsync(x => x.Id == planTypeId);
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return null;
        }

        public async Task<List<MembershipPlanBenefits>> GetBenefitsByPlanIdAsync(int membershipPlanId)
        {
            try
            {
                return await _ctx.MembershipPlanBenefits
                    .Where(x => x.MembershipPlanId == membershipPlanId)
                    .OrderBy(x => x.DisplayOrder)
                    .ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public async Task<TermsConditions> GetTermsConditionsByIdAsync(int tcId)
        {
            try
            {
                return await _ctx.TermsConditions.FirstOrDefaultAsync(x => x.Id == tcId);
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return null;
        }

        public string GetDescriptionByAspNetUserId(string aspNetUserId)
        {
            try
            {
                var query = _ctx.MembershipCard.Include(x => x.MembershipPlan)
                    .Where(x => x.IsActive && x.Customer.AspNetUserId == aspNetUserId);
                query = query.Where(x => x.MembershipStatus.IsActive && x.MembershipStatus.Type == Constants.StatusType.MembershipCard);
                return query.FirstOrDefaultAsync().Result.MembershipPlan?.Description;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return null;
        }

        /// <summary>
        /// This will return Order summary details based on the membership pllan Id provided
        /// </summary>
        /// <param name="MembershipPlanId"></param>
        /// <returns></returns>
        public async Task<OrderSummaryDataModel> GetOrderSummaryDetails(int MembershipPlanId)
        {
            try
            {
                var plan = await _ctx.MembershipPlan.Where(x =>
                        x.Id == MembershipPlanId)
                    .Include(x => x.MembershipPlanPaymentProvider)
                    .FirstOrDefaultAsync();

                return mapToOrderSummary(plan);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return null;
        }

        public async Task<OrderSummaryDataModel> GetOrderDetails(int membershipLevelId)
        {
            //This is rubbish, it just picks up the first active "membershipLevelId" plan in the database
            try
            {
                var plan = await _ctx.MembershipPlan.Where(x =>
                        x.IsActive && x.MembershipLevelId == membershipLevelId)
                    .Include(x => x.MembershipPlanPaymentProvider)
                    .FirstOrDefaultAsync();

                return mapToOrderSummary(plan);
            }
            catch (Exception e)
            {
               _logger.Error(e);
            }

            return null;
        }

        public MembershipPlan GetStandardPlan(int partnerId, int cardProviderId)
        {
            return _ctx.MembershipPlan.AsNoTracking()
                .Include(x => x.MembershipPlanType)
                .OrderBy(x => x.MembershipLevel.Level).FirstOrDefault(x => x.IsActive && x.PartnerId == partnerId && x.CardProviderId == cardProviderId);
        }

        public MembershipPlan GetDiamondPlan(int partnerId, int cardProviderId)
        {
            return _ctx.MembershipPlan.AsNoTracking()
                .Include(x => x.MembershipPlanType)
                .OrderByDescending(x => x.MembershipLevel.Level).FirstOrDefault(x => x.IsActive && x.PartnerId == partnerId && x.CardProviderId == cardProviderId);
        }

        #region Private Methods

        private OrderSummaryDataModel mapToOrderSummary(MembershipPlan data)
        {
            if (data == null)
                return null;
            OrderSummaryDataModel model = new OrderSummaryDataModel
            {
                CardPrice = data.CustomerCardPrice,
                OrderName = data.Description,
                MembershipPlanId = data.Id,
                MinimumValue = data.MinimumValue,
                PaymentFee = data.PaymentFee
            };
            var provider =
                data.MembershipPlanPaymentProvider?.FirstOrDefault(x =>
                    x.PaymentProviderId == (int)Enums.PaymentProvider.PayPal);
            if (provider == null)
                return model;
            model.SubscriptionAppAndCardRef = provider.SubscribeAppAndCardRef;
            model.SubscriptionAppRef = provider.SubscribeAppRef;
            model.OneOffPaymentRef = provider.OneOffPaymentRef;
            return model;
        }

         #endregion

    }
}
