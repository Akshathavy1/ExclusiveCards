using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;



namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    class CustomerAccountServiceTest
    {
        //private ICustomerAccountService _customerAccountService;
        
        string _registrationCode;
        private ServiceTestsHelper _helper;


        [SetUp]
        public void Setup()
        {
            _helper = new ServiceTestsHelper();
            _registrationCode = "ABC" + DateTime.UtcNow.Ticks.ToString();
            _helper.CreateRegistrationCodeAndMembershipPlan(_registrationCode);
        }


        [Test]
        public void CreateAccountFromRegistrationCodeTest_Valid()
        {
            _helper.CreateNewCustomerAndMembership(_registrationCode);
        }


        

    }
}
