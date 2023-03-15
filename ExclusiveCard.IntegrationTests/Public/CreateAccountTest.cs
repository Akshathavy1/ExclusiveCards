using ExclusiveCard.Enums;
using ExclusiveCard.Website.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests.Public
{
    public class CreateAccountTest
    {
        [SetUp]
        public void Setup()
        {
        }

      

        
        //TODO: fix assert investment test
        //[Test]
        //public async Task AssertInvestment()
        //{
        //    //Get all status
        //    var status = await ServiceHelper.Instance.StatusService.GetAll();

        //    //Validate Registration Code ExTam3
        //    var regCode = "ExTam3";
        //    var registrationCodes = await ServiceHelper.Instance.MembershipRegistrationCodeService.GetAllAsync();
        //    var code = registrationCodes?.FirstOrDefault(x => x.RegistartionCode == regCode);
        //    var membershipPlan = new Services.Models.DTOs.MembershipPlan();
        //    var membershipCard = new Services.Models.DTOs.MembershipCard();
        //    var accountDetail = new Services.Models.DTOs.UserAccountDetails();

        //    //Validate registration code and get user Token to create user
        //    var userToken = await ServiceHelper.Instance.UserAccountService.ValidateRegistrationCode(regCode);
        //    if (userToken != null && userToken.Token != Guid.Empty)
        //    {
        //        //Get Membership Plan using userToken
        //        //Membership Personal Complimentary - v1
        //        int? complimentaryPlan = null;
        //        int? registrationCodeId = null;
        //        var membershipDetails = await ServiceHelper.Instance.MembershipService.Get(new Guid(userToken.Token.ToString()));
        //        if (membershipDetails?.MembershipRegistrationCode != null && membershipDetails.MembershipRegistrationCode.MembershipPlanId > 0)
        //        {
        //            complimentaryPlan = membershipDetails.MembershipRegistrationCode.MembershipPlanId;
        //            registrationCodeId = membershipDetails.MembershipRegistrationCodeId;
        //            membershipPlan = await ServiceHelper.Instance.MembershipPlanService.Get(
        //                membershipDetails.MembershipRegistrationCode.MembershipPlanId, true);
        //        }

        //        //Create User Account
        //        var accountDetails = new Services.Models.DTOs.UserAccountDetails
        //        {
        //            Title = "Mr",
        //            Forename = "Winston",
        //            Surname = "Peter",
        //            Email = "win.peter2007@gmail.com",
        //            DateOfBirth = Convert.ToDateTime("23/01/1990"),
        //            Password = "Abcd@1234",
        //            PostCode = "BT5 6YT",
        //            SecurityQuestionId = 1,
        //            SecurityAnswer = "abcd",
        //            Town = "Blackburn",
        //            Country = "UK",
        //            County = null
        //        };

        //        string appUrl = ServiceHelper.Instance.Settings.Value.AppUrl;
        //        var token = await ServiceHelper.Instance.UserAccountService.CreateAccount(accountDetails, appUrl);
        //        if (token?.Id > 0)
        //        {
        //            accountDetail.Id = (int)token.Id;
        //            if (userToken.Token != Guid.Empty)
        //            {
        //                await ServiceHelper.Instance.MembershipService.Delete(userToken.Token);
        //            }

        //            //Doesn't want to wait till this completes
        //            await ServiceHelper.Instance.AccountService.BuyMembershipCardWithCashback(membershipPlan, membershipCard,
        //                accountDetail.Id, membershipPlan.Id, true, null, "GB", complimentaryPlan,
        //                registrationCodeId, EmailTemplateType.WelcomeEmail);

        //            //if PaidByEmployer is True Create Diamond MembershipCard
        //            if (membershipPlan.PaidByEmployer && membershipPlan.PartnerId.HasValue)
        //            {
        //                await ServiceHelper.Instance.AccountService.CreateDiamondMembership(membershipCard, (int)membershipPlan.PartnerId);
        //            }
        //        }
        //    }

        //    var customer = ServiceHelper.Instance.CustomerService.GetDetails(accountDetail.Id);

        //    //Get Active Membership Card
        //    var activeCard =
        //        ServiceHelper.Instance.PMembershipCardService.GetActiveMembershipCard(customer.AspNetUserId);

        //    //create Customer Registration
        //    PlanUpgradeViewModel model = new PlanUpgradeViewModel()
        //    {
        //        MembershipPlanId = activeCard.MembershipPlanId
        //    };

        //    var registration = new Services.Models.RequestModels.StagingModels.CustomerRegistration
        //    {
        //        Data = model.ToJson(),
        //        AspNetUserId = customer.AspNetUserId,
        //        CustomerPaymentId = Guid.NewGuid(),
        //        StatusId = (int)status?.FirstOrDefault(x => x.IsActive && x.Type == Data.Constants.StatusType.CustomerCreation && x.Name == Data.Constants.Status.New)?.Id
        //    };
        //    var data = await ServiceHelper.Instance.StagingCustomerRegistrationService.AddAsync(registration);

        //    //Create Cashback Transactions for Invested
        //    var trans = await ServiceHelper.Instance.AccountService.CreateInvestment(registration.CustomerPaymentId.ToString(),
        //        "win.peter2007@gmail.com");
        //    bool success = false;
        //    bool.TryParse(trans, out success);

        //    var transactions =
        //        await ServiceHelper.Instance.CashbackTransactionService.GetRecentTransactionsByMembershipCard(activeCard.Id,
        //            2);

        //    Assert.IsNotNull(registrationCodes, "Registration codes not found.");
        //    Assert.IsNotNull(code, "ExTam3 not found.");
        //    Assert.IsNotNull(membershipPlan, "Membership Plan not found");
        //    Assert.IsNotNull(membershipCard, "Membership card creation failed");
        //    Assert.IsNotNull(accountDetail, "Customer account creation failed");
        //    Assert.IsNotNull(data, "Customer Registration is null");
        //    Assert.IsTrue(success, "Cashback transaction creation for Account Boost failed");
        //    Assert.IsNotNull(transactions, "Account Boost transactions not found");
        //}
    }
}
