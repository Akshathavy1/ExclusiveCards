using NUnit.Framework;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.Admin
{
    class AdminMerchantTest
    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public async Task AddMerchant()
        {
            dto.Merchant merchant = Common.Common.BuildMerchantModel();
            dto.Merchant merchantAdded = await ServiceHelper.Instance.MerchantService.Add(merchant);
            Assert.IsNotNull(merchant, "Initialize customer model is null.");
            Assert.IsNotNull(merchantAdded, "Customer data not found.");
            Assert.IsTrue(merchantAdded.Id > 0, "Merchant not created");
            Assert.AreEqual(merchant.Name, merchantAdded.Name, "Merchant Name is not same");
            Assert.AreEqual(merchant.ContactName, merchantAdded.ContactName, "Merchnat ContactName is not same");
            Assert.AreEqual(merchant.ShortDescription, merchantAdded.ShortDescription, "Merchnat shortdescription is not same");
            Assert.AreEqual(merchant.LongDescription, merchantAdded.LongDescription, "Merchnat longdescription is not same");
            Assert.AreEqual(merchant.Terms, merchantAdded.Terms, "Merchant terms are not same");
            Assert.AreEqual(merchant.WebsiteUrl, merchantAdded.WebsiteUrl, "Merchant websiteurl is not same");
            Assert.AreEqual(merchant.IsDeleted, merchantAdded.IsDeleted, "merchant is deleted");


            Assert.IsNotNull(merchant.ContactDetail, "Initial Contact details for the customer is null.");
            Assert.IsNotNull(merchantAdded.ContactDetail, "Contact details for the customer is null.");
            Assert.AreEqual(merchant.ContactDetail.Address1, merchantAdded.ContactDetail.Address1, "Merchant contact detail Address 1 is not same");
            Assert.AreEqual(merchant.ContactDetail.Address2, merchantAdded.ContactDetail.Address2, "Merchant contact detail Address 2 is not same");
            Assert.AreEqual(merchant.ContactDetail.Address3, merchantAdded.ContactDetail.Address3, "Merchant contact detail Address 3 is not same");
            Assert.AreEqual(merchant.ContactDetail.Town, merchantAdded.ContactDetail.Town, "Merchant contact detail Town is not same");
            Assert.AreEqual(merchant.ContactDetail.District, merchantAdded.ContactDetail.District, "Merchant contact detail District is not same");
            Assert.AreEqual(merchant.ContactDetail.PostCode, merchantAdded.ContactDetail.PostCode, "Merchant contact detail PostCode is not same");
            Assert.AreEqual(merchant.ContactDetail.CountryCode, merchantAdded.ContactDetail.CountryCode, "Merchant contact detail CountryCode is not same");
            Assert.AreEqual(merchant.ContactDetail.Latitude, merchantAdded.ContactDetail.Latitude, "Merchant contact detail Latitude is not same");
            Assert.AreEqual(merchant.ContactDetail.Longitude, merchantAdded.ContactDetail.Longitude, "Merchant contact detail Longitude is not same");
            Assert.AreEqual(merchant.ContactDetail.LandlinePhone, merchantAdded.ContactDetail.LandlinePhone, "Merchant contact detail LandlinePhone is not same");
            Assert.AreEqual(merchant.ContactDetail.MobilePhone, merchantAdded.ContactDetail.MobilePhone, "Merchant contact detail MobilePhone is not same");
            Assert.AreEqual(merchant.ContactDetail.EmailAddress, merchantAdded.ContactDetail.EmailAddress, "Merchant contact detail EmailAddress is not same");
            Assert.AreEqual(merchant.ContactDetail.IsDeleted, merchantAdded.ContactDetail.IsDeleted, "Merchant contact detail IsDeleted is not same");



        }
    }
}