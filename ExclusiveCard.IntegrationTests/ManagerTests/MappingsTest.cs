using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    class MappingsTest
    {
        IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = Configuration.ServiceProvider.GetService<IMapper>();

        }

        [Test]
        public void AutomapperMappingsTest()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
