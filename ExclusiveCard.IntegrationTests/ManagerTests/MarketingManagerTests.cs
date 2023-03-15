using db = ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using ExclusiveCard.Enums;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using AutoMapper;
using Microsoft.SqlServer.Server;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    class MarketingManagerTests
    {
        private IMapper _mapper;

        private IMarketingManager _marketingManager;

        [SetUp]
        public void Setup()
        {
            _mapper = Configuration.ServiceProvider.GetService<IMapper>();
            _marketingManager = Configuration.ServiceProvider.GetService<IMarketingManager>();
        }

        [Test]
        public async Task AddMissingContactListsTest()
        {
           var listsAdded = await _marketingManager.AddMissingContactLists();
        }

        [Test]
        public async Task RemoveOptedOutContactsTest()
        {
            var contactsRemoved = await _marketingManager.RemoveOptedOutContacts();
        }

        [Test]
        public async Task AddOptedInContactsTest()
        {
            var contactsAdded = await _marketingManager.AddOptedInContacts();
        }

        [Test]
        public async Task RemoveOptedOutCampaignsTest()
        {
            var eventsRemoved = await _marketingManager.RemoveOptedOutCampaigns();
        }

        [Test]
        public async Task AddOptedInCampaignsTest()
        {
            var eventsAdded = await _marketingManager.AddOptedInCampaigns();
        }

    }
}
