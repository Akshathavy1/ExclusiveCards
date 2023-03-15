using System;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface ICustomerPaymentService
    {
        #region Writes

        Task<CustomerPayment> Add(CustomerPayment customerPayment);

        Task<CustomerPayment> Update(CustomerPayment customerPayment);

        Task DeleteAsync(CustomerPayment customerPayment);

        #endregion

        #region Reads

        Task<CustomerPayment> GetByPaymentProviderRef(string paymentProviderRef);

        Task<CustomerPayment> GetByCustomerPaymentProviderId(string paymentProviderRef);

        Task<CustomerPayment> GetByCustomerPaymentDateAmount(int customerId,
            DateTime paymenDate, decimal amount);

        #endregion

    }
}
