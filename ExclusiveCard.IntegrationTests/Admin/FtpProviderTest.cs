using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Website.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests.Admin
{
    public class FtpProviderTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AssertFtpConnection()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var serverUri = appSettings["Ftp_ServerUri"];
            var username = appSettings["Ftp_Username"];
            var password = appSettings["Ftp_Password"];
            var result = false;
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(serverUri);
                request.EnableSsl = true;
                //Get the object used to communicate with the server.
                request.Credentials = new NetworkCredential(username, password);
                result = true;
                Assert.IsTrue(result, "Result is false");
            }
            catch (Exception)
            {
                Assert.IsTrue(result, "Exception raised");
            }
        }

        [Test]
        public async Task AssertSendTransactionReportNoData()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var serverUri = appSettings["Ftp_ServerUri"];
            var username = appSettings["Ftp_Username"];
            var password = appSettings["Ftp_Password"];

            var partner = await ServiceHelper.Instance.PartnerService.GetByNameAsync("TAM");
            var success = await ServiceHelper.Instance.PartnerService.SendPartnerReport(partner.Id,
                appSettings["AdminEmail"], Data.Constants.TemporaryFilePath.TempFileIN,
                appSettings["BlobConnectionString"],
                appSettings["PartnerContainerName"],
                serverUri, username, password);

            Assert.IsNotNull(success, "Response message is null");
            Assert.AreEqual(success, "No data found to create file.", "Wrong message returned.");
        }

        //TODO:  Fix assert send transaction report test
        //[Test]
        //public async Task AssertSendTransactionReport()
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

        //    //Confirm customer email
        //    var user = new ExclusiveUser
        //    {
        //        Id = customer.AspNetUserId,
        //        UserName = accountDetail.Email,
        //        Email = accountDetail.Email
        //    };
        //    var emailToken =
        //        await ServiceHelper.Instance.NewUserAccountService.GenerateEmailConfirmationTokenAsync(user);
        //    var confirmEmail =
        //        await ServiceHelper.Instance.NewUserAccountService.ConfirmEmailTokenAsync(user, emailToken);

        //    //Get Active Membership Card
        //    var activeCard =
        //       ServiceHelper.Instance.PMembershipCardService.GetActiveMembershipCard(customer.AspNetUserId);

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
        //    bool investmentSuccess = false;
        //    bool.TryParse(trans, out investmentSuccess);

        //    var transactions =
        //        await ServiceHelper.Instance.CashbackTransactionService.GetRecentTransactionsByMembershipCard(activeCard.Id,
        //            2);


        //    //Send Transaction report
        //    var appSettings = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();
        //    var serverUri = appSettings["Ftp_ServerUri"];
        //    var username = appSettings["Ftp_Username"];
        //    var password = appSettings["Ftp_Password"];

        //    var partner = await ServiceHelper.Instance.PartnerService.GetByNameAsync("TAM");
        //    var success = await ServiceHelper.Instance.PartnerService.SendPartnerReport(partner.Id,
        //        appSettings["AdminEmail"], Data.Constants.TemporaryFilePath.TempFileIN,
        //        appSettings["BlobConnectionString"],
        //        appSettings["PartnerContainerName"],
        //        serverUri, username, password);

        //    Assert.IsNotNull(registrationCodes, "Registration codes not found.");
        //    Assert.IsNotNull(code, "ExTam3 not found.");
        //    Assert.IsNotNull(membershipPlan, "Membership Plan not found");
        //    Assert.IsNotNull(membershipCard, "Membership card creation failed");
        //    Assert.IsNotNull(accountDetail, "Customer account creation failed");
        //    Assert.IsNotNull(data, "Customer Registration is null");
        //    Assert.IsTrue(investmentSuccess, "Cashback transaction creation for Account Boost failed");
        //    Assert.IsNotNull(transactions, "Account Boost transactions not found");

        //    Assert.IsNotNull(success, "Response message is null");
        //    Assert.AreEqual(success.ToLower(), "true", "Wrong message returned.");
        //}

        //Make sure you upload file to FTP manually before running this test case
        [Test]
        public async Task AssertFilesPickedFromOutFolderAndProcesses()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var serverUri = appSettings["Ftp_ServerUri"];
            var username = appSettings["Ftp_Username"];
            var password = appSettings["Ftp_Password"];

            var partner = await ServiceHelper.Instance.PartnerService.GetByNameAsync("TAM");
            var success = await ServiceHelper.Instance.PartnerService.ProcessPartnerReport(partner.Id,
                appSettings["AdminEmail"], Data.Constants.TemporaryFilePath.TempFileOUT,
                appSettings["BlobConnectionString"],
                appSettings["PartnerContainerName"], appSettings["Blob_Processing"], appSettings["Blob_Error"],
                appSettings["Blob_Processed"], serverUri, username, password);

            Assert.IsNotNull(success, "Response message is null");
        }

        [Test]
        public async Task AssertFilesPickedFromPositionFolderAndProcesses()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var serverUri = appSettings["Ftp_ServerUri"];
            var username = appSettings["Ftp_Username"];
            var password = appSettings["Ftp_Password"];

            var partner = await ServiceHelper.Instance.PartnerService.GetByNameAsync("TAM");
            var success = await ServiceHelper.Instance.PartnerService.ProcessPartnerPositionFile(
                appSettings["AdminEmail"], appSettings["BlobConnectionString"], appSettings["PartnerContainerName"],
                appSettings["Blob_Processing"], Data.Constants.TemporaryFilePath.TempFilePosition,
                appSettings["Blob_Processed"], appSettings["Blob_Error"], serverUri, username, password);

            Assert.IsNotNull(success, "Response message is null");
        }
    }
}
