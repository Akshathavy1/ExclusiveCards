using ExclusiveCard.Enums;
using ExclusiveCard.Managers;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;

namespace ExclusiveCard.Services.Admin
{
    /// <summary>
    /// Replacement class for the original PayPalIPNSerivce (quicker to rewrite than polish that turd)
    /// This service is responsible for processing Instant Payment Notifications (IPN) received from Paypal,
    /// when a customer payment is taken
    /// There are 3 main events we are interested in
    /// 1) A diamond membership upgrade
    /// 2) An automatic renewal of a diamond membership via a reoccuring payment (PayPal subscription)
    /// 3) An account boost
    /// This service will interpert the IPN, decide which event it relates to and fire of the managers to do the 
    /// appropriate data processing.
    /// </summary>
    public class PayPalService : IPayPalService
    {
        #region Private fields and Constructor

        private const string PAYPAL_PROVIDER = "PayPal";
        private const string IPN_TYPE = "txn_type";
        private const string CUSTOMER_PROVIDER_REF = "custom";
        private const string PAYMENT_PROVIDER_REF = "txn_id";
        private const string PAYMENT_DATE = "payment_date";
        private const string PAYMENT_AMOUNT = "mc_gross";
        private const string CURRENCY_CODE = "mc_currency";
        private const string DETAILS = "transaction_subject";
        private const string SUBSCRIPTION_ID = "subscr_id";
        private const string REOCCURING_SUBS_ID = "recurring_payment_id";
        private const string SUBSCRIPTION_LENGTH = "payment_cycle";
        private const string BOOST_DETAILS = "item_name";

        private const string IPN_RENEWAL = "recurring_payment";
        private const string IPN_UPGRADE = "subscr_payment";
        private const string IPN_BOOST = "web_accept";

        private int _paymentProviderId = 0;
        private readonly ILogger _logger;

        private readonly IPaymentManager _paymentManager;
        private readonly ICustomerManager _customerManager;
        private readonly IMembershipManager _membershipManager;
        private readonly ICashbackManager _cashbackManager;
        private readonly IEmailManager _emailManager;


        public PayPalService(IPaymentManager paymentManager, ICustomerManager customerManager, IMembershipManager membershipManager, ICashbackManager cashbackManager, IEmailManager emailService)
        {
            _paymentManager = paymentManager;
            _customerManager = customerManager;
            _membershipManager = membershipManager;
            _cashbackManager = cashbackManager;
            _emailManager = emailService;

            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the IPN messages that are received from PayPal and carries out the required processing.
        /// IPN = Instant Payment Notification
        /// All IPNs are saved to the Exclusive.PaymentNotification table regardless of whether valid, duplicates etc
        /// Every valid IPN will result in a new record added to Excluisve.CustomerPayment.  This table is used to 
        /// determine whether the IPN has been processed before or not. 
        /// Valid IPN types (as Apr 2020) are
        ///  - Account Boosts
        ///  - Diamond Account Upgrade (these are new subscription sign ups)
        ///  - Renewals - Automatic renewal of membership subscription - unless customer cancels it prior to renewal.
        /// </summary>
        /// <param name="ipn">A string holding the full IPN as received fro Paypal callback</param>
        /// <param name="adminEmail">The email address to send error messages to (if error occurs during processing)</param>
        /// <returns>Id of the new record in the Exclusive.PaymentNotification table</returns>
        public async Task<int> ProcessIPN(string ipn, string adminEmail)
        {
            int ipnId = 0;
            int customerPaymentId = 0;
            int? customerId = null;
            MembershipCard membershipCard = null;
            string ipnType = string.Empty;
            string paymentProviderRef = string.Empty;

            try
            {
                // ii. Look up Paypal Payment provider from the Db
                GetPaymentProviderId();

                // i.  Read key data from the ipn
                var ipnDict = ParsePaypalIPN(ipn);
                ipnType = ipnDict.GetValueOrDefault(IPN_TYPE);
                string customerProviderRef = ipnDict.GetValueOrDefault(CUSTOMER_PROVIDER_REF);

                // 1.  Save the IPN to the Db. Every IPN received is saved in case needed for audit / debug later
                ipnId = _paymentManager.CreatePaymentNotification(_paymentProviderId, customerProviderRef, ipnType, ipn);



                // Now process rest of the fields in the IPN
                paymentProviderRef = ipnDict.GetValueOrDefault(PAYMENT_PROVIDER_REF);                
                DateTime? paymentDate = ConvertDate(ipnDict.GetValueOrDefault(PAYMENT_DATE));
                decimal paymentAmount = Convert.ToDecimal(ipnDict.GetValueOrDefault(PAYMENT_AMOUNT));
                string currencyCode = ipnDict.GetValueOrDefault(CURRENCY_CODE);
                string subscriptionLength = ipnDict.GetValueOrDefault(SUBSCRIPTION_LENGTH);

                string subscriptionId = null;
                if (ipnType == IPN_RENEWAL)
                    subscriptionId = ipnDict.GetValueOrDefault(REOCCURING_SUBS_ID);
                else
                    subscriptionId = ipnDict.GetValueOrDefault(SUBSCRIPTION_ID);

                string details = ipnDict.GetValueOrDefault(DETAILS);
                string boostDetails = ipnDict.GetValueOrDefault(BOOST_DETAILS);
                if (ipnType == IPN_BOOST)
                    details = boostDetails;

              

                
                // 2.  Check whether this IPN (or duplicate of it) has been processed before and is a recongnised IPN type
                if (ipnType != null && paymentProviderRef != null && (ipnType == IPN_RENEWAL || ipnType == IPN_UPGRADE || ipnType == IPN_BOOST))
                {
                    bool alreadyProcessed = CheckIfAlreadyProcessed(paymentProviderRef);

                    // If not already processed , save the customer payment record
                    if (!alreadyProcessed)
                    {
                        try
                        {
                            customerId = FindCustomerId(ipnType, customerProviderRef, subscriptionId);
                            membershipCard = GetMembershipCard(customerId);
                            
                            customerPaymentId = SaveCustomerPayment(ipnId, customerId, membershipCard?.Id, (DateTime)paymentDate, paymentAmount, currencyCode, details, paymentProviderRef);
                        }
                        catch (Exception ex)
                        {
                            // Log any exception that happens if customer payment record cannot be saved.
                            _logger.Error(ex, "IPN Procssing failed - Could not save the customer Payment record for ipn Id " + ipnId.ToString());
                        }
                    }
                }

                // 3.  Carry on with rest of IPN processing only if customer payment record was successfully saved above
                //     If payment record already existed / failed for any reason, then rest of process should not continue.
                //     IE,  no payment, no diamond card 
                if (customerPaymentId != 0)
                {
                    switch (ipnType)
                    {
                        case IPN_RENEWAL:
                            ProcessRenewal(membershipCard, subscriptionId, (DateTime)paymentDate, paymentAmount, details, subscriptionLength);
                            // No staging record to delete on renewals - they arrive direct from Paypal without redirecting from our site first
                            break;

                        case IPN_UPGRADE:
                            ProcessDiamondUpgrade(membershipCard, subscriptionId, (DateTime)paymentDate, paymentAmount, details);
                            DeleteStagingCustomerRegistration(subscriptionId);
                            break;

                        case IPN_BOOST:
                            ProcessAccountBoost(membershipCard?.Id, paymentAmount, (DateTime)paymentDate, currencyCode);
                            DeleteStagingCustomerRegistration(subscriptionId);
                            break;

                    }
                }

            }
            catch(Exception ex)
            {
                _logger.Error(ex, "IPN processing failed due to error.  IPN = " + ipn);
                await SendOutEmail(paymentProviderRef, ipnType, adminEmail, ex.Message);
            }

            return ipnId;

        }


        #endregion

        #region Private Methods

        private void ProcessRenewal(MembershipCard card, string subscriptionId, DateTime paymentDate, decimal paymentAmount, string paymentDetails, string subscriptionLength)
        {
            // Search for Payment subscription and update (or create if not found)
            var subscription = _paymentManager.GetPaymentSubscription(subscriptionId);
            
            // If not found, create a new one
            if (subscription == null)
            {
                // Find diamond plan from the standard plan id
                var diamondPlan = _membershipManager.GetDiamondMembershipPlan(card.MembershipPlanId);
                CreatePaymentSubscription(card, subscriptionId, paymentDate.AddYears(1), paymentAmount, subscriptionLength, diamondPlan.Id);
            }
            // else update the existing one
            else
            {
                subscription.NextPaymentDate = paymentDate;
                subscription.NextPaymentAmount = paymentAmount;
                subscription.PaymentType = subscriptionLength;
                _paymentManager.UpdatePaymentSubscription(subscription);
            }

            // Renew membership card.
            // As of writing, subscriptions are fixed to 1 year only 
            // Therefore, duration is set from the plan, and not dynamically calculated from the IPN
            _membershipManager.Renew(card, paymentDetails);

            // Send out renewal email
            var customer = _customerManager.Get(card.CustomerId ?? 0);
            if (customer != null && customer.ContactDetail != null)
            {
                _emailManager.SendEmailAsync(customer.ContactDetail.EmailAddress,
                                        (int)Enums.EmailTemplateType.CardRenewedThankYouEmail,
                                        new { Name = $"{customer.Forename} {customer.Surname}" });
            }
            else
                _logger.Warn("Unable to send renewal email confirmation for membershipcardId " + card.Id.ToString());

        }

        private void ProcessDiamondUpgrade(MembershipCard card, string subscriptionId, DateTime paymentDate, decimal paymentAmount, string paymentDetails)
        {
            // Work out subscription frequency - value is duration,  stored as number of days,  on membership plan
            string subscriptionLength = "Custom";
            if (card.MembershipPlan.Duration == 1)
                subscriptionLength = "Daily";
            else if (card.MembershipPlan.Duration == 7)
                subscriptionLength = "Weekly";
            else if (card.MembershipPlan.Duration == 14)
                subscriptionLength = "Fortnightly";
            else if (card.MembershipPlan.Duration == 28)
                subscriptionLength = "Fourweekly";
            else if (card.MembershipPlan.Duration > 28 && card.MembershipPlan.Duration < 32)
                subscriptionLength = "Monthly";
            else if (card.MembershipPlan.Duration >= 360 && card.MembershipPlan.Duration < 367)
                subscriptionLength = "Yearly";

            paymentDate = paymentDate.AddDays(card.MembershipPlan.Duration);

            // Find diamond plan from the standard plan id
            var diamondPlan = _membershipManager.GetDiamondMembershipPlan(card.MembershipPlanId);
            CreatePaymentSubscription(card, subscriptionId, paymentDate, paymentAmount, subscriptionLength, diamondPlan.Id);

            // Create a Diamond membership card
            _membershipManager.UpgradeToDiamond(card, paymentDetails, paymentAmount);

            
        }

        private void ProcessAccountBoost(int? membershipCardId, decimal paymentAmount, DateTime paymentDate, string currencyCode)
        {
            if (membershipCardId == null)
                throw new Exception("Account Boost failed - The membership card id was null");

            // Add a CashbackTransaction of type User_Paid for the boost amount
            _cashbackManager.AddAccountBoost((int)membershipCardId, paymentAmount, (DateTime)paymentDate, currencyCode);
        }


        private void CreatePaymentSubscription(MembershipCard card, string subscriptionId, DateTime paymentDate, decimal paymentAmount, string subscriptionLength, int planId)
        {
            
            // Insert a new payment subscription record
            var subscription = new PaymentSubscription()
            {
                CustomerId = (int)card.CustomerId,
                SubscriptionId = subscriptionId,
                StatusId = (int)PaypalSubscription.Active,
                NextPaymentDate = paymentDate ,
                NextPaymentAmount = paymentAmount,
                PaymentType = subscriptionLength,
                MembershipPlanId = planId
            };
            _paymentManager.CreatePaymentSubscription(subscription);
        }

       

        private MembershipCard GetMembershipCard(int? customerId)
        {
            MembershipCard membershipCard = null;
            if (customerId != null)
            {
                membershipCard = _membershipManager.GetActiveMembershipCard((int)customerId);
            }

            return membershipCard;
        }


        // Converts a date from the format in the  IPN.
        // Copied from original quality code, seems to work but no guarantees        
        private DateTime? ConvertDate(string dateIn)
        {
            DateTime? dateOutNullable = null;

            if (dateIn != null)
            {
                //Reduced the date formats by generalizng for PST/PDT as that zonal info is processed before
                string[] dateFormats = { "HH:mm:ss MMM dd, yyyy zzz", "HH:mm:ss MMM. dd, yyyy zzz",
                                        "HH:mm:ss dd MMM yyyy zzz", "HH:mm:ss dd MMM. yyyy zzz"
                };


                var decodedPaymentDate = HttpUtility.UrlDecode(dateIn);
                DateTime.TryParseExact(decodedPaymentDate.Replace("PST", "-08:00").Replace("PDT", "-07:00"),
                    dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOut);
            
                dateOutNullable = dateOut.ToUniversalTime();
            }

            return dateOutNullable;
        }

        private int? FindCustomerId(string ipnType, string customerProviderRef, string subscriptionId)
        {
            int? customerId = null;

            // If iPN is a renewal (reoccuring payment), find the customer id from the paypal subscription
            if (ipnType == IPN_RENEWAL)
            {
                customerId = _paymentManager.GetCustomerIdFromSubscription(subscriptionId);
            }
            // otherwise check the staging.CustomerRegistration table for the ASPNETUserId and use that to find customer
            else
            {
                
                var userID = _paymentManager.FindUserIdFromStaging(customerProviderRef);
                if (userID != null)
                {
                    customerId = _customerManager.FindCustomerId(userID);
                }
            }

            return customerId;
        }

        private int SaveCustomerPayment(int ipnId, int? customerId, int? membershipCardId,   DateTime paymentDate, decimal paymentAmount, string currencyCode, string details, string paymentProviderRef)
        {
            var customerPayment = new CustomerPayment()
            {
                PaymentProviderId = _paymentProviderId,
                CustomerId = customerId,
                MembershipCardId = membershipCardId,
                PaymentDate = paymentDate,
                Amount = paymentAmount,
                CurrencyCode = currencyCode,
                Details = details,
                PaymentNotificationId = ipnId,
                PaymentProviderRef = paymentProviderRef
            };

            var customerPaymentId = _paymentManager.CreateCustomerPayment(customerPayment);
            return customerPaymentId;
        }

        private void GetPaymentProviderId()
        {
            if (_paymentProviderId == 0)
            {
                _paymentProviderId = _paymentManager.GetPaymentProviderId(PAYPAL_PROVIDER);
            }
        }

        private Dictionary<string, string> ParsePaypalIPN(string postedRaw)
        {
            var result = new Dictionary<string, string>();
            var keyValuePairs = postedRaw.Split('&');
            foreach (var kvp in keyValuePairs)
            {
                var keyvalue = kvp.Split('=');
                var key = keyvalue[0];
                var value = keyvalue[1];
                result.Add(key, value);
            }
            return result;
        }

        private bool CheckIfAlreadyProcessed(string paymentProviderRef)
        {
            bool ok = false;

            // To see if an IPN has already been processed, the system looks for a customer payment record
            // which has a matching payment provider reference. 
            // Duplicate IPNs may be sent from PayPal for the same transaction. We need to ensure the transaction is 
            // only processed the once. Payal provides a unique transaction Id for each transaction, which we call the 
            // paymentProviderRef. Duplicate IPNs will contain the same PaymentProviderRef.  
            
            // When an IPN is received, the notification is always saved, duplicate or not.
            // The customerPayment record is created next regardless of the type of transaction, so that is done first
            // By checking the PaymentProviderRef does not exist, we can ensure we don't process a single customer payment
            // more than once.
            ok = _paymentManager.CheckCustomerPaymentExists(paymentProviderRef);

            return ok;

        }

        private void DeleteStagingCustomerRegistration(string subscriptionId)
        {
            _paymentManager.DeleteStagingCustomerRegistration(subscriptionId);
        }

        // Legacy code.  Email service not been reviewed yet. Don't shoot the messenger. 
        private async Task SendOutEmail(string transactionId, string transactionType, string adminEmail, string message)
        {
            try
            {
                var email = new Email()
                {
                    EmailTo = new List<string>() { adminEmail },
                    BodyHtml = $"Dear Admin,<br/><p>Failed to process {transactionType} IPN with transaction Id - {transactionId}. {message}</p>",
                    Subject = $"IPN failure {transactionType} IPN {transactionId}",
                    BodyPlainText = $"Dear Admin, Failed to process {transactionType} IPN with transaction Id - {transactionId}. {message}"
                };


                var res = await _emailManager.SendEmailAsync(email);
                if (res != true.ToString())   // WTF?  Ever heard of returning a bool???
                    _logger.Error("Error sending email when failed to process IPN.");
            }
            catch (Exception ex )
            {
                _logger.Error(ex, $"Error sending email when failed to process IPN.");
            }
        }

        #endregion
    }
}
