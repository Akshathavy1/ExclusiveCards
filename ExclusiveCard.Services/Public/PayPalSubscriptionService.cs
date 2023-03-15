using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    public class PayPalSubscriptionService : IPayPalSubscriptionService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IPayPalSubscriptionManager _payPalSubscriptionManager;
        private readonly IPaymentNotificationService _paymentNotificationService;
        private readonly IOLD_MembershipPlanService _membershipPlanService;
        private readonly IStatusServices _statusService;
        private readonly ICustomerPaymentService _customerPaymentService;
        private readonly Managers.IEmailManager _emailManager;
        private readonly Interfaces.Public.IMembershipCardService _cardService;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public PayPalSubscriptionService(IMapper mapper,
            IPayPalSubscriptionManager payPalSubscriptionManager,
            IPaymentNotificationService paymentNotificationService,
            IOLD_MembershipPlanService membershipPlanService,
            IStatusServices statusService,
            ICustomerPaymentService customerPaymentService,
            Managers.IEmailManager emailService,
            Interfaces.Public.IMembershipCardService cardService)
        {
            _mapper = mapper;
            _payPalSubscriptionManager = payPalSubscriptionManager;
            _paymentNotificationService = paymentNotificationService;
            _membershipPlanService = membershipPlanService;
            _statusService = statusService;
            _customerPaymentService = customerPaymentService;
            _emailManager = emailService;
            _cardService = cardService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Manage Notification and Subscription

        public async Task<bool> ManagePayPalSubscriptionNotification(int customerId, int membershipPlanId,
            string customerPaymentProviderId, string adminEmail, int? membershipCardId)
        {
            try
            {
                //Get Membership Plan and Status
                var membershipPlan = await _membershipPlanService.Get(membershipPlanId, false);
                var statuses = await _statusService.GetAll();

                //check if paypal Subscription created. Create if not exists
                dto.PayPalSubscription payPalSubscriptionExists =
                    await GetByCustomerId(customerId);

                //Get SignupSubscription data from payment notification
                dto.PaymentNotification notification =
                    await _paymentNotificationService.GetByCustomerProviderId(
                        customerPaymentProviderId.ToString(), (int)Enums.PaymentProvider.PayPal,
                        Data.Constants.Keys.SubscriptionSignUp);

                if (notification != null && !string.IsNullOrEmpty(notification.FullMessage))
                {
                    //Reduced the date formats by generalizng for PST/PDT as that zonal info is processed before
                    string[] dateFormats =
                    {
                            "HH:mm:ss MMM dd, yyyy zzz", "HH:mm:ss MMM. dd, yyyy zzz",
                            //"HH:mm:ss MMM dd, yyyy PDT", "HH:mm:ss MMM. dd, yyyy PDT",
                            "HH:mm:ss dd MMM yyyy zzz", "HH:mm:ss dd MMM. yyyy zzz",
                            //"HH:mm:ss dd MMM yyyy PDT", "HH:mm:ss dd MMM. yyyy PDT"
                        };

                    //Parse IPN into dict
                    IDictionary postedData = ParsePaypalIPNByString(notification.FullMessage);
                    //Get next Payment date
                    DateTime paymentDate = DateTime.UtcNow;
                    var decodedSubscriptionDate =
                        System.Web.HttpUtility.UrlDecode(postedData["subscr_date"].ToString());
                    DateTime.TryParseExact(
                        decodedSubscriptionDate.Replace("PST", "-08:00").Replace("PDT", "-07:00"), dateFormats,
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out paymentDate);
                    paymentDate = paymentDate.ToUniversalTime();
                    if (postedData["period3"].ToString() == "1+Y")
                    {
                        paymentDate = paymentDate.AddYears(1);
                    }

                    //Get Membership plan type
                    string paymentType = "Yearly";
                    switch (membershipPlan.Duration)
                    {
                        case 365:
                            paymentType = "Yearly";
                            break;
                        case 30:
                            paymentType = "Monthly";
                            break;
                        default:
                            paymentType = "Yearly";
                            break;
                    }

                    if (payPalSubscriptionExists == null)
                    {
                        //TODO: there are few null check in PayPalStatusId and which may be an issue if we parse to int and the value is null
                        //Create PayPal Subscription
                        dto.PayPalSubscription subscription = new dto.PayPalSubscription
                        {
                            CustomerId = customerId,
                            PayPalId = postedData["subscr_id"].ToString(),
                            PayPalStatusId = (int)statuses?.FirstOrDefault(x =>
                               x.IsActive && x.Type == Data.Constants.StatusType.PaypalSubscription &&
                               x.Name == Data.Constants.Status.Active)?.Id,
                            NextPaymentDate = paymentDate,
                            NextPaymentAmount = Convert.ToDecimal(postedData["mc_amount3"].ToString()),
                            PaymentType = paymentType,
                            MembershipPlanId = membershipPlan?.Id
                        };

                        payPalSubscriptionExists = await Add(subscription);
                        if (payPalSubscriptionExists == null)
                        {
                            dto.Email email = new dto.Email
                            {
                                Subject = $"PayPal subscription creation failed during creating customer",
                                BodyHtml =
                                    $"Dear Admin,<br/><p><Could not create PayPal Subscription for customer Id {customerId} during creation of membership card.></p>",
                                BodyPlainText =
                                    $"Dear Admin, Could not create PayPal Subscription for customer Id {customerId} during creation of membership card.",
                                EmailTo = new List<string> { adminEmail }
                            };
                            var res = await _emailManager.SendEmailAsync(email);
                            if (res != true.ToString())
                            {
                                _logger.Error(
                                    $"Could not create PayPal Subscription for customer Id {customerId} during creation of membership card.");
                            }
                        }
                    }

                    //Subscriber Payment check for txn Id and update Customer Payment
                    notification = await _paymentNotificationService.GetByCustomerProviderId(
                        customerPaymentProviderId.ToString(), (int)Enums.PaymentProvider.PayPal,
                        Data.Constants.Keys.SubscriptionPayment);
                    if (notification != null)
                    {
                        postedData = ParsePaypalIPNByString(notification.FullMessage);
                    }

                    //Check if customerId and MembershipCard Id updated in Customer Payment
                    if (postedData.Contains("txn_id"))
                    {
                        dto.CustomerPayment customerPayment =
                            await _customerPaymentService.GetByPaymentProviderRef(
                                postedData["txn_id"].ToString());

                        //Get MembershipCard
                        var card = await _cardService.GetByCustomerPlan(customerId, membershipPlanId);

                        if (customerPayment != null && !customerPayment.CustomerId.HasValue)
                        {
                            dto.CustomerPayment payment = new dto.CustomerPayment
                            {
                                Id = customerPayment.Id,
                                CustomerId = customerId,
                                MembershipCardId = membershipCardId,
                                PaymentProviderId = customerPayment.PaymentProviderId,
                                PaymentDate = customerPayment.PaymentDate,
                                Amount = customerPayment.Amount,
                                CurrencyCode = customerPayment.CurrencyCode,
                                Details = customerPayment.Details,
                                CashbackTransactionId = customerPayment.CashbackTransactionId,
                                PaymentNotificationId = customerPayment.PaymentNotificationId,
                                PaymentProviderRef = customerPayment.PaymentProviderRef
                            };
                            await _customerPaymentService.Update(payment);
                        }

                        int pending = statuses.FirstOrDefault(x =>
                            x.IsActive && x.Name == Data.Constants.Status.Pending &&
                            x.Type == Data.Constants.StatusType.MembershipCard).Id;
                        int active = statuses.FirstOrDefault(x =>
                            x.IsActive && x.Name == Data.Constants.Status.Active &&
                            x.Type == Data.Constants.StatusType.MembershipCard).Id;

                        if (card.Id == membershipCardId && card.StatusId == pending)
                        {
                            card.StatusId = active;
                            await _cardService.Update(card);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        #endregion


        #region Writes

        //Add PayPalSubscription
        public async Task<dto.PayPalSubscription> Add(dto.PayPalSubscription payPalSubscription)
        {
            PayPalSubscription req = _mapper.Map<PayPalSubscription>(payPalSubscription);
            return MapToDTO(await _payPalSubscriptionManager.Add(req));
        }

        //Update Paypal subscription
        public async Task<dto.PayPalSubscription> Update(dto.PayPalSubscription payPalSubscription)
        {
            PayPalSubscription req = _mapper.Map<PayPalSubscription>(payPalSubscription);
            return MapToDTO(await _payPalSubscriptionManager.Update(req));
        }

        //Delete Paypal subscription
        public async Task DeleteAsync(dto.PayPalSubscription payPalSubscription)
        {
            PayPalSubscription req = _mapper.Map<PayPalSubscription>(payPalSubscription);
            await _payPalSubscriptionManager.Update(req);
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.PayPalSubscription> GetByPayPalId(string paypalId)
        {
            return MapToDTO(await _payPalSubscriptionManager.GetByPayPalId(paypalId));
        }

        public async Task<Models.DTOs.PayPalSubscription> GetByCustomerId(int customerId)
        {
            return MapToDTO(await _payPalSubscriptionManager.GetByCustomerId(customerId));
        }

        #endregion

        #region Private Members

        private dto.PayPalSubscription MapToDTO(PayPalSubscription payPal)
        {
            if (payPal == null)
                return null;
            dto.PayPalSubscription payPalSubscription = new dto.PayPalSubscription
            {
                CustomerId = payPal.CustomerId,
                PayPalId = payPal.PayPalId,
                PayPalStatusId = payPal.PayPalStatusId,
                NextPaymentDate = payPal.NextPaymentDate,
                NextPaymentAmount = payPal.NextPaymentAmount,
                PaymentType = payPal.PaymentType,
                MembershipPlanId = payPal.MembershipPlanId
            };
            if (payPal.Id > 0)
            {
                payPalSubscription.Id = payPal.Id;
            }

            return payPalSubscription;
        }

        private Dictionary<string, string> ParsePaypalIPNByString(string postedRaw)
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

        #endregion
    }
}
