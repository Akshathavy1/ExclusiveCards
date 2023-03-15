using NUnit.Framework;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.IntegrationTests.Admin
{
    class AdminUpdtaeMerchant
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task UpdateMerchant()
        {
            dto.Merchant merchant = Common.Common.BuildMerchantModel();
            dto.Merchant merchantupdated = await ServiceHelper.Instance.MerchantService.Update(merchant);
            Assert.IsNotNull(merchant, "Initialize customer model is null.");
            Assert.IsNotNull(merchantupdated, "Customer data not found.");
            Assert.IsTrue(merchantupdated.Id > 0, "Merchant not updated");
            Assert.AreEqual(merchant.Name, merchantupdated.Name, "Merchant Name is not same");
            Assert.AreEqual(merchant.ContactName, merchantupdated.ContactName, "Merchnat ContactName is not same");
            Assert.AreEqual(merchant.ShortDescription, merchantupdated.ShortDescription,
                "Merchnat shortdescription is not same");
            Assert.AreEqual(merchant.LongDescription, merchantupdated.LongDescription,
                "Merchnat longdescription is not same");
            Assert.AreEqual(merchant.Terms, merchantupdated.Terms, "Merchant terms are not same");
            Assert.AreEqual(merchant.WebsiteUrl, merchantupdated.WebsiteUrl, "Merchant websiteurl is not same");
            Assert.AreEqual(merchant.IsDeleted, merchantupdated.IsDeleted, "merchant is deleted");


            Assert.IsNotNull(merchant.ContactDetail, "Initial Contact details for the customer is null.");
            Assert.IsNotNull(merchantupdated.ContactDetail, "Contact details for the customer is null.");
            Assert.AreEqual(merchant.ContactDetail.Address1, merchantupdated.ContactDetail.Address1,
                "Merchant contact detail Address 1 is not same");
            Assert.AreEqual(merchant.ContactDetail.Address2, merchantupdated.ContactDetail.Address2,
                "Merchant contact detail Address 2 is not same");
            Assert.AreEqual(merchant.ContactDetail.Address3, merchantupdated.ContactDetail.Address3,
                "Merchant contact detail Address 3 is not same");
            Assert.AreEqual(merchant.ContactDetail.Town, merchantupdated.ContactDetail.Town,
                "Merchant contact detail Town is not same");
            Assert.AreEqual(merchant.ContactDetail.District, merchantupdated.ContactDetail.District,
                "Merchant contact detail District is not same");
            Assert.AreEqual(merchant.ContactDetail.PostCode, merchantupdated.ContactDetail.PostCode,
                "Merchant contact detail PostCode is not same");
            Assert.AreEqual(merchant.ContactDetail.CountryCode, merchantupdated.ContactDetail.CountryCode,
                "Merchant contact detail CountryCode is not same");
            Assert.AreEqual(merchant.ContactDetail.Latitude, merchantupdated.ContactDetail.Latitude,
                "Merchant contact detail Latitude is not same");
            Assert.AreEqual(merchant.ContactDetail.Longitude, merchantupdated.ContactDetail.Longitude,
                "Merchant contact detail Longitude is not same");
            Assert.AreEqual(merchant.ContactDetail.LandlinePhone, merchantupdated.ContactDetail.LandlinePhone,
                "Merchant contact detail LandlinePhone is not same");
            Assert.AreEqual(merchant.ContactDetail.MobilePhone, merchantupdated.ContactDetail.MobilePhone,
                "Merchant contact detail MobilePhone is not same");
            Assert.AreEqual(merchant.ContactDetail.EmailAddress, merchantupdated.ContactDetail.EmailAddress,
                "Merchant contact detail EmailAddress is not same");
            Assert.AreEqual(merchant.ContactDetail.IsDeleted, merchantupdated.ContactDetail.IsDeleted,
                "Merchant contact detail IsDeleted is not same");
        }
    }
}
