using System;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;

namespace ExclusiveCard.Services.Public
{
   [Obsolete]
   public class CustomerBankDetailService : ICustomerBankDetailService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ICustomerBankDetailManager _customerBankDetailManager;

        #endregion

        #region Constructor

        public CustomerBankDetailService(IMapper mapper, ICustomerBankDetailManager customerBankDetailManager)
        {
            _mapper = mapper;
            _customerBankDetailManager = customerBankDetailManager;
        }

        #endregion

        #region Writes

        //Add CustomerBankDetail
        public async Task<Models.DTOs.CustomerBankDetail> Add(Models.DTOs.CustomerBankDetail customerBankDetail)
        {
            CustomerBankDetail req = ManualMappings.MapCustomerBankDetailReq(customerBankDetail);
            return MapToDto(
                await _customerBankDetailManager.Add(req));
        }

        //Update CustomerBankDetail
        public async Task<Models.DTOs.CustomerBankDetail> Update(Models.DTOs.CustomerBankDetail customerBankDetail)
        {
            CustomerBankDetail req = ManualMappings.MapCustomerBankDetailReq(customerBankDetail);
            return MapToDto(
                await _customerBankDetailManager.Update(req));
        }

        public async Task<bool> Delete(Models.DTOs.CustomerBankDetail customerBankDetail)
        {
            CustomerBankDetail req = ManualMappings.MapCustomerBankDetailReq(customerBankDetail);
            return 
                await _customerBankDetailManager.Delete(req);
        }

        #endregion

        #region Reads

        public Models.DTOs.CustomerBankDetail Get(int? customerId, int? bankDetailId)
        {
            return MapToDto(_customerBankDetailManager.Get(customerId, bankDetailId));
        }

        #endregion

        private Models.DTOs.CustomerBankDetail MapToDto(CustomerBankDetail detail)
        {
            if (detail == null)
                return null;
            return new Models.DTOs.CustomerBankDetail
            {
                CustomerId = detail.CustomerId,
                BankDetailsId = detail.BankDetailsId,
                MandateAccepted = detail.MandateAccepted,
                DateMandateAccepted = detail.DateMandateAccepted,
                IsDeleted = detail.IsDeleted,
                IsActive = detail.IsActive
            };
        }
    }
}
