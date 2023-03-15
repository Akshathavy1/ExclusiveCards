using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using newManagers = ExclusiveCard.Managers;

namespace ExclusiveCard.Services.Admin
{
    [Obsolete("DO NOT write any new code or add references to this, use Exclusivecard.Services.Admin.MembershipService instead")]
    public class OLD_MembershipPlanService : IOLD_MembershipPlanService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IMembershipPlanManager _membershipPlanManager;
        private readonly IMembershipPlanPaymentProviderManager _planPaymentProviderManager;

        private readonly newManagers.IMembershipManager _newMembershipManager;

        #endregion

        #region Constructor

        public OLD_MembershipPlanService(IMapper mapper, IMembershipPlanManager membershipPlanManager,
            IMembershipPlanPaymentProviderManager membershipPlanPaymentProviderManager, newManagers.IMembershipManager newMembershipManager)
        {
            _mapper = mapper;
            _membershipPlanManager = membershipPlanManager;
            _planPaymentProviderManager = membershipPlanPaymentProviderManager;

            _newMembershipManager = newMembershipManager;
        }

        #endregion

        #region Writes

        //Add Plan
        public async Task<Models.DTOs.MembershipPlan> Add(Models.DTOs.MembershipPlan membershipPlan)
        {
            MembershipPlan req = _mapper.Map<MembershipPlan>(membershipPlan);
            return _mapper.Map<Models.DTOs.MembershipPlan>(
                await _membershipPlanManager.Add(req));
        }

        //Update Plan
        public async Task<Models.DTOs.MembershipPlan> Update(Models.DTOs.MembershipPlan membershipPlan)
        {
            MembershipPlan req = _mapper.Map<MembershipPlan>(membershipPlan);
            return _mapper.Map<Models.DTOs.MembershipPlan>(
                await _membershipPlanManager.Update(req));
        }

        //Delete Plan
        public async Task<Models.DTOs.MembershipPlan> DeleteAsync(Models.DTOs.MembershipPlan membershipPlan)
        {
            MembershipPlan req = _mapper.Map<MembershipPlan>(membershipPlan);
            return _mapper.Map<Models.DTOs.MembershipPlan>(
                await _membershipPlanManager.DeleteAsync(req));
        }

        #endregion

        #region Writes Plan Type

        //Add Plan Type
        public async Task<Models.DTOs.MembershipPlanType> AddPlanTypeAsync(Models.DTOs.MembershipPlanType membershipPlanType)
        {
            MembershipPlanType req = _mapper.Map<MembershipPlanType>(membershipPlanType);
            return _mapper.Map<Models.DTOs.MembershipPlanType>(
                await _membershipPlanManager.AddPlanTypeAsync(req));
        }

        //Update Plan type
        public async Task<Models.DTOs.MembershipPlanType> UpdatePlanTypeAsync(Models.DTOs.MembershipPlanType membershipPlan)
        {
            MembershipPlanType req = _mapper.Map<MembershipPlanType>(membershipPlan);
            return _mapper.Map<Models.DTOs.MembershipPlanType>(
                await _membershipPlanManager.UpdatePlanTypeAsync(req));
        }

        //Delete Plan type
        public async Task<Models.DTOs.MembershipPlanType> DeletePlanTypeAsync(Models.DTOs.MembershipPlanType membershipPlan)
        {
            MembershipPlanType req = _mapper.Map<MembershipPlanType>(membershipPlan);
            return _mapper.Map<Models.DTOs.MembershipPlanType>(
                await _membershipPlanManager.DeletePlanTypeAsync(req));
        }

        #endregion

        #region Reads

        public Models.DTOs.MembershipPlan GetDiamondPlan(int partnerId)
        {
            return _mapper.Map<Models.DTOs.MembershipPlan>(_membershipPlanManager.GetDiamondPlan(partnerId));
        }

        public async Task<Models.DTOs.MembershipPlan> Get(int id, bool includePartner = false)
        {
            return _mapper.Map<Models.DTOs.MembershipPlan>(await _membershipPlanManager.Get(id, includePartner));
        }

        //Get MembershipPlan by planTypeId, partnerId, duration, countryCode
        public async Task<Models.DTOs.MembershipPlan> GetMembershipPlan(int planTypeId, int? partnerId, int duration, string countryCode)
        {
            return _mapper.Map<Models.DTOs.MembershipPlan>(await _membershipPlanManager.GetMembershipPlan(planTypeId, partnerId, duration, countryCode));
        }

        //Get Payment providers for the membership plan
        public async Task<List<Models.DTOs.MembershipPlanPaymentProvider>> GetPaymentProvidersForMembershipPlanAsync(
            int membershipPlanId)
        {
            return _mapper.Map<List<Models.DTOs.MembershipPlanPaymentProvider>>(
                await _planPaymentProviderManager.Get(membershipPlanId));
        }

        public async Task<Models.DTOs.MembershipPlan> GetByDescriptionAsync(string description)
        {
            return _mapper.Map<Models.DTOs.MembershipPlan>(await _membershipPlanManager.GetByDescriptionAsync(description));
        }

        public async Task<Models.DTOs.MembershipPlanType> GetByPlanTypeDescriptionAsync(string description)
        {
            return _mapper.Map<Models.DTOs.MembershipPlanType>(
                await _membershipPlanManager.GetByPlanTypeDescriptionAsync(description));
        }

        public async Task<Models.DTOs.MembershipPlanType> GetPlanTypeByIdAsync(int planTypeId)
        {
            return _mapper.Map<Models.DTOs.MembershipPlanType>(
                await _membershipPlanManager.GetPlanTypeByIdAsync(planTypeId));
        }

        public async Task<List<Models.DTOs.MembershipPlanBenefits>> GetBenefitsByPlanIdAsync(int membershipPlanId)
        {
            return ManualMappings.MapPlanBenefitsList(
                await _membershipPlanManager.GetBenefitsByPlanIdAsync(membershipPlanId), _mapper);
        }

        public async Task<Models.DTOs.TermsConditions> GetTermsConditionsByIdAsync(int tcId)
        {
            return _mapper.Map<Models.DTOs.TermsConditions>(
                await _membershipPlanManager.GetTermsConditionsByIdAsync(tcId));
        }

        public string GetDescriptionByUserId(string aspNetUserId)
        {
            return _membershipPlanManager.GetDescriptionByAspNetUserId(aspNetUserId);
        }

        /// <summary>
        /// Provides order summary details for the diamond plan where the
        /// membership plan type matches <paramref name="standardPlanId"/> and 
        /// the card provider id is the same 
        /// if none found the default exclusive diamond for that plan type will be used
        /// </summary>
        /// <param name="standardPlanId"></param>
        /// <returns>order summary model based on the diamond plan found</returns>
        public async Task<Models.DTOs.OrderSummary> GetOrderSummaryForDiamondPlan(int standardPlanId)
        {
            ExclusiveCard.Services.Models.DTOs.MembershipPlan diamondPlan = _newMembershipManager.GetDiamondMembershipPlan(standardPlanId);
            var summary = await _membershipPlanManager.GetOrderSummaryDetails(diamondPlan.Id);
            var dtoResult = _mapper.Map<Models.DTOs.OrderSummary>(summary);
            return dtoResult;
        }

        public async Task<Models.DTOs.OrderSummary> GetOrderSummary(int membershipLevelId)
        {
            var resp = await _membershipPlanManager.GetOrderDetails(membershipLevelId);
            var dtoResult = _mapper.Map<Models.DTOs.OrderSummary>(resp);

            return dtoResult;
        }

        public Models.DTOs.MembershipPlan GetStandardPlan(int partnerId, int cardProviderId)
        {
            return _mapper.Map<Models.DTOs.MembershipPlan>(_membershipPlanManager.GetStandardPlan(partnerId, cardProviderId));
        }

        public Models.DTOs.MembershipPlan GetDiamondPlan(int partnerId, int cardProviderId)
        {
            return _mapper.Map<Models.DTOs.MembershipPlan>(_membershipPlanManager.GetDiamondPlan(partnerId, cardProviderId));
        }

        #endregion
    }
}
