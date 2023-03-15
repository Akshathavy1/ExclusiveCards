using NUnit.Framework;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests
{
    public class EmailServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public async Task TestEmail()
        {
            string forename = "Phani";
            object surname = "A";
            await ServiceHelper.Instance.EmailManager.SendEmailAsync("test678@sharklasers.com", 0,
                new { Name = $"{forename} {surname}" });
        }
    }
}
