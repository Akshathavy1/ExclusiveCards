using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ICustomerPaymentManager
    {
        Task<CustomerPayment> Add(CustomerPayment customerPayment);
        Task<CustomerPayment> Update(CustomerPayment customerPayment);
        Task DeleteAsync(CustomerPayment customerPayment);
        Task<CustomerPayment> GetByPaymentProviderRef(string paymentProviderRef);
        Task<CustomerPayment> GetByCustomerPaymentProviderId(string customerPaymentProviderId);
        Task<List<CustomerPayment>> GetAll();

        Task<CustomerPayment> GetByCustomerPaymentDateAmount(int customerId, DateTime paymentDate,
            decimal amount);
    }
}