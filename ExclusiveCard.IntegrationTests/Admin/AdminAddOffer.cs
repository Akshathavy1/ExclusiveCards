using NUnit.Framework;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;



namespace ExclusiveCard.IntegrationTests.Admin
{
    class AdminAddOffer
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddOffer()
        {
            dto.Offer offer = Common.Common.BuildOfferModel();
            dto.Offer offerAdded = await ServiceHelper.Instance.OfferService.Add(offer);
            Assert.IsNotNull(offer, "Initialize customer model is null.");
            Assert.IsNotNull(offerAdded, "Customer data not found.");
            Assert.IsTrue(offerAdded.Id > 0, "Merchant not created");
            Assert.AreEqual(offer.ValidFrom, offerAdded.ValidFrom, "Valid From is not same");
            Assert.AreEqual(offer.ValidTo, offerAdded.ValidTo, "Valid To is not same");
            Assert.AreEqual(offer.ShortDescription, offerAdded.ShortDescription, "Offer shortdescription is not same");
            Assert.AreEqual(offer.LongDescription, offerAdded.LongDescription, "Offer longdescription is not same");
            Assert.AreEqual(offer.Terms, offerAdded.Terms, "Offer terms are not same");
            Assert.AreEqual(offer.LinkUrl, offerAdded.LinkUrl, "Offer Linkurl is not same");
            Assert.AreEqual(offer.OfferCode, offerAdded.OfferCode, "OfferCode is not same");

        }
    }
}
