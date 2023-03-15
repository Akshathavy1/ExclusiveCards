using db = ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using AutoMapper;
using Microsoft.SqlServer.Server;
using ExclusiveCard.Enums;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{

    class CashbackManagerTest
    {
        private IMapper _mapper;

        private ICashbackManager _cashbackManager;
        private ICustomerManager _customerManager;
        private IUserManager _userManager;
        private IRepository<db.CashbackSummary> _summaryRepo;
        private IRepository<db.MembershipCard> _cardRepo;
        private IRepository<db.MembershipPlan> _planRepo;

        private db.MembershipPlan _plan;

        [SetUp]
        public void Setup()
        {
            _mapper = Configuration.ServiceProvider.GetService<IMapper>();

            _cashbackManager = Configuration.ServiceProvider.GetService<ICashbackManager>();
            _customerManager = Configuration.ServiceProvider.GetService<ICustomerManager>();
            _userManager = Configuration.ServiceProvider.GetService<IUserManager>();

            _summaryRepo = Configuration.ServiceProvider.GetService<IRepository<db.CashbackSummary>>();
            _cardRepo = Configuration.ServiceProvider.GetService<IRepository<db.MembershipCard>>();
            _planRepo = Configuration.ServiceProvider.GetService<IRepository<db.MembershipPlan>>();

            _plan = CreateStandardPlan();
        }

        [Test]
        public void CreateCashbackSummaryTest_Valid()
        {
            var card = CreateMembershipCard();
            Assert.IsNotNull(card, "Could not create a membership card");
            Assert.IsTrue(card.Id > 0, "Could not save membership card to Db");

            _cashbackManager.CreateSummaryRecords(card.Id, ExclusiveCard.Enums.MembershipPlanTypeEnum.PartnerReward);

            var rewardSummary = _summaryRepo.Filter(x => x.MembershipCardId == card.Id && x.AccountType == 'R').FirstOrDefault();
            Assert.IsNotNull(rewardSummary, "Reward summary Not found");

            var deductSummary = _summaryRepo.Filter(x => x.MembershipCardId == card.Id && x.AccountType == 'D').FirstOrDefault();
            Assert.IsNotNull(deductSummary, "Deduction summary Not found");



        }

        
        [Test]
        public void GetCashbackSplitTest()
        {
            CashbackTransaction cashbackTran = new CashbackTransaction() { CashbackAmount = 100 };

            //Check plan with no benefactor still returns amounts
            var plan1 = CreatePlanWithCustomerCashbackAndDeduction();
            Assert.IsNotNull(plan1, "Cashback and deduction plan failed to create");
            var amounts = _cashbackManager.GetCashbackValuesForPlan(cashbackTran, plan1);
            Assert.IsTrue(amounts.Item1 + amounts.Item2 + amounts.Item3 > 0, "No values calculated for plan1");

            //Check plan with deduction and new benefactor returns amounts
            var plan2 = CreatePlanWithCustomerCashbackDeductionAndBenefactor();
            Assert.IsNotNull(plan2, "Cashback, deduction and benefactor plan failed to create");
            var amounts1 = _cashbackManager.GetCashbackValuesForPlan(cashbackTran, plan2);
            Assert.IsTrue(amounts1.Item1 + amounts1.Item2 + amounts1.Item3 > 0, "No values calculated for plan2");
            Assert.IsTrue(amounts1.Item3 > 0, "No value calculated for Benefactor");

            //Tidy up...
            _planRepo.Delete(plan1.Id);
            _planRepo.Delete(plan2.Id);
            _planRepo.SaveChanges();

        }

        [Test]
        public void CreateCashbackTransactionsTest()
        {
            var card = CreateMembershipCard();
            Assert.IsNotNull(card, "Could not create a membership card");
            Assert.IsTrue(card.Id > 0, "Could not save membership card to Db");

            db.CashbackSummary summaryR = new db.CashbackSummary()
            { CurrencyCode = "GBP",
                AccountType = 'R',
                MembershipCardId = card.Id
            };
            _summaryRepo.Create(summaryR);

            db.CashbackSummary summaryD = new db.CashbackSummary()
            {
                CurrencyCode = "GBP",
                AccountType = 'D',
                MembershipCardId = card.Id
            };
            _summaryRepo.Create(summaryD);

            db.CashbackSummary summaryB = new db.CashbackSummary()
            {
                CurrencyCode = "GBP",
                AccountType = 'B',
                MembershipCardId = card.Id
            };
            _summaryRepo.Create(summaryB);

            _summaryRepo.SaveChanges();

            CashbackTransaction cashbackTran = new CashbackTransaction() 
                                {   CashbackAmount = 100, 
                                    MembershipCardId = card.Id, 
                                    StatusId = (int)Enums.Cashback.Received,
                                    CurrencyCode = "GBP"
            };
            var plan = CreatePlanWithCustomerCashbackDeductionAndBenefactor();
            Assert.IsNotNull(plan, "Cashback and deduction plan failed to create");

            _cashbackManager.CreateCashbackTransactions(cashbackTran, plan);
        }

        [Test]
        public void GetCashbackBalancesTest_Valid()
        {
            var card = CreateMembershipCard();
            Assert.IsNotNull(card, "Could not create a membership card");
            Assert.IsTrue(card.Id > 0, "Could not save membership card to Db");

            _cashbackManager.CreateSummaryRecords(card.Id, ExclusiveCard.Enums.MembershipPlanTypeEnum.PartnerReward);

            var rewardSummary = _summaryRepo.Filter(x => x.MembershipCardId == card.Id && x.AccountType == 'R').FirstOrDefault();
            Assert.IsNotNull(rewardSummary, "Reward summary Not found");

            var deductSummary = _summaryRepo.Filter(x => x.MembershipCardId == card.Id && x.AccountType == 'D').FirstOrDefault();
            Assert.IsNotNull(deductSummary, "Reward summary Not found");

            _cashbackManager.AddToCustomerBalance(card.Id, 'R', 1.53M, 3.33M, 101M);
            
            rewardSummary.PendingAmount = 1.53M;
            rewardSummary.ConfirmedAmount = 3.33M;
            rewardSummary.ReceivedAmount = 101M;
            

            var balances = _cashbackManager.GetCustomerBalances((int)card.CustomerId);
            Assert.IsNotNull(balances);
            Assert.AreEqual(rewardSummary.PendingAmount, balances.PendingAmount);
            Assert.AreEqual(rewardSummary.ConfirmedAmount, balances.ConfirmedAmount);
            Assert.AreEqual(rewardSummary.ReceivedAmount, balances.ReceivedAmount);


        }


        #region private methods

        private MembershipPlan CreatePlanWithCustomerCashbackAndDeduction(int numberOfCards = 1000, int numberofPlans = 1000,float custCashback = 80.0f, float deduction = 20.0f)
        {

            var plan = new MembershipPlan()
            {
                MembershipPlanTypeId = (int)MembershipPlanTypeEnum.PartnerReward,
                Duration = 365,
                NumberOfCards = numberofPlans,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(1),
                IsActive = true,
                MembershipLevelId = 1,
                CustomerCashbackPercentage = custCashback,
                DeductionPercentage = deduction,
                WhitelabelId = 1
            };

            var dbplan = _mapper.Map<db.MembershipPlan>(plan);
            _planRepo.Create(dbplan);
            _planRepo.SaveChanges();

            plan.Id = dbplan.Id;

            return plan;
        }

        private MembershipPlan CreatePlanWithCustomerCashbackDeductionAndBenefactor(int numberOfCards = 1000, int numberofPlans = 1000, float custCashback = 70.0f, float deduction = 20.0f, float benefactor = 10.0f)
        {
            var plan = new MembershipPlan()
            {
                MembershipPlanTypeId = (int)MembershipPlanTypeEnum.PartnerReward,
                Duration = 365,
                NumberOfCards = numberofPlans,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(1),
                IsActive = true,
                MembershipLevelId = 1,
                CustomerCashbackPercentage = custCashback,
                DeductionPercentage = deduction,
                BenefactorPercentage = benefactor,
                WhitelabelId = 1
            };

            var dbplan = _mapper.Map<db.MembershipPlan>(plan);
            _planRepo.Create(dbplan);
            _planRepo.SaveChanges();

            plan.Id = dbplan.Id;

            return plan;
        }

        private db.MembershipPlan CreateStandardPlan(int numberOfCards = 1000, int numberofPlans = 1000)
        {
            
            var plan = new db.MembershipPlan()
            {
                MembershipPlanTypeId = (int)MembershipPlanTypeEnum.PartnerReward,
                Duration = 365,
                NumberOfCards = numberofPlans,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(1),
                IsActive = true,
                MembershipLevelId = 1,
                WhitelabelId = 1
            };

            _planRepo.Create(plan);
            _planRepo.SaveChanges();

            return plan;
        }

        private db.MembershipCard CreateMembershipCard()
        {
            var user = CreateTestUser();
            var customer = CreateCustomer(user.Id);

            var card = new db.MembershipCard()
            {
                CustomerId = customer.Id,
                MembershipPlanId = _plan.Id,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(1),
                StatusId = 1,
                IsActive = true,
                IsDeleted = false,
                PhysicalCardRequested = false
                //RegistrationCode = code.Id
            };
            _cardRepo.Create(card);
            _cardRepo.SaveChanges();

            return card;
        }

        private ExclusiveUser CreateTestUser()
        {
            var testUserName = "TestUser" + DateTime.UtcNow.Ticks.ToString();
            var testUser = new ExclusiveUser()
            {
                UserName = testUserName,
            };

            testUser.Id = _userManager.CreateAsync(testUser).Result;
            return testUser;

        }


        private Customer CreateCustomer(string userId)
        {
            var customer = new Customer()
            {
                AspNetUserId = userId,
                Forename = "A",
                Surname = "TestUser",
            };
            customer = _customerManager.Create(customer);
            return customer;
        }

        #endregion

    }
}
