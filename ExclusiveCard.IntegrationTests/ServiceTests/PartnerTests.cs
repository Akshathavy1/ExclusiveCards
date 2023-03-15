using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    public class PartnerTests
    {
        IPartnerService _partnerService;
        IUserManager _userManager;
        IRepository<ExclusiveUser> _userRepo;
        IRepository<Partner> _partnerRepo;

        string _testUserName;
        string _password;
        Dto.ExclusiveUser _testUser;
        Partner _testpartner;

        [OneTimeSetUp]
        public void Setup()
        {
            _partnerService = Configuration.ServiceProvider.GetService<IPartnerService>();
            _userRepo = Configuration.ServiceProvider.GetService<IRepository<ExclusiveUser>>();
            _partnerRepo = Configuration.ServiceProvider.GetService<IRepository<Partner>>();
            _userManager = Configuration.ServiceProvider.GetService<IUserManager>();


            //need to create records - ASPNetUser & Partner
            //CreateTestUserAndPartner();
            //## No EF class curently exists for [Exclusive].[AspNetUsers]
        }

        /// <summary>
        /// This creates the mobile App's partner and partner user if they don't already exist
        /// Possibly need to migrate this code the main site startup temporarily to ensure 
        /// the mobile app partner is created on the platform - need to talk to Ian about how he 
        /// set up the consumer rights user/partner combination
        /// </summary>
        /// <returns></returns>
        [Test]
        public async System.Threading.Tasks.Task CreateMobiletUserAndPartner()
        {
            //### make sure the PartnerAPI role exists before you run this (there may not be one in live)!!!! ###
            byte[] MAUP = new byte[] { 104, 0, 89, 0, 109, 0, 113, 0, 83, 0, 100, 0, 122, 0, 118, 0, 64, 0, 108, 0, 121, 0, 113, 0, 73, 0, 46, 0, 73, 0, 57, 0, 90, 0, 115, 0 };
            string MobileAppUserName = System.Text.Encoding.Unicode.GetString(MAUP);

            byte[] MAPP = new byte[] { 89, 0, 81, 0, 103, 0, 113, 0, 79, 0, 52, 0, 66, 0, 51, 0, 85, 0, 86, 0, 97, 0, 35, 0, 86, 0, 63, 0, 114, 0, 57, 0 };
            string MobileAppUserpassword = System.Text.Encoding.Unicode.GetString(MAPP);

            var existingMobileUser = await _userManager.GetUserAsync(MobileAppUserName);
            if (existingMobileUser == null)
            {
                Dto.ExclusiveUser MobileUser = new Dto.ExclusiveUser()
                {
                    UserName = MobileAppUserName,
                    Email = MobileAppUserName
                };
                var result = await _userManager.CreateAsync(MobileUser, MobileAppUserpassword, Enums.Roles.PartnerAPI.ToString());
                existingMobileUser = await _userManager.GetUserAsync(MobileAppUserName);
            }

            var existingPartner = _partnerRepo.Get(P => P.Name == Data.Constants.Keys.MobileAppPartner);
            if (existingPartner == null)
            {
                Partner mobilePartner = new Partner()
                {
                    Name = Data.Constants.Keys.MobileAppPartner,
                    IsDeleted = false,
                    Type = (int)Enums.PartnerType.CardProvider,
                };
                _partnerRepo.Create(mobilePartner);
                _partnerRepo.SaveChanges();
                existingPartner = _partnerRepo.Get(P => P.Name == Data.Constants.Keys.MobileAppPartner);
            }

            if (existingPartner != null && existingPartner.AspNetUserId != existingMobileUser.Id)
            {
                existingPartner.AspNetUserId = existingMobileUser.Id;
                _partnerRepo.Update(existingPartner);
                _partnerRepo.SaveChanges();
            }
        }

        private void CreateTestUserAndPartner()
        {
            _testUserName = "TestUser" + DateTime.UtcNow.Ticks.ToString();
            _password = "Password1!";
            _testpartner = new Partner()
            {
                Name = "TestPartner",
                IsDeleted = false,
                Type = 1
            };
            _partnerRepo.Create(_testpartner);
            
            _testUser = new Dto.ExclusiveUser()
            {
                UserName = _testUserName,
                Email = _testUserName
            };
            //_userRepo.Create(_testUser);
            var result = _userManager.CreateAsync(_testUser, _password, "PartnerAPI");


            //_userRepo.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            if(_testpartner != null)
                _partnerRepo.Delete(_testpartner);
        }

        [Test]
        public void ValidLogin()
        {
            var loginResponse = _partnerService.LoginAsync(_testUserName, _password, "IJWAATest.co.uk").Result;

            Assert.IsNotNull(loginResponse, "Response record received");
            //Assert.IsNotNull(loginResponseDto.Token, "Response contained token");
        }

        [Test]
        public void TestInvalidLogin()
        {
            var loginResponse = _partnerService.LoginAsync("BillAndTed", "HappyCampers", "IJWAATest.co.uk").Result;
            Assert.IsNull(loginResponse, "Response record returned unexpectidly");
            //if (loginResponseDto != null)
            //    Assert.IsNull(loginResponseDto.Token, "Response unexpectidly contained a token");
        }
    }
}
