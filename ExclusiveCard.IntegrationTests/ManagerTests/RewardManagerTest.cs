using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using db = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    class RewardManagerTest
    {
        IRewardManager _rewardManager;
        IRepository<db.Partner> _partnerRepo;
        IRepository<db.PartnerRewards> _rewardsRepo;
        //db.Partner _partner;

        [OneTimeSetUp]
        public void Setup()
        {
            _rewardManager = Configuration.ServiceProvider.GetService<IRewardManager>();
            _partnerRepo = Configuration.ServiceProvider.GetService<IRepository<db.Partner>>();
            _rewardsRepo = Configuration.ServiceProvider.GetService<IRepository<db.PartnerRewards>>();

        }

        private db.Partner CreatePartner()
        {
            string name = "TestPartner" + DateTime.UtcNow.Ticks.ToString();
            var partner = new db.Partner()
            {
                Name = name,
                Type = 1,
                IsDeleted = false
            };

            // Create a reward partner
            _partnerRepo.Create(partner);
            _partnerRepo.SaveChanges();

            partner = _partnerRepo.Filter(x => x.Name == name).First();
            Assert.IsNotNull(partner.Id);
            Assert.IsTrue(partner.Id > 0);


            return partner;

        }

        private dto.Customer CreateCustomer()
        {
            var customer = new dto.Customer()
            {
                Forename = "Another",
                Surname = "Tester"
            };

            return customer;
        }

        // For reasons as yet not understood, this test kills the internet ... well ok, just the rest of 
        // the tests, after it is run. So nuked it for now. 
        // Guess the reason is that the dbcontext is shared by other tests and gets broken by an error when
        // attempting to insert a record in violation of a foreign key constraint.
        //
        // FIXED - Was the vandelism that had been done to EF Context to disable tracking and the stupidity of 
        // hacking of the generic repo class that was screwing it up.  Fixed that and magically the issues
        // go away. 
        //[Test]
        public void CreatePartnerRewardTest_NoPartner()
        {
            var customer = CreateCustomer();

            try
            {
                int rewardId = _rewardManager.CreatePartnerReward(-23, customer);
                Assert.IsTrue(rewardId == 0, "Reward created despite partner being invalid");
            }
            catch (Exception) { }
        }

        [Test]
        public void CreatePartnerRewardTest_Valid()
        {

            var customer = CreateCustomer();
            var partner = CreatePartner();

            Assert.IsTrue(partner.Id > 0);
            //var rewardMgr = Configuration.ServiceProvider.GetService<IRewardManager>();

            int rewardId = _rewardManager.CreatePartnerReward(partner.Id, customer);
            Assert.IsTrue(rewardId > 0, "Create reward Failed - no Id set");
        }



        [Test]
        public void CreatePartnerRewardTest_CustomerNameClash()
        {
            var partner = CreatePartner();
            Assert.IsTrue(partner.Id > 0, "Partner not created");
            var customer = CreateCustomer();

            int rewardId1 = _rewardManager.CreatePartnerReward(partner.Id, customer);
            int rewardId2 = _rewardManager.CreatePartnerReward(partner.Id, customer);
            int rewardId3 = _rewardManager.CreatePartnerReward(partner.Id, customer);
            int rewardId4 = _rewardManager.CreatePartnerReward(partner.Id, customer);
            int rewardId5 = _rewardManager.CreatePartnerReward(partner.Id, customer);
            Assert.IsTrue(rewardId1 > 0, "Create reward 1 Failed - no Id set");
            Assert.IsTrue(rewardId2 > 0, "Create reward 2 Failed - no Id set");
            Assert.IsTrue(rewardId3 > 0, "Create reward 3 Failed - no Id set");
            Assert.IsTrue(rewardId4 > 0, "Create reward 4 Failed - no Id set");
            Assert.IsTrue(rewardId5 > 0, "Create reward 5 Failed - no Id set");


        }

        [Test]
        public void GetRewardSummaryTest_Valid()
        {
            const decimal latestValue = 101M;
            const decimal withdrawn = 21.22M;
            DateTime lastUpdate = DateTime.UtcNow.AddHours(-12);

            var partner = CreatePartner();
            var customer = CreateCustomer();
            Assert.IsTrue(partner.Id > 0, "Partner not created");

            int rewardId = _rewardManager.CreatePartnerReward(partner.Id, customer);
            Assert.IsTrue(rewardId > 0, "Create reward Failed - no Id set");

            var reward = _rewardsRepo.GetById(rewardId);
            Assert.IsNotNull(reward, "Reward not found in db");

            // set the values 
            reward.LatestValue = latestValue;
            reward.ValueDate = lastUpdate;
            reward.TotalConfirmedWithdrawn = withdrawn;
            _rewardsRepo.Update(reward);
            _rewardsRepo.SaveChanges();

            // Get the summary 
            var rewardSummary = _rewardManager.GetRewardSummary(rewardId);
            Assert.IsNotNull(rewardSummary);
            Assert.AreEqual(reward.LatestValue, rewardSummary.CurrentValue);
            Assert.AreEqual(reward.TotalConfirmedWithdrawn, rewardSummary.Withdrawn);
            Assert.AreEqual(reward.ValueDate, rewardSummary.LastUpdatedDate);


        }
    }
}
