using AutoMapper;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    internal class SSOManagerTests
    {
        private ISSOManager _sSOManager;

        [SetUp]
        public void Setup()
        {
            _sSOManager = Configuration.ServiceProvider.GetService<ISSOManager>();
        }

        [Test]
        public async Task GetAllSSOConfigurationsTest(bool hasData)
        {
            var sSOConfigurations = await _sSOManager.GetAllSSOConfigurations();
        }
    }
}