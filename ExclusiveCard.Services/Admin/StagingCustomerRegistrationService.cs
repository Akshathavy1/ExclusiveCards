using System;
using AutoMapper;
using System.Threading.Tasks;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;
using data = ExclusiveCard.Data.StagingModels;
using ST = ExclusiveCard.Services.Models.DTOs.StagingModels;


namespace ExclusiveCard.Services.Admin
{
    public class StagingCustomerRegistrationService : IStagingCustomerRegistrationService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IStagingCustomerRegistrationManager _manager;

        #endregion

        #region Constructor

        public StagingCustomerRegistrationService(IMapper mapper, IStagingCustomerRegistrationManager stagingCustomerRegistrationManager)
        {
            _mapper = mapper;
            _manager = stagingCustomerRegistrationManager;
        }

        #endregion

        #region Writes

        //Add ST.CustomerRegistration
        public async Task<ST.CustomerRegistration> AddAsync(ST.CustomerRegistration customerRegistration)
        {
            var custRegEntity  = _mapper.Map<data.CustomerRegistration>(customerRegistration);
            custRegEntity = await _manager.AddAsync(custRegEntity);
            var custRegStaging = _mapper.Map<ST.CustomerRegistration>(custRegEntity);

            return custRegStaging;
        }

        //update ST.CustomerRegistration
        public async Task<ST.CustomerRegistration> UpdateAsync(ST.CustomerRegistration customerRegistration)
        {
            return _mapper.Map<ST.CustomerRegistration>(
                await _manager.UpdateAsync(_mapper.Map<data.CustomerRegistration>(customerRegistration)));
        }

        #endregion

        #region Reads

        //Get ST.CustomerRegistration By CustomerPaymentId
        public async Task<ST.CustomerRegistration> GetByCustomerPaymentIdAsync(Guid customerPaymentId, int? statusId = null)
        {
            return _mapper.Map<ST.CustomerRegistration>(
                await _manager.GetByCustomerPaymentIdAsync(customerPaymentId, statusId));
        }

        #endregion
    }
}
