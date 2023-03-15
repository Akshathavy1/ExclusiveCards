using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
   public class MembershipCardAffiliateReferenceService : IMembershipCardAffiliateReferenceService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IMembershipCardAffiliateReferenceManager _membershipCardAffiliateReferenceManager;

        #endregion

        #region Constructor

        public MembershipCardAffiliateReferenceService(IMapper mapper, IMembershipCardAffiliateReferenceManager membershipCardAffiliateReferenceManager)
        {
            _mapper = mapper;
            _membershipCardAffiliateReferenceManager = membershipCardAffiliateReferenceManager;
        }

        #endregion

        #region Writes

        //Add Customer
        public async Task<Models.DTOs.MembershipCardAffiliateReference> Add(Models.DTOs.MembershipCardAffiliateReference membershipCard)
        {
            MembershipCardAffiliateReference req = _mapper.Map<MembershipCardAffiliateReference>(membershipCard);
            return MapToDto(
                await _membershipCardAffiliateReferenceManager.Add(req));
        }

        //Update Customer
        public async Task<Models.DTOs.MembershipCardAffiliateReference> Update(Models.DTOs.MembershipCardAffiliateReference membershipCard)
        {
            MembershipCardAffiliateReference req = _mapper.Map<MembershipCardAffiliateReference>(membershipCard);
            return MapToDto(
                await _membershipCardAffiliateReferenceManager.Update(req));
        }

        #endregion

        #region Reads

        //Get Membership Card Affiliate Reference based on ref
        public async Task<Models.DTOs.MembershipCardAffiliateReference> Get(int affiliateId, string reference)
        {
            return MapToDto(
                await _membershipCardAffiliateReferenceManager.Get(affiliateId, reference));
        }

        //Get All Membership card affiliate Reference
        public async Task<List<Models.DTOs.MembershipCardAffiliateReference>> GetAll()
        {
            return MapToList(
                await _membershipCardAffiliateReferenceManager.GetAll());
        }

        #endregion

        private List<Models.DTOs.MembershipCardAffiliateReference> MapToList(
            List<MembershipCardAffiliateReference> data)
        {
            if (data == null || data.Count == 0)
                return null;

            var dtoList = new List<Models.DTOs.MembershipCardAffiliateReference>();

            dtoList.AddRange(data.Select(MapToDto));

            return dtoList;
        }

        private Models.DTOs.MembershipCardAffiliateReference MapToDto(MembershipCardAffiliateReference data)
        {
            if (data == null)
                return null;

            var dto = new Models.DTOs.MembershipCardAffiliateReference
            {
                AffiliateId = data.AffiliateId,
                MembershipCardId = data.MembershipCardId,
                CardReference = data.CardReference
            };

            if (data.Affiliate != null)
            {
                dto.Affiliate = new Models.DTOs.Affiliate
                {
                    Id = data.Affiliate.Id,
                    Name = data.Affiliate.Name
                };
            }

            if (data.MembershipCard != null)
            {
                dto.MembershipCard = new Models.DTOs.MembershipCard
                {
                    Id = data.MembershipCard.Id,
                    CustomerId = data.MembershipCard.CustomerId,
                    MembershipPlanId = data.MembershipCard.MembershipPlanId,
                    CardNumber = data.MembershipCard.CardNumber,
                    ValidFrom = data.MembershipCard.ValidFrom,
                    ValidTo = data.MembershipCard.ValidTo, DateIssued = data.MembershipCard.DateIssued,
                    PhysicalCardRequested = data.MembershipCard.PhysicalCardRequested,
                    CustomerPaymentProviderId = data.MembershipCard.CustomerPaymentProviderId,
                    IsActive = data.MembershipCard.IsActive, IsDeleted = data.MembershipCard.IsDeleted,
                    PhysicalCardStatusId = data.MembershipCard.PhysicalCardStatusId,
                    RegistrationCode = data.MembershipCard.RegistrationCode,
                    PartnerRewardId = data.MembershipCard.PartnerRewardId,
                    TermsConditionsId = data.MembershipCard.TermsConditionsId
                };

                if (data.MembershipCard.MembershipPlan != null)
                {
                    dto.MembershipCard.MembershipPlan = new Models.DTOs.MembershipPlan
                    {
                        Id = data.MembershipCard.MembershipPlan.Id,
                        PartnerId = data.MembershipCard.MembershipPlan.PartnerId,
                        MembershipPlanTypeId = data.MembershipCard.MembershipPlan.MembershipPlanTypeId,
                        NumberOfCards = data.MembershipCard.MembershipPlan.NumberOfCards,
                        ValidFrom = data.MembershipCard.MembershipPlan.ValidFrom,
                        ValidTo = data.MembershipCard.MembershipPlan.ValidTo,
                        CustomerCardPrice = data.MembershipCard.MembershipPlan.CustomerCardPrice,
                        PartnerCardPrice = data.MembershipCard.MembershipPlan.PartnerCardPrice,
                        CustomerCashbackPercentage = data.MembershipCard.MembershipPlan.CustomerCashbackPercentage,
                        DeductionPercentage = data.MembershipCard.MembershipPlan.DeductionPercentage,
                        BenefactorPercentage = data.MembershipCard.MembershipPlan.BenefactorPercentage,
                        Description = data.MembershipCard.MembershipPlan.Description,
                        CurrencyCode = data.MembershipCard.MembershipPlan.CurrencyCode,
                        Duration = data.MembershipCard.MembershipPlan.Duration,
                        IsActive = data.MembershipCard.MembershipPlan.IsActive,
                        IsDeleted = data.MembershipCard.MembershipPlan.IsDeleted,
                        MembershipLevelId = data.MembershipCard.MembershipPlan.MembershipLevelId,
                        PaidByEmployer = data.MembershipCard.MembershipPlan.PaidByEmployer,
                        MinimumValue = data.MembershipCard.MembershipPlan.MinimumValue,
                        PaymentFee = data.MembershipCard.MembershipPlan.PaymentFee
                    };
                }
            }

            return dto;
        }
    }
}
