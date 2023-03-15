using NUnit.Framework;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests.Public
{
    public class HomeScreenTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public async Task AssertOfferHubDataCreated()
        {
            var merchant = new Services.Models.DTOs.Merchant
            {
                Name = "Merchant 1",
                ContactName = "Contact Person",
                ShortDescription = "Merchant Short Description",
                LongDescription = "Merchant Long Description",
                Terms = "Merchant Terms",
                WebsiteUrl = "Merchant website url",
                IsDeleted = false,
                FeatureImageOfferText = false,
                BrandColour = "#FFFFF"
            };

            var merchantCreated = await ServiceHelper.Instance.MerchantService.Add(merchant);

            Assert.IsNotNull(merchantCreated, "Merchant not created");
            Assert.IsTrue(merchantCreated.Id > 0, "Merchant Id not greater than 0");
            Assert.AreEqual(merchant.Name, merchantCreated.Name, "Merchant name differs");
            Assert.AreEqual(merchant.ContactName, merchantCreated.ContactName, "Merchant contact name differs");
            Assert.AreEqual(merchant.ShortDescription, merchantCreated.ShortDescription, "Merchant short description differs");

        }

        [Test]
        public async Task AssertOfferHubGetById()
        {
            var merchant = new Services.Models.DTOs.Merchant
            {
                Name = "Merchant 1",
                ContactName = "Contact Person",
                ShortDescription = "Merchant Short Description",
                LongDescription = "Merchant Long Description",
                Terms = "Merchant Terms",
                WebsiteUrl = "Merchant website url",
                IsDeleted = false,
                FeatureImageOfferText = false,
                BrandColour = "#FFFFF"
            };

            var merchantCreated = await ServiceHelper.Instance.MerchantService.Add(merchant);

            Assert.IsNotNull(merchantCreated, "Merchant not created");
            Assert.IsTrue(merchantCreated.Id > 0, "Merchant Id not greater than 0");
            Assert.AreEqual(merchant.Name, merchantCreated.Name, "Merchant name differs");
            Assert.AreEqual(merchant.ContactName, merchantCreated.ContactName, "Merchant contact name differs");
            Assert.AreEqual(merchant.ShortDescription, merchantCreated.ShortDescription, "Merchant short description differs");
        }

        [Test]
        public async Task AssertOfferHubDataUpdated()
        {
            var merchant = new Services.Models.DTOs.Merchant
            {
                Name = "Merchant 1",
                ContactName = "Contact Person",
                ShortDescription = "Merchant Short Description",
                LongDescription = "Merchant Long Description",
                Terms = "Merchant Terms",
                WebsiteUrl = "Merchant website url",
                IsDeleted = false,
                FeatureImageOfferText = false,
                BrandColour = "#FFFFF"
            };

            var merchantCreated = await ServiceHelper.Instance.MerchantService.Add(merchant);

            Assert.IsNotNull(merchantCreated, "Merchant not created");
            Assert.IsTrue(merchantCreated.Id > 0, "Merchant Id not greater than 0");
            Assert.AreEqual(merchant.Name, merchantCreated.Name, "Merchant name differs");
            Assert.AreEqual(merchant.ContactName, merchantCreated.ContactName, "Merchant contact name differs");
            Assert.AreEqual(merchant.ShortDescription, merchantCreated.ShortDescription, "Merchant short description differs");
        }

        [Test]
        public void AssertWhiteLabelSetting()
        {
            var url = "https://localhost:44325/tp/";
            var data = ServiceHelper.Instance.WhiteLabelService.GetSiteSettings(url);

            Assert.IsNotNull(data, "whiteListSettings not found");
            Assert.AreEqual(data.URL, url, "Url differs");
        }

        [Test]
        public void AssertWhiteLabelString()
        {
            var url = "https://localhost:44325/tp/";
            var sourceString = "Hello our company name is {{DisplayName}} and company number is {{CompanyNumber}}";

            var data = ServiceHelper.Instance.WhiteLabelService.WhiteLabelString(url, sourceString);

            Assert.IsNotEmpty(data, "whiteListString is not empty");
            Assert.IsNotNull(data, "whiteListString is not null");
            Assert.IsTrue(!data.Contains("{{DisplayName}}"), "Contains DisplayName");
            Assert.IsTrue(!data.Contains("{{CompanyNumber}}"), "Contains CompanyNumber");
        }
    }
}
