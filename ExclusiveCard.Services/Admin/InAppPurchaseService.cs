using ExclusiveCard.Enums;
using Microsoft.Extensions.Configuration;
using ExclusiveCard.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using System.Threading.Tasks;
using AutoMapper;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Net;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.AndroidPublisher.v3;

namespace ExclusiveCard.Services.Admin
{
    public class InAppPurchaseService : IInAppPurchaseService
    {
        private const string IAP_SUBSCRIPTION = "Subscription";
        private const string IAP_PURCHASE = "InAppPurchase";

        private static HttpClient _client = new HttpClient();
        private static JsonSerializer _serializer = new JsonSerializer();

        //## Need to change this so they are read from appsetting.json...
        private static string AppleProductionUrl = "https://buy.itunes.apple.com/verifyReceipt";
        private static string AppleTestUrl = "https://sandbox.itunes.apple.com/verifyReceipt";
        private static string GooglePlayAccount = "exclusiverewards2@pc-api-6342797585269275076-388.iam.gserviceaccount.com";
        //NOTE: best way I could get \n in an app setting
        private static string GooglePlayKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDVS1jlrEuBhcBd\nJ9hzNinx0ESayWmGLr+8HhnQD62lMAm1ZmhOUkQyi7KtdIkL31qe4moYF0/MMWbH\n3nxPiKHKt/Yx065UUHabZy+HV/dfKhMQVG2iZ5XIr8o93DO/niU8ZN6nEB1f8LiN\naP1QBxlPDtiAojWZEeMiGjDyJVMBOlMJkg8QbCiB6adeLsNgQfouMSiNBfjYBjIV\nbcv1DPkEVZemJDolZWotk2H+wobxlem+Z+2bJ7AwIoFJuyQy6kBX0RECqZme7zck\n97pnZHVYunij6kSm8HX7BseNvxZt0IVfRA5HC4Y3K5zfar05V5S/arppvp7IRRfS\n5VtCVtjrAgMBAAECggEAAxDiWGOVlkScWU/JE6DKyILSaaJr0pvPOy3Tx1nQDsBV\n+XxdN8EXEidP0m/mWb8yHrSN/rciHcwxU7GwmJ1pRNHOIt/KtD3xYGu5n2mde5YX\nSkB33K/QZeVLRmfi6QSNXbWXMVygFM2uZmaK8pFmywR2tUkfn9MIztWvKiULTIu3\nengVQK2Cd/Z21H5+eJJvmdCcqOFl1hFsnWYx6BjFPwdWDaQjewW6Wbv0fHqqyab7\niOoEkcnUhSC6/eEo87RdrM+Nl6IfOHziwCNVar/wfGV3nrftRAbosy3v5HApmAX3\nGsgFRi7PWPHbSM4I2UWO7tgiIfjGIrgfTmbWEd+nOQKBgQD/n7y04KWg47Eo7A8y\njPU9e1cZ928N40Q2PeMz5cwF/s2xaLGw/Re7FqOquKdfaQ8e7iPGHgNHXDp5ZrAY\nT/tyq3iEM/siFZjnTl58jr5tIKMQo3+aZWn1dF7yo/enKa4jq/DJKhGWsC+pC3sb\nrN0uSOWoUAoJjiQ79uuHKwEQ3QKBgQDVm6tsYZHwYnRLy6J0VquOZPhqZF/eYX1o\nfFz8jZnY5dLrIz4iBHrB70THKcODoknh91NRgdjypDsGISRJhQP8unDR+ZQ2M/C+\nGNShKsZo3s1r6TIA2k3YdThRUuRZBdsBpeXeonWoNvUBgd82lzl5cM7ZptU6br0Y\nJgivcfRQZwKBgGyarU4b88K7elVslbhgcwFAGe2KVsCLwjtZZ+Jwy5Hwg+vQE06i\n+SYfpEGwWkwLEsWNX92YesQixyU1H7P0p1w8xeNFQnlku6XqgfJhoz4yE5XKITiz\nxaTnYD+uslVvO/Ej6BVPrlFbYaRsoE1N0FsN1aUA/IK6xWJEsiSnfLhxAoGAW35F\nC7QzFVqmY3zChcBF1UUMXk3F/nOViraSAZ8JQa0XDZ5X06xFhGHe/Mu3sd9GdCJC\nrsvGSA/uNC7n4xG1Zn/ZKScUw661tEbgdHPk7jBYnpsQzuqyoz96MuMlabgnWWpc\nrmK5sKZxhQbCvBhEBu85umJKPLAIEJrh53wQ4IsCgYEA00ziqWTITOo+2okZTR4v\n0i8DLX6sE4ysie3ZQNBYyw66NuME7whxzuWf7bSZ1yWB9wggPauWc4yVq3yRU3ie\no7+nmOU4wLCzM9dHyz2PrE4LSKgqcAjyPcGg18M+W+PSE8ScdiOfvA+MUS6OFvBq\nDPoVwoXPcG7rrr2ic3+T9jU=\n-----END PRIVATE KEY-----\n".Replace("\\n", "\n");
        //## Need to change this so they are read from appsetting.json...

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        private readonly IUserManager _userManager;
        private readonly IPaymentManager _paymentManager;
        private readonly ICustomerManager _customerManager;
        private readonly IMembershipManager _membershipManager;
        //private IConfiguration AppSettings { get; set; }

        public InAppPurchaseService(IMapper mapper, IUserManager userManager, IPaymentManager paymentManager, ICustomerManager customerManager, IMembershipManager membershipManager)
        {
            _userManager = userManager;
            _paymentManager = paymentManager;
            _customerManager = customerManager;
            _membershipManager = membershipManager;
            //_cashbackManager = cashbackManager;
            //_emailManager = emailService;
            _mapper = mapper;
            //_logger = LogManager.GetCurrentClassLogger();
            _logger = LogManager.GetLogger(GetType().FullName);
            //AppSettings = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //AppleProductionUrl = AppSettings["AppleProductionUrl"];
            //AppleTestUrl = AppSettings["AppleTestUrl"];
            //GooglePlayAccount = AppSettings["GooglePlayAccount"];
            //NOTE: best way I could get \n in an app setting
            //GooglePlayKey = AppSettings["GooglePlayKey"]?.Replace("\\n", "\n");
        }

        #region Apple validation

        public async Task<AppleResponse> PostAppleReceipt(bool toLive, AppleReceipt receipt)
        {
            string url = toLive ? AppleProductionUrl : AppleTestUrl;

            AppleResponse appleResponse = null;
            try
            {
                string json = string.Empty;
                if (receipt.Data != null)
                    json = new JObject(new JProperty("receipt-data", receipt.Data)).ToString();
                else if (!string.IsNullOrWhiteSpace(receipt.Data2))
                    json = new JObject(new JProperty("receipt-data", receipt.Data2)).ToString();
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await _client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                // 1.  Save the iap to the Db. Every iap received is saved in case needed for audit / debug later
                int iapId = _paymentManager.CreatePaymentNotification((int)Enums.PaymentProvider.AppleIAP, receipt.TransactionId,
                    receipt.Id, receipt.ToString(), DateTime.UtcNow);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    appleResponse = _serializer.Deserialize<AppleResponse>(jsonReader);
                }
                appleResponse.PaymentNotificationId = iapId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "");
            }
            return appleResponse;
        }

        public async Task<bool> ProcessAppleDiamond(AppleResponse appleResponse, AppleReceipt receipt)
        {
            if (appleResponse.Status == AppleStatus.Success)
            {
                try
                {
                    var customerPayment = new CustomerPayment()
                    {
                        PaymentProviderId = (int)ExclusiveCard.Enums.PaymentProvider.AppleIAP, //payment.PaymentProvider,
                        //CustomerId = payment.CustomerId, //gets populated in subscription call
                        //MembershipCardId = membershipCardId, //gets populated in subscription call
                        PaymentDate = DateTime.UtcNow, //payment.TransactionDateUtc,
                        Amount = 19.99M, //I don't think you can get this from the receipt (need to check AppleResponse.Receipt string content) and sending it from app is not safe 
                        CurrencyCode = "GBP", //I don't think you can get this from the receipt (need to check AppleResponse.Receipt string content) and sending it from app is not safe 
                        Details = appleResponse.Receipt.ToString(), //json,
                        PaymentNotificationId = appleResponse.PaymentNotificationId,
                        PaymentProviderRef = receipt.TransactionId
                    };
                    await Subscription(receipt.TransactionId, appleResponse.PaymentNotificationId, receipt.Customer, appleResponse.Receipt.ToString(), customerPayment);
                }
                catch
                {
                    appleResponse.Status = AppleStatus.NotAvailable; //Better to report unavailable than success with creating the diamond upgrade
                    throw;
                }
            }
            return appleResponse.Status == AppleStatus.Success;

        }

        #endregion

        #region Android validation
        private static ServiceAccountCredential _credential = new ServiceAccountCredential
        (
            new ServiceAccountCredential.Initializer(GooglePlayAccount)
            {
                Scopes = new[] { AndroidPublisherService.Scope.Androidpublisher }
            }.FromPrivateKey(GooglePlayKey)
        );
        private static AndroidPublisherService _googleService = new AndroidPublisherService
        (
            new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
                ApplicationName = "Azure Function",
            }
        );

        public async Task<GoogleResponse> PostGoogleReceipt(GoogleReceipt receipt)
        {
            try
            {
                //Extract the order details from the PurchaseToken field (this contains the full order details from google not just the purchase token)
                var order = JsonConvert.DeserializeObject<InAppOrder>(receipt.PurchaseToken);

                // 1.  Save the iap to the Db. Every iap received is saved in case needed for audit / debug later
                int iapId = _paymentManager.CreatePaymentNotification((int)Enums.PaymentProvider.GoogleIAP, receipt.PurchaseToken, receipt.Id, receipt.ToString(), DateTime.UtcNow);

                //This is for purchases...
                //var request = _googleService.Purchases.Products.Get(receipt.BundleId, receipt.Id, order.PurchaseToken);
                //This is for subscriptions...
                var request = _googleService.Purchases.Subscriptions.Get(receipt.BundleId, receipt.Id, order.PurchaseToken);
                var purchaseState = await request.ExecuteAsync();

                //Convert the ProductPurchase to a google response...
                GoogleResponse googleResponse = new GoogleResponse()
                {
                    AcknowledgementState= purchaseState.AcknowledgementState,
                    AutoRenewing= purchaseState.AutoRenewing,
                    CancelReason= purchaseState.CancelReason,
                    CountryCode= purchaseState.CountryCode,
                    ExpiryTimeMillis= purchaseState.ExpiryTimeMillis,
                    LinkedPurchaseToken= purchaseState.LinkedPurchaseToken,
                    PaymentState= purchaseState.PaymentState,
                    PriceAmountMicros= purchaseState.PriceAmountMicros,
                    PriceCurrencyCode= purchaseState.PriceCurrencyCode,
                    PromotionCode= purchaseState.PromotionCode,
                    PromotionType= purchaseState.PromotionType,
                    StartTimeMillis= purchaseState.StartTimeMillis,
                    UserCancellationTimeMillis= purchaseState.UserCancellationTimeMillis,
                    DeveloperPayload= purchaseState.DeveloperPayload,
                    ETag= purchaseState.ETag,
                    Kind= purchaseState.Kind,
                    OrderId= purchaseState.OrderId,
                    //Exclusive reference:
                    PaymentNotificationId = iapId
                };

                return googleResponse;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, $"IAP Procssing failed - Could not validate google receipt {receipt}");

            }
            return null;
        }

        public async Task<bool> ProcessGoogleDiamond(GoogleResponse response, GoogleReceipt receipt)
        {
            //Check state and create diamond as appropriate here
            if (response.PaymentState.HasValue)
            {
                try
                {
                    var customerPayment = new CustomerPayment()
                    {
                        PaymentProviderId = (int)ExclusiveCard.Enums.PaymentProvider.GoogleIAP, //payment.PaymentProvider,
                        PaymentDate = DateTime.UtcNow, //payment.TransactionDateUtc,
                        Amount = 19.99M, //I don't think you can get this from the receipt (need to check Receipt string content) and sending it from app is not safe 
                        CurrencyCode = "GBP", //I don't think you can get this from the receipt (need to check Receipt string content) and sending it from app is not safe 
                        Details = $"{response}", //.Receipt.ToString(), //json,
                        PaymentNotificationId = response.PaymentNotificationId, //IapId
                        PaymentProviderRef = receipt.TransactionId
                    };
                    await Subscription(receipt.TransactionId, (int)customerPayment.PaymentNotificationId, receipt.Customer, customerPayment.Details, customerPayment);
                }
                catch
                {
                    response.PaymentState = -1; //Better to report unavailable than success with creating the diamond upgrade
                    throw;
                }
            }

            return (response.PaymentState.HasValue);

        }
        #endregion

        public async Task Subscription(string uniqueId, int iapId, string customer, string purchase, CustomerPayment customerPayment)
        {
            bool alreadyProcessed = CheckIfAlreadyProcessed(uniqueId);
            // If not already processed , save the customer payment record
            if (!alreadyProcessed)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(customer);
                    if (user != null)
                    {
                        customerPayment.CustomerId = _customerManager.FindCustomerId(user.Id) ?? 0;
                        MembershipCard membershipCard = GetMembershipCard(customerPayment.CustomerId);
                        if (membershipCard != null)
                        {
                            customerPayment.MembershipCardId = membershipCard.Id;
                            _ = _paymentManager.CreateCustomerPayment(customerPayment);
                            ProcessDiamondUpgrade(membershipCard, iapId.ToString(), DateTime.UtcNow, customerPayment.Amount, purchase);
                            DeleteStagingCustomerRegistration(iapId.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log any exception that happens if customer payment record cannot be saved.
                    _logger.Error(ex, "IAP Procssing failed - Could not save the customer Payment record for iap Id " + iapId.ToString());
                    throw;
                }
            }
        }

        public int ProcessInAppPurchase(PurchaseResult purchase)
        {
            int iapId = 0;
            int customerPaymentId = 0;
            MembershipCard membershipCard = null;
            string ipnType = string.Empty;

            //purchase.PurchaseToken = customerProviderRef
            //purchase. = ipnType 

            if (purchase == null)
                return iapId;

            try
            {
                // 1.  Save the iap to the Db. Every iap received is saved in case needed for audit / debug later
                iapId = _paymentManager.CreatePaymentNotification(purchase.PaymentProvider, purchase.PurchaseToken, purchase.ItemType, purchase.ToString(), purchase.TransactionDateUtc);

                // 2.  Check whether this IPN (or duplicate of it) has been processed before and is a recongnised IPN type
                if (purchase != null && !string.IsNullOrWhiteSpace(purchase.PurchaseToken) && purchase.CustomerId > 0)
                {
                    bool alreadyProcessed = CheckIfAlreadyProcessed(purchase.PurchaseToken);

                    // If not already processed , save the customer payment record
                    if (!alreadyProcessed)
                    {
                        try
                        {
                            membershipCard = GetMembershipCard(purchase.CustomerId);
                            if (membershipCard == null)
                                return iapId;

                            customerPaymentId = SaveCustomerPayment(iapId, membershipCard.Id, purchase);
                        }
                        catch (Exception ex)
                        {
                            // Log any exception that happens if customer payment record cannot be saved.
                            _logger.Error(ex, "IAP Procssing failed - Could not save the customer Payment record for iap Id " + iapId.ToString());
                        }
                    }

                }
                // 3.  Carry on with rest of IPN processing only if customer payment record was successfully saved above
                //     If payment record already existed / failed for any reason, then rest of process should not continue.
                //     IE,  no payment, no diamond card 
                if (customerPaymentId != 0)
                {
                    switch (purchase.ItemType)
                    {
                        case IAP_SUBSCRIPTION:
                            ProcessDiamondUpgrade(membershipCard, purchase.PurchaseOrderId, purchase.TransactionDateUtc, purchase.PaymentAmount, purchase.ToString());
                            DeleteStagingCustomerRegistration(purchase.PurchaseOrderId);
                            break;

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"IAP processing failed due to error. IAP = {purchase}");
            }

            return iapId;

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

        private void CreatePaymentSubscription(MembershipCard card, string subscriptionId, DateTime paymentDate, decimal paymentAmount, string subscriptionLength, int planId)
        {

            // Insert a new payment subscription record
            var subscription = new PaymentSubscription()
            {
                CustomerId = (int)card.CustomerId,
                SubscriptionId = subscriptionId,
                StatusId = (int)PaypalSubscription.Active, //yes I know, but is it worth creating another enum?
                NextPaymentDate = paymentDate,
                NextPaymentAmount = paymentAmount,
                PaymentType = subscriptionLength,
                MembershipPlanId = planId
            };
            _paymentManager.CreatePaymentSubscription(subscription);
        }

        private void DeleteStagingCustomerRegistration(string subscriptionId)
        {
            _paymentManager.DeleteStagingCustomerRegistration(subscriptionId);
        }

        private int SaveCustomerPayment(int ipnId, int membershipCardId, PurchaseResult payment)
        {
            var customerPayment = new CustomerPayment()
            {
                PaymentProviderId = payment.PaymentProvider,
                CustomerId = payment.CustomerId,
                MembershipCardId = membershipCardId,
                PaymentDate = payment.TransactionDateUtc,
                Amount = payment.PaymentAmount,
                CurrencyCode = payment.Currency,
                Details = payment.ToString(),
                PaymentNotificationId = ipnId,
                PaymentProviderRef = payment.PurchaseToken
            };

            var customerPaymentId = _paymentManager.CreateCustomerPayment(customerPayment);
            return customerPaymentId;
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

        private MembershipCard GetMembershipCard(int? customerId)
        {
            MembershipCard membershipCard = null;
            if (customerId != null)
            {
                membershipCard = _membershipManager.GetActiveMembershipCard((int)customerId);
            }

            return membershipCard;
        }

    }
}
