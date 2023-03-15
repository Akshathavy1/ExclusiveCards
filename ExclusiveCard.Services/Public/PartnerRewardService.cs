using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    [Obsolete("Replaced by the RewardManager")]
    public class PartnerRewardService : IPartnerRewardService
    {
        #region Private Members and constructor

        private readonly IPartnerRewardManager _manager;
        private readonly IMapper _mapper;

        public PartnerRewardService(IMapper mapper, IPartnerRewardManager partnerRewardManager)
        {
            _mapper = mapper;
            _manager = partnerRewardManager;
        }

        #endregion

        #region Writes

        public async Task<DTOs.PartnerRewards> AddAsync(DTOs.PartnerRewards reward)
        {
            var req = _mapper.Map<PartnerRewards>(reward);
            return _mapper.Map<DTOs.PartnerRewards>(await _manager.AddAsync(req));
        }

        public async Task<DTOs.PartnerRewards> UpdateAsync(DTOs.PartnerRewards reward)
        {
            var req = _mapper.Map<PartnerRewards>(reward);
            return _mapper.Map<DTOs.PartnerRewards>(await _manager.UpdateAsync(req));
        }

        #endregion

        #region Reads

        public async Task<DTOs.PartnerRewards> GetByIdAsync(int id)
        {
            return _mapper.Map<DTOs.PartnerRewards>(await _manager.GetByIdAsync(id));
        }

        public async Task<DTOs.PartnerRewards> GetByRewardKey(string key)
        {
            //Return PartnerRewards with list of membershipcards and only Partner
            var partnerData = await _manager.GetByRewardKey(key);
            if (partnerData != null)
            {
                return MapPartnerReward(partnerData);
            }
            else
                return null;
        }

        public async Task<List<DTOs.PartnerRewards>> GetAllAsync()
        {
            var partnerData = await _manager.GetAllAsync();
            return MapPartnerRewards(partnerData);
        }

        #endregion

        public List<DTOs.PartnerRewards> MapPartnerRewards(List<PartnerRewards> data)
        {
            List<DTOs.PartnerRewards> dtos = new List<DTOs.PartnerRewards>();
            if (data == null)
            {
                return null;
            }
            foreach (var da in data)
            {
                dtos.Add(MapPartnerReward(da));
            }
            return dtos;
        }

        public DTOs.PartnerRewards MapPartnerReward(PartnerRewards data)
        {
            if (data == null)
            {
                return null;
            }
            return new DTOs.PartnerRewards
            {
                Id = data.Id,
                CreatedDate = data.CreatedDate,
                PartnerId = data.PartnerId,
                RewardKey = data.RewardKey,
                LatestValue = data.LatestValue,
                ValueDate = data.ValueDate,
                TotalConfirmedWithdrawn = data.TotalConfirmedWithdrawn,
                Password = data.Password,
                MembershipCards = MapMembershipCards(data.MembershipCards),
                Partner = MapPartner(data.Partner)
            };
        }

        public DTOs.PartnerDto MapPartner(Partner data)
        {
            if (data == null)
                return null;

            return new DTOs.PartnerDto
            {
                Id = data.Id,
                Name = data.Name,
                ContactDetailId = data.ContactDetailId,
                BankDetailsId = data.BankDetailsId,
                IsDeleted = data.IsDeleted,
                Type = data.Type,
                ImagePath = data.ImagePath
            };
        }

        public List<DTOs.MembershipCard> MapMembershipCards(ICollection<MembershipCard> cards)
        {
            if (cards == null)
                return null;

            var dtos = new List<DTOs.MembershipCard>();

            foreach (var card in cards)
            {
                dtos.Add(MapMembershipCard(card));
            }

            return dtos;
        }

        public DTOs.MembershipCard MapMembershipCard(MembershipCard card)
        {
            if (card == null)
                return null;

            DTOs.MembershipCard dto = new DTOs.MembershipCard
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
                RegistrationCode = card.RegistrationCode,
                PartnerRewardId = card.PartnerRewardId,
                TermsConditionsId = card.TermsConditionsId
            };

            if (card.Customer != null)
            {
                dto.Customer = new DTOs.Customer();
                dto.Customer = MapCustomer(card.Customer);
            }

            return dto;
        }

        public static DTOs.Customer MapCustomer(Customer customer)
        {
            if (customer == null)
                return null;

            var dto = new DTOs.Customer
            {
                Id = customer.Id,
                AspNetUserId = customer.AspNetUserId,
                ContactDetailId = customer.ContactDetailId,
                Title = customer.Title,
                Forename = customer.Forename,
                Surname = customer.Surname,
                DateOfBirth = customer.DateOfBirth,
                IsActive = customer.IsActive,
                IsDeleted = customer.IsDeleted,
                DateAdded = customer.DateAdded,
                MarketingNewsLetter = customer.MarketingNewsLetter,
                MarketingThirdParty = customer.MarketingThirdParty
            };

            return dto;
        }

        public DTOs.PartnerRewards MapPartnerRewardReq(DTOs.PartnerRewards data)
        {
            if (data == null)
            {
                return null;
            }
            return new DTOs.PartnerRewards
            {
                Id = data.Id,
                CreatedDate = data.CreatedDate,
                PartnerId = data.PartnerId,
                RewardKey = data.RewardKey,
                LatestValue = data.LatestValue,
                ValueDate = data.ValueDate,
                TotalConfirmedWithdrawn = data.TotalConfirmedWithdrawn,
                Password = data.Password,
            };
        }
    }
}
