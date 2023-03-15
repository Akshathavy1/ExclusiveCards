using System;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    [Obsolete("Use the ExclusiveCard.Managers.PaymentManager class in place of this CRUD.CustomerPaymentService")]
    public class CustomerPaymentService : ICustomerPaymentService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ICustomerPaymentManager _customerPaymentManager;

        #endregion

        #region Constructor

        public CustomerPaymentService(IMapper mapper, ICustomerPaymentManager customerPaymentManager)
        {
            _mapper = mapper;
            _customerPaymentManager = customerPaymentManager;

        }

        #endregion

        #region Writes

        //Add CustomerPayment

        public async Task<Models.DTOs.CustomerPayment> Add(DTOs.CustomerPayment customerPayment)
        {
            CustomerPayment req = MapToData(customerPayment);
            return MapToDto(await _customerPaymentManager.Add(req));
        }

        //Update CustomerPayment

        public async Task<Models.DTOs.CustomerPayment> Update(DTOs.CustomerPayment customerPayment)
        {
            CustomerPayment req = MapToData(customerPayment);
            return MapToDto(await _customerPaymentManager.Update(req));
        }

        //Delete customer payment
        public async Task DeleteAsync(DTOs.CustomerPayment customerPayment)
        {
            CustomerPayment req = MapToData(customerPayment);
            await _customerPaymentManager.DeleteAsync(req);
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.CustomerPayment> GetByPaymentProviderRef(string paymentProviderRef)
        {
            return MapToDto(await _customerPaymentManager.GetByPaymentProviderRef(paymentProviderRef));
        }

        public async Task<Models.DTOs.CustomerPayment> GetByCustomerPaymentProviderId (string customerPaymentProviderId)
        {
            return MapToDto(await _customerPaymentManager.GetByCustomerPaymentProviderId(customerPaymentProviderId));
        }

        public async Task<Models.DTOs.CustomerPayment> GetByCustomerPaymentDateAmount(int customerId,
            DateTime paymenDate, decimal amount)
        {
            return MapToDto(
                await _customerPaymentManager.GetByCustomerPaymentDateAmount(customerId, paymenDate, amount));
        }

        #endregion

        #region Private methods

        private CustomerPayment MapToData(DTOs.CustomerPayment payment)
        {
            CustomerPayment customer = new CustomerPayment
            {
                CustomerId = payment.CustomerId,
                MembershipCardId = payment.MembershipCardId,
                PaymentProviderId = payment.PaymentProviderId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                CurrencyCode = payment.CurrencyCode,
                Details = payment.Details,
                CashbackTransactionId = payment.CashbackTransactionId,
                PaymentNotificationId = payment.PaymentNotificationId,
                PaymentProviderRef = payment.PaymentProviderRef
            };
            if (payment.Id > 0)
            {
                customer.Id = payment.Id;
            }
            return customer;
        }

        private Models.DTOs.CustomerPayment MapToDto(CustomerPayment payment)
        {
            if (payment == null)
                return null;

            Models.DTOs.CustomerPayment customer = new Models.DTOs.CustomerPayment
            {
                CustomerId = payment.CustomerId,
                MembershipCardId = payment.MembershipCardId,
                PaymentProviderId = payment.PaymentProviderId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                CurrencyCode = payment.CurrencyCode,
                Details = payment.Details,
                CashbackTransactionId = payment.CashbackTransactionId,
                PaymentNotificationId = payment.PaymentNotificationId,
                PaymentProviderRef = payment.PaymentProviderRef
            };
            if (payment.Id > 0)
            {
                customer.Id = payment.Id;
            }
            return customer;
        }

        #endregion
    }
}
