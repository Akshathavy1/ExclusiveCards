using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    class MarketingServiceTests
    {
        IMarketingService _MarketingService;

        [OneTimeSetUp]
        public void Setup()
        {
            _MarketingService = Configuration.ServiceProvider.GetService<IMarketingService>();
        }


    }
}
