using AutoMapper;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{

    public class MembershipManagerTest
    {
        IMembershipManager _membershipManager;
        IRepository<MembershipPendingToken> _tokenRepo;
        IRepository<MembershipRegistrationCode> _codeRepo;
        IRepository<MembershipPlan> _planRepo;
        IRepository<MembershipCard> _cardRepo;
        IRepository<Customer> _custRepo;
        IRepository<PartnerRewards> _rewardRepo;
        IRepository<Partner> _partnerRepo;
        IRepository<WhiteLabelSettings> _whiteLabelSettingsRepo;
        Partner _partner = null;
        //IMapper _mapper;

        IRepository<CustomerPayment> _customerPaymentRepo;
        IPaymentManager _paymentManager;

        //
        
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
        int ipnId = 0;
        string paymentProviderRef = string.Empty;
        //

        WhiteLabelSettings _whiteLabel = null;

        private const string PAYPAL_PROVIDER = "PayPal";
        string _registrationCode;
        int _paymentProviderId = 0;
        MembershipRegistrationCode _code = new MembershipRegistrationCode();
        MembershipPlan _plan = new MembershipPlan();
        MembershipPlan _bplan = new MembershipPlan();

        string _diamondRegistrationCode;
        MembershipRegistrationCode _diamondCode = new MembershipRegistrationCode();
        MembershipPlan _diamondPlan = new MembershipPlan();

        [SetUp]
        public void Setup()
        {
            _membershipManager = Configuration.ServiceProvider.GetService<IMembershipManager>();
            _tokenRepo = Configuration.ServiceProvider.GetService<IRepository<MembershipPendingToken>>();
            _codeRepo = Configuration.ServiceProvider.GetService<IRepository<MembershipRegistrationCode>>();
            _planRepo = Configuration.ServiceProvider.GetService<IRepository<MembershipPlan>>();
            _cardRepo = Configuration.ServiceProvider.GetService<IRepository<MembershipCard>>();
            _custRepo = Configuration.ServiceProvider.GetService<IRepository<Customer>>();
            _rewardRepo = Configuration.ServiceProvider.GetService<IRepository<PartnerRewards>>();
            _partnerRepo = Configuration.ServiceProvider.GetService<IRepository<Partner>>();
            _whiteLabelSettingsRepo = Configuration.ServiceProvider.GetService<IRepository<WhiteLabelSettings>>();
            _customerPaymentRepo= Configuration.ServiceProvider.GetService<IRepository<CustomerPayment>>();
            _paymentManager = Configuration.ServiceProvider.GetService<IPaymentManager>();
            _whiteLabel = CreateWhiteLabel();

            _partner = CreatePartner();

            _registrationCode = "ABC" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(_registrationCode, _whiteLabel, ref _plan, ref _code);
            CreateStandardPlan(_registrationCode, _whiteLabel, ref _bplan, ref _code,planType:3);

            _diamondRegistrationCode = "DIA" + DateTime.UtcNow.Ticks.ToString();
            CreateDiamondPlan(_diamondRegistrationCode, _whiteLabel, ref _diamondPlan, ref _diamondCode);
        }

        [TearDown]
        public void TearDown()
        {
            if(_plan != null && _code != null)
                DeleteCodeAndPlan(_plan, _code);
            if (_plan != null && _diamondCode != null)
                DeleteCodeAndPlan(_diamondPlan, _diamondCode);
            if(_partner != null)
                DeletePartner(_partner);
            if(_whiteLabel != null)
                DeleteWhiteLabel(_whiteLabel);
        }

        #region private methods

        private void DeleteWhiteLabel(WhiteLabelSettings whiteLabel)
        {
            _whiteLabelSettingsRepo.Delete(whiteLabel);
            _whiteLabelSettingsRepo.SaveChanges();
        }

        private void DeletePlan(MembershipPlan membershipPlan)
        {
            _planRepo.Delete(membershipPlan);
            _planRepo.SaveChanges();
        }

        private void DeletePartner(Partner partner)
        {
            _partnerRepo.Delete(partner);
            _partnerRepo.SaveChanges();
        }

        private void DeletePartnerRewards(PartnerRewards partnerRewards)
        {
            _rewardRepo.Delete(partnerRewards);
            _rewardRepo.SaveChanges();
        }

        private void DeleteCodeAndPlan(MembershipPlan plan, MembershipRegistrationCode code)
        {
            _codeRepo.Delete(code);
            _planRepo.Delete(plan);
            _codeRepo.SaveChanges();
            _planRepo.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationCode"></param>
        /// <param name="plan"></param>
        /// <param name="code"></param>
        /// <param name="numberOfCards"></param>
        /// <param name="numberofPlans"></param>
        /// <param name="planType">3= beneficiary, 4 = partner rewards (standard)</param>
        private void CreateStandardPlan(string registrationCode, WhiteLabelSettings whitelabel, ref MembershipPlan plan, ref MembershipRegistrationCode code, int numberOfCards = 1000, int numberofPlans = 1000, int planType = 4, Partner partner = null)
        {
            if (plan ==null || plan.Id == 0)
            {
                plan = new MembershipPlan()
                {
                    MembershipPlanTypeId = planType,
                    Duration = 365,
                    NumberOfCards = numberofPlans,
                    ValidFrom = DateTime.UtcNow.AddDays(-1),
                    ValidTo = DateTime.UtcNow.AddDays(1),
                    IsActive = true,
                    CardProviderId = (partner == null? _partner.Id: partner.Id),
                    MembershipLevelId = 1,
                    WhitelabelId = whitelabel.Id,
                    Description  = $"TestStandardPlan PlanType{planType} {DateTime.UtcNow.Ticks}"
                };
                _planRepo.Create(plan);
                _planRepo.SaveChanges();
            }
            code = new MembershipRegistrationCode()
            {
                RegistartionCode = registrationCode,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(1),
                NumberOfCards = numberOfCards,
                IsActive = true,
                MembershipPlan = plan
            };
            _codeRepo.Create(code);
            _codeRepo.SaveChanges();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationCode"></param>
        /// <param name="plan"></param>
        /// <param name="code"></param>
        /// <param name="numberOfCards"></param>
        /// <param name="numberofPlans"></param>
        /// <param name="playtype">3= beneficiary, 4 = partner rewards (standard)</param>
        private void CreateDiamondPlan(string registrationCode, WhiteLabelSettings whitelabel, ref MembershipPlan plan, ref MembershipRegistrationCode code, int numberOfCards = 1000, int numberofPlans = 1000, int planType = 4, Partner partner = null)
        {
            if (plan == null || plan.Id == 0)
            {
                plan = new MembershipPlan()
                {
                    MembershipPlanTypeId = planType,
                    Duration = 365,
                    NumberOfCards = numberofPlans,
                    ValidFrom = DateTime.UtcNow.AddDays(-1),
                    ValidTo = DateTime.UtcNow.AddDays(1),
                    PaidByEmployer = true,
                    MembershipLevelId = 2,
                    CardProviderId = (partner == null ? _partner.Id : partner.Id),
                    IsActive = true,
                    WhitelabelId = whitelabel.Id,
                    Description = $"TestDiamondPlan PlanType{planType} {DateTime.UtcNow.Ticks}",
                    CustomerCardPrice =6.99m
                };
                _planRepo.Create(plan);
                _planRepo.SaveChanges();
            }
            code = new MembershipRegistrationCode()
            {
                RegistartionCode = registrationCode,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(1),
                NumberOfCards = numberOfCards,
                IsActive = true,
                MembershipPlan = plan
            };
            _codeRepo.Create(code);
            _codeRepo.SaveChanges();

        }

        private MembershipCard CreateMembershipCard(MembershipPlan plan, MembershipRegistrationCode code, int? customerId = null )
        {

            var card = new MembershipCard()
            {
                MembershipPlanId = plan.Id,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(1),
                StatusId = 1,
                IsActive = true,
                IsDeleted = false,
                PhysicalCardRequested = false,
                RegistrationCode = code.Id
            };
            if (customerId != null)
                card.CustomerId = customerId;

            _cardRepo.Create(card);
            _cardRepo.SaveChanges();

            return card;
        }

        private Customer CreateCustomer()
        {
            try
            {
                var customer = new Customer()
                {
                    Forename = "Another",
                    Surname = "TestingCustomer",
                    IsActive = true,
                    IsDeleted = false,
                    DateAdded = DateTime.UtcNow,
                    MarketingNewsLetter = false,
                    MarketingThirdParty = false
                };

                _custRepo.Create(customer);
                _custRepo.SaveChanges();
                return customer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

        }

        private dto.MembershipPlan MapPlan(MembershipPlan plan)
        {
            var dtoPlan = new dto.MembershipPlan()
            {
                CardProviderId = plan.CardProviderId,
                Duration = plan.Duration,
                Id = plan.Id,
                MembershipLevelId = plan.MembershipLevelId,
                MembershipPlanTypeId = plan.MembershipPlanTypeId,
                PaidByEmployer = plan.PaidByEmployer
            };

            return dtoPlan;
        }

        private WhiteLabelSettings CreateWhiteLabel()
        {
            try
            {
                var whiteLabelSettings = new WhiteLabelSettings()
                {
                    DisplayName = "TestWhiteLabel" + DateTime.UtcNow.Ticks.ToString(),
                    CardName = "TestWhiteLabel" + DateTime.UtcNow.Ticks.ToString(),
                };
                _whiteLabelSettingsRepo.Create(whiteLabelSettings);
                _whiteLabelSettingsRepo.SaveChanges();
                return whiteLabelSettings;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        private Partner CreatePartner()
        {
            // Note - no ASPNETUser getting created with this test partner.  Not needed so far in these tests

            var partner = new Partner()
            {
                Name = "TestPartner" + DateTime.UtcNow.Ticks.ToString(),
                Type = 1,
                IsDeleted = false
            };

            // Create a reward partner
            _partnerRepo.Create(partner);
            _partnerRepo.SaveChanges();

            return partner;

        }
        

        private PartnerRewards CreatePartnerRewards()
        {
            var reward = new PartnerRewards()
            {
                RewardKey = "T" + DateTime.UtcNow.Ticks.ToString(),
                CreatedDate = DateTime.UtcNow,
                LatestValue = 0,
                TotalConfirmedWithdrawn = 0,
                PartnerId = _partner.Id
            };

            _rewardRepo.Create(reward);
            _rewardRepo.SaveChanges();

            return reward;
        }

        

        #endregion

        #region Pending Token Tests

        [Test]
        public void GetMembershipPlanFromPendingToken_TokenFound()
        {
            Assert.IsNotNull(_code, "Unable to find a registration code, validate token");
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate token");

            //insert a token
            Guid? token = Guid.NewGuid();
            var pendingToken = new MembershipPendingToken()
            {
                MembershipRegistrationCodeId = _code.Id,
                Token = (Guid)token,
                DateCreated = DateTime.UtcNow
            };
            _tokenRepo.Create(pendingToken);
            _tokenRepo.SaveChanges();

            var plan = _membershipManager.GetMembershipPlanFromPendingToken(token, out int? registrationCodeId);
            Assert.IsNotNull(plan, "Unable to find plan from pending token");
            Assert.IsTrue(plan.IsActive, "Plan is inactive");
            Assert.IsTrue(plan.ValidFrom < DateTime.UtcNow, "Plan is not valid - not started");
            Assert.IsTrue(plan.ValidTo > DateTime.UtcNow, "Plan is not valid - expired");
            Assert.IsFalse(plan.IsDeleted, "Plan is deleted");
            Assert.IsNotNull(registrationCodeId);
            Assert.AreEqual(pendingToken.MembershipRegistrationCodeId, (int)registrationCodeId);
            // Delete the token once again
            _tokenRepo.Delete(pendingToken);
            _tokenRepo.SaveChanges();

        }


        [Test]
        public void GetMembershipPlanFromPendingToken_TokenNotFound()
        {
            Assert.IsNotNull(_code, "Unable to find a registration code, validate token");
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate token");

            // make up a nonexisting  token
            Guid? token = Guid.NewGuid();

            var plan = _membershipManager.GetMembershipPlanFromPendingToken(token, out int? registrationCodeId);
            Assert.IsNull(plan, "Plan returned unexpectedly, when a non-existant token searched for");

        }

        [Test]
        public void GetMembershipPlanFromPendingToken_CodeNotActive()
        {
            Assert.IsNotNull(_code, "Unable to find a registration code, validate token");
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate token");

            // Make plan inactive 
            bool isActive = _code.IsActive;
            _code.IsActive = false;
            _codeRepo.SaveChanges();

            //insert a token
            Guid? token = Guid.NewGuid();
            var pendingToken = new MembershipPendingToken()
            {
                MembershipRegistrationCodeId = _code.Id,
                Token = (Guid)token
            };
            _tokenRepo.Create(pendingToken);
            _tokenRepo.SaveChanges();

            var plan = _membershipManager.GetMembershipPlanFromPendingToken(token, out int? registrationCodeId);
            Assert.IsNull(plan, "Plan returned unexpectedly, when the code was inactive");

            // Delete the token once again
            _tokenRepo.Delete(pendingToken);
            _tokenRepo.SaveChanges();

            // Set Plan back as it was
            _code.IsActive = isActive;
            _codeRepo.SaveChanges();
        }

        [Test]
        public void GetMembershipPlanFromPendingToken_CodeExpired()
        {
            Assert.IsNotNull(_code, "Unable to find a registration code, validate token");
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate  token");

            // Make plan expired 
            DateTime validTo = _code.ValidTo;
            _code.ValidTo = DateTime.UtcNow.AddDays(-1);
            _codeRepo.SaveChanges();

            //insert a token
            Guid? token = Guid.NewGuid();
            var pendingToken = new MembershipPendingToken()
            {
                MembershipRegistrationCodeId = _code.Id,
                Token = (Guid)token
            };
            _tokenRepo.Create(pendingToken);
            _tokenRepo.SaveChanges();

            var plan = _membershipManager.GetMembershipPlanFromPendingToken(token, out int? registrationCodeId);
            Assert.IsNull(plan, "Plan returned unexpectedly, when the code was expired");

            // Delete the token once again
            _tokenRepo.Delete(pendingToken);
            _tokenRepo.SaveChanges();

            // Set Plan back as it was
            _code.ValidTo = validTo;
            _codeRepo.SaveChanges();
        }

        #endregion

        #region GetMembershipPlan tests

        [Test]
        public void GetOriginalMembershipplan_Test()
        {
            #region setup
            var customer = CreateCustomer();
            Assert.IsNotNull(customer, "No customer");
            Assert.IsTrue(customer.Id > 0, "Customer not saved to db");

            Assert.IsTrue(_plan.CardProviderId > 0, "Card Provider Id = 0");

            var card1 = CreateMembershipCard(_plan, _code, (int?)customer.Id);
            Assert.IsNotNull(card1, "No Card");
            Assert.IsTrue(card1.Id > 0, "Card not saved to db");

            var card2 = CreateMembershipCard(_plan, _code, (int?)customer.Id);
            Assert.IsNotNull(card2, "No Card");
            Assert.IsTrue(card2.Id > 0, "Card not saved to db");
            #endregion

            var membershipCard = _membershipManager.GetOriginalMembershipCard(customer.Id);
            Assert.IsNotNull(membershipCard, "membership card not found");
            Assert.IsTrue(card1.Id == membershipCard.Id, "Original card not returned");

        }

        [Test]
        public void GetDiamondMembershipPlan_ValidationTest()
        {
            #region Set up

            var whiteLabelWithDiamondPlans = CreateWhiteLabel();

            var whiteLabelNoDiamondPlans = CreateWhiteLabel();

            //standard
            MembershipPlan standardPartnerRewardsPlan = null;
            MembershipRegistrationCode standardPartnerRewardsCode = null;
            MembershipPlan standardBenefitRewardsPlan = null;
            MembershipRegistrationCode standardBenefitRewardsCode = null;
            //diamond
            MembershipPlan diamondPartnerRewardsPlan = null;
            MembershipRegistrationCode diamondPartnerRewardsCode = null;
            MembershipPlan diamonBenefitRewardsPlan = null;
            MembershipRegistrationCode diamonBenefitRewardsCode = null;
            //no diamond standards
            MembershipPlan standardPartnerRewardsNoDiamondPlan = null;
            MembershipRegistrationCode standardPartnerRewardsNoDiamondCode = null;
            MembershipPlan standardBenefitRewardsNoDiamondPlan = null;
            MembershipRegistrationCode standardBenefitRewardsNoDiamondCode = null;

            #region Set Up Plans

            string registrationCode = "";
            Partner partnerWithDiamonds = CreatePartner();
            Partner partnerNoDiamonds = CreatePartner();

            #region Set up standard/diamond plans on same partner...
            //standard rewards account
            registrationCode = "SPR" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, whiteLabelWithDiamondPlans, ref standardPartnerRewardsPlan, ref standardPartnerRewardsCode, planType: 4, partner: partnerWithDiamonds);
            //benefit rewards account
            registrationCode = "SBR" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, whiteLabelWithDiamondPlans, ref standardBenefitRewardsPlan, ref standardBenefitRewardsCode, planType: 3, partner: partnerWithDiamonds);

            //diamond standard rewards account
            registrationCode = "DPR" + DateTime.UtcNow.Ticks.ToString();
            CreateDiamondPlan(registrationCode, whiteLabelWithDiamondPlans, ref diamondPartnerRewardsPlan, ref diamondPartnerRewardsCode, planType: 4, partner: partnerWithDiamonds);
            //diamond benefit rewards account
            registrationCode = "DBR" + DateTime.UtcNow.Ticks.ToString();
            CreateDiamondPlan(registrationCode, whiteLabelWithDiamondPlans, ref diamonBenefitRewardsPlan, ref diamonBenefitRewardsCode, planType: 3, partner: partnerWithDiamonds);
            #endregion

            #region Set up standard plans where the partner has no diamond plan
            //Unmatched standard rewards plans
            //standard rewards account
            registrationCode = "SPRND" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, whiteLabelNoDiamondPlans, ref standardPartnerRewardsNoDiamondPlan, ref standardPartnerRewardsNoDiamondCode, planType: 4, partner: partnerNoDiamonds);
            //benefit rewards account
            registrationCode = "SBRND" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, whiteLabelNoDiamondPlans, ref standardBenefitRewardsNoDiamondPlan, ref standardBenefitRewardsNoDiamondCode, planType: 3, partner: partnerNoDiamonds);
            #endregion

            #endregion

            #endregion

            #region Partner with partner reward, beneficiary reward, partner reward diamond, beneficiary reward diamond

            #region Partner Reward, diamond plan test
            // Find associated diamond plan from the standard plan
            var diamondPlanForStandard = _membershipManager.GetDiamondMembershipPlan(standardPartnerRewardsPlan.Id);
            Assert.IsTrue(diamondPlanForStandard.MembershipPlanTypeId == standardPartnerRewardsPlan.MembershipPlanTypeId,"Standard plan, plan types differ");
            Assert.IsTrue(diamondPlanForStandard.CardProviderId == standardPartnerRewardsPlan.CardProviderId, "Standard plan, card providers differ");
            #endregion

            #region Beneficiary Reward, diamond plan test

            //Find associated diamond plan from beneficiary plan
            var diamondPlanForBenefit = _membershipManager.GetDiamondMembershipPlan(standardBenefitRewardsPlan.Id);
            Assert.IsTrue(diamondPlanForBenefit.MembershipPlanTypeId == standardBenefitRewardsPlan.MembershipPlanTypeId, "Beneficiary plan, plan types differ");
            Assert.IsTrue(diamondPlanForBenefit.CardProviderId == standardBenefitRewardsPlan.CardProviderId, "Beneficiary plan, card providers differ");

            #endregion

            #endregion

            #region Partner with partner reward, beneficiary reward but no diamond plans

            #region Partner Reward no diamond test
            // Find default (Exclusive) diamond plan from the standard plan
            var unmatchedDiamondPlanForStandard = _membershipManager.GetDiamondMembershipPlan(standardPartnerRewardsNoDiamondPlan.Id);
            Assert.IsTrue(unmatchedDiamondPlanForStandard.MembershipPlanTypeId == standardPartnerRewardsNoDiamondPlan.MembershipPlanTypeId, "Unmatched Standard plan, plan types differ");
            Assert.IsTrue(unmatchedDiamondPlanForStandard.CardProviderId != standardPartnerRewardsNoDiamondPlan.CardProviderId, "Unmatched Standard, card providers are the same");

            var exclusiveStandardDiamond = _membershipManager.GetDiamondMembershipPlan("Exclusive Media", 1, 4);
            Assert.IsNotNull(exclusiveStandardDiamond, "Exclusive has no Diamond partner reward (standard) plan");
            Assert.IsTrue(exclusiveStandardDiamond.Id == unmatchedDiamondPlanForStandard.Id, "Unmatched partner reward (Standard) Diamond is not from Exclusive");
            #endregion

            #region Benefit Reward, no diamond test

            //Find default (Exclusive) diamond plan from the beneficiary plan
            var unmatchedDiamondPlanForBenefit = _membershipManager.GetDiamondMembershipPlan(standardBenefitRewardsNoDiamondPlan.Id);
            Assert.IsTrue(unmatchedDiamondPlanForBenefit.MembershipPlanTypeId == standardBenefitRewardsNoDiamondPlan.MembershipPlanTypeId, "Unmatched Beneficiary plan, plan types differ");
            Assert.IsTrue(unmatchedDiamondPlanForBenefit.CardProviderId != standardBenefitRewardsNoDiamondPlan.CardProviderId, "Unmatched Beneficiary, card providers are the same");

            var exclusiveBeneficiaryDiamond = _membershipManager.GetDiamondMembershipPlan("Exclusive Media", 1, 3);
            Assert.IsNotNull(exclusiveBeneficiaryDiamond, "Exclusive has no Diamond Beneficiary plan");
            Assert.IsTrue(exclusiveBeneficiaryDiamond.Id == unmatchedDiamondPlanForBenefit.Id, "Unmatched Beneficiary Diamond is not from Exclusive");
            #endregion

            #endregion

            #region Tear down

            DeleteCodeAndPlan(standardPartnerRewardsPlan, standardPartnerRewardsCode);
            DeleteCodeAndPlan(standardBenefitRewardsPlan, standardBenefitRewardsCode);
            DeleteCodeAndPlan(diamondPartnerRewardsPlan, diamondPartnerRewardsCode);
            DeleteCodeAndPlan(diamonBenefitRewardsPlan, diamonBenefitRewardsCode);
            DeleteCodeAndPlan(standardPartnerRewardsNoDiamondPlan, standardPartnerRewardsNoDiamondCode);
            DeleteCodeAndPlan(standardBenefitRewardsNoDiamondPlan, standardBenefitRewardsNoDiamondCode);

            DeletePartner(partnerWithDiamonds);
            DeletePartner(partnerNoDiamonds);

            DeleteWhiteLabel(whiteLabelWithDiamondPlans);
            DeleteWhiteLabel(whiteLabelNoDiamondPlans);

            #endregion

        }

        [Test]
        public void GetMembershipPlanFromRegistrationCode_CodeValid()
        {
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(_code, "Unable to find a registration code, to validate registration code");

            var plan = _membershipManager.GetMembershipPlanFromRegistrationCode(_registrationCode);
            Assert.IsNotNull(plan, "Unable to find plan from registration code");
            Assert.IsTrue(plan.IsActive, "Plan is inactive");
            Assert.IsTrue(plan.ValidFrom < DateTime.UtcNow, "Plan is not valid - not started");
            Assert.IsTrue(plan.ValidTo > DateTime.UtcNow, "Plan is not valid - expired");
            Assert.IsFalse(plan.IsDeleted, "Plan is deleted");
        }

        [Test]
        public void GetMembershipPlanFromRegistrationCode_CodeInvalid()
        {
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(_code, "Unable to find a registration code, to validate registration code");

            var plan = _membershipManager.GetMembershipPlanFromRegistrationCode("FishandChips");
            Assert.IsNull(plan, "Plan returned unexpectedly, when the registration code was invalid");
        }

        [Test]
        public void GetMembershipPlanFromRegistrationCode_PlanNotActive()
        {
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(_code, "Unable to find a registration code, to validate registration code");

            // Make plan inactive 
            bool isActive = _plan.IsActive;
            _plan.IsActive = false;
            _planRepo.SaveChanges();

            //insert a token
            Guid? token = Guid.NewGuid();
            var pendingToken = new MembershipPendingToken()
            {
                MembershipRegistrationCodeId = _code.Id,
                Token = (Guid)token
            };
            _tokenRepo.Create(pendingToken);
            _tokenRepo.SaveChanges();

            var plan = _membershipManager.GetMembershipPlanFromPendingToken(token, out int? registrationCodeId);
            Assert.IsNull(plan, "Plan returned unexpectedly, when the plan was inactive");

            // Delete the token once again
            _tokenRepo.Delete(pendingToken);
            _tokenRepo.SaveChanges();

            // Set Plan back as it was
            _plan.IsActive = isActive;
            _planRepo.SaveChanges();
        }

        [Test]
        public void GetMembershipPlanFromRegistrationCode_PlanExpired()
        {
            Assert.IsNotNull(_plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(_code, "Unable to find a registration code, to validate registration code");

            // Make plan expired 
            DateTime validTo = _plan.ValidTo;
            _plan.ValidTo = DateTime.UtcNow.AddDays(-1);
            _planRepo.SaveChanges();

            //insert a token
            Guid? token = Guid.NewGuid();
            var pendingToken = new MembershipPendingToken()
            {
                MembershipRegistrationCodeId = _code.Id,
                Token = (Guid)token
            };
            _tokenRepo.Create(pendingToken);
            _tokenRepo.SaveChanges();

            var plan = _membershipManager.GetMembershipPlanFromPendingToken(token, out int? registrationCodeId);
            Assert.IsNull(plan, "Plan returned unexpectedly, when the plan was inactive");

            // Delete the token once again
            _tokenRepo.Delete(pendingToken);
            _tokenRepo.SaveChanges();

            // Set Plan back as it was
            _plan.ValidTo = validTo;
            _planRepo.SaveChanges();
        }

        #endregion

        #region CreatePendingMembershipToken tests

        [Test]
        public void CreatePendingMembershipTokenTest_Valid()
        {
            var code = new MembershipRegistrationCode();
            var plan = new MembershipPlan();

            string registrationCode = "DEF" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, _whiteLabel, ref plan, ref code);
            Assert.IsNotNull(plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(code, "Unable to find a registration code, to validate registration code");

            var dtoPlan = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            Assert.IsNotNull(dtoPlan, "Cannot find created plan in the Db");

            var token = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNotNull(token, "The token was not created");
        }

        [Test]
        public void CreatePendingMembershipTokenTest_NoCodes()
        {
            var code = new MembershipRegistrationCode();
            var plan = new MembershipPlan();

            string registrationCode = "DEF" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, _whiteLabel, ref plan, ref code, -1);
            Assert.IsNotNull(plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(code, "Unable to find a registration code, to validate registration code");

            var dtoPlan = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            Assert.IsNotNull(dtoPlan, "Cannot find created plan in the Db");

            var token = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNull(token, "The token was created, even though no codes available");
        }


        [Test]
        public void CreatePendingMembershipTokenTest_NoPlans()
        {
            var code = new MembershipRegistrationCode();
            var plan = new MembershipPlan();

            string registrationCode = "DEF" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, _whiteLabel, ref plan, ref code, 1000, -1);
            Assert.IsNotNull(plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(code, "Unable to find a registration code, to validate registration code");

            var dtoPlan = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            Assert.IsNotNull(dtoPlan, "Cannot find created plan in the Db");

            var token = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNull(token, "The token was created, even though no plans available");
        }

        [Test]
        public void CreatePendingMembershipTokenTest_TooManyPendingCards()
        {
            var code = new MembershipRegistrationCode();
            var plan = new MembershipPlan();

            string registrationCode = "DEF" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, _whiteLabel, ref plan, ref code, 1, 1000);
            Assert.IsNotNull(plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(code, "Unable to find a registration code, to validate registration code");

            var dtoPlan = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            Assert.IsNotNull(dtoPlan, "Cannot find created plan in the Db");

            var token = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNotNull(token, "Unable to create the 1st pending token");

            var token2 = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNull(token2, "2nd pending token created but should only have allowed 1");
        }

        [Test]
        public void CreatePendingMembershipTokenTest_TooManyPendingPlans()
        {
            var code = new MembershipRegistrationCode();
            var plan = new MembershipPlan();

            string registrationCode = "DEF" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, _whiteLabel, ref plan, ref code, 1000, 1);
            Assert.IsNotNull(plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(code, "Unable to find a registration code, to validate registration code");

            var dtoPlan = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            Assert.IsNotNull(dtoPlan, "Cannot find created plan in the Db");

            var token = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNotNull(token, "Unable to create the 1st pending token");

            var token2 = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNull(token2, "2nd pending token created but should only have allowed 1");
        }

        [Test]
        public void CreatePendingMembershipTokenTest_TooManyMembershipPlans()
        {
            var code = new MembershipRegistrationCode();
            var plan = new MembershipPlan();

            string registrationCode = "DEF" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, _whiteLabel, ref plan, ref code, 1000, 1);
            Assert.IsNotNull(plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(code, "Unable to find a registration code, to validate registration code");

            var dtoPlan = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            Assert.IsNotNull(dtoPlan, "Cannot find created plan in the Db");

            var token = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNotNull(token, "Unable to create the 1st pending token");

            var card = CreateMembershipCard(plan, code);
            Assert.IsTrue(card.Id != 0, "Could not create a membership card");

            var token2 = _membershipManager.CreatePendingToken(dtoPlan, registrationCode);
            Assert.IsNull(token2, "2nd pending token created but should only have allowed 1");
        }

        [Test]
        public void CreatePendingMembershipTokenTest_TooManyMembershipCards()
        {
            var code = new MembershipRegistrationCode();
            var plan = new MembershipPlan();

            string registrationCode = "DEF" + DateTime.UtcNow.Ticks.ToString();
            CreateStandardPlan(registrationCode, _whiteLabel, ref plan, ref code, 1, 1000);
            Assert.IsNotNull(plan, "Unable to find a membership plan, to validate registration code");
            Assert.IsNotNull(code, "Unable to find a registration code, to validate registration code");

            //  Create a 2nd code and add a membership to that code first
            var registrationCode2 = "GHI" + DateTime.UtcNow.Ticks.ToString();
            var code2 = new MembershipRegistrationCode();
            CreateStandardPlan(registrationCode2, _whiteLabel, ref plan, ref code2, 1, 1000);
            Assert.IsNotNull(code2, "Unable to find 2nd registration code, to validate registration code");

            var dtoPlan1 = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            Assert.IsNotNull(dtoPlan1, "Cannot find created plan in the Db");
            Assert.IsNotNull(dtoPlan1.MembershipRegistrationCodes, "No codes found in the Db for the plan");

            // create 2nd plan object, for the 2nd code (even though same plan, this keeps our test simpler)
            var dtoPlan2 = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode2);
            Assert.IsNotNull(dtoPlan2, "Cannot find plan in the Db");
            Assert.IsNotNull(dtoPlan2.MembershipRegistrationCodes, "No codes found in the Db for the 2nd code");


            var token = _membershipManager.CreatePendingToken(dtoPlan1, registrationCode);
            Assert.IsNotNull(token, "Unable to create the 1st pending token");

            var card1 = CreateMembershipCard(plan, code2);
            Assert.IsTrue(card1.Id != 0, "Could not create first membership card");

            // This should work, as we can have 1000 cards against the plan but with different registration codes.
            var token2 = _membershipManager.CreatePendingToken(dtoPlan2, registrationCode2);
            Assert.IsNotNull(token2, "2nd pending token could not be created");

            // Create another card, using the original membership code
            var card2 = CreateMembershipCard(plan, code);
            Assert.IsTrue(card2.Id != 0, "Could not create a 2nd membership card");

            // This should now fail, as we are not allowed more than 1 card on the 1st code
            var token3 = _membershipManager.CreatePendingToken(dtoPlan1, registrationCode);
            Assert.IsNull(token3, "2nd pending token created but should only have allowed 1");
        }

        #endregion

        #region CreateMembershipCards tests

        [Test]
        public void CreateMembershipCardsTest_Standard()
        {
            var customer = CreateCustomer();
            Assert.IsNotNull(customer);
            Assert.IsTrue(customer.Id > 0);

            var reward = CreatePartnerRewards();
            Assert.IsNotNull(reward);
            Assert.IsTrue(reward.Id > 0);
            dto.MembershipPlan plan=null;

            plan = MapPlan(_bplan);

            var cards = _membershipManager.CreateMembershipCards(plan, customer.Id, _code.Id, null, reward.Id, "GB");
            Assert.IsNotNull(cards, "Cards not found");
            Assert.IsTrue(cards.Count == 1, "Card count not 1 as expected");
            Assert.IsTrue(cards.First().Id > 0, "Card not saved to Db - id = 0");

        }

        [Test]
        public void CreateMembershipCardsTest()
        {
            try
            {
                #region Set up

                var whiteLabelWithDiamondPlans = CreateWhiteLabel();

                var whiteLabelNoDiamondPlans = CreateWhiteLabel();

                //standard
                MembershipPlan standardPartnerRewardsPlan = null;
                MembershipRegistrationCode standardPartnerRewardsCode = null;
                MembershipPlan standardBenefitRewardsPlan = null;
                MembershipRegistrationCode standardBenefitRewardsCode = null;
                //diamond
                MembershipPlan diamondPartnerRewardsPlan = null;
                MembershipRegistrationCode diamondPartnerRewardsCode = null;
                MembershipPlan diamonBenefitRewardsPlan = null;
                MembershipRegistrationCode diamonBenefitRewardsCode = null;
                //no diamond standards
                MembershipPlan standardPartnerRewardsNoDiamondPlan = null;
                MembershipRegistrationCode standardPartnerRewardsNoDiamondCode = null;
                MembershipPlan standardBenefitRewardsNoDiamondPlan = null;
                MembershipRegistrationCode standardBenefitRewardsNoDiamondCode = null;

                #region Set Up Plans

                string registrationCode = "";
                Partner partnerWithDiamonds = CreatePartner();
                Partner partnerNoDiamonds = CreatePartner();

                #region Set up standard/diamond plans on same partner...
                //standard rewards account
                registrationCode = "SPR" + DateTime.UtcNow.Ticks.ToString();
                CreateStandardPlan(registrationCode, whiteLabelWithDiamondPlans, ref standardPartnerRewardsPlan, ref standardPartnerRewardsCode, planType: 4, partner: partnerWithDiamonds);
                //benefit rewards account
                registrationCode = "SBR" + DateTime.UtcNow.Ticks.ToString();
                CreateStandardPlan(registrationCode, whiteLabelWithDiamondPlans, ref standardBenefitRewardsPlan, ref standardBenefitRewardsCode, planType: 3, partner: partnerWithDiamonds);

                //diamond standard rewards account
                registrationCode = "DPR" + DateTime.UtcNow.Ticks.ToString();
                CreateDiamondPlan(registrationCode, whiteLabelWithDiamondPlans, ref diamondPartnerRewardsPlan, ref diamondPartnerRewardsCode, planType: 4, partner: partnerWithDiamonds);
                //diamond benefit rewards account
                registrationCode = "DBR" + DateTime.UtcNow.Ticks.ToString();
                CreateDiamondPlan(registrationCode, whiteLabelWithDiamondPlans, ref diamonBenefitRewardsPlan, ref diamonBenefitRewardsCode, planType: 3, partner: partnerWithDiamonds);
                #endregion

                #region Set up standard plans where the partner has no diamond plan
                //Unmatched standard rewards plans
                //standard rewards account
                registrationCode = "SPRND" + DateTime.UtcNow.Ticks.ToString();
                CreateStandardPlan(registrationCode, whiteLabelNoDiamondPlans, ref standardPartnerRewardsNoDiamondPlan, ref standardPartnerRewardsNoDiamondCode, planType: 4, partner: partnerNoDiamonds);
                //benefit rewards account
                registrationCode = "SBRND" + DateTime.UtcNow.Ticks.ToString();
                CreateStandardPlan(registrationCode, whiteLabelNoDiamondPlans, ref standardBenefitRewardsNoDiamondPlan, ref standardBenefitRewardsNoDiamondCode, planType: 3, partner: partnerNoDiamonds);
                #endregion

                #endregion

                #endregion

                #region Partner with partner reward, beneficiary reward, partner reward diamond, beneficiary reward diamond

                #region Partner Reward, diamond plan test
                // Find associated diamond plan from the standard plan
                var diamondPlanForStandard = _membershipManager.GetDiamondMembershipPlan(standardPartnerRewardsPlan.Id);
                Assert.IsTrue(diamondPlanForStandard.MembershipPlanTypeId == standardPartnerRewardsPlan.MembershipPlanTypeId, "Standard plan, plan types differ");
                Assert.IsTrue(diamondPlanForStandard.CardProviderId == standardPartnerRewardsPlan.CardProviderId, "Standard plan, card providers differ");
                #endregion

                #region Beneficiary Reward, diamond plan test

                //Find associated diamond plan from beneficiary plan
                var diamondPlanForBenefit = _membershipManager.GetDiamondMembershipPlan(standardBenefitRewardsPlan.Id);
                Assert.IsTrue(diamondPlanForBenefit.MembershipPlanTypeId == standardBenefitRewardsPlan.MembershipPlanTypeId, "Beneficiary plan, plan types differ");
                Assert.IsTrue(diamondPlanForBenefit.CardProviderId == standardBenefitRewardsPlan.CardProviderId, "Beneficiary plan, card providers differ");

                #endregion

                #endregion

                #region Partner with partner reward, beneficiary reward but no diamond plans

                #region Partner Reward no diamond test
                // Find default (Exclusive) diamond plan from the standard plan
                var unmatchedDiamondPlanForStandard = _membershipManager.GetDiamondMembershipPlan(standardPartnerRewardsNoDiamondPlan.Id);
                Assert.IsTrue(unmatchedDiamondPlanForStandard.MembershipPlanTypeId == standardPartnerRewardsNoDiamondPlan.MembershipPlanTypeId, "Unmatched Standard plan, plan types differ");
                Assert.IsTrue(unmatchedDiamondPlanForStandard.CardProviderId != standardPartnerRewardsNoDiamondPlan.CardProviderId, "Unmatched Standard, card providers are the same");

                var exclusiveStandardDiamond = _membershipManager.GetDiamondMembershipPlan("Exclusive Media", 1, 4);
                Assert.IsNotNull(exclusiveStandardDiamond, "Exclusive has no Diamond partner reward (standard) plan");
                Assert.IsTrue(exclusiveStandardDiamond.Id == unmatchedDiamondPlanForStandard.Id, "Unmatched partner reward (Standard) Diamond is not from Exclusive");
                #endregion

                #region Benefit Reward, no diamond test

                //Find default (Exclusive) diamond plan from the beneficiary plan
                var unmatchedDiamondPlanForBenefit = _membershipManager.GetDiamondMembershipPlan(standardBenefitRewardsNoDiamondPlan.Id);
                Assert.IsNotNull(unmatchedDiamondPlanForBenefit, "Exclusive has no Diamond Beneficiary plan");
                Assert.IsTrue(unmatchedDiamondPlanForBenefit.MembershipPlanTypeId == standardBenefitRewardsNoDiamondPlan.MembershipPlanTypeId, "Unmatched Beneficiary plan, plan types differ");
                Assert.IsTrue(unmatchedDiamondPlanForBenefit.CardProviderId != standardBenefitRewardsNoDiamondPlan.CardProviderId, "Unmatched Beneficiary, card providers are the same");

                var exclusiveBeneficiaryDiamond = _membershipManager.GetDiamondMembershipPlan("Exclusive Media", 1, 3);
                Assert.IsNotNull(exclusiveBeneficiaryDiamond, "Exclusive has no Diamond Beneficiary plan");
                Assert.IsTrue(exclusiveBeneficiaryDiamond.Id == unmatchedDiamondPlanForBenefit.Id, "Unmatched Beneficiary Diamond is not from Exclusive");
                #endregion

                #endregion
                //

                #region Create Customer

                var customer1 = CreateCustomer();
                Assert.IsNotNull(customer1);
                Assert.IsTrue(customer1.Id > 0);

                var customer2 = CreateCustomer();
                Assert.IsNotNull(customer2);
                Assert.IsTrue(customer2.Id > 0);

                var customer3 = CreateCustomer();
                Assert.IsNotNull(customer3);
                Assert.IsTrue(customer3.Id > 0);

                var customer4 = CreateCustomer();
                Assert.IsNotNull(customer4);
                Assert.IsTrue(customer4.Id > 0);
                #endregion

                #region Create Partner Rewards
                var reward1 = CreatePartnerRewards();
                Assert.IsNotNull(reward1);
                Assert.IsTrue(reward1.Id > 0);

                var reward2 = CreatePartnerRewards();
                Assert.IsNotNull(reward2);
                Assert.IsTrue(reward2.Id > 0);

                var reward3 = CreatePartnerRewards();
                Assert.IsNotNull(reward3);
                Assert.IsTrue(reward3.Id > 0);

                var reward4 = CreatePartnerRewards();
                Assert.IsNotNull(reward4);
                Assert.IsTrue(reward4.Id > 0);


                dto.MembershipPlan plan1 = null;
                dto.MembershipPlan plan2 = null;
                dto.MembershipPlan plan3 = null;
                dto.MembershipPlan plan4 = null;

                plan1 = MapPlan(standardPartnerRewardsPlan);
                plan2 = MapPlan(standardBenefitRewardsPlan);
                plan3 = MapPlan(standardPartnerRewardsNoDiamondPlan);
                plan4 = MapPlan(standardBenefitRewardsNoDiamondPlan);
                #endregion

                #region Create Create MembershipCards
                //var cards1 = _membershipManager.CreateMembershipCards(plan1, customer1.Id, _code.Id, null, reward1.Id, "GB");
                //var cards2 = _membershipManager.CreateMembershipCards(plan2, customer2.Id, _code.Id, null, reward2.Id, "GB");
                //var cards3 = _membershipManager.CreateMembershipCards(plan3, customer3.Id, _code.Id, null, reward3.Id, "GB");
                var cards4 = _membershipManager.CreateMembershipCards(plan4, customer4.Id, _code.Id, null, reward4.Id, "GB");


                //Assert.IsNotNull(cards1, "Cards not found");
                //Assert.IsTrue(cards1.Count == 1, "Card count not 1 as expected");
                //Assert.IsTrue(cards1.First().Id > 0, "Card not saved to Db - id = 0");

                //Assert.IsNotNull(cards2, "Cards not found");
                //Assert.IsTrue(cards2.Count == 1, "Card count not 1 as expected");
                //Assert.IsTrue(cards2.First().Id > 0, "Card not saved to Db - id = 0");

                //Assert.IsNotNull(cards3, "Cards not found");
                //Assert.IsTrue(cards3.Count == 1, "Card count not 1 as expected");
                //Assert.IsTrue(cards3.First().Id > 0, "Card not saved to Db - id = 0");

                Assert.IsNotNull(cards4, "Cards not found");
                Assert.IsTrue(cards4.Count == 1, "Card count not 1 as expected");
                Assert.IsTrue(cards4.First().Id > 0, "Card not saved to Db - id = 0");
                #endregion


                var dtoCard = new List<dto.MembershipCard>();
                //dtoCard.Add(cards1[0]);
                //dtoCard.Add(cards2[0]);
                //dtoCard.Add(cards3[0]);
                dtoCard.Add(cards4[0]);
                double paymentAmount = 14.99;
                foreach (var item in dtoCard)
                {
                    var payDetails = Guid.NewGuid().ToString();
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(5000);
                        CreateCustomerPayment(item, payDetails);

                    });
                    
                    var diamondCard1 =_membershipManager.UpgradeToDiamond(item, payDetails, (decimal)paymentAmount);

                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(5000);
                        Assert.IsNotNull(diamondCard1, "Cards not found");
                        if (item.MembershipPlan != null)
                        {
                            var diamondrd1 = _membershipManager.GetMembershipPlan(diamondCard1.MembershipPlanId);
                            Assert.IsNotNull(diamondrd1, "Cards not found");
                            Assert.IsTrue(diamondrd1.MembershipLevelId == 2, "Card count not 1 as expected");
                            Assert.IsTrue(diamondrd1.MembershipPlanTypeId == item.MembershipPlan.MembershipPlanTypeId, "Card count not 1 as expected");
                        }
                    });
                    
                }


                #region Tear down

                DeleteCodeAndPlan(standardPartnerRewardsPlan, standardPartnerRewardsCode);
                DeleteCodeAndPlan(standardBenefitRewardsPlan, standardBenefitRewardsCode);
                DeleteCodeAndPlan(diamondPartnerRewardsPlan, diamondPartnerRewardsCode);
                DeleteCodeAndPlan(diamonBenefitRewardsPlan, diamonBenefitRewardsCode);
                DeleteCodeAndPlan(standardPartnerRewardsNoDiamondPlan, standardPartnerRewardsNoDiamondCode);
                DeleteCodeAndPlan(standardBenefitRewardsNoDiamondPlan, standardBenefitRewardsNoDiamondCode);

                DeletePartner(partnerWithDiamonds);
                DeletePartner(partnerNoDiamonds);

                DeleteWhiteLabel(whiteLabelWithDiamondPlans);
                DeleteWhiteLabel(whiteLabelNoDiamondPlans);

                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            //
           

        }

        private string GetAcountBoostIPN(string customerProviderRef, string paymentProviderRef, DateTime? paymentDate = null, decimal paymentAmount = 11.00M, string currencyCode = "GBP", string details = "")
        {
            DateTime payDate = paymentDate ?? DateTime.UtcNow.AddMinutes(-15);
            string paymentDateString = payDate.ToString("hh:mm:ss+MMM+dd,+yyyy+PST");
            string accountBoostIPN = "mc_gross={0}&protection_eligibility=Ineligible&payer_id=ND6ZF72RVRVT2&payment_date={1}&payment_status=Completed&charset=windows-1252&first_name=Joe&mc_fee=0.57&notify_version=3.9&custom={2}&payer_status=unverified&business=info%40exclusivecard.co.uk&quantity=1&verify_sign=AYpKICe66PtxRdX481S7eVDEQaKLAm7tcDZAXQNhgfpsshkIrNlNqPff&payer_email=joe.bloggs%40test.com&txn_id={3}&payment_type=instant&last_name=Bloggs&receiver_email=info%40exclusivecard.co.uk&payment_fee=&shipping_discount=0.00&receiver_id=DFMVDP42SQLQJ&insurance_amount=0.00&txn_type=web_accept&item_name={5}&discount=0.00&mc_currency={4}&item_number=11003&residence_country=GB&shipping_method=Default&transaction_subject=&payment_gross=&ipn_track_id=6e88fc3eec431";
            accountBoostIPN = string.Format(accountBoostIPN, paymentAmount.ToString(), paymentDateString, customerProviderRef, paymentProviderRef, currencyCode, details);

            return accountBoostIPN;
        }

        private dto.CustomerPayment CreateCustomerPayment(dto.MembershipCard card,string payDetails)
        {
            try
            {
                Guid customerProviderRe = Guid.NewGuid();
                string ACCOUNT_BOOST_DETAILS = "Account+Boost";
                var testDate = DateTime.UtcNow;
                // generate unique payment provider ref based on current time and date, in a PayPal format
                string time = DateTime.UtcNow.ToString("hhmmmss");
                string dateish = DateTime.UtcNow.ToString("msddmm");
                string paymentProviderRe = "1AB" + time + "C" + dateish + "D";
                // Generate our fake IPN
                var paymentDat = testDate.AddHours(-8); // take 8 hours off to allow for PST and a lag between payment entered and IPN arriving
                string ipn = GetAcountBoostIPN(customerProviderRe.ToString(), paymentProviderRe, paymentDat, details: ACCOUNT_BOOST_DETAILS);


                //string PAYPAL_PROVIDER = "PayPal";
                //string IPN_TYPE = "txn_type";
                //string CUSTOMER_PROVIDER_REF = "custom";
                //string PAYMENT_PROVIDER_REF = "txn_id";
                //string PAYMENT_DATE = "payment_date";
                //string PAYMENT_AMOUNT = "mc_gross";
                //string CURRENCY_CODE = "mc_currency";
                //string DETAILS = "transaction_subject";
                //string SUBSCRIPTION_ID = "subscr_id";
                //string REOCCURING_SUBS_ID = "recurring_payment_id";
                //string SUBSCRIPTION_LENGTH = "payment_cycle";
                //string BOOST_DETAILS = "item_name";

                //string IPN_RENEWAL = "recurring_payment";
                //string IPN_UPGRADE = "subscr_payment";
                //string IPN_BOOST = "web_accept";
                //int ipnId = 0;
                //string paymentProviderRef = string.Empty;
                var ipnDict = ParsePaypalIPN(ipn);
                string ipnType = string.Empty;
                ipnType = ipnDict.GetValueOrDefault(IPN_TYPE);
                string customerProviderRef = ipnDict.GetValueOrDefault(CUSTOMER_PROVIDER_REF);

                GetPaymentProviderId();
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


                dto.CustomerPayment customerPayment = new dto.CustomerPayment()
                {
                    PaymentProviderId = _paymentProviderId,
                    CustomerId = card.CustomerId,
                    MembershipCardId = card.Id,
                    PaymentDate = DateTime.UtcNow,
                    Amount = paymentAmount,
                    CurrencyCode = currencyCode,
                    Details = details,
                    PaymentNotificationId = ipnId,
                    PaymentProviderRef = paymentProviderRef,

                };
                var customerPaymentId = _paymentManager.CreateCustomerPayment(customerPayment);
                customerPayment.Id = customerPaymentId;
                return customerPayment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        
        private void GetPaymentProviderId()
        {
            if (_paymentProviderId == 0)
            {
                _paymentProviderId = _paymentManager.GetPaymentProviderId(PAYPAL_PROVIDER);
            }
        }
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
        [Test]
        public void CreateMembershipCardsTest_UniqueCardNumber()
        {
            var customer = CreateCustomer();
            Assert.IsNotNull(customer);
            Assert.IsTrue(customer.Id > 0);

            var reward = CreatePartnerRewards();
            Assert.IsNotNull(reward);
            Assert.IsTrue(reward.Id > 0);

            var plan = MapPlan(_plan);
            var codeId = _code.Id;

            var cards1 = _membershipManager.CreateMembershipCards(plan, customer.Id, _code.Id, null, reward.Id, "GB");
            var cards2 = _membershipManager.CreateMembershipCards(plan, customer.Id, _code.Id, null, reward.Id, "GB");
            var cards3 = _membershipManager.CreateMembershipCards(plan, customer.Id, _code.Id, null, reward.Id, "GB");

            Assert.IsNotNull(cards1, "Cards not found");
            Assert.IsTrue(cards1.Count == 1, "Card count not 1 as expected");
            Assert.IsTrue(cards1.First().Id > 0, "Card not saved to Db - id = 0");

            Assert.IsNotNull(cards2, "Cards not found");
            Assert.IsTrue(cards2.Count == 1, "Card count not 1 as expected");
            Assert.IsTrue(cards2.First().Id > 0, "Card not saved to Db - id = 0");

            Assert.IsNotNull(cards3, "Cards not found");
            Assert.IsTrue(cards3.Count == 1, "Card count not 1 as expected");
            Assert.IsTrue(cards3.First().Id > 0, "Card not saved to Db - id = 0");



            // This bit Not working, don't like creating 2 txns using same DbContext.  Need to test at higher level.
            //var membershipManager1 = Configuration.ServiceProvider.GetService<IMembershipManager>();
            //var membershipManager2 = Configuration.ServiceProvider.GetService<IMembershipManager>();
            //var cards1 = Task.Run( () => (membershipManager1.CreateMembershipCards(plan, customer.Id, codeId, null, reward.Id, "GB")));
            //var cards2 = Task.Run(() => (membershipManager2.CreateMembershipCards(plan, customer.Id, codeId, null, reward.Id, "GB")));

            //Assert.IsNotNull(cards1.Result, "Cards not found");
            //Assert.IsTrue(cards1.Result.Count == 1, "Card count not 1 as expected");
            //Assert.IsTrue(cards1.Result.First().Id > 0, "Card not saved to Db - id = 0");

            //Assert.IsNotNull(cards2.Result, "Cards2 not found");
            //Assert.IsTrue(cards2.Result.Count == 1, "Card2 count not 1 as expected");
            //Assert.IsTrue(cards2.Result.First().Id > 0, "Card2 not saved to Db - id = 0");

        }

        [Test]
        public void CreateMembershipCardsTest_Diamond()
        {
            var customer = CreateCustomer();
            Assert.IsNotNull(customer);
            Assert.IsTrue(customer.Id > 0);

            var reward = CreatePartnerRewards();
            Assert.IsNotNull(reward);
            Assert.IsTrue(reward.Id > 0);

            var plan = MapPlan(_diamondPlan);

            var cards = _membershipManager.CreateMembershipCards(plan, customer.Id, _diamondCode.Id, null, reward.Id, "GB");
            Assert.IsNotNull(cards, "Cards not found");
            Assert.IsTrue(cards.Count == 2, "Card count not 2 as expected");

            foreach (var card in cards)
            {
                Assert.IsTrue(card.Id > 0, "Card not saved to Db - id = 0");
            }

        }

        #endregion

        #region MembershipCard Tests

        [Test]
        public void GetCardProviderTest_Valid()
        {
            var customer = CreateCustomer();
            Assert.IsNotNull(customer, "No customer");
            Assert.IsTrue(customer.Id > 0, "Customer not saved to db");

            Assert.IsTrue(_plan.CardProviderId > 0, "Card Provider Id = 0");

            var card = CreateMembershipCard(_plan, _code, (int?)customer.Id);
            Assert.IsNotNull(card, "No Card");
            Assert.IsTrue(card.Id > 0, "Card not saved to db");
            
            var partner = _membershipManager.GetCardProvider(card.Id);
            Assert.IsNotNull(partner, "Card provider not found");
            Assert.AreEqual(partner.Id, _plan.CardProviderId, "Card provider Id not matched to plan");
                
        }
        #endregion



    }
}
