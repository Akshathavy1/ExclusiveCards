using ExclusiveCard.Data.Models;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests.Admin
{
    public class AdminCustomerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddUser()
        {
            string password = "Abcd@1234";
            //Create Identity user model
            ExclusiveUser user = Common.Common.BuildUserModel();
            //Add user
            bool result =
                await ServiceHelper.Instance.UserService.CreateAsync(user, password, Enums.Roles.User.ToString());

            //Get the added user
            ExclusiveUser storedUser = await ServiceHelper.Instance.UserService.FindByEmailAsync(user.Email);

            //Assertions
            Assert.IsNotNull(user, "Initialization of user model is null.");
            Assert.IsTrue(result, "Expected true.");
            Assert.IsNotNull(storedUser, "User not found.");
            Assert.AreEqual(user.Email, storedUser.Email, "Emails are not same.");
            Assert.AreEqual(user.UserName, storedUser.UserName, "Usernames are not same.");
            await ServiceHelper.Instance.UserService.DeleteAsync(storedUser);
        }

        // TODO: Remove obsolete customer tests
        //[Test]
        //public async Task AddUserAndCustomer()
        //{
        //    string password = "Abcd@1234";
        //    //Initialize user model
        //    ExclusiveUser user = Common.Common.BuildUserModel();

        //    //Add user
        //    bool result =
        //        await ServiceHelper.Instance.UserService.CreateAsync(user, password, Enums.Roles.User.ToString());

        //    //Get user back
        //    ExclusiveUser storedUser = await ServiceHelper.Instance.UserService.FindByEmailAsync(user.Email);
        //    //Create customer Model
        //    RequestModel.Customer customer = Common.Common.BuildCustomerModel();
        //    dto.Customer customerAdded = null;
        //    if (storedUser != null)
        //    {
        //        customer.AspNetUserId = storedUser.Id;
        //        //Add Customer
        //        customerAdded = await ServiceHelper.Instance.CustomerService.Add(customer);
        //    }

        //    //Assert user creation
        //    Assert.IsNotNull(user, "Initialization of user model is null.");
        //    Assert.IsTrue(result, "Expected true.");
        //    Assert.IsNotNull(storedUser, "User not found.");
        //    Assert.AreEqual(user.Email, storedUser.Email, "Emails are not same.");
        //    Assert.AreEqual(user.UserName, storedUser.UserName, "Usernames are not same.");

        //    //Assert Customer data
        //    Assert.IsNotNull(customer, "Initialize customer model is null.");
        //    Assert.IsNotNull(customerAdded, "Customer data not found.");
        //    Assert.AreEqual(customer.Title, customerAdded.Title, "Customer title is not same");
        //    Assert.AreEqual(customer.Forename, customerAdded.Forename, "Customer Forename is not same");
        //    Assert.AreEqual(customer.Surname, customerAdded.Surname, "Customer Surname is not same");
        //    Assert.AreEqual(customer.DateAdded, customerAdded.DateAdded, "Customer date added is not same");
        //    Assert.AreEqual(customer.DateOfBirth, customerAdded.DateOfBirth, "Customer Date of birth is not same");
        //    Assert.AreEqual(customer.IsActive, customerAdded.IsActive, "Customer is not Active");
        //    Assert.AreEqual(customer.IsDeleted, customerAdded.IsDeleted, "Customer is deleted");
        //    Assert.AreEqual(customer.MarketingNewsLetter, customerAdded.MarketingNewsLetter, "Customer MarketingNewsLetter is not same");
        //    Assert.AreEqual(customer.MarketingThirdParty, customerAdded.MarketingThirdParty, "Customer MarketingThirdParty is not same");

        //    Assert.IsNotNull(customer.ContactDetail, "Initial Contact details for the customer is null.");
        //    Assert.IsNotNull(customerAdded.ContactDetail, "Contact details for the customer is null.");
        //    Assert.AreEqual(customer.ContactDetail.Address1, customerAdded.ContactDetail.Address1, "Customer contact detail Address 1 is not same");
        //    Assert.AreEqual(customer.ContactDetail.Address2, customerAdded.ContactDetail.Address2, "Customer contact detail Address 2 is not same");
        //    Assert.AreEqual(customer.ContactDetail.Address3, customerAdded.ContactDetail.Address3, "Customer contact detail Address 3 is not same");
        //    Assert.AreEqual(customer.ContactDetail.Town, customerAdded.ContactDetail.Town, "Customer contact detail Town is not same");
        //    Assert.AreEqual(customer.ContactDetail.District, customerAdded.ContactDetail.District, "Customer contact detail District is not same");
        //    Assert.AreEqual(customer.ContactDetail.PostCode, customerAdded.ContactDetail.PostCode, "Customer contact detail PostCode is not same");
        //    Assert.AreEqual(customer.ContactDetail.CountryCode, customerAdded.ContactDetail.CountryCode, "Customer contact detail CountryCode is not same");
        //    Assert.AreEqual(customer.ContactDetail.Latitude, customerAdded.ContactDetail.Latitude, "Customer contact detail Latitude is not same");
        //    Assert.AreEqual(customer.ContactDetail.Longitude, customerAdded.ContactDetail.Longitude, "Customer contact detail Longitude is not same");
        //    Assert.AreEqual(customer.ContactDetail.LandlinePhone, customerAdded.ContactDetail.LandlinePhone, "Customer contact detail LandlinePhone is not same");
        //    Assert.AreEqual(customer.ContactDetail.MobilePhone, customerAdded.ContactDetail.MobilePhone, "Customer contact detail MobilePhone is not same");
        //    Assert.AreEqual(customer.ContactDetail.EmailAddress, customerAdded.ContactDetail.EmailAddress, "Customer contact detail EmailAddress is not same");
        //    Assert.AreEqual(customer.ContactDetail.IsDeleted, customerAdded.ContactDetail.IsDeleted, "Customer contact detail IsDeleted is not same");
        //    //Delete customer
        //    customer.Id = customerAdded.Id;
        //    customer.ContactDetailId = customerAdded.ContactDetailId;
        //    customer.ContactDetail.Id = customerAdded.ContactDetail.Id;
        //    await ServiceHelper.Instance.CustomerService.DeleteAsync(customer);
        //    //Delete user
        //    await ServiceHelper.Instance.UserService.DeleteAsync(storedUser);
        //}

        //Add Membership card, fetch using admin service and update using admin service
        //[Test]
        //public async Task AddMembershipCard()
        //{
        //    string password = "Abcd@1234";
        //    //Create Identity user model
        //    ExclusiveUser user = Common.Common.BuildUserModel();
        //    //Add user
        //    bool result =
        //        await ServiceHelper.Instance.UserService.CreateAsync(user, password, Enums.Roles.User.ToString());
        //    //Get the added user
        //    ExclusiveUser storedUser = await ServiceHelper.Instance.UserService.FindByEmailAsync(user.Email);
        //    //Create Cusotmer Model
        //    RequestModel.Customer customer = Common.Common.BuildCustomerModel();
        //    dto.Customer customerAdded = null;
        //    if (storedUser != null)
        //    {
        //        customer.AspNetUserId = storedUser.Id;
        //        //Add customer
        //        customerAdded = await ServiceHelper.Instance.CustomerService.Add(customer);
        //    }
        //    //Create membership card model
        //    dto.MembershipCard membershipCard = Common.Common.BuildMembershipCardModel();
        //    dto.MembershipCard membershipCardAdded = null;
        //    if (membershipCard != null)
        //    {
        //        membershipCard.CustomerId = customerAdded?.Id;
        //        //Add membership card
        //        membershipCardAdded =
        //            await ServiceHelper.Instance.PMembershipCardService.Add(membershipCard, null, null, null);
        //    }

        //    //Get membership Card by Admin membership card service
        //    var membershipCardExisting =
        //        await ServiceHelper.Instance.MembershipCardService.GetMembershipCardsForCustomerAsync(customerAdded.Id,
        //            true);

        //    //update membership card by admin membership card Service
        //    var physicalCardStatus =
        //        await ServiceHelper.Instance.MembershipCardService.UpdatePhysicalCardDetailsAsync(
        //            membershipCardAdded.Id, true, 12);

        //    //Assert user creation
        //    Assert.IsNotNull(user, "Initialization of user model is null.");
        //    Assert.IsTrue(result, "Expected true.");
        //    Assert.IsNotNull(storedUser, "User not found.");

        //    //Assert Customer data
        //    Assert.IsNotNull(customer, "Initialize customer model is null.");
        //    Assert.IsNotNull(customerAdded, "Customer data not found.");

        //    //Assert Customer Membership Card
        //    Assert.IsNotNull(membershipCard, "Intial model of membership card is null.");
        //    Assert.IsNotNull(membershipCardAdded, "Failed to find membership card.");
        //    Assert.AreEqual(membershipCard.IsActive, membershipCardAdded.IsActive, "Membership Card IsActive differs.");
        //    Assert.AreEqual(membershipCard.IsDeleted, membershipCardAdded.IsDeleted, "Membership Card IsDeleted differs.");
        //    Assert.AreEqual(membershipCard.StatusId, membershipCardAdded.StatusId, "Membership Card Status differs.");
        //    Assert.AreEqual(membershipCard.PhysicalCardStatusId, membershipCardAdded.PhysicalCardStatusId, "Membership Card PhysicalCardStatus differs.");
        //    Assert.AreEqual(membershipCard.PhysicalCardRequested, membershipCardAdded.PhysicalCardRequested, "Membership Card PhysicalCardRequested differs.");
        //    Assert.AreEqual(membershipCard.CustomerId, membershipCardAdded.CustomerId, "Membership Card Customer Id differs.");
        //    Assert.AreEqual(membershipCard.CardNumber, membershipCardAdded.CardNumber, "Membership Card Card Number differs.");
        //    Assert.AreEqual(membershipCard.DateIssued, membershipCardAdded.DateIssued, "Membership Card DateIssued differs.");
        //    Assert.AreEqual(membershipCard.ValidFrom, membershipCardAdded.ValidFrom, "Membership Card Valid from date differs.");
        //    Assert.AreEqual(membershipCard.ValidTo, membershipCardAdded.ValidTo, "Membership Card Valid to date differs.");
        //    Assert.AreEqual(membershipCard.MembershipPlanId, membershipCardAdded.MembershipPlanId, "Membership Card Membership plan differs.");

        //    //Assert Customer Membership Card Admin service
        //    Assert.IsNotNull(membershipCardExisting, "Membership card details list from Admin service is null.");
        //    dto.MembershipCard card = membershipCardExisting.FirstOrDefault(x => x.Id == membershipCardAdded.Id);

        //    Assert.IsNotNull(card, "List doesn't contain any card");
        //    Assert.AreEqual(card.IsActive, membershipCardAdded.IsActive, "Membership Card from Admin service, IsActive differs.");
        //    Assert.AreEqual(card.IsDeleted, membershipCardAdded.IsDeleted, "Membership Card from Admin service, IsDeleted differs.");
        //    Assert.AreEqual(card.StatusId, membershipCardAdded.StatusId, "Membership Card from Admin service, Status differs.");
        //    Assert.AreEqual(card.PhysicalCardStatusId, membershipCardAdded.PhysicalCardStatusId, "Membership Card from Admin service, PhysicalCardStatus differs.");
        //    Assert.AreEqual(card.PhysicalCardRequested, membershipCardAdded.PhysicalCardRequested, "Membership Card from Admin service, PhysicalCardRequested differs.");
        //    Assert.AreEqual(card.CustomerId, membershipCardAdded.CustomerId, "Membership Card from Admin service, Customer Id differs.");
        //    Assert.AreEqual(card.CardNumber, membershipCardAdded.CardNumber, "Membership Card from Admin service, Card Number differs.");
        //    Assert.AreEqual(card.DateIssued, membershipCardAdded.DateIssued, "Membership Card from Admin service, DateIssued differs.");
        //    Assert.AreEqual(card.ValidFrom, membershipCardAdded.ValidFrom, "Membership Card from Admin service, Valid from date differs.");
        //    Assert.AreEqual(card.ValidTo, membershipCardAdded.ValidTo, "Membership Card from Admin service, Valid to date differs.");
        //    Assert.AreEqual(card.MembershipPlanId, membershipCardAdded.MembershipPlanId, "Membership Card from Admin service, Membership plan differs.");

        //    //Assert Customer Membership Card Admin update physical card service
        //    Assert.IsNotNull(physicalCardStatus, "Upate Membership Card Physical card status is null.");
        //    Assert.AreNotSame(card.PhysicalCardStatusId, physicalCardStatus.PhysicalCardStatusId, "Physical card status is same.");

        //    //Delete Membership Card
        //    membershipCard.Id = membershipCardAdded.Id;
        //    await ServiceHelper.Instance.MembershipCardService.DeleteAsync(membershipCard);

        //    //Delete customer
        //    customer.Id = customerAdded.Id;
        //    customer.ContactDetailId = customerAdded.ContactDetailId;
        //    customer.ContactDetail.Id = customerAdded.ContactDetail.Id;
        //    await ServiceHelper.Instance.CustomerService.DeleteAsync(customer);
        //    //Delete user
        //    await ServiceHelper.Instance.UserService.DeleteAsync(storedUser);
        //}

        [Test]
        public async Task GetMembershipCardToSendOut()
        {
            var a = await ServiceHelper.Instance.MembershipCardService.GetCardsToSendOutAsync();
        }

        [Test]
        public void EnsureGetAllPagedIsReturn()
        {
            var customerPagedResult = ServiceHelper.Instance.AdminCustomerService.GetAllPagedSearch(1, 20);
            Assert.IsNotNull(customerPagedResult);
            Assert.IsTrue(customerPagedResult.Results.Count <= 20);
        }

        [Test]
        public void EnsureGetPagedSearchIsValid()
        {
            var customerPagedSearch =
                ServiceHelper.Instance.AdminCustomerService.GetPagedSearch(null, null, null, null, null, null, null, null, null, 1, 20);
            Assert.IsTrue(customerPagedSearch.Results.Count <= 20);
        }

        //TODO: Refactor updateusername test to use new customer account creation
        //[Test]
        //public async Task EnsureUpdateUsernameUpdatesEmail()
        //{
        //    string password = "Abcd@1234";
        //    //Initialize user model
        //    ExclusiveUser user = Common.Common.BuildUserModel();

        //    //Add user
        //    bool result =
        //        await ServiceHelper.Instance.UserService.CreateAsync(user, password, Enums.Roles.User.ToString());

        //    //Get user back
        //    ExclusiveUser storedUser = await ServiceHelper.Instance.UserService.FindByEmailAsync(user.Email);
        //    //Create customer Model
        //    RequestModel.Customer customer = Common.Common.BuildCustomerModel();
        //    dto.Customer customerAdded = null;
        //    if (storedUser != null)
        //    {
        //        customer.AspNetUserId = storedUser.Id;
        //        //Add Customer
        //        customerAdded = await ServiceHelper.Instance.CustomerService.Add(customer);
        //    }

        //    //Get the user back
        //    ExclusiveUser editedUser = await ServiceHelper.Instance.UserService.FindByEmailAsync(user.Email);

        //    //Assert user creation
        //    Assert.IsNotNull(user, "Initialization of user model is null.");
        //    Assert.IsTrue(result, "Expected true.");
        //    Assert.IsNotNull(storedUser, "User not found.");
        //    Assert.AreEqual(user.Email, storedUser.Email, "Emails are not same.");
        //    Assert.AreEqual(user.UserName, storedUser.UserName, "Usernames are not same.");

        //    //Assert Customer data
        //    Assert.IsNotNull(customer, "Initialize customer model is null.");
        //    Assert.IsNotNull(customerAdded, "Customer data not found.");
        //    Assert.AreEqual(customer.Title, customerAdded.Title, "Customer title is not same");
        //    Assert.AreEqual(customer.Forename, customerAdded.Forename, "Customer Forename is not same");
        //    Assert.AreEqual(customer.Surname, customerAdded.Surname, "Customer Surname is not same");
        //    Assert.AreEqual(customer.DateAdded, customerAdded.DateAdded, "Customer date added is not same");
        //    Assert.AreEqual(customer.DateOfBirth, customerAdded.DateOfBirth, "Customer Date of birth is not same");
        //    Assert.AreEqual(customer.IsActive, customerAdded.IsActive, "Customer is not Active");
        //    Assert.AreEqual(customer.IsDeleted, customerAdded.IsDeleted, "Customer is deleted");
        //    Assert.AreEqual(customer.MarketingNewsLetter, customerAdded.MarketingNewsLetter, "Customer MarketingNewsLetter is not same");
        //    Assert.AreEqual(customer.MarketingThirdParty, customerAdded.MarketingThirdParty, "Customer MarketingThirdParty is not same");

        //    Assert.IsNotNull(customer.ContactDetail, "Initial Contact details for the customer is null.");
        //    Assert.IsNotNull(customerAdded.ContactDetail, "Contact details for the customer is null.");
        //    Assert.AreEqual(customer.ContactDetail.Address1, customerAdded.ContactDetail.Address1, "Customer contact detail Address 1 is not same");
        //    Assert.AreEqual(customer.ContactDetail.Address2, customerAdded.ContactDetail.Address2, "Customer contact detail Address 2 is not same");
        //    Assert.AreEqual(customer.ContactDetail.Address3, customerAdded.ContactDetail.Address3, "Customer contact detail Address 3 is not same");
        //    Assert.AreEqual(customer.ContactDetail.Town, customerAdded.ContactDetail.Town, "Customer contact detail Town is not same");
        //    Assert.AreEqual(customer.ContactDetail.District, customerAdded.ContactDetail.District, "Customer contact detail District is not same");
        //    Assert.AreEqual(customer.ContactDetail.PostCode, customerAdded.ContactDetail.PostCode, "Customer contact detail PostCode is not same");
        //    Assert.AreEqual(customer.ContactDetail.CountryCode, customerAdded.ContactDetail.CountryCode, "Customer contact detail CountryCode is not same");
        //    Assert.AreEqual(customer.ContactDetail.Latitude, customerAdded.ContactDetail.Latitude, "Customer contact detail Latitude is not same");
        //    Assert.AreEqual(customer.ContactDetail.Longitude, customerAdded.ContactDetail.Longitude, "Customer contact detail Longitude is not same");
        //    Assert.AreEqual(customer.ContactDetail.LandlinePhone, customerAdded.ContactDetail.LandlinePhone, "Customer contact detail LandlinePhone is not same");
        //    Assert.AreEqual(customer.ContactDetail.MobilePhone, customerAdded.ContactDetail.MobilePhone, "Customer contact detail MobilePhone is not same");
        //    Assert.AreEqual(customer.ContactDetail.EmailAddress, customerAdded.ContactDetail.EmailAddress, "Customer contact detail EmailAddress is not same");
        //    Assert.AreEqual(customer.ContactDetail.IsDeleted, customerAdded.ContactDetail.IsDeleted, "Customer contact detail IsDeleted is not same");

        //    //Edit Identity user and edit username
        //    ExclusiveUser editUserReq = storedUser;
        //    dto.ContactDetail contactReq = Common.Common.MapContactDetailToReq(customerAdded?.ContactDetail);
        //    dto.ContactDetail contactEdited = null;
        //    if (editUserReq != null)
        //    {
        //        editUserReq.UserName = "test11@email.com";
        //        editUserReq.Email = editUserReq.UserName;
        //        bool userEditResult = await ServiceHelper.Instance.UserService.UpdateUserAsync(editUserReq);
        //        if (contactReq != null)
        //        {
        //            contactReq.EmailAddress = editUserReq.UserName;
        //        }
        //        contactEdited = await ServiceHelper.Instance.ContactDetailService.Update(contactReq);
        //    }


        //    Assert.IsNotNull(editedUser, "Edited User is null");
        //    Assert.AreEqual(editedUser.UserName, editUserReq.UserName, "Username is not same as edited");
        //    Assert.AreEqual(editedUser.Email, editUserReq.Email, "Email is not same as edited");

        //    Assert.IsNotNull(contactEdited, "Edited contact detail is null");
        //    Assert.AreNotEqual(customer?.ContactDetail?.EmailAddress, contactEdited?.EmailAddress, "Edited email is as before");
        //    Assert.AreEqual(contactReq?.EmailAddress, contactEdited?.EmailAddress, "Edited email is not same for Contact detail");

        //    //Delete customer
        //    customer.Id = customerAdded.Id;
        //    customer.ContactDetailId = customerAdded.ContactDetailId;
        //    customer.ContactDetail.Id = customerAdded.ContactDetail.Id;
        //    await ServiceHelper.Instance.CustomerService.DeleteAsync(customer);
        //    //Delete user
        //    await ServiceHelper.Instance.UserService.DeleteAsync(storedUser);
        //}

        //[Test]
        //public async Task Logintest()
        //{
        //    List<dto.Customer> customers=new List<dto.Customer>(); 
        //    string password = "Abcd@1234";
        //    for (int i = 0; i < 20; i++)
        //    {
        //        //Initialize user model
        //        ExclusiveUser user = Common.Common.BuildUserModel();

        //        //Add user
        //        bool result =
        //            await ServiceHelper.Instance.UserService.CreateAsync(user, password, Enums.Roles.User.ToString());

        //        //Get user back
        //        ExclusiveUser storedUser = await ServiceHelper.Instance.UserService.FindByEmailAsync(user.Email);
        //        //Create customer Model
        //        RequestModel.Customer customer = Common.Common.BuildCustomerModel();
        //        if (storedUser != null)
        //        {
        //            customer.AspNetUserId = storedUser.Id;
        //            //Add Customer
        //            dto.Customer customerAdded = await ServiceHelper.Instance.CustomerService.Add(customer);
        //            customers.Add(customerAdded);
        //        }
        //    }

        //    for (int k = 0; k < 100000; k++)
        //    {
        //        for (int j = 0; j < 20; j++)
        //        {
        //            var result = await ServiceHelper.Instance.UserAccountService.Login(
        //                customers[j].ContactDetail.EmailAddress,
        //                password);
        //            Assert.IsTrue(result != null);
        //        }
        //    }
        //}
    }
}
