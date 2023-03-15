using System;
using System.Collections.Generic;
using System.Text;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    class OfferImportServiceTests
    {
        IOfferImportService _offerImportService;

        [OneTimeSetUp]
        public void Setup()
        {
            _offerImportService = Configuration.ServiceProvider.GetService<IOfferImportService>();
        }

        // TODO: Decide whether we need end to end service test here
        //       Option B is stick with the Manager tests only. 
    }

    
}
