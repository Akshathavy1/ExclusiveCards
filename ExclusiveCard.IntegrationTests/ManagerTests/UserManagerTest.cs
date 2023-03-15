using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using db = ExclusiveCard.Data.Models;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{

    public class UserManagerTest
    {
        IRepository<db.ExclusiveUser> _userRepo;
        IUserManager _userManager;

        string _testUserName;
        db.ExclusiveUser _testUser;

        [OneTimeSetUp]
        public void Setup()
        {
            _userManager = Configuration.ServiceProvider.GetService<IUserManager>();
            _userRepo = Configuration.ServiceProvider.GetService<IRepository<db.ExclusiveUser>>();

            CreateTestUser();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            try
            {
                _userRepo.Delete(_testUser);
                _userRepo.SaveChanges();
            }
            catch
            {
                // don't stop the testing just cause clean up failed. 
            }
        }

        private void CreateTestUser()
        {
            _testUserName = "TestUserA" + DateTime.UtcNow.Ticks.ToString();
            _testUser = new db.ExclusiveUser()
            {
                UserName = _testUserName,
                NormalizedUserName = _testUserName.ToUpper(),
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            _userRepo.Create(_testUser);
            _userRepo.SaveChanges();
        }

        [Test]
        public void CheckIfUserExistsTest_WithUser()
        {
            var result = _userManager.CheckExistsAsync(_testUserName).Result;
            Assert.IsTrue(result, "User was not found");
        }

        [Test]
        public void CheckIfUserExistsTest_WithoutUser()
        {
            var result = _userManager.CheckExistsAsync("Fish and Chipps").Result;
            Assert.IsFalse(result, "User was found, despite not existing. One for the philosophers there.");

        }

        [Test]
        public void CreateUserTest_Valid()
        {
            var username = "TestUserB" + DateTime.UtcNow.Ticks.ToString();
            var email = username + "@test.com";
            
            var user = new ExclusiveCard.Services.Models.DTOs.ExclusiveUser()
            {
                UserName = username,
                Email = email                
            };


            var userId = _userManager.CreateAsync(user).Result;

            Assert.IsNotNull(userId);
        }

        //[Test]
        //public void CustomerLoginTest_Valid()
        //{
        //    var username = "TestUserB" + DateTime.UtcNow.Ticks.ToString();
        //    var email = username + "@test.com";
        //    string password = "P@ssword1!";
        //    var user = new ExclusiveCard.Services.Models.DTOs.ExclusiveUser()
        //    {
        //        UserName = username,
        //        Email = email,
        //    };


        //    var userId = _userManager.CreateAsync(user, password, "User").Result;
        //    Assert.IsNotNull(userId);

        //    // no worky - says HTTPContext is null. 
        //    var result = _userManager.CustomerLoginAsync(username, password).Result;
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.Email == username);
        //    Assert.IsTrue(result.Id > 0);
        //}

        //[Test]
        //public void PartnerLoginTest_Valid()
        //{
        //    var username = "TestPartner" + DateTime.UtcNow.Ticks.ToString();
        //    var email = username + "@test.com";
        //    string password = "Password1!";
        //    var user = new ExclusiveCard.Services.Models.DTOs.ExclusiveUser()
        //    {
        //        UserName = username,
        //        Email = email,
        //    };


        //    var userId = _userManager.CreateAsync(user, password, "Partner").Result;
        //    Assert.IsNotNull(userId);

        //    // no worky - says HTTPContext is null. 
        //    var result = _userManager.CustomerLoginAsync(username, password).Result;
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.Email == username);
        //    Assert.IsTrue(result.Id > 0);
        //}

    }

}
