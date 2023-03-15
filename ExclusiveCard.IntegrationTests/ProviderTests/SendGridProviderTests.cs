using ExclusiveCard.Enums;
using ExclusiveCard.Providers.Marketing.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests
{
    public class SendGridProviderTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task CheckCustomFieldDefinitiom()
        {
            var requestModel = new CustomFieldRequestModel
            {
                Name = "color_mode",
                FiledType = CustomFieldType.Text.ToString()
            };

            var response = await ServiceHelper.Instance.SendGrid.CreateCustomFields(requestModel);

            Assert.IsNotNull(response, "response is null");
        }

        [Test]
        public async Task GetCustomFieldDefinitiom()
        {
            var response = await ServiceHelper.Instance.SendGrid.GetCustomFields();
            Assert.IsNotNull(response, "response is null");
        }

        //[Test]
        //public async Task CheckCreateSender()
        //{
        //    var response = await ServiceHelper.Instance.EmailProvider.CreateSenders();

        //    var result = response.ToString();
        //    Assert.IsNotNull(result, "result is null");
        //    Assert.IsNotEmpty(result, "result is empty");
        //    Assert.IsTrue(result.Contains("success"), "failure in creating senders");
        //}

        [Test]
        public async Task GetSenders()
        {
            var response = await ServiceHelper.Instance.SendGrid.GetSenders();

            Assert.IsNotNull(response, "result is null");
            Assert.IsNotEmpty(response, "result is empty");
        }

        [Test]
        public async Task TestCreateMarketingLists()
        {
            var list = new MarketingList
            {
                Name = $"{Guid.NewGuid().ToString().Substring(0,8)}{DateTime.UtcNow.Millisecond}"
            };

            var response = await ServiceHelper.Instance.SendGrid.CreateLists(list);

            Assert.IsNotNull(response, "result is null");
            Assert.IsTrue(response.errors.Count == 0, "Error");
        }

        [Test]
        public async Task TestGetMarketingLists()
        {
            var response = await ServiceHelper.Instance.SendGrid.GetLists();

            Assert.IsNotNull(response, "result is null");
            Assert.IsTrue(response.marketingListResponse.Count > 0, "No records");
        }

        [Test]
        public async Task TestGetMarketingListById()
        {
            var list = new MarketingList
            {
                Name = $"{Guid.NewGuid().ToString().Substring(0, 8)}{DateTime.UtcNow.Millisecond}"
            };

            var newlist = await ServiceHelper.Instance.SendGrid.CreateLists(list);

            Assert.IsNotNull(newlist, "Unable to create marketing list");
            Assert.IsTrue(newlist.errors.Count == 0, $"Error when creating marketing list {newlist.errors[0].Message}");

            string id = newlist.Id; //"29304879-8992-483e-bcc8-79eb529d98c3";
            
            var response = await ServiceHelper.Instance.SendGrid.GetListById(id);

            Assert.IsNotNull(newlist, "marke not found");
            Assert.IsTrue(response.errors.Count == 0, $"Error returning marketing list {response.errors[0].Message}");
        }

        [Test]
        public async Task TestCreateContact()
        {
            List<MarketingContact> customerList = new List<MarketingContact>();
            MarketingContact customer = new MarketingContact()
            {
                Email = "Mack1234@sharklasers.com",
                FirstName = "MackBS",
            };

            customerList.Add(customer);

            CreateContactsRequest testmodel = new CreateContactsRequest();
            testmodel.Contacts = customerList;
            var res = await ServiceHelper.Instance.SendGrid.CreateContact(testmodel);
            Assert.IsNotNull(res, "Contact not created");
            Assert.IsTrue(res.errors.Count == 0, $"Error creating contact {res.errors[0].Message}");
        }

        [Test]
        public async Task TestSearchContact()
        {
            List<MarketingContact> searchContact = new List<MarketingContact>()
            {
                new MarketingContact { Email ="test24021@sharklasers.com", FirstName = "test24021" },
                new MarketingContact { Email ="test24022@sharklasers.com", FirstName = "test24022" },
                new MarketingContact { Email ="test24023@sharklasers.com", FirstName = "test24023" }
            };

            var response = await ServiceHelper.Instance.SendGrid.SearchContact(searchContact);
        }

        [Test]
        public async Task TestDeleteContact()
        {
            var deleteGridContactlist = new List<MarketingContactId>()
            {
                new MarketingContactId {Id = "95c12ae1-ac13-4011-a490-5789c0a0aff2"},
                new MarketingContactId {Id = "cfa4480e-9eeb-4c1c-affa-452da9cee0bd"}
            };

            var res = await ServiceHelper.Instance.SendGrid.DeleteContact(deleteGridContactlist);
        }
    }
}
