using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using db = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    class CustomerManagerTest
    {
        private ICustomerManager _customerManager;
        private IUserManager _userManager;
        private IRepository<db.Customer> _customerRepo;
        private IRepository<db.SecurityQuestion> _securityQuestionRepo;

        [SetUp]
        public void Setup()
        {
            _customerManager = Configuration.ServiceProvider.GetService<ICustomerManager>();
            _userManager = Configuration.ServiceProvider.GetService<IUserManager>();
            _customerRepo = Configuration.ServiceProvider.GetService<IRepository<db.Customer>>();
            _securityQuestionRepo = Configuration.ServiceProvider.GetService<IRepository<db.SecurityQuestion>>();

        }

        [TearDown]
        public void TearDown()
        {

        }


        [Test]
        public void CreateCustomerTest_UserAndCustomer()
        {
            var user = CreateTestUser();
            var customer = CreateCustomer(user.Id);

            customer = _customerManager.Create(customer);
            Assert.IsNotNull(customer.IdentityUser, "No IdentityUser set, customer is missing their ASPNETUser");
            Assert.AreEqual(customer.IdentityUser.Id, customer.AspNetUserId, "Since we are stuck with duplicate property for IdentityUser.Id, please at least make sure it gets set");
            Assert.IsTrue(customer.Id > 0, "CustomerId is not set, therefore customer not created properly.");
            Assert.IsTrue(customer.DateAdded > DateTime.Today, "Date added fied is invalid - customer may not be created properly");

        }

        [Test]
        public void CreateCustomerTest_WithContactDetail()
        {
            var user = CreateTestUser();
            var customer = CreateCustomer(user.Id);
            customer.ContactDetail = CreateContactDetail();

            customer = _customerManager.Create(customer);
            Assert.IsNotNull(customer.IdentityUser, "No IdentityUser set, customer is missing their ASPNETUser");
            Assert.AreEqual(customer.IdentityUser.Id, customer.AspNetUserId, "Since we are stuck with duplicate property for IdentityUser.Id, please at least make sure it gets set");
            Assert.IsTrue(customer.Id > 0, "CustomerId is not set, therefore customer not created properly.");
            Assert.IsTrue(customer.DateAdded > DateTime.Today, "Date added fied is invalid - customer may not be created properly");

            Assert.IsNotNull(customer.ContactDetail, "ContactDetail is null, did contact detail actually get created?");
            Assert.IsTrue(customer.ContactDetailId > 0, "The contact Details id is missing - were contact details saved?");
        }

        [Test]
        public void CreateCustomerTest_WithSecurityQuestion()
        {
            var user = CreateTestUser();
            var customer = CreateCustomer(user.Id);
            var question = CreateSecurityQuestion();

            customer.CustomerSecurityQuestions = new List<dto.CustomerSecurityQuestion>();
            var custSecurityQuestion = CreateSecurityQuestion();
            customer.CustomerSecurityQuestions.Add(custSecurityQuestion);

            customer = _customerManager.Create(customer);
            Assert.IsNotNull(customer.IdentityUser, "No IdentityUser set, customer is missing their ASPNETUser");
            Assert.AreEqual(customer.IdentityUser.Id, customer.AspNetUserId, "Since we are stuck with duplicate property for IdentityUser.Id, please at least make sure it gets set");

            Assert.IsTrue(customer.Id > 0, "CustomerId is not set, therefore customer not created properly.");
            Assert.IsTrue(customer.DateAdded > DateTime.Today, "Date added fied is invalid - customer may not be created properly");

            Assert.IsNotNull(customer.CustomerSecurityQuestions, "No security question answers found - list is null");
            Assert.IsTrue(customer.CustomerSecurityQuestions.Count == 1, "No security question answers found or too many. Just not the right amount (1)");

            var savedAnswer = customer.CustomerSecurityQuestions.First();
            Assert.IsNotNull(savedAnswer, "saved answer is null");
            Assert.AreEqual(savedAnswer.CustomerId, customer.Id);
            Assert.IsTrue(savedAnswer.SecurityQuestionId > 0);
        }

        [Test]
        public void CreateCustomerTest_WithBankDetails()
        {
            var user = CreateTestUser();
            var customer = CreateCustomer(user.Id);
            var bankDetails = CreateBankDetails();

            customer.CustomerBankDetails = new List<dto.CustomerBankDetail>();
            customer.CustomerBankDetails.Add(bankDetails);

            customer = _customerManager.Create(customer);
            Assert.IsNotNull(customer.IdentityUser, "No IdentityUser set, customer is missing their ASPNETUser");
            Assert.AreEqual(customer.IdentityUser.Id, customer.AspNetUserId, "Since we are stuck with duplicate property for IdentityUser.Id, please at least make sure it gets set");

            Assert.IsTrue(customer.Id > 0, "CustomerId is not set, therefore customer not created properly.");
            Assert.IsTrue(customer.DateAdded > DateTime.Today, "Date added fied is invalid - customer may not be created properly");

            Assert.IsNotNull(customer.CustomerBankDetails, "Customer Bank Details list is null");
            var savedCustBankDetails = customer.CustomerBankDetails.FirstOrDefault();
            Assert.IsNotNull(savedCustBankDetails, "Customer bank details were not found");
            Assert.AreEqual(savedCustBankDetails.CustomerId, customer.Id, "Customer Id on customer bank details incorrect");
            Assert.IsNotNull(savedCustBankDetails.BankDetail, "Bank details missing from CustomerBankDetails");
            Assert.IsTrue(savedCustBankDetails.BankDetail.Id > 0);
            Assert.IsNotNull(savedCustBankDetails.BankDetail.AccountNumber, "Account Number is null");
            Assert.IsNotNull(savedCustBankDetails.BankDetail.SortCode, "SortCode  is null");





        }





        private dto.ExclusiveUser CreateTestUser()
        {
            var testUserName = "TestUser" + DateTime.UtcNow.Ticks.ToString();
            var testUser = new dto.ExclusiveUser()
            {
                UserName = testUserName,
            };

            testUser.Id = _userManager.CreateAsync(testUser).Result;
            return testUser;

        }


        private dto.Customer CreateCustomer(string userId)
        {
            var customer = new dto.Customer()
            {
                AspNetUserId = userId,
                Forename = "A",
                Surname = "TestUser",
            };

            return customer;
        }

        private dto.ContactDetail CreateContactDetail()
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

        private dto.CustomerSecurityQuestion CreateSecurityQuestion()
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

        private dto.CustomerBankDetail CreateBankDetails()
        {
            var dtoBankDetail = new dto.BankDetail
            {
                AccountName = "test account",
                AccountNumber = "12345678",
                SortCode = "121212",
                BankName = "A Bank"

            };
            var dtoCustomerBankDetails = new dto.CustomerBankDetail
            {
                MandateAccepted = true,
                DateMandateAccepted = DateTime.Now,
                BankDetail = dtoBankDetail

            };

            return dtoCustomerBankDetails;
        }
    }
}
