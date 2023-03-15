using System;
using System.Collections.Generic;
using System.Text;
using dto = ExclusiveCard.Services.Models.DTOs;
using sta = ExclusiveCard.Data.StagingModels;

namespace ExclusiveCard.Managers
{
    public interface IPaymentManager
    {
        bool CheckCustomerPaymentExists(string paymentProviderRef);

        string FindUserIdFromStaging(string customerPaymentRef);

        int? GetCustomerIdFromSubscription(string subscriptionId);

        sta.CustomerRegistration GetStagingCustomerRegistration(string customerPaymentRef);

        void DeleteStagingCustomerRegistration(string customerPaymentRef);

        int GetPaymentProviderId(string name);

        int CreatePaymentNotification(int paymentProviderId, string customerPaymentProviderId, string transactionType, string message, DateTime? dateReceived = null);

        int CreateCustomerPayment(dto.CustomerPayment customerPayment);

        dto.PaymentSubscription GetPaymentSubscription(string subscriptionId);

        int CreatePaymentSubscription(dto.PaymentSubscription subscription);

        void UpdatePaymentSubscription(dto.PaymentSubscription subscription);
    }
}
