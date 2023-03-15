using ExclusiveCard.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ExclusiveCard.IntegrationTests.ServiceTests
{
    class UserServiceTests
    {
        IUserService _UserService;
        string userName = "MyTestTesting@Testing.com";

        [OneTimeSetUp]
        public void Setup()
        {
            _UserService = Configuration.ServiceProvider.GetService<IUserService>();
        }

        [Test]
        public async System.Threading.Tasks.Task ChangePassword()
        {
            var exclusiveUser = await _UserService.FindByNameAsync(userName);
            Assert.IsNotNull(exclusiveUser, "Did not find user");

            //##Note that the password requires a non-alphanumeric
            Assert.IsTrue(await _UserService.AddNewPassword(exclusiveUser, "Abcd@1234"), "Password not changed");
        }
    }
}
