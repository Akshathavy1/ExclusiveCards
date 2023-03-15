using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs.StagingModels;
using System;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IStagingCustomerRegistrationService
    {
        #region Writes

        Task<CustomerRegistration> AddAsync(CustomerRegistration customerRegistration);
        Task<CustomerRegistration> UpdateAsync(CustomerRegistration customerRegistration);

        #endregion

        #region Reads

        Task<CustomerRegistration> GetByCustomerPaymentIdAsync(Guid customerPaymentId, int? statusId = null);

        #endregion

    }
}
