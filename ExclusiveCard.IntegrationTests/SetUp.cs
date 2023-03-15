using NUnit.Framework;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests
{

    [SetUpFixture]
    public class SetUpTests
    {
        [OneTimeSetUp]
        public static async Task SetUpDbAndIoc()
        {
            await Configuration.ConfigureServices();
        }
    }
}
