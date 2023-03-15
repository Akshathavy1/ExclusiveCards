using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    [Obsolete("Replaced by CustomerAccountService and MembershipManager")]
    public class MembershipCardService : Interfaces.Public.IMembershipCardService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IMembershipCardManager _membershipCardManager;
        private readonly Managers.IMembershipManager _cardManager;

        #endregion

        #region Constructor

        public MembershipCardService(IMapper mapper,
            IMembershipCardManager membershipCardManager,
            Managers.IMembershipManager cardManager)
        {
            _mapper = mapper;
            _membershipCardManager = membershipCardManager;
            _cardManager = cardManager;
        }

        #endregion

        #region Writes

        //Add MembershipCard
        public async Task<dto.MembershipCard> Add(dto.MembershipCard membershipCard, string prefix, string digits, string suffix)
        {
            MembershipCard req = _mapper.Map<MembershipCard>(membershipCard);
            return _mapper.Map<dto.MembershipCard>(
                await _membershipCardManager.Add(req, prefix, digits, suffix));
        }

        //Update MembershipCard
        public async Task<dto.MembershipCard> Update(dto.MembershipCard membershipCard)
        {
            MembershipCard req = _mapper.Map<MembershipCard>(membershipCard);
            return _mapper.Map<dto.MembershipCard>(
                await _membershipCardManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<dto.MembershipCard> Get(int id)
        {
            return _mapper.Map<dto.MembershipCard>(await _membershipCardManager.Get(id));
        }

        public async Task<dto.MembershipCard> GetLastRecord()
        {
            return _mapper.Map<dto.MembershipCard>(await _membershipCardManager.GetLastRecord());
        } 

        public async Task<List<dto.MembershipCard>> GetAll(int customerId)
        {
            return ManualMappings.MapMembershipCards(await _membershipCardManager.GetAll(customerId));
        }

        public dto.MembershipCard GetByMembershipCard(string cardNumber)
        {
            return _mapper.Map<dto.MembershipCard>(_membershipCardManager.GetByMembershipCard(cardNumber));
        }

        public dto.MembershipCard GetPlanProviderByMembershipCardId(int id)
        {
            return MapToDTo(_membershipCardManager.GetPlanProviderByMembershipCardId(id));
        }

        public async Task<dto.MembershipCard> GetByCustomerProviderId (string customerProviderId)
        {
            return MapToDTo(await _membershipCardManager.GetByCustomerProviderId(customerProviderId));
        }

        public async Task<dto.MembershipCard> GetByCustomerPlan (int customerId, int? planId)
        {
            return _mapper.Map<dto.MembershipCard>(await _membershipCardManager.GetByCustomerPlan(customerId, planId));
        }

        public async Task<dto.MembershipCard> GetByAspNetUserId(string aspNetUserId)
        {
            return MapToDTo(
                await _membershipCardManager.GetByAspNetUserId(aspNetUserId));
        }

        public dto.MembershipCard GetActiveMembershipCard(string aspNetUserId)
        {
            return _cardManager.GetActiveMembershipCard(aspNetUserId);
        }

        public dto.MembershipCard GetDiamondMembershipCard(string aspNetUserId)
        {
            return _cardManager.GetDiamondMembershipCard(aspNetUserId);
        }

        #endregion

        #region Private Members

        private dto.MembershipCard MapToDTo(MembershipCard membership)
        {
            if(membership == null)
                return  new dto.MembershipCard();
            dto.MembershipCard membershipCard = new dto.MembershipCard
            {
                CustomerId = membership.CustomerId,
                MembershipPlanId = membership.MembershipPlanId,
                CardNumber = membership.CardNumber,
                ValidFrom = membership.ValidFrom,
                ValidTo = membership.ValidTo,
                DateIssued = membership.DateIssued,
                StatusId = membership.StatusId,
                PhysicalCardRequested = membership.PhysicalCardRequested,
                CustomerPaymentProviderId = membership.CustomerPaymentProviderId,
                IsActive = membership.IsActive,
                IsDeleted = membership.IsDeleted,
                PhysicalCardStatusId = membership.PhysicalCardStatusId,
                RegistrationCode = membership.RegistrationCode,
                PartnerRewardId = membership.PartnerRewardId,
                TermsConditionsId = membership.TermsConditionsId
            };
            if (membership.Id > 0)
            {
                membershipCard.Id = membership.Id;
            }

            if (membership.MembershipPlan != null)
            {
                membershipCard.MembershipPlan = new dto.MembershipPlan
                {
                    Id = membership.MembershipPlan.Id,
                    PartnerId = membership.MembershipPlan.PartnerId,
                    MembershipPlanTypeId = membership.MembershipPlan.MembershipPlanTypeId,
                    NumberOfCards = membership.MembershipPlan.NumberOfCards,
                    ValidFrom = membership.MembershipPlan.ValidFrom,
                    ValidTo = membership.MembershipPlan.ValidTo,
                    CustomerCardPrice = membership.MembershipPlan.CustomerCardPrice,
                    PartnerCardPrice = membership.MembershipPlan.PartnerCardPrice,
                    CustomerCashbackPercentage = membership.MembershipPlan.CustomerCashbackPercentage,
                    DeductionPercentage = membership.MembershipPlan.DeductionPercentage,
                    BenefactorPercentage = membership.MembershipPlan.BenefactorPercentage,
                    Description = membership.MembershipPlan.Description,
                    CurrencyCode = membership.MembershipPlan.CurrencyCode,
                    IsActive = membership.MembershipPlan.IsActive,
                    IsDeleted = membership.MembershipPlan.IsDeleted,
                    MembershipLevelId = membership.MembershipPlan.MembershipLevelId,
                    MinimumValue = membership.MembershipPlan.MinimumValue,
                    PaymentFee = membership.MembershipPlan.PaymentFee
                };

                if (membership.MembershipPlan.MembershipLevel != null)
                {
                    membershipCard.MembershipPlan.MembershipLevel = new dto.MembershipLevel
                    {
                        Id = membership.MembershipPlan.MembershipLevel.Id,
                        Description = membership.MembershipPlan.MembershipLevel.Description,
                        Level = membership.MembershipPlan.MembershipLevel.Level
                    };
                }

                if (membership.MembershipPlan.MembershipPlanPaymentProvider == null ||
                    membership.MembershipPlan.MembershipPlanPaymentProvider.Count <= 0) return membershipCard;

                membershipCard.MembershipPlan.MembershipPlanPaymentProvider = new List<dto.MembershipPlanPaymentProvider>();
                foreach (var provider in membership.MembershipPlan.MembershipPlanPaymentProvider)
                {
                    membershipCard.MembershipPlan.MembershipPlanPaymentProvider.Add(new dto.MembershipPlanPaymentProvider
                    {
                        MembershipPlanId = provider.MembershipPlanId,
                        PaymentProviderId = provider.PaymentProviderId,
                        SubscribeAppRef = provider.SubscribeAppRef,
                        SubscribeAppAndCardRef = provider.SubscribeAppAndCardRef,
                        OneOffPaymentRef = provider.OneOffPaymentRef
                    });
                }
            }

            return membershipCard;
        }



        #endregion
    }
}
