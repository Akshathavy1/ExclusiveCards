using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
    //TODO: Move the mappings into the proper automappers mapping profile file
    // Writing bespoke mappers in a service is BANNED!!!
    public class PartnerRewardWithdrawalService : IPartnerRewardWithdrawalService
    {
        #region Private members and constructor

        private readonly IPartnerRewardWithdrawalManager _manager;
        private readonly IMapper _mapper;

        public PartnerRewardWithdrawalService(IMapper mapper, IPartnerRewardWithdrawalManager partnerRewardWithdrawalManager)
        {
            _mapper = mapper;
            _manager = partnerRewardWithdrawalManager;
        }

        #endregion

        #region Writes

        public async Task<dto.PartnerRewardWithdrawal> AddAsync(dto.PartnerRewardWithdrawal reward)
        {
            var request = MapPartnerRewardWithdrawalReq(reward);
            return MapPartnerRewardWithdrawal(await _manager.AddAsync(request));
        }

        public async Task<dto.PartnerRewardWithdrawal> UpdateAsync(dto.PartnerRewardWithdrawal reward)
        {
            var request = MapPartnerRewardWithdrawalReq(reward);
            return MapPartnerRewardWithdrawal(await _manager.UpdateAsync(request));
        }

        public async Task<List<dto.PartnerRewardWithdrawal>> BulkUpdateAsync(List<dto.PartnerRewardWithdrawal> reward)
        {
            var request = MapToPartnerRewardWithdrawalsReq(reward, _mapper);
            return MapToPartnerRewardWithdrawalsDto(await _manager.BulkUpdateAsync(request));
        }

        public async Task<dto.PartnerRewardWithdrawal> UpdateErrorAsync(int partnerRewardId, int errorStatus)
        {
            return MapPartnerRewardWithdrawal(
                await _manager.UpdateErrorAsync(partnerRewardId, errorStatus));
        }

        public async Task<dto.PartnerRewardWithdrawal> UpdateConfirmationAsync(int partnerRewardId, int successStatus,
            decimal amountConfirmed)
        {
            return MapPartnerRewardWithdrawal(
                await _manager.UpdateConfirmationAsync(partnerRewardId, successStatus, amountConfirmed));
        }

        #endregion

        #region Reads

        public async Task<dto.PartnerRewardWithdrawal> GetByIdAsync(int id)
        {
            return MapPartnerRewardWithdrawal(await _manager.GetByIdAsync(id));
        }
       

        public async Task<List<dto.TamWithdrawalDataModel>> GetWithdrawalReport(int statusId)
        {
            var resp = await _manager.GetWithdrawalReport(statusId);
            var dtoModel = _mapper.Map<List<dto.TamWithdrawalDataModel>>(resp);
            
            return dtoModel;
        }

        public async Task<List<dto.PartnerRewardWithdrawal>> GetAllAsync()
        {
            return MapToPartnerRewardWithdrawalsDto(await _manager.GetAllAsync());
        }

        public async Task<List<dto.PartnerRewardWithdrawal>> GetAllPendingAsync()
        {
            return MapToPartnerRewardWithdrawalsDto(await _manager.GetAllPendingAsync());
        }

        public async Task<dto.WithdrawalRequestModel> GetWithdrawalDataForRequest(int membershipCardId)
        {
            var resp = await _manager.GetWithdrawalDataForRequest(membershipCardId);
            var dtoResult = _mapper.Map<dto.WithdrawalRequestModel>(resp);
            return dtoResult;
        }

        public async Task<dto.PagedResult<dto.TransactionLog>> GetTransactionLog(string userId, int page,
            int pageSize, Enums.TransactionLogSortOrder sortOrder)
        {
            var resp = await _manager.GetTransactionLog(userId, page, pageSize, sortOrder);
            var dtoResult = _mapper.Map<dto.PagedResult<dto.TransactionLog>>(resp);
            return dtoResult;
        }

        public async Task<dto.PagedResult<dto.TransactionLog>> GetWithdrawalLog(string userId, int page,
            int pageSize)
        {
            var resp = await _manager.GetWithdrawalLog(userId, page, pageSize);
            var dtoResult = _mapper.Map<dto.PagedResult<dto.TransactionLog>>(resp);
            return dtoResult;
        }

        public async Task<dto.PagedResult<dto.PartnerRewardWithdrawal>> GetWithdrawalsForPayments(int partnerId,
            int status, int page, int pageSize, Enums.WithdrawalSortOrder sortOrder)
        {
            var resp = await _manager.GetWithdrawalsForPayments(partnerId, status, page, pageSize, sortOrder);
            var dtoResult = _mapper.Map<dto.PagedResult<dto.PartnerRewardWithdrawal>>(resp);
            return dtoResult;

        }

        #endregion

       

        public List<Data.Models.PartnerRewardWithdrawal> MapToPartnerRewardWithdrawalsReq(
            List<Models.DTOs.PartnerRewardWithdrawal> data, IMapper mapper)
        {
            if (data == null || data.Count == 0)
                return null;

            List<Data.Models.PartnerRewardWithdrawal> models = new List<Data.Models.PartnerRewardWithdrawal>();
            foreach (Models.DTOs.PartnerRewardWithdrawal partnerRewardWithdrawal in data)
            {
                models.Add(MapPartnerRewardWithdrawalReq(partnerRewardWithdrawal));
            }

            return models;
        }

        public Data.Models.PartnerRewardWithdrawal MapPartnerRewardWithdrawalReq(Models.DTOs.PartnerRewardWithdrawal data)
        {
            if (data == null)
                return null;
            var response = _mapper.Map<dto.PartnerRewardWithdrawal>(data);
            return _mapper.Map<Data.Models.PartnerRewardWithdrawal>(response);
        }



        public List<dto.PartnerRewardWithdrawal> MapToPartnerRewardWithdrawalsDto(
            List<Data.Models.PartnerRewardWithdrawal> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<dto.PartnerRewardWithdrawal> models = new List<dto.PartnerRewardWithdrawal>();
            models.AddRange(data.Select(MapPartnerRewardWithdrawal));

            return models;
        }

        public dto.PartnerRewardWithdrawal MapPartnerRewardWithdrawal(Data.Models.PartnerRewardWithdrawal data)
        {
            if (data == null)
                return null;
            var dto = new dto.PartnerRewardWithdrawal();
            dto = _mapper.Map<dto.PartnerRewardWithdrawal>(data);

            if (data.PartnerReward != null && dto.PartnerReward == null)
            {
                dto.PartnerReward = new dto.PartnerRewards();
                dto.PartnerReward = _mapper.Map<dto.PartnerRewards>(data.PartnerReward);
            }

            var dataMembershipCard =
                data.PartnerReward?.MembershipCards?.FirstOrDefault(x => x.IsActive && x.ValidTo >= DateTime.UtcNow);
            if (dataMembershipCard != null)
            {
                dto.PartnerReward.MembershipCards = new List<dto.MembershipCard>();
                var dtoMembershipCard = _mapper.Map<dto.MembershipCard>(dataMembershipCard);
                if (dataMembershipCard.Customer != null && dtoMembershipCard.Customer == null)
                {
                    dtoMembershipCard.Customer = new dto.Customer();
                    dtoMembershipCard.Customer = _mapper.Map<dto.Customer>(dataMembershipCard.Customer);
                }
                dto.PartnerReward.MembershipCards.Add(dtoMembershipCard);
            }

            return dto;
        }

        public async Task<dto.PartnerRewardWithdrawal> GetByBankDetailIdAsync(int id)
        {
            return MapPartnerRewardWithdrawal(await _manager.GetByBankDetailIdAsync(id));
        }
    }
}
