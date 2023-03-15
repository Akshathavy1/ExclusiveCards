using System;
using System.Threading.Tasks;
using ST = ExclusiveCard.Data.StagingModels;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IStagingCustomerRegistrationManager
    {
        Task<ST.CustomerRegistration> AddAsync(ST.CustomerRegistration customerSerializeData);
        Task<ST.CustomerRegistration> UpdateAsync(ST.CustomerRegistration customerSerializeData);
        Task<ST.CustomerRegistration> GetByCustomerPaymentIdAsync(Guid customerPaymentId, int? statusId);
    }
}