using System;
using System.Collections.Generic;
using System.Text;
using ExclusiveCard.Data.Repositories;
using db = ExclusiveCard.Data.Models;
using sta = ExclusiveCard.Data.StagingModels;

using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using NUnit.Framework;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Models.DTOs;
using System.Linq;
using NUnit.Framework.Internal;
//using ExclusiveCard.IntegrationTests.Common;

namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    class PayPalServiceTests
    {
        private const string EMAIL_ADDRESS = "test@ijustwantanapp.com";
        private const string CURRENCY_CODE = "GBP";
        private const string ACCOUNT_BOOST_SUMMARY = "Account Boost Deposit";
        private const string ACCOUNT_BOOST_DETAILS = "Account+Boost";
        private const string UPGRADE_DETAILS = "Diamond+Membership";
        private const string RENEWAL_DETAILS = "Diamond+Membership+Renewal";
        private const decimal ACCOUNT_BOOST_AMOUNT = 11;
        private const int PAYPAL_ID = 2;


        private const string IPN_RENEWAL = "recurring_payment";
        private const string IPN_UPGRADE = "subscr_payment";
        private const string IPN_BOOST = "web_accept";

        private IPayPalService _paypalService;
        private ServiceTestsHelper _helper;
        private IRepository<sta.CustomerRegistration> _stagingCustRegRepo;
        private IRepository<db.PaymentNotification> _paymentNotificationRepo;
        private IRepository<db.CustomerPayment> _customerPaymentRepo;
        private IRepository<db.CashbackTransaction> _cashbackTransRepo;
        private IRepository<db.CashbackSummary> _cashbackSummaryRepo;
        private IRepository<db.MembershipCard> _membershipCardRepo;
        private IRepository<db.MembershipPlan> _membershipPlanRepo;
        private IRepository<db.PayPalSubscription> _paymentSubscriptionRepo;
        

        [OneTimeSetUp]
        public void Setup()
        {
            _paypalService = Configuration.ServiceProvider.GetService<IPayPalService>();
            _stagingCustRegRepo = Configuration.ServiceProvider.GetService<IRepository<sta.CustomerRegistration>>();
            _paymentNotificationRepo = Configuration.ServiceProvider.GetService<IRepository<db.PaymentNotification>>();
            _customerPaymentRepo = Configuration.ServiceProvider.GetService<IRepository<db.CustomerPayment>>();
            _cashbackTransRepo = Configuration.ServiceProvider.GetService<IRepository<db.CashbackTransaction>>();
            _cashbackSummaryRepo = Configuration.ServiceProvider.GetService<IRepository<db.CashbackSummary>>();
            _membershipCardRepo = Configuration.ServiceProvider.GetService<IRepository<db.MembershipCard>>();
            _membershipPlanRepo = Configuration.ServiceProvider.GetService <IRepository<db.MembershipPlan>>();
            _paymentSubscriptionRepo = Configuration.ServiceProvider.GetService<IRepository<db.PayPalSubscription>>();

            _helper = new ServiceTestsHelper();
            
        }


        #region Account Boost

        [Test]
        public void AccountBoostIPN_Valid()
        {
            
            Test_AccountBoostIPN_Valid(out string ipn, out int ipnId);
        }

        [Test]
        public void AccountBoostIPN_Duplicate()
        {
             
            // Process an IPN as normal
            Test_AccountBoostIPN_Valid(out string ipn, out int ipnId1);
            Assert.IsTrue(ipnId1 > 0, "First IPN not saved");

            // Now lets try and run the same IPN message in again
            // Call the paypal service, let see if this baby works
            var ipnId2 = _paypalService.ProcessIPN(ipn, EMAIL_ADDRESS).Result;

            // Should still save the IPN to the Db
            Assert.IsTrue(ipnId2 > 0, "Second IPN not saved");

            // But should be different record to the 1st
            Assert.AreNotEqual(ipnId1, ipnId2, "Same to saem IPN record");

            // First IPN should have saved customer record
            var custPayment1 = _customerPaymentRepo.Get(x => x.PaymentNotificationId == ipnId1);
            Assert.IsNotNull(custPayment1, "First Customer payment not found.");

            // Validate customerPayment record not created
            var custPayment2 = _customerPaymentRepo.Get(x => x.PaymentNotificationId == ipnId2);
            Assert.IsNull(custPayment2, "Customer payment found for duplicate Ipn. Oh dear.");
        }



        [Test]
        public void AccountBoostIPN_MissingCustRegistration()
        {
            var testDate = DateTime.UtcNow;

            // Create a new customer in the Db
            var code = _helper.CreateRegistrationCodeAndMembershipPlan();
            var cust = _helper.CreateNewCustomerAndMembership(code.RegistartionCode);
            var card = _helper.GetActiveMembershipCard(cust.Id);

            Guid customerProviderRef = Guid.NewGuid();

            // generate unique payment provider ref based on current time and date, in a PayPal format
            string time = DateTime.UtcNow.ToString("hhmmmss");
            string dateish = DateTime.UtcNow.ToString("msddmm");
            string paymentProviderRef = "1AB" + time + "C" + dateish + "D";

            // Generate our fake IPN
            var paymentDate = testDate.AddHours(-8); // take 8 hours off to allow for PST and a lag between payment entered and IPN arriving
            string ipn = GetAcountBoostIPN(customerProviderRef.ToString(), paymentProviderRef, paymentDate, details: ACCOUNT_BOOST_DETAILS);

            try
            {
                // Call the paypal service, 
                _paypalService.ProcessIPN(ipn, EMAIL_ADDRESS);
                
            }
            catch (Exception) 
            {
                Assert.Fail("IPN should not throw exceptions even if staging.CustomerRegistration record is missing");
            }

            // 1. Check IPN saved to Exclusive.PaymentNotification.  This should still have saved
            ValidateIPN(testDate, customerProviderRef, ipn, "web_accept");

            // 2. Check customer payment - should still existin but cust amd membership card will be null.
            //    This is required behaviour, as the PaymentProviderRef will still link back to the IPN. Shows a valid payment was received but needs matching to someone.
            ValidateCustomerPayment(cust, card, paymentProviderRef, ACCOUNT_BOOST_DETAILS, ACCOUNT_BOOST_AMOUNT, true, true);

            var txn = _cashbackTransRepo.Get(x => x.MembershipCardId == card.Id && x.AccountType == 'R' && x.Summary == ACCOUNT_BOOST_SUMMARY);
            Assert.IsNull(txn, "No cashback txn should be created if staging.CustomerRegistration record is missing");
            var txn2 = _cashbackTransRepo.Get(x => x.MembershipCardId == card.Id && x.AccountType == 'D' && x.Summary == ACCOUNT_BOOST_SUMMARY);
            Assert.IsNull(txn2, "No cashback txn should be created if staging.CustomerRegistration record is missing");

            var summary = _cashbackSummaryRepo.Get(x => x.MembershipCardId == card.Id && x.AccountType == 'R');
            Assert.IsNotNull(summary);
            Assert.AreEqual(0, summary.PaidAmount, "Customer R Paid Balance should still be 0");

            var summary2 = _cashbackSummaryRepo.Get(x => x.MembershipCardId == card.Id && x.AccountType == 'D');
            Assert.IsNotNull(summary2);
            Assert.AreEqual(0, summary2.PaidAmount, "Customer D Paid Balance should still be 0");

        }

        #endregion

        #region Upgrades

        [Test]
        public void DiamondUpgradeIPN_Valid()
        {
            Test_Upgrade_Valid(out DateTime testDate, out string subscriptionId, out db.MembershipPlan diamondPlan, out Customer cust, out MembershipCard card, out db.MembershipRegistrationCode code, out db.MembershipCard diamondCard);
        }

        #endregion

        #region Renewals

        [Test]
        public void RenewalIPN_Valid()
        {
            // 1)  For first part of test, we create a membership and then process an upgrade IPN to get a diamond membership setup
            //     This is exactly the same as for the upgrade code. If that test is failing, so will this!
            Test_Upgrade_Valid(out DateTime testDate, out string subscriptionId, out db.MembershipPlan diamondPlan, out Customer cust, out MembershipCard card, out db.MembershipRegistrationCode code, out db.MembershipCard diamondCard);

            // Set the membership card to expired.
            var dbCard = _membershipCardRepo.GetById(card.Id);
            dbCard.StatusId = (int)MembershipCardStatus.Expired;
            dbCard.ValidTo = DateTime.UtcNow.AddDays(-1);
            _membershipCardRepo.Update(dbCard);
            _membershipCardRepo.SaveChanges();

            // 2.  NOW TIME FOR A RENEWAL TEST
            string renewalPaymentProviderRef = CreatePaymentProviderRef();
            var renewalPayDate = testDate.AddHours(-2); // Just make this different to original
            decimal priceRise = 11.22M;

            string renewalIPN = GetRenewalIPN(subscriptionId, renewalPaymentProviderRef, renewalPayDate, null, diamondPlan.CustomerCardPrice + priceRise, details: RENEWAL_DETAILS);
            // 3 Call the paypal service, let see if this baby works
            int renewalIpnId = _paypalService.ProcessIPN(renewalIPN, EMAIL_ADDRESS).Result;

            // 4. Check IPN saved to Exclusive.PaymentNotification
            ValidateIPN(testDate, Guid.Empty, renewalIPN, IPN_RENEWAL);

            // 5. Check customer payment
            var dtoDiamondCard = _helper.GetActiveMembershipCard((int)card.CustomerId);
            ValidateCustomerPayment(cust, dtoDiamondCard, renewalPaymentProviderRef, RENEWAL_DETAILS, diamondPlan.CustomerCardPrice + priceRise);

            // 6. Validate the paypal subscription
            ValidatePayPalSubscription((int)card.CustomerId, subscriptionId, testDate, diamondPlan.CustomerCardPrice + priceRise, diamondPlan.Id);

            // 7. Validate the renewed membership Card
            DateTime validTo = testDate.AddDays(365);  // Renewals currently hard coded to a year.  This is becuase PayPal only has a single subscription product defined, with duration 1 year. Will need updating if business change to more flexible subscrtipon options in the future. 
            ValidateMembershipCard(diamondCard, (int)MembershipCardStatus.Active, (int)card.CustomerId, diamondPlan.Id, code.Id, testDate, validTo, "EX", true, true);


        }


        #endregion

        #region General IPNs

        [Test]
        public void IgnoredIPN_Valid()
        {
            string ipn = GetIgnoredIPN();

             // Call the paypal service
            var ipnId = _paypalService.ProcessIPN(ipn, EMAIL_ADDRESS).Result;

            // Validate it was saved, even though nothing else will have happened
            Assert.IsTrue(ipnId > 0, "Payment Notification Id not set");
            var ipnEntity = _paymentNotificationRepo.GetById(ipnId);
            Assert.IsNotNull(ipnEntity, "Payment notifcation not found in Db");
            Assert.AreEqual(ipn, ipnEntity.FullMessage, "Ipn changed when saved to db");

            // Validate customerPayment record not created
            var custPayment = _customerPaymentRepo.Get(x => x.PaymentNotificationId == ipnId);
            Assert.IsNull(custPayment, "Customer payment found for IPN that should have been ignored.");
        }


        #endregion



        #region private methods

        #region Do some testing

        // main account boost testing
        private void Test_AccountBoostIPN_Valid(out string ipn, out int ipnId )
        {
            var code = _helper.CreateRegistrationCodeAndMembershipPlan();
            var testDate = DateTime.UtcNow;
            // Create a new customer in the Db
            var cust = _helper.CreateNewCustomerAndMembership(code.RegistartionCode);
            var card = _helper.GetActiveMembershipCard(cust.Id);

            // Add the staging.CustomerRegistration record (as happens before user is transerred to paypal )
            Guid customerProviderRef = CreateStagingCustomerRegistration(cust.AspNetUserId, code);
           
            

            string paymentProviderRef = CreatePaymentProviderRef();


            // Generate our fake IPN
            var paymentDate = testDate.AddHours(-8); // take 8 hours off to allow for PST and a lag between payment entered and IPN arriving
            ipn = GetAcountBoostIPN(customerProviderRef.ToString(), paymentProviderRef, paymentDate, details:ACCOUNT_BOOST_DETAILS);

            // Call the paypal service, let see if this baby works
            ipnId = _paypalService.ProcessIPN(ipn, EMAIL_ADDRESS).Result;

            // Now to check it all was ok,

            // 1. Check IPN saved to Exclusive.PaymentNotification
            ValidateIPN(testDate, customerProviderRef, ipn, IPN_BOOST);

            // 2. Check customer payment
            ValidateCustomerPayment(cust, card, paymentProviderRef, ACCOUNT_BOOST_DETAILS, ACCOUNT_BOOST_AMOUNT);

            // 3. Validate Cashback transaction exists
            decimal cashbackAmount = ACCOUNT_BOOST_AMOUNT - card.MembershipPlan.PaymentFee;
            ValidateAccountBoostCashbackTransaction(card.Id, 'R', ACCOUNT_BOOST_AMOUNT, cashbackAmount, testDate, testDate);
            ValidateAccountBoostCashbackTransaction(card.Id, 'D', ACCOUNT_BOOST_AMOUNT, card.MembershipPlan.PaymentFee, testDate, testDate);

            // 4. Validate Cashback Summaries
            ValidateCashbackSummary(card.Id, 'R', cashbackAmount);
            ValidateCashbackSummary(card.Id, 'D', card.MembershipPlan.PaymentFee);
        }


        private void Test_Upgrade_Valid(out DateTime testDate, out string subscriptionId, out db.MembershipPlan diamondPlan, out Customer cust, out MembershipCard card, out db.MembershipRegistrationCode code, out db.MembershipCard diamondCard, int planTypeId = 4)
        {
            testDate = DateTime.UtcNow;
            // Create a new customer in the Db
            code = _helper.CreateRegistrationCodeAndMembershipPlan(planTypeId: planTypeId);
            cust = _helper.CreateNewCustomerAndMembership(code.RegistartionCode);
            card = _helper.GetActiveMembershipCard(cust.Id);

            // Add the staging.CustomerRegistration record (as happens before user is transerred to paypal )
            Guid customerProviderRef = CreateStagingCustomerRegistration(cust.AspNetUserId, code);

            string paymentProviderRef = CreatePaymentProviderRef();
            subscriptionId = CreateSubscriptionId();

            // find the plans
            var planId = code.MembershipPlanId;
            var standardPlan = _membershipPlanRepo.Get(x => x.Id == planId);
            Assert.IsNotNull(standardPlan, "standard plan not found");
            diamondPlan = _membershipPlanRepo.Include(y => y.MembershipLevel).FirstOrDefault(x => x.CardProviderId == standardPlan.CardProviderId && x.MembershipLevel.Level == 10);
            Assert.IsNotNull(diamondPlan, "diamond plan not found");


            // Generate our fake IPN
            var paymentDate = testDate.AddHours(-8); // take 8 hours off to allow for PST and a lag between payment entered and IPN arriving
            string ipn = GetUpgradeIPN(subscriptionId, customerProviderRef.ToString(), paymentProviderRef, paymentDate, diamondPlan.CustomerCardPrice, details: UPGRADE_DETAILS);

            // Call the paypal service, let see if this baby works
            int ipnId = _paypalService.ProcessIPN(ipn, EMAIL_ADDRESS).Result;

            // 1. Check IPN saved to Exclusive.PaymentNotification
            ValidateIPN(testDate, customerProviderRef, ipn, IPN_UPGRADE);


            // 2. Check customer payment
            ValidateCustomerPayment(cust, card, paymentProviderRef, UPGRADE_DETAILS, diamondPlan.CustomerCardPrice);

            // 3. Check for valid diamond membership card
            var validTo = testDate.AddDays(diamondPlan.Duration);
            var customerId = card.CustomerId;
            var diamondPlanId = diamondPlan.Id;
            diamondCard = _membershipCardRepo.Get(x => x.CustomerId == customerId && x.MembershipPlanId == diamondPlanId);
            ValidateMembershipCard(diamondCard, (int)MembershipCardStatus.Active, (int)card.CustomerId, diamondPlan.Id, code.Id, testDate, validTo, "EX", true, true);

            // Check the card has cashback summary records created
            ValidateCashbackSummary(diamondCard.Id, 'R', 0);
            ValidateCashbackSummary(diamondCard.Id, 'D', 0);

            // Check the paypal subscription record was created
            ValidatePayPalSubscription((int)diamondCard.CustomerId, subscriptionId, testDate, diamondPlan.CustomerCardPrice, diamondPlan.Id);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validation for cashback txnx
        /// </summary>
        /// <param name="membershipCardId"></param>
        /// <param name="accountType"></param>
        /// <param name="purchaseAmount"></param>
        /// <param name="cashbackAmount"></param>
        /// <param name="paymentDate"></param>
        /// <param name="testDate"></param>        
        private void ValidateAccountBoostCashbackTransaction(int? membershipCardId, char accountType, decimal purchaseAmount, decimal cashbackAmount, DateTime paymentDate, DateTime testDate)
        {
            var txn = _cashbackTransRepo.Get(x => x.MembershipCardId == membershipCardId && x.AccountType == accountType && x.Summary == ACCOUNT_BOOST_SUMMARY);
            Assert.IsNotNull(txn, "Cashback Transaction not found for account type  " + accountType);
            Assert.AreEqual(txn.PurchaseAmount, purchaseAmount, "cashback transaction has incorrect purchase amount");
            Assert.AreEqual(txn.CashbackAmount, cashbackAmount, "cashback transaction has incorrect cashback amount");
            Assert.AreEqual(txn.CurrencyCode, CURRENCY_CODE, "currecncy code incorrect");
            Assert.AreEqual(txn.StatusId, (int)Cashback.UserPaid, "StatusId incorrect");
            Assert.IsTrue(paymentDate.AddSeconds(-1) <= txn.TransactionDate && paymentDate.AddSeconds(1) > txn.TransactionDate, "Transaction date does not match");  // transaction date should be within 1 second of actual - covers loss of accuracy when time saved only to second 
            Assert.IsTrue(testDate.AddSeconds(-1) <= txn.DateReceived && testDate.AddMinutes(1) > txn.DateReceived, "Date Received is not correct"); // Received data allowable within a minute - in case tests are slow to run!
            Assert.IsNull(txn.FileId, "FileId should not be set when txn initially created");
            Assert.IsNull(txn.PaymentStatusId, "Payment Status should not be set when txn initally created");
            Assert.IsNull(txn.MerchantId, "Account boost should not be linked to any merchant");
        }

        /// <summary>
        ///  Validation for Cashback Summary records
        /// </summary>
        /// <param name="membershipCardId"></param>
        /// <param name="accountType"></param>
        /// <param name="cashbackAmount"></param>
        private void ValidateCashbackSummary(int? membershipCardId, char accountType, decimal cashbackAmount = 0)
        {
            var summary = _cashbackSummaryRepo.Get(x => x.MembershipCardId == membershipCardId && x.AccountType == accountType);
            Assert.IsNotNull(summary, "Cashback summary not found for account type " + accountType);
            Assert.AreEqual(summary.PaidAmount, cashbackAmount, "Paid summary incorrect");
            Assert.AreEqual(summary.PendingAmount, 0, "Pending summary incorrect");
            Assert.AreEqual(summary.ReceivedAmount, 0, "Received summary incorrect");
            Assert.AreEqual(summary.ConfirmedAmount, 0, "Confirmed summary incorrect");
        }

        /// <summary>
        /// Validation for saving IPNs to PaymentNotification table
        /// </summary>
        /// <param name="testDate"></param>
        /// <param name="customerProviderRef"></param>
        /// <param name="ipn"></param>
        /// <param name="txnType"></param>
        private void ValidateIPN(DateTime testDate, Guid customerProviderRef, string ipn, string txnType)
        {
            db.PaymentNotification notification = null;
            if (customerProviderRef == Guid.Empty)
                notification = _paymentNotificationRepo.Get(x => x.FullMessage == ipn);
            else
                notification = _paymentNotificationRepo.Get(x => x.CustomerPaymentProviderId == customerProviderRef.ToString());

            Assert.IsNotNull(notification, "No IPN found in Exclusive.PaymentNotification table");
            Assert.AreEqual(notification.TransactionType, txnType, "IPN Transaction type not saved correctly");
            Assert.AreEqual(notification.PaymentProviderId, PAYPAL_ID, "IPN payment provider Id not saved correctly");
            Assert.AreEqual(notification.FullMessage, ipn, "IPN full message not saved correctly");
            Assert.IsTrue(notification.DateReceived > testDate, "IPN Date Received not saved correctly");
        }


        /// <summary>
        /// Validation for customer payment record
        /// </summary>
        /// <param name="cust"></param>
        /// <param name="card"></param>
        /// <param name="paymentProviderRef"></param>
        /// <param name="missingCustomer"></param>
        /// <param name="missingMembershipCard"></param>
        private void ValidateCustomerPayment(Customer cust, MembershipCard card, string paymentProviderRef, string paymentDetails, decimal paymentAmount,  bool missingCustomer = false, bool missingMembershipCard = false)
        {
            var customerPayment = _customerPaymentRepo.Get(x => x.PaymentProviderRef == paymentProviderRef);
            Assert.IsNotNull(customerPayment, "Customer Payment not found");
            Assert.AreEqual(customerPayment.Amount, paymentAmount, "customer Payment amount incorrect");
            Assert.AreEqual(customerPayment.CurrencyCode, CURRENCY_CODE, "customer payment currency code incorrect");
            Assert.AreEqual(customerPayment.Details, paymentDetails, "Customer payment details incorrect");
            Assert.AreEqual(customerPayment.PaymentProviderId, PAYPAL_ID, "Customer Payment  paymentproviderId not saved correctly");

            
            if (missingCustomer)
                Assert.IsNull(customerPayment.CustomerId);
            else
                Assert.AreEqual(customerPayment.CustomerId, cust.Id, "Customer payment has incorrect/missing customer Id");
            
            if (missingMembershipCard)
                Assert.IsNull(customerPayment.MembershipCardId);
            else
                Assert.AreEqual(customerPayment.MembershipCardId, card.Id, "Customer payment has incorrect/missing membership Card Id ");
            
        }

        private void ValidateMembershipCard(db.MembershipCard card, int statusId, int customerId, int planId, int registrationCodeId, DateTime startDate, DateTime validTo, string cardNumberPrefix = "EX", bool isPartnerReward = true,  bool isDiamond = false)
        {
            Assert.IsNotNull(card, "Membership card not found");
            Assert.AreEqual((int)MembershipCardStatus.Active, card.StatusId, "Card status not correct");
            Assert.AreEqual(planId, card.MembershipPlanId, "Incorrect plan");
            Assert.IsNotNull(card.CardNumber, "No card number");
            Assert.IsTrue(card.CardNumber.StartsWith(cardNumberPrefix), "Card number prefix wrong");
            Assert.AreEqual(card.ValidFrom.Date,  startDate.Date, "Valid From date incorrect");
            Assert.AreEqual(card.ValidTo.Date , validTo.Date, "Valid To date incorrect");
            Assert.AreEqual(startDate.Date, ((DateTime)card.DateIssued).Date, "Date Issued incorrect");
            Assert.IsTrue(card.IsActive == true, "Card record not active");
            Assert.IsTrue(card.IsDeleted == false, "Card record soft deleted");
            Assert.AreEqual(registrationCodeId, card.RegistrationCode, "Registration code incorrect");
            if (isPartnerReward)
                Assert.IsNotNull(card.PartnerRewardId, "Partner Reward Id null");
            
            //TODO:  Add Ts&Cs check back in before release
            //Assert.IsNotNull(card.TermsConditionsId, "Terms and Conditions Id missing");

            if (isDiamond)
            {
                Assert.IsTrue(card.MembershipPlan.MembershipLevelId == 2, "Not a diamond card");
                var plan = _membershipPlanRepo.GetById(card.MembershipPlanId);
                Assert.IsTrue(plan.MembershipLevelId == 2, "Not a diamond card"); // nasty hard coded check on membership level cause having a membership level table is a pain in the butt
            }

        }

        private void ValidatePayPalSubscription(int customerId, string subscriptionId, DateTime startdate, decimal paymentAmount, int planId, string paymentType = "Yearly")
        {
            var subscription = _paymentSubscriptionRepo.Get(x => x.CustomerId == customerId);
            Assert.IsNotNull(subscription, "paypal subscription not found");
            Assert.AreEqual(subscriptionId, subscription.PayPalId, "Paypal Subsciption Id not correct");
            Assert.AreEqual((int)PaypalSubscription.Active, subscription.PayPalStatusId, "Subscription status not active");
            Assert.IsTrue(subscription.NextPaymentDate < startdate.AddYears(1), "Next payment date less than 1 year from start");
            Assert.AreEqual(paymentAmount, subscription.NextPaymentAmount, "Next payment amount incorrect");
            Assert.AreEqual(paymentType, subscription.PaymentType, "Payment type differs");
            Assert.AreEqual(planId, subscription.MembershipPlanId, "Membership plan incorrect");
        }

        
        #endregion

        #region make test IPN messages


        private string GetAcountBoostIPN(string customerProviderRef, string paymentProviderRef, DateTime? paymentDate = null, decimal paymentAmount= 11.00M, string currencyCode = "GBP", string details = "" )
        {
            DateTime payDate = paymentDate ?? DateTime.UtcNow.AddMinutes(-15);
            string paymentDateString = payDate.ToString("hh:mm:ss+MMM+dd,+yyyy+PST");
            string accountBoostIPN = "mc_gross={0}&protection_eligibility=Ineligible&payer_id=ND6ZF72RVRVT2&payment_date={1}&payment_status=Completed&charset=windows-1252&first_name=Joe&mc_fee=0.57&notify_version=3.9&custom={2}&payer_status=unverified&business=info%40exclusivecard.co.uk&quantity=1&verify_sign=AYpKICe66PtxRdX481S7eVDEQaKLAm7tcDZAXQNhgfpsshkIrNlNqPff&payer_email=joe.bloggs%40test.com&txn_id={3}&payment_type=instant&last_name=Bloggs&receiver_email=info%40exclusivecard.co.uk&payment_fee=&shipping_discount=0.00&receiver_id=DFMVDP42SQLQJ&insurance_amount=0.00&txn_type=web_accept&item_name={5}&discount=0.00&mc_currency={4}&item_number=11003&residence_country=GB&shipping_method=Default&transaction_subject=&payment_gross=&ipn_track_id=6e88fc3eec431";
            accountBoostIPN = string.Format(accountBoostIPN, paymentAmount.ToString(), paymentDateString, customerProviderRef, paymentProviderRef,  currencyCode, details);

            return accountBoostIPN;
        }

        private string GetUpgradeIPN(string subscriptionId, string customerProviderRef, string paymentProviderRef, DateTime? paymentDate = null, decimal paymentAmount = 6.99M, string currencyCode = "GBP", string details = "")
        {
            DateTime payDate = paymentDate ?? DateTime.UtcNow.AddMinutes(-15);
            string paymentDateString = payDate.ToString("hh:mm:ss+MMM+dd,+yyyy+PST");
            string upgradeIPN = "transaction_subject={6}&payment_date={0}&txn_type=subscr_payment&subscr_id={1}&last_name=Bloggs&residence_country=GB&item_name=Diamond+Membership&payment_gross=&mc_currency={2}&business=info%40exclusivecard.co.uk&payment_type=instant&protection_eligibility=Ineligible&verify_sign=Ap01abCisjPO4YYMjrU7IqDSHv5xA-A6dScig49tH9WMWPg13QUxap9p&payer_status=unverified&payer_email=joe.blogs%40test.com&txn_id={3}&receiver_email=info%40exclusivecard.co.uk&first_name=Joe&payer_id=ABB1M6ED8EKEY&receiver_id=BBCVDP42SQLQJ&contact_phone=%2B44+7890123456&item_number=11001&payment_status=Completed&payment_fee=&mc_fee=0.71&btn_id=196410216&mc_gross={4}&custom={5}&charset=windows-1252&notify_version=3.9&ipn_track_id=82e36420c2da4";
            upgradeIPN = string.Format(upgradeIPN, paymentDateString, subscriptionId, currencyCode, paymentProviderRef, paymentAmount.ToString(), customerProviderRef, details);

            return upgradeIPN;
        }

        private string GetRenewalIPN(string subscriptionId, string paymentProviderRef, DateTime? paymentDate = null, DateTime? nextPaymentDate = null ,decimal paymentAmount = 6.99M, string currencyCode = "GBP",  string details = "")
        {
            string frequency = "Yearly";
            string nextPaymentAmount = paymentAmount.ToString();
            string initialPaymentAmount = nextPaymentAmount;
            

            DateTime payDate = paymentDate ?? DateTime.UtcNow.AddMinutes(-15);            
            string paymentDateString = payDate.ToString("hh:mm:ss+MMM+dd,+yyyy+PST");

            DateTime nextPayDate = nextPaymentDate ?? payDate.AddYears(1);
            string nextPaymentDateString = nextPayDate.ToString("hh:mm:ss+MMM+dd,+yyyy+PST");

            string renewalIPN = "mc_gross={0}&period_type=+Regular&outstanding_balance=0.00&next_payment_date={1}&protection_eligibility=Ineligible&payment_cycle={2}&tax=0.00&payer_id=WYGPC4HCGZK92&payment_date={3}&payment_status=Completed&product_name=Exclusive+Card+Membership&charset=windows-1252&recurring_payment_id={4}&first_name=joe&mc_fee=1.22&notify_version=3.9&amount_per_cycle={5}&payer_status=unverified&currency_code={6}&business=info%40exclusivecard.co.uk&verify_sign=CDijhfb44RbCQDvFATBOYZBOVYSLIMWd8FA93o4w4jRhaDu5pq4Mzgqf&payer_email=joe.bloggs%40test.com&initial_payment_amount={7}&profile_status=Active&amount={0}&txn_id={8}&payment_type=instant&last_name=bloggs&receiver_email=info%40exclusivecard.co.uk&payment_fee=&receiver_id=DEFGDP42SQLQJ&txn_type=recurring_payment&mc_currency={6}&residence_country=GB&receipt_id=5116-1234-5678-7360&transaction_subject={9}&payment_gross=&shipping=0.00&product_type=1&time_created=10%3A19%3A11+Mar+21%2C+2017+PDT&ipn_track_id=7bdeca751ea37";
            renewalIPN = string.Format(renewalIPN, paymentAmount.ToString(), nextPaymentDateString, frequency, paymentDateString, subscriptionId, nextPaymentAmount, currencyCode, initialPaymentAmount, paymentProviderRef, details);

            return renewalIPN;
        }

        private string GetIgnoredIPN()
        {
            // return a valid paypal message, but one that we don't bother to process
            string ipn = "txn_type=subscr_signup&subscr_id=I-E0MCJDN8EGPG&last_name=Bloggs&residence_country=GB&mc_currency=GBP&item_name=Diamond+Membership&business=info%40exclusivecard.co.uk&recurring=1&verify_sign=ANHtlbxWT-Xw6qWbUS0HYIlJHH6hASWnxEfhWW35wWU0r2np3gCHRdun&payer_status=unverified&payer_email=joe.blogss.test.com&first_name=joe&receiver_email=info%40exclusivecard.co.uk&payer_id=AMU3M1JN8EKYU&reattempt=1&item_number=11001&subscr_date=07%3A29%3A58+Jan+27%2C+2020+PST&btn_id=196410216&custom=e123c5dd-ddb6-4966-a392-52b821e7db1b&charset=windows-1252&notify_version=3.9&period3=1+Y&mc_amount3=5.99&ipn_track_id=82e37770c2da4";

            return ipn;
        }


        #endregion

        #region create other test data

        private Guid CreateStagingCustomerRegistration(string userId, db.MembershipRegistrationCode code)
        {

            Guid customerProviderRef = Guid.NewGuid();
            string data = "{\"MembershipPlanId\":" + code.MembershipPlanId.ToString();

            var stagingCustReg = new sta.CustomerRegistration()
            {
                CustomerPaymentId = customerProviderRef,
                AspNetUserId = userId,
                Data = data, 
                StatusId = (int)CustomerCreation.New
            };

            _stagingCustRegRepo.Create(stagingCustReg);
            _stagingCustRegRepo.SaveChanges();

            return customerProviderRef;
        }

        /// <summary>
        /// Generate unique payment provider ref based on current time and date, in a PayPal format
        /// </summary>
        /// <returns></returns>
        private string CreatePaymentProviderRef()
        {
            
            string time = DateTime.UtcNow.ToString("hhmmss");
            string ticks = DateTime.UtcNow.ToString("FFFFFF");
            string paymentProviderRef = "1AB" + time + "C" + ticks + "D";

            return paymentProviderRef;
        }

        /// <summary>
        /// Generate a unique fake paypal subscription id based on date & time
        /// </summary>
        /// <returns></returns>
        private string CreateSubscriptionId()
        {
            string time = DateTime.UtcNow.ToString("hhmmss");
            string dateish = DateTime.UtcNow.ToString("MMMdd");
            string subId = "I - " + dateish + "B" + time;

            return subId;
        }

        #endregion

        #endregion
    }
}
