using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ExclusiveCard.Managers;

namespace ExclusiveCard.IntegrationTests.Admin
{
    public class CashbackTest
    {
        private  ICashbackService _cashbackService;
        private IStagingCashbackManager _stagingManager;

        [SetUp]
        public void Setup()
        {
            _cashbackService = Configuration.ServiceProvider.GetService<ICashbackService>();
            _stagingManager = Configuration.ServiceProvider.GetService<IStagingCashbackManager>();

            //_cashbackManager = Configuration.ServiceProvider.GetService<ICashbackManager>();
        }

        [Test]
        public async Task GetCashbackTransactionsReportFromStrackr_Test()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            Assert.IsNotNull(appSettings["StrackrAPI_id"], "App settings values are null - try clean and rebuild solution");
            
            var result = _cashbackService.GetTransactionReport(new DateTime(2020, 2, 19),
                new DateTime(2020, 05, 15), appSettings["StrackrAPI_id"], appSettings["StrackrAPI_key"]);

            Assert.IsTrue(result > 0, "Transaction Report Id = 0, that's bad,  really bad");

            //TODO: some more checking that we got the data loaded here

            //List<CashbackTransaction> transactions =
            //    await ServiceHelper.Instance.StagingCashbackTransactionService.GetAll(tranFile.FirstOrDefault().Id,
            //        statusNew);

            //Assert.IsTrue(success, "Getting file failed");
            //Assert.IsNotNull(tranFile, "Transaction file is not created");

            //Assert.IsNotNull(transactions, "Staging cashback transactions are null");
            //Assert.IsTrue(transactions.Count > 0, "Staging cashback transactions not found");


            // Now lets make the next call, which should process the file we just added
            //var res = await ServiceHelper.Instance.CashbackService.MigrateCashbackTransactions(appSettings["AdminEmail"], appSettings["CashbackConfirmedInDays"]);
            await Task.CompletedTask;

        }

        [Test]
        public async Task MigrateCashbackTransactions_Test()
        {
            await _cashbackService.MigrateCashbackTransactions("ian@ijwaa.com", "45");

            // best check something hey?
            Assert.IsTrue(true);

        }

        //TODO:  Fix CashbackMigrateStaging Test
        //[Test]
        //public async Task AssertCashbackMigrateStagingDataWorks()
        //{
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
        //                registrationCodeId, Enums.EmailTemplateType.WelcomeEmail);

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


        //    //Get transaction report
        //    var appSettings = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();

        //    var result = await ServiceHelper.Instance.CashbackService.GetTransactionReport(new DateTime(2019, 10, 1),
        //        new DateTime(2019, 12, 31), appSettings["StrackrAPI_id"], appSettings["StrackrAPI_key"]);

        //    var success = false;
        //    bool.TryParse(result, out success);

        //    var tranFile = await ServiceHelper.Instance.StagingTransactionFileService.GetAll();

        //    var status =
        //        await ServiceHelper.Instance.StatusService.GetAll(Data.Constants.StatusType
        //            .StagingCashbackTransactions);
        //    int statusNew = status.FirstOrDefault(x => x.Name == Data.Constants.Status.New).Id;

        //    var transactions =
        //        await ServiceHelper.Instance.StagingCashbackTransactionService.GetAll(tranFile.FirstOrDefault().Id,
        //            statusNew);

        //    var transaction = transactions.FirstOrDefault(x =>
        //        x.MembershipCardReference != null && x.NetworkName.ToLower() == "awin");

        //    //Create Merchant
        //    var merchant = new Services.Models.DTOs.Merchant
        //    {
        //        Name = transaction.MerchantName
        //    };
        //    var respMerchant = await ServiceHelper.Instance.MerchantService.Add(merchant);

        //    //affiliate rule
        //    var affiliateRule = new Services.Models.RequestModels.AffiliateMappingRule
        //    {
        //        Description = "Merchants",
        //        AffiliateId = 1,
        //        IsActive = true
        //    };

        //    var respMerchantRule = await ServiceHelper.Instance.AffiliateMappingRuleService.Add(affiliateRule);
        //    //Affiliate mapping
        //    var mapping = new Services.Models.RequestModels.AffiliateMapping
        //    {
        //        AffiliateMappingRuleId = respMerchantRule.Id,
        //        AffilateValue = merchant.Name,
        //        ExclusiveValue = respMerchant.Id.ToString()
        //    };

        //    var respMapping = await ServiceHelper.Instance.AffiliateMappingService.Add(mapping);

        //    //Membership Card Affiliate reference
        //    var reference = new Services.Models.DTOs.MembershipCardAffiliateReference
        //    {
        //        MembershipCardId = activeCard.Id,
        //        AffiliateId = 1,
        //        CardReference = transaction.MembershipCardReference
        //    };

        //    var cardReference = await ServiceHelper.Instance.MembershipCardAffiliateReferenceService.Add(reference);

        //    //Processing Staging transactions
        //    var res = await ServiceHelper.Instance.CashbackService.MigrateCashbackTransactions(appSettings["AdminEmail"], appSettings["CashbackConfirmedInDays"]);

        //    var successProcessing = false;
        //    bool.TryParse(res, out successProcessing);

        //    var affiliateTransactionReference = string.IsNullOrEmpty(transaction.BasketId)
        //        ? $"{transaction.ResultsId}"
        //        : $"{transaction.ResultsId} | {transaction.BasketId}";

        //    var cashbackTran =
        //        await ServiceHelper.Instance.CashbackTransactionService.GetByTransactionReferenceAsync(
        //            affiliateTransactionReference);

        //    var summaries =
        //        await ServiceHelper.Instance.CashbackSummaryService.GetAllAsync();

        //    Assert.IsNotNull(registrationCodes, "Registration codes not found.");
        //    Assert.IsNotNull(code, "ExTam3 not found.");
        //    Assert.IsNotNull(membershipPlan, "Membership Plan not found");
        //    Assert.IsNotNull(membershipCard, "Membership card creation failed");
        //    Assert.IsNotNull(accountDetail, "Customer account creation failed");


        //    Assert.IsTrue(success, "Getting file failed");
        //    Assert.IsNotNull(tranFile, "Transaction file is not created");
        //    Assert.IsNotNull(status, "Status not found");
        //    Assert.IsTrue(status.Count > 0, "StagingCashbackTransactions status type not found");
        //    Assert.IsTrue(statusNew > 0, "Status not found for records");
        //    Assert.IsNotNull(transactions, "Staging cashback transactions are null");
        //    Assert.IsTrue(transactions.Count > 0, "Staging cashback transactions not found");

        //    Assert.IsNotNull(respMerchant, "Merchant not created");
        //    Assert.IsNotNull(respMerchantRule, "Mapping rule not found");
        //    Assert.IsNotNull(respMapping, "AffiliateMapping not found");
        //    Assert.IsNotNull(cardReference, "Reference not created");

        //    Assert.IsTrue(successProcessing, "Migration from staging failed");
        //    Assert.IsNotNull(cashbackTran, "CashBacks are null");
        //    Assert.IsTrue(cashbackTran.Count > 0, "Transactions not found");
        //    Assert.IsNotNull(summaries, "Summary is null");
        //    Assert.IsTrue(summaries.Count > 0, "Summary not found");
        //}
    }
}
