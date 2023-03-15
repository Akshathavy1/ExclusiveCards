using db = ExclusiveCard.Data.Models;
using sta = ExclusiveCard.Data.StagingModels;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ExclusiveCard.Enums;

namespace ExclusiveCard.Managers
{
    /// <summary>
    /// PaymentManager will be responsible for managing payments made by the customer. 
    /// This is specifically payments/refunds a customer makes/receives on their accounnt via a third party provider such as PayPal.
    /// Movement of money to/from a RewardPartner (eg TAM) is not considered a CustomerPayment, and will be handled separately 
    /// by a RewardManager
    /// Money received from Cashback is also not classed as a CustomerPayment and will be dealt with by the Cashback Manager
    /// 
    /// Also note that this manager is generic and not tied to any specific payment provider. 
    /// Specific payment providers like PayPal must use their own manager for any non-generic actions. 
    /// </summary>
    public class PaymentManager : IPaymentManager
    {
        #region Private fields and Constructor

        private readonly IRepository<db.PaymentNotification> _paymentNotificationRepo;
        private readonly IRepository<db.PaymentProvider> _paymentProviderRepo;
        private readonly IRepository<db.CustomerPayment> _customerPaymentRepo;
        private readonly IRepository<sta.CustomerRegistration> _stagingCustomerRegRepo;
        private readonly IRepository<db.PayPalSubscription> _paymentSubscriptionRepo;
        
        private readonly IMapper _mapper;


        public PaymentManager(IRepository<db.PaymentNotification> paymentNotificationRepo, IRepository<db.PaymentProvider> paymentProviderRepo
                              , IRepository<db.CustomerPayment> customerPaymentRepo, IRepository<sta.CustomerRegistration> stagingCustomerRegRepo
                              , IRepository<db.PayPalSubscription> paymentSubscriptionRepo,  IMapper mapper)
        {
            _paymentNotificationRepo = paymentNotificationRepo;
            _paymentProviderRepo = paymentProviderRepo;
            _customerPaymentRepo = customerPaymentRepo;
            _stagingCustomerRegRepo = stagingCustomerRegRepo;
            _paymentSubscriptionRepo = paymentSubscriptionRepo;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks whether a customer payment record exists based on the supplied reference value
        /// The reference value will be held in the CustomerPayment.PaymentProviderReference column
        /// </summary>
        /// <param name="paymentProviderRef">a unique transaction id supplied by the payment provider (ie. PayPal)</param>
        /// <returns></returns>
        public bool CheckCustomerPaymentExists(string paymentProviderRef)
        {
            bool exists = false;

            var customerPayment = _customerPaymentRepo.Get(x => x.PaymentProviderRef == paymentProviderRef);
            if (customerPayment != null)
                exists = true;

            return exists;
        }

        public string FindUserIdFromStaging(string customerPaymentRef)
        {
            string userId = null;
            if (Guid.TryParse(customerPaymentRef, out Guid customerPaymentId))
            {
                var customerReg = _stagingCustomerRegRepo.Get(x => x.CustomerPaymentId == customerPaymentId);
                if (customerReg != null)
                    userId = customerReg.AspNetUserId;
            }

            return userId;
        }

        public void DeleteStagingCustomerRegistration(string customerPaymentRef)
        {
            if (Guid.TryParse(customerPaymentRef, out Guid customerPaymentId))
            {
                var customerReg = _stagingCustomerRegRepo.Get(x => x.CustomerPaymentId == customerPaymentId);
                if (customerReg != null)
                {
                    _stagingCustomerRegRepo.Delete(customerReg);
                    _stagingCustomerRegRepo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Finds the record in Staging.CustomerRegistratio, based on supplied customer payment reference
        /// Records in this table are created before customers are redirected to a payment provider website to take payment
        /// </summary>
        public sta.CustomerRegistration GetStagingCustomerRegistration(string customerPaymentRef)
        {
            sta.CustomerRegistration stagingCustReg = null;

            if (Guid.TryParse(customerPaymentRef, out Guid customerPaymentId))
            {
                var customerReg = _stagingCustomerRegRepo.Get(x => x.CustomerPaymentId == customerPaymentId);
                if (customerReg != null)
                    stagingCustReg = _mapper.Map<sta.CustomerRegistration>(customerReg);
            }

            return stagingCustReg;
        }

        public dto.PaymentSubscription GetPaymentSubscription(string subscriptionId)
        {
            dto.PaymentSubscription subscription = null;

            var subscriptionEntity = _paymentSubscriptionRepo.Get(x => x.PayPalId == subscriptionId);
            if (subscriptionEntity != null)
                subscription = _mapper.Map<dto.PaymentSubscription>(subscriptionEntity);

            return subscription;
        }

        public int? GetCustomerIdFromSubscription(string subscriptionId)
        {
            var subscription = _paymentSubscriptionRepo.Get(x => x.PayPalId == subscriptionId);
            return subscription?.CustomerId;
        }

        /// <summary>
        /// Returns the Id of the payment provider based on its name
        /// Used to get the foreign key value of the Payment Provider from the Db
        /// Name check done against PaymentProvider.Name column
        /// </summary>
        /// <param name="name">Name of payment provider to locate</param>
        /// <returns>The Id of the matching record in the PaymentProvider table</returns>
        public int GetPaymentProviderId(string name)
        {
            int providerId = 0;
            
            var provider = _paymentProviderRepo.Get(x => x.Name == name && x.IsActive == true);
            if (provider != null)
                providerId = provider.Id;

            return providerId;
        }

        /// <summary>
        /// Saves a payment notification message to the Exclusive.PaymentNotification table in the Database.
        /// A payment notification is the data received from a payment provider (e.g. Paypal) in a callback
        /// that confirms a payment has been received. 
        /// The message can be in any format and is saved as a string value
        /// It cannot be assumed that the payment relates to a customer payment. At the time of writing
        /// PayPal is only used for customer payments but the system may be extended to other entities (e.g. partners)
        /// in the future, and may use different payment providers
        /// </summary>
        /// <param name="paymentProviderId">Id of the payment provider</param>
        /// <param name="customerPaymentProviderId">Unique id to indicate the source of the payment. Named CustomerPaymentProviderId in db - even though may not always be from a customer. Beware!</param>
        /// <param name="transactionType">Type of the message, as defined by the payment provider</param>
        /// <param name="message">The message.  Could be any format.</param>
        /// <returns></returns>
        public int CreatePaymentNotification(int paymentProviderId, string customerPaymentProviderId, string transactionType, string message, DateTime? dateReceived = null)
        {
            var notification = new db.PaymentNotification()
            {
                PaymentProviderId = paymentProviderId,
                TransactionType = transactionType,
                DateReceived = dateReceived ?? DateTime.UtcNow,
                FullMessage = message,
                CustomerPaymentProviderId = customerPaymentProviderId
            };

            _paymentNotificationRepo.Create(notification);
            _paymentNotificationRepo.SaveChanges();

            var paymentNotificationId = notification.Id;
            return paymentNotificationId;
        }

        public int CreateCustomerPayment(dto.CustomerPayment customerPayment)
        {
            
            var custPaymentEntity = _mapper.Map<db.CustomerPayment>(customerPayment);
            _customerPaymentRepo.Create(custPaymentEntity);
            _customerPaymentRepo.SaveChanges();

            return custPaymentEntity.Id;
        }

        /// <summary>
        /// Saves a details about a payment subscription to the database.
        /// The table is unfortnately named after PayPal but in theory can be used by other providers (as and when they are supported). 
        /// </summary>
        /// <param name="subscription">A PaymentSubscription data transfer object, containing the data to save</param>
        /// <returns>The Id of the new payment subscription record</returns>
        public int CreatePaymentSubscription(dto.PaymentSubscription subscription)
        {
            var subscriptionEntity = _mapper.Map<db.PayPalSubscription>(subscription);
            _paymentSubscriptionRepo.Create(subscriptionEntity);
            _paymentSubscriptionRepo.SaveChanges();

            return subscriptionEntity.Id;

        }

        /// <summary>
        /// Performs a limited update on an existing payment subscription record
        /// Allows updates to the date and amount of next payment and the subscription length (payment type)
        /// Also updates the status to active. 
        /// Changes to the customer / membership plan / subscription id are not allowed - create a new subscription instead.
        /// </summary>
        /// <param name="subscription"></param>
        public void UpdatePaymentSubscription(dto.PaymentSubscription subscription)
        {
            var subscriptionEntity = _paymentSubscriptionRepo.GetById(subscription.Id);
            subscriptionEntity.NextPaymentDate = subscription.NextPaymentDate;
            subscriptionEntity.NextPaymentAmount = subscription.NextPaymentAmount;
            subscriptionEntity.PaymentType = subscription.PaymentType;
            subscriptionEntity.PayPalStatusId = (int)PaypalSubscription.Active;
            _paymentSubscriptionRepo.Update(subscriptionEntity);
            _paymentSubscriptionRepo.SaveChanges();
        }

        #endregion
    }
}
