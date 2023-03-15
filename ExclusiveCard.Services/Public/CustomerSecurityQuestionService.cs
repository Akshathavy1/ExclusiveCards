using System;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;

namespace ExclusiveCard.Services.Public
{
    [Obsolete]

   public class CustomerSecurityQuestionService: ICustomerSecurityQuestionService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ICustomerSecurityQuestionManager _customerSecurityQuestionManager;

        #endregion

        #region Constructor

        public CustomerSecurityQuestionService(IMapper mapper, ICustomerSecurityQuestionManager customerSecurityQuestionManager)
        {
            _mapper = mapper;
            _customerSecurityQuestionManager = customerSecurityQuestionManager;
        }

        #endregion

        #region Writes

        //Add Customer
        public async Task<Models.DTOs.CustomerSecurityQuestion> Add(Models.DTOs.CustomerSecurityQuestion customerSecurityQuestion)
        {
            CustomerSecurityQuestion req = _mapper.Map<CustomerSecurityQuestion>(customerSecurityQuestion);
            return _mapper.Map<Models.DTOs.CustomerSecurityQuestion>(
                await _customerSecurityQuestionManager.Add(req));
        }

        //Update Customer
        public async Task<Models.DTOs.CustomerSecurityQuestion> Update(Models.DTOs.CustomerSecurityQuestion customerSecurityQuestion)
        {
            CustomerSecurityQuestion req = _mapper.Map<CustomerSecurityQuestion>(customerSecurityQuestion);
            return _mapper.Map<Models.DTOs.CustomerSecurityQuestion>(
                await _customerSecurityQuestionManager.Update(req));
        }

        #endregion

        #region
        public Models.DTOs.CustomerSecurityQuestion Get(int customerId)
        {
            return _mapper.Map<Models.DTOs.CustomerSecurityQuestion>(_customerSecurityQuestionManager.Get(customerId));
        }
        #endregion
    }
}
