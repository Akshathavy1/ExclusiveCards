using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class MembershipCardService : IMembershipCardService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IMembershipCardManager _membershipCardManager;

        #endregion

        #region Constructor

        public MembershipCardService(IMapper mapper, IMembershipCardManager membershipCardManager)
        {
            _mapper = mapper;
            _membershipCardManager = membershipCardManager;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.MembershipCard> UpdatePhysicalCardDetailsAsync(int membershipCardId, bool physicalCardRequested, int physicalCardStatusId)
        {
            return _mapper.Map<Models.DTOs.MembershipCard>(await _membershipCardManager.UpdatePhysicalCardDetailsAsync(membershipCardId, physicalCardRequested, physicalCardStatusId));
        }

        public async Task<Models.DTOs.MembershipCard> DeleteAsync(Models.DTOs.MembershipCard membershipCard)
        {
            var req = _mapper.Map<Data.Models.MembershipCard>(membershipCard);
            return _mapper.Map<Models.DTOs.MembershipCard>(await _membershipCardManager.DeleteAsync(req));
        }

        #endregion

        #region Reads
        
        //View/Edit Membership cards in admin panel
        public async Task<List<Models.DTOs.MembershipCard>> GetMembershipCardsForCustomerAsync(int customerId, bool onlyValidCards)
        {
            return Map(await _membershipCardManager.GetAll(customerId, onlyValidCards));
        }

        //Sendout Cards in Admin Panel
        public async Task<List<Models.DTOs.MembershipCard>> GetCardsToSendOutAsync()
        {
            return MapDetails(await _membershipCardManager.GetCardsToSendOutAsync());
        }

        #endregion

        #region Private Methods
        
        //Only membership Card Mapping as Auto mapper not working
        private List<Models.DTOs.MembershipCard> Map(List<Data.Models.MembershipCard> cards)
        {
            return cards.Select(card => new Models.DTOs.MembershipCard
                {
                    Id = card.Id,
                    CustomerId = card.CustomerId,
                    MembershipPlanId = card.MembershipPlanId,
                    CardNumber = card.CardNumber,
                    ValidFrom = card.ValidFrom,
                    ValidTo = card.ValidTo,
                    DateIssued = card.DateIssued,
                    StatusId = card.StatusId,
                    PhysicalCardRequested = card.PhysicalCardRequested,
                    CustomerPaymentProviderId = card.CustomerPaymentProviderId,
                    IsActive = card.IsActive,
                    IsDeleted = card.IsDeleted,
                    PhysicalCardStatusId = card.PhysicalCardStatusId,
                    RegistrationCode = card.RegistrationCode
                })
                .ToList();
        }

        //Only customer Id and AspNetuser Id is mapped
        private List<Models.DTOs.MembershipCard> MapDetails(List<Data.Models.MembershipCard> cards)
        {
            List<Models.DTOs.MembershipCard> membershipCards = new List<Models.DTOs.MembershipCard>();
            foreach (Data.Models.MembershipCard card in cards)
            {
                Models.DTOs.MembershipCard membershipCard = new Models.DTOs.MembershipCard
                {
                    Id = card.Id,
                    CustomerId = card.CustomerId,
                    MembershipPlanId = card.MembershipPlanId,
                    CardNumber = card.CardNumber,
                    ValidFrom = card.ValidFrom,
                    ValidTo = card.ValidTo,
                    DateIssued = card.DateIssued,
                    StatusId = card.StatusId,
                    PhysicalCardRequested = card.PhysicalCardRequested,
                    CustomerPaymentProviderId = card.CustomerPaymentProviderId,
                    IsActive = card.IsActive,
                    IsDeleted = card.IsDeleted,
                    PhysicalCardStatusId = card.PhysicalCardStatusId,
                    RegistrationCode = card.RegistrationCode    
                };
                if (card.Customer != null)
                {
                    membershipCard.Customer = new Models.DTOs.Customer
                    {
                        Id = card.Customer.Id,
                        AspNetUserId = card.Customer.AspNetUserId,
                        NINumber = card.Customer.NINumber
                    };
                }

                membershipCards.Add(membershipCard);
            }
            return membershipCards;
        }

        #endregion
    }
}
