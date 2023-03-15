using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    [Obsolete("Replaced by CustomerAccountService and CustomerManager")]
    public class BankDetailService : IBankDetailService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IBankDetailManager _bankDetailManager;

        #endregion

        #region Constructor

        public BankDetailService(IMapper mapper, IBankDetailManager bankDetailManager)
        {
            _mapper = mapper;
            _bankDetailManager = bankDetailManager;
        }

        #endregion

        #region Writes

        //Add BankDetail
        public async Task<dto.BankDetail> Add(dto.BankDetail bankDetail)
        {
            BankDetail req = ManualMappings.MapBankDetailReq(bankDetail);
            return ManualMappings.MapBankDetail(
                await _bankDetailManager.Add(req));
        }

        //Update BankDetail
        public async Task<dto.BankDetail> Update(dto.BankDetail bankDetail)
        {
            BankDetail req = ManualMappings.MapBankDetailReq(bankDetail);
            return ManualMappings.MapBankDetail(
                await _bankDetailManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<dto.BankDetail> Get(int id)
        {
            return ManualMappings.MapBankDetail(await _bankDetailManager.Get(id));
        }

        public async Task<List<dto.BankDetail>> GetAll()
        {
            return ManualMappings.MapBankDetails(await _bankDetailManager.GetAll());
        }

        #endregion
    }
}
