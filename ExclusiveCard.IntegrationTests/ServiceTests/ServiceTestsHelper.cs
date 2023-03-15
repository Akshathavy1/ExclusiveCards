using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using ExclusiveCard.Data.Repositories;
using db = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Managers;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    class ServiceTestsHelper
    {
        private ICustomerAccountService _customerAccountService;
        private IMembershipManager _membershipManager;
        private IRepository<db.MembershipRegistrationCode> _codeRepo;
        private IRepository<db.SecurityQuestion> _securityQuestionRepo;
        private IRepository<WhiteLabelSettings> _whiteLabelSettingsRepo;

        private WhiteLabelSettings _whiteLabel = null;

        public ServiceTestsHelper()
        {
            _codeRepo = Configuration.ServiceProvider.GetService<IRepository<db.MembershipRegistrationCode>>();
            _securityQuestionRepo = Configuration.ServiceProvider.GetService<IRepository<db.SecurityQuestion>>();
            _customerAccountService = Configuration.ServiceProvider.GetService<ICustomerAccountService>();
            _membershipManager = Configuration.ServiceProvider.GetService<IMembershipManager>();
            _whiteLabelSettingsRepo = Configuration.ServiceProvider.GetService<IRepository<WhiteLabelSettings>>();

            //Membership plans are now linked to white labels...
            _whiteLabel = CreateWhiteLabel();
        }

        public db.MembershipRegistrationCode CreateRegistrationCodeAndMembershipPlan(string registrationCode = null, int planTypeId = 4)
        {
            var partner = CreatePartner();

            // Create standard code;
            GetRegistrationCode(ref registrationCode);
            var code = CreateCode(partner, registrationCode, 1, planTypeId); // hack for membership level Ids because the membership level table is a pain in the butt

            // Create Diamond Plan
            string diamondReg = null;
            GetRegistrationCode(ref diamondReg);
            var diamondCode = CreateCode(partner, diamondReg, 2, planTypeId);

            // Return the standard code only 
            return code;
        }

        public db.MembershipRegistrationCode CreateCode(db.Partner partner, string registrationCode, int levelId = 0, int planTypeId = 4)
        {
            decimal cardPrice = 0M;
            if (levelId == 2)
                cardPrice = 6.99M;

            // Create plan and registration code
            var plan = new db.MembershipPlan()
            {
                MembershipPlanTypeId = planTypeId, //4 = Partner Rewards, 3 = Beneficiary Rewards
                Duration = 365,
                NumberOfCards = 1000,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(1),
                IsActive = true,
                CardProvider = partner,
                PaymentFee = 1,
                MembershipLevelId = levelId,
                CustomerCardPrice = cardPrice,
                WhitelabelId = _whiteLabel.Id,
                Description = $"TestPlan PlanType {planTypeId}, MembershipLevel {(levelId == 2? "Diamond": "Standard")} {DateTime.UtcNow.Ticks}"
            };

            var code = new db.MembershipRegistrationCode()
            {
                RegistartionCode = registrationCode,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(1),
                NumberOfCards = 1000,
                IsActive = true,
                MembershipPlan = plan
            };
            _codeRepo.Create(code);
            _codeRepo.SaveChanges();

            return code;
        }

        public dto.Customer CreateNewCustomerAndMembership(string registrationCode = null)
        {
            GetRegistrationCode(ref registrationCode);

            var customerAccount = CreateCustomerAccount();
            var confirmURL = "https://localhost/Account/Confirm";
            var result = _customerAccountService.CreateAccountFromRegistrationCode(customerAccount, registrationCode, confirmURL);
            Assert.IsNotNull(result);
            customerAccount.Customer.Id = (int)result.Id;
            return customerAccount.Customer;

        }

        public dto.MembershipCard GetActiveMembershipCard(int customerId)
        {
            var card = _membershipManager.GetActiveMembershipCard(customerId);
            return card;
        }

        //public dto.MembershipCard GetDiamondCard(int customerId)
        //{
        //    var card = _membershipManager.GetDiamondMembershipCard(customerId);
        //    return card;
        //}

        public void GetRegistrationCode (ref string registrationCode)
        {
            if (registrationCode == null)
                registrationCode =  "ABC" + DateTime.UtcNow.Ticks.ToString();

        }

        public dto.CustomerAccountDto CreateCustomerAccount()
        {
            string userName = "user" + DateTime.UtcNow.Ticks.ToString();
            var securityQuestion = CreateSecurityQuestion();
            var contactDetail = CreateContactDetail();
            var customer = CreateCustomer();
            customer.CustomerSecurityQuestions = new List<dto.CustomerSecurityQuestion>();
            customer.CustomerSecurityQuestions.Add(securityQuestion);
            customer.ContactDetail = contactDetail;

            var customerAccount = new dto.CustomerAccountDto()
            {
                Customer = customer,
                Username = userName,
                CountryCode = "GB",
                TermsConditionsId = 1
            };

            return customerAccount;
        }

        private WhiteLabelSettings CreateWhiteLabel()
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

        public dto.Customer CreateCustomer()
        {
            var customer = new dto.Customer()
            {
                Forename = "A",
                Surname = "TestUser",
                DateOfBirth = DateTime.UtcNow.AddYears(-31),
                MarketingNewsLetter = false,
                Title = "Missing"
            };

            return customer;
        }


        public db.Partner CreatePartner()
        {
            var partner = new db.Partner()
            {
                Name = "TestPartner" + DateTime.UtcNow.Ticks.ToString(),
                Type = 1,
                IsDeleted = false
            };

            // Create a reward partner
            //_partnerRepo.Create(partner);

            return partner;

        }

        public dto.ContactDetail CreateContactDetail()
        {
            var contactDetail = new dto.ContactDetail
            {
                Address1 = "1 Long Test Lane",
                Town = "Test Town",
                District = "Test District",
                PostCode = "TE3 3TN",
                MobilePhone = "07890 123456"
            };

            return contactDetail;
        }

        public dto.CustomerSecurityQuestion CreateSecurityQuestion()
        {
            var question = _securityQuestionRepo.Filter().FirstOrDefault();
            Assert.IsNotNull(question, "No question defined in Exclusive.SecurityQuestion table");

            var custSecurityQuestion = new dto.CustomerSecurityQuestion()
            {
                SecurityQuestionId = question.Id,
                Answer = "a testing answer to a testing question",
            };

            return custSecurityQuestion;

        }
    }
}
