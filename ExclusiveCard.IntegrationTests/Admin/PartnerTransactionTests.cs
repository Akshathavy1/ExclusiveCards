using ExclusiveCard.Data.Constants;
using ExclusiveCard.Enums;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using data = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.IntegrationTests.Admin
{
    public class PartnerTransactionTests
    {
        private readonly int _partnerId = 1;
        [SetUp]
        public void Setup()
        {
        }

        //TODO: Refactor PartnerTransactionTests to get txns from the repo as the old CashbackTransactions service is no more. 
        [Test]
        public async Task AssertSendTransactionReportData()
        {
            var status = await ServiceHelper.Instance.StatusService.GetAll();
            //Create customer
            IdentityRole iRole = new IdentityRole { Name = Roles.User.ToString(), NormalizedName = "User" };
            var roleResult = await ServiceHelper.Instance.UserService.CreateRoleAsync(iRole);
            //Initialize user model
            data.ExclusiveUser user = Common.Common.BuildUserModel();
            //Add user
            data.ExclusiveUser respUser = await Common.Customer.CreateUser(user, Roles.User);

            //Create customer Model
            dto.Customer customer = Common.Common.BuildCustomerModel();

            dto.Customer respCustomer = null;

            if (!string.IsNullOrEmpty(respUser.Id))
            {
                respCustomer = await Common.Customer.CreateCustomer(respUser.Id, customer);
            }

            DateTime current = DateTime.Now;
            string month = current.Month.ToString().Length == 1
                ? $"0{current.Month.ToString()}"
                : current.Month.ToString();
            string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();
            //Create PartnerReward
            dto.PartnerRewards reward = new dto.PartnerRewards
            {
                RewardKey = $"{current.Year}{month}{day}{current.Hour}{current.Minute}{current.Second}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}",
                PartnerId = _partnerId,
                CreatedDate = current
            };
            dto.PartnerRewards resPartnerRewards = await ServiceHelper.Instance.PartnerRewardService.AddAsync(reward);
            //Create Membership card
            dto.MembershipCard membershipCard = Common.Common.BuildMembershipCardModel();
            membershipCard.PartnerRewardId = resPartnerRewards.Id;
            dto.MembershipCard membershipCardAdded = await Common.Customer.CreatMembershipCard(respCustomer.Id, 4, 1.ToString(), membershipCard);


            int receivedStatus = status
                .FirstOrDefault(x => x.IsActive && x.Type == StatusType.Cashback && x.Name == Status.Received).Id;
            //Create cashback transactions and cashback summary
            //var trans = await Common.Cashback.CreateTransactions(membershipCardAdded.Id, _partnerId, receivedStatus);
            //var cashbacks = await Common.Cashback.CreateCashbackSummary(membershipCardAdded.Id, _partnerId);


            //Check if file already exists in processing state for the partner
            var fileExistsWithProcessing =
                ServiceHelper.Instance.PartnerTransactionService.CheckIfFileExistsWithProcessingState(_partnerId);


            //Get file data
            var fileData = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionReport(_partnerId);
            int fileId = fileData.FirstOrDefault().FileId;
            var fileTotal = fileData.Select(x => x.Amount).Sum();

            

            //var transactionTotal = transactions.Select(x => x.CashbackAmount).Sum();

            Assert.IsNotNull(respUser, "User not found");
            Assert.IsNotNull(respCustomer, "Customer creation failed");
            Assert.IsNotNull(resPartnerRewards, "Partner reward creation failed");
            Assert.IsNotNull(membershipCardAdded, "Membership card creation failed");
            Assert.IsNotNull(status, "Status not found");
            //Assert.IsNotNull(trans, "Creation of transactions failed");
            //Assert.IsNotNull(cashbacks, "Creation of Cashback failed");
            //Assert transactions sum of cash back received and summary cash back received matches
            Assert.IsFalse(fileExistsWithProcessing, "File exists in processing state");
            Assert.IsNotNull(fileData, "Transaction data is null");
            Assert.IsTrue(fileData.Count > 0, "No Transaction data found");
            //Assert.IsNotNull(transactions, "Updated transactions not found");
            //Assert.IsTrue(transactions.Count > 0, "Transactions are empty");
            //Assert.AreEqual(fileTotal, transactionTotal, "Total Amount do not match");
        }

        [Test]
        public async Task AssertSendTransactionReportForDifferentAccountTypes()
        {
            var status = await ServiceHelper.Instance.StatusService.GetAll();
            //Create customer
            IdentityRole iRole = new IdentityRole { Name = Roles.User.ToString(), NormalizedName = "User" };
            var roleResult = await ServiceHelper.Instance.UserService.CreateRoleAsync(iRole);
            //Initialize user model
            data.ExclusiveUser user = Common.Common.BuildUserModel();
            //Add user
            data.ExclusiveUser respUser = await Common.Customer.CreateUser(user, Roles.User);

            //Create customer Model
            dto.Customer customer = Common.Common.BuildCustomerModel();

            dto.Customer respCustomer = null;

            if (!string.IsNullOrEmpty(respUser.Id))
            {
                respCustomer = await Common.Customer.CreateCustomer(respUser.Id, customer);
            }

            DateTime current = DateTime.Now;
            string month = current.Month.ToString().Length == 1
                ? $"0{current.Month.ToString()}"
                : current.Month.ToString();
            string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();
            //Create PartnerReward
            dto.PartnerRewards reward = new dto.PartnerRewards
            {
                RewardKey = $"{current.Year}{month}{day}{current.Hour}{current.Minute}{current.Second}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}",
                PartnerId = _partnerId,
                CreatedDate = current
            };
            dto.PartnerRewards resPartnerRewards = await ServiceHelper.Instance.PartnerRewardService.AddAsync(reward);
            //Create Membership card
            dto.MembershipCard membershipCard = Common.Common.BuildMembershipCardModel();
            membershipCard.PartnerRewardId = resPartnerRewards.Id;
            dto.MembershipCard membershipCardAdded = await Common.Customer.CreatMembershipCard(respCustomer.Id, 4, 1.ToString(), membershipCard);


            int receivedStatus = status
                .FirstOrDefault(x => x.IsActive && x.Type == StatusType.Cashback && x.Name == Status.Received).Id;
            //Create cashback transactions and cashback summary
            //var trans = await Common.Cashback.CreateTransactions(membershipCardAdded.Id, _partnerId, receivedStatus);
            //var cashbacks = await Common.Cashback.CreateCashbackSummaries(membershipCardAdded.Id, _partnerId);


            //Check if file already exists in processing state for the partner
            var fileExistsWithProcessing =
                ServiceHelper.Instance.PartnerTransactionService.CheckIfFileExistsWithProcessingState(_partnerId);

            //Get file data
            var fileData = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionReport(_partnerId);
            var fileId = fileData.FirstOrDefault().FileId;
            var fileResp = await ServiceHelper.Instance.PartnerTransactionService.GetByIdAsync(fileId);

            var fileTotal = fileData.Select(x => x.Amount).Sum();

            ////Get transactions for the file
            //var transactions =
            //    await ServiceHelper.Instance.CashbackTransactionService.GetAllTransactionForFileAsync(fileResp.Id);

          //  var transactionTotal = transactions.Select(x => x.CashbackAmount).Sum();

            Assert.IsNotNull(respUser, "User not found");
            Assert.IsNotNull(respCustomer, "Customer creation failed");
            Assert.IsNotNull(resPartnerRewards, "Partner reward creation failed");
            Assert.IsNotNull(membershipCardAdded, "Membership card creation failed");
            Assert.IsNotNull(status, "Status not found");
            //Assert.IsNotNull(trans, "Creation of transactions failed");
            //Assert.IsNotNull(cashbacks, "Creation of Cashback failed");
            //Assert transactions sum of cash back received and summary cash back received matches
            Assert.IsFalse(fileExistsWithProcessing, "File exists in processing state");
            Assert.IsNotNull(fileResp, "File not created");
            Assert.IsNotNull(fileData, "Transaction data is null");
            Assert.IsTrue(fileData.Count > 0, "No Transaction data found");
            //Assert.IsNotNull(transactions, "Updated transactions not found");
            //Assert.IsTrue(transactions.Count > 0, "Transactions are empty");
            //Assert.AreEqual(fileTotal, transactionTotal, "Total Amount do not match");
        }

        //Assert second file will not generate
        [Test]
        public async Task AssertSendTransactionReportWithFileExists()
        {
            var status = await ServiceHelper.Instance.StatusService.GetAll();
            //Create customer
            IdentityRole iRole = new IdentityRole { Name = Roles.User.ToString(), NormalizedName = "User" };
            var roleResult = await ServiceHelper.Instance.UserService.CreateRoleAsync(iRole);
            //Initialize user model
            data.ExclusiveUser user = Common.Common.BuildUserModel();
            //Add user
            data.ExclusiveUser respUser = await Common.Customer.CreateUser(user, Roles.User);

            //Create customer Model
            dto.Customer customer = Common.Common.BuildCustomerModel();

            dto.Customer respCustomer = null;

            if (!string.IsNullOrEmpty(respUser.Id))
            {
                respCustomer = await Common.Customer.CreateCustomer(respUser.Id, customer);
            }

            DateTime current = DateTime.Now;
            string month = current.Month.ToString().Length == 1
                ? $"0{current.Month.ToString()}"
                : current.Month.ToString();
            string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();
            //Create PartnerReward
            dto.PartnerRewards reward = new dto.PartnerRewards
            {
                RewardKey = $"{current.Year}{month}{day}{current.Hour}{current.Minute}{current.Second}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}",
                PartnerId = _partnerId,
                CreatedDate = current
            };
            dto.PartnerRewards resPartnerRewards = await ServiceHelper.Instance.PartnerRewardService.AddAsync(reward);
            //Create Membership card
            dto.MembershipCard membershipCard = Common.Common.BuildMembershipCardModel();
            membershipCard.PartnerRewardId = resPartnerRewards.Id;
            dto.MembershipCard membershipCardAdded = await Common.Customer.CreatMembershipCard(respCustomer.Id, 4, 1.ToString(), membershipCard);


            int receivedStatus = status
                .FirstOrDefault(x => x.IsActive && x.Type == StatusType.Cashback && x.Name == Status.Received).Id;
            //Create cashback transactions and cashback summary
            //var trans = await Common.Cashback.CreateTransactions(membershipCardAdded.Id, _partnerId, receivedStatus);
            //var cashbacks = Common.Cashback.CreateCashbackSummary(membershipCardAdded.Id, _partnerId);


            //Check if file already exists in processing state for the partner
            var fileExistsWithProcessing =
                ServiceHelper.Instance.PartnerTransactionService.CheckIfFileExistsWithProcessingState(_partnerId);

            //Get file data
            var fileData = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionReport(_partnerId);
            var fileId = fileData.FirstOrDefault().FileId;
            var fileResp = await ServiceHelper.Instance.PartnerTransactionService.GetByIdAsync(fileId);

            var fileTotal = fileData.Select(x => x.Amount).Sum();

            //Get transactions for the file
            //var transactions =
            //    await ServiceHelper.Instance.CashbackTransactionService.GetAllTransactionForFileAsync(fileResp.Id);

            //var transactionTotal = transactions.Select(x => x.CashbackAmount).Sum();

            //Check if file already exists in processing state for the partner
            var fileExists =
                ServiceHelper.Instance.PartnerTransactionService.CheckIfFileExistsWithProcessingState(_partnerId);

            Assert.IsNotNull(respUser, "User not found");
            Assert.IsNotNull(respCustomer, "Customer creation failed");
            Assert.IsNotNull(resPartnerRewards, "Partner reward creation failed");
            Assert.IsNotNull(membershipCardAdded, "Membership card creation failed");
            Assert.IsNotNull(status, "Status not found");
            //Assert.IsNotNull(trans, "Creation of transactions failed");
            //Assert.IsNotNull(cashbacks, "Creation of Cashback failed");
            //Assert transactions sum of cash back received and summary cash back received matches
            Assert.IsFalse(fileExistsWithProcessing, "File exists in processing state");
            Assert.IsNotNull(fileResp, "File not created");
            Assert.IsNotNull(fileData, "Transaction data is null");
            Assert.IsTrue(fileData.Count > 0, "No Transaction data found");
            //Assert.IsNotNull(transactions, "Updated transactions not found");
            //Assert.IsTrue(transactions.Count > 0, "Transactions are empty");
            //Assert.AreEqual(fileTotal, transactionTotal, "Total Amount do not match");
            Assert.IsTrue(fileExists, "File exists status wrong");
        }

        //assert no data
        [Test]
        public async Task AssertSendTransactionReportNoData()
        {
            var status = await ServiceHelper.Instance.StatusService.GetAll();
            //Create customer
            IdentityRole iRole = new IdentityRole { Name = Roles.User.ToString(), NormalizedName = "User" };
            var roleResult = await ServiceHelper.Instance.UserService.CreateRoleAsync(iRole);
            //Initialize user model
            data.ExclusiveUser user = Common.Common.BuildUserModel();
            //Add user
            data.ExclusiveUser respUser = await Common.Customer.CreateUser(user, Roles.User);

            //Create customer Model
            dto.Customer customer = Common.Common.BuildCustomerModel();

            dto.Customer respCustomer = null;

            if (!string.IsNullOrEmpty(respUser.Id))
            {
                respCustomer = await Common.Customer.CreateCustomer(respUser.Id, customer);
            }

            DateTime current = DateTime.Now;
            string month = current.Month.ToString().Length == 1
                ? $"0{current.Month.ToString()}"
                : current.Month.ToString();
            string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();
            //Create PartnerReward
            dto.PartnerRewards reward = new dto.PartnerRewards
            {
                RewardKey = $"{current.Year}{month}{day}{current.Hour}{current.Minute}{current.Second}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}",
                PartnerId = _partnerId,
                CreatedDate = current
            };
            dto.PartnerRewards resPartnerRewards = await ServiceHelper.Instance.PartnerRewardService.AddAsync(reward);
            //Create Membership card
            dto.MembershipCard membershipCard = Common.Common.BuildMembershipCardModel();
            membershipCard.PartnerRewardId = resPartnerRewards.Id;
            dto.MembershipCard membershipCardAdded = await Common.Customer.CreatMembershipCard(respCustomer.Id, 4, 1.ToString(), membershipCard);


            //Check if file already exists in processing state for the partner
            var fileExistsWithProcessing =
                ServiceHelper.Instance.PartnerTransactionService.CheckIfFileExistsWithProcessingState(_partnerId);

            //Get file data
            var fileData = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionReport(_partnerId);
            var fileId = fileData.FirstOrDefault().FileId;
            var fileResp = await ServiceHelper.Instance.PartnerTransactionService.GetByIdAsync(fileId);

            var fileTotal = fileData.Select(x => x.Amount).Sum();

            //Get transactions for the file
            //var transactions =
            //    await ServiceHelper.Instance.CashbackTransactionService.GetAllTransactionForFileAsync(fileResp.Id);

            //var transactionTotal = transactions.Select(x => x.CashbackAmount).Sum();

            Assert.IsNotNull(respUser, "User not found");
            Assert.IsNotNull(respCustomer, "Customer creation failed");
            Assert.IsNotNull(resPartnerRewards, "Partner reward creation failed");
            Assert.IsNotNull(membershipCardAdded, "Membership card creation failed");
            Assert.IsNotNull(status, "Status not found");
            //Assert transactions sum of cash back received and summary cash back received matches
            Assert.IsFalse(fileExistsWithProcessing, "File exists in processing state");
            Assert.IsNotNull(fileResp, "File not created");
            Assert.IsNotNull(fileData, "Transaction data is null");
            Assert.IsTrue(fileData.Count == 0, "Transaction data found");
            //Assert.IsNotNull(transactions, "Updated transactions not found");
            //Assert.IsTrue(transactions.Count == 0, "Transactions are not empty");
            //Assert.AreEqual(fileTotal, transactionTotal, "Total Amount do not match");
        }

        //assert no data return when partner rewardkey not found
        [Test]
        public async Task AssertSendTransactionReportWithNoRewardKey()
        {
            var status = await ServiceHelper.Instance.StatusService.GetAll();
            //Create customer
            IdentityRole iRole = new IdentityRole { Name = Roles.User.ToString(), NormalizedName = "User" };
            var roleResult = await ServiceHelper.Instance.UserService.CreateRoleAsync(iRole);
            //Initialize user model
            data.ExclusiveUser user = Common.Common.BuildUserModel();
            //Add user
            data.ExclusiveUser respUser = await Common.Customer.CreateUser(user, Roles.User);

            //Create customer Model
            dto.Customer customer = Common.Common.BuildCustomerModel();

            dto.Customer respCustomer = null;

            if (!string.IsNullOrEmpty(respUser.Id))
            {
                respCustomer = await Common.Customer.CreateCustomer(respUser.Id, customer);
            }

            DateTime current = DateTime.Now;
            string month = current.Month.ToString().Length == 1
                ? $"0{current.Month.ToString()}"
                : current.Month.ToString();
            string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();

            //Create Membership card
            dto.MembershipCard membershipCard = Common.Common.BuildMembershipCardModel();

            dto.MembershipCard membershipCardAdded = await Common.Customer.CreatMembershipCard(respCustomer.Id, 4, 1.ToString(), membershipCard);


            int receivedStatus = status
                .FirstOrDefault(x => x.IsActive && x.Type == StatusType.Cashback && x.Name == Status.Received).Id;
            //Create cashback transactions and cashback summary
            //var trans = await Common.Cashback.CreateTransactions(membershipCardAdded.Id, _partnerId, receivedStatus);
            //var cashbacks = await Common.Cashback.CreateCashbackSummary(membershipCardAdded.Id, _partnerId);


            //Check if file already exists in processing state for the partner
            var fileExistsWithProcessing =
                ServiceHelper.Instance.PartnerTransactionService.CheckIfFileExistsWithProcessingState(_partnerId);

            //Get file data
            var fileData =
                await ServiceHelper.Instance.PartnerTransactionService.GetTransactionReport(_partnerId);
            var fileId = fileData.FirstOrDefault().FileId;
            var fileResp = await ServiceHelper.Instance.PartnerTransactionService.GetByIdAsync(fileId);

            Assert.IsNotNull(respUser, "User not found");
            Assert.IsNotNull(respCustomer, "Customer creation failed");
            Assert.IsNotNull(membershipCardAdded, "Membership card creation failed");
            Assert.IsNotNull(status, "Status not found");
            //Assert.IsNotNull(trans, "Creation of transactions failed");
            //Assert.IsNotNull(cashbacks, "Creation of Cashback failed");
            //Assert transactions sum of cash back received and summary cash back received matches
            Assert.IsFalse(fileExistsWithProcessing, "File exists in processing state");
            Assert.IsNotNull(fileResp, "File not created");
            Assert.IsNotNull(fileData, "Transaction data is null");
            Assert.IsTrue(fileData.Count == 0, "Transaction data found");
        }

        [Test]
        public async Task AssertTransactionSearch()
        {
            var statuses = await ServiceHelper.Instance.StatusService.GetAll(StatusType.FilePayment);
            var partners = await ServiceHelper.Instance.PartnerService.GetAllAsync((int)PartnerType.RewardPartner);
            //Create files
            var files = await Common.File.CreateFiles(partners.FirstOrDefault().Id);
            //Search for unpaid files
            var resultUnpaid = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionsAsync(
                statuses.FirstOrDefault(x => x.Name == Status.Unpaid).Id, partners.FirstOrDefault().Id, 1, 20, TransactionSortOrder.DateDesc);
            //Search for paid files
            var resultPaid = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionsAsync(
                statuses.FirstOrDefault(x => x.Name == Status.Paid).Id, partners.FirstOrDefault().Id, 1, 20, TransactionSortOrder.DateDesc);

            //Assertions
            Assert.IsNotNull(statuses, "Status null");
            Assert.IsTrue(statuses.Count > 0, "Status not found");
            Assert.IsNotNull(partners, "Partner is null");
            Assert.IsTrue(partners.Count > 0, "Reward Partner not found");
            Assert.IsNotNull(files, "Creation of files failed");
            Assert.IsTrue(files.Count > 0, "Files not created");
            Assert.IsNotNull(resultPaid.Results, "Paid transactions not found");
            Assert.IsTrue(resultPaid.Results.Count > 0, "Paid transactions are empty");
            Assert.IsNotNull(resultUnpaid.Results, "Unpaid transactions are null");
            Assert.IsTrue(resultUnpaid.Results.Count > 0, "Unpaid transactions are empty");
        }

        [Test]
        public async Task AssertTransactionSearchNoData()
        {
            var statuses = await ServiceHelper.Instance.StatusService.GetAll(StatusType.FilePayment);
            var partners = await ServiceHelper.Instance.PartnerService.GetAllAsync((int)PartnerType.RewardPartner);
            //Search for unpaid files
            var resultUnpaid = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionsAsync(
                statuses.FirstOrDefault(x => x.Name == Status.Unpaid).Id, partners.FirstOrDefault().Id, 1, 20, TransactionSortOrder.DateDesc);
            //Search for paid files
            var resultPaid = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionsAsync(
                statuses.FirstOrDefault(x => x.Name == Status.Paid).Id, partners.FirstOrDefault().Id, 1, 20, TransactionSortOrder.DateDesc);

            //Assertions
            Assert.IsNotNull(statuses, "Status null");
            Assert.IsTrue(statuses.Count > 0, "Status not found");
            Assert.IsNotNull(partners, "Partner is null");
            Assert.IsTrue(partners.Count > 0, "Reward Partner not found");
            Assert.IsEmpty(resultPaid.Results, "Paid transactions found");
            Assert.IsEmpty(resultUnpaid.Results, "Unpaid transactions are not null");
        }

        [Test]
        public async Task AssertTransactionUpdateToPaid()
        {
            //Create customer
            IdentityRole iRole = new IdentityRole { Name = Roles.BackOfficeUser.ToString(), NormalizedName = "BackOfficeUser" };
            var roleResult = await ServiceHelper.Instance.UserService.CreateRoleAsync(iRole);
            //Initialize user model
            data.ExclusiveUser user = Common.Common.BuildUserModel();
            //Add user
            data.ExclusiveUser respUser = await Common.Customer.CreateUser(user, Roles.BackOfficeUser);

            var statuses = await ServiceHelper.Instance.StatusService.GetAll(StatusType.FilePayment);
            var partners = await ServiceHelper.Instance.PartnerService.GetAllAsync((int)PartnerType.RewardPartner);
            //Create files
            var files = await Common.File.CreateFiles(partners.FirstOrDefault().Id);
            //Search for unpaid files
            var resultUnpaid = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionsAsync(
                statuses.FirstOrDefault(x => x.Name == Status.Unpaid).Id, partners.FirstOrDefault().Id, 1, 20, TransactionSortOrder.DateDesc);
            //Search for paid files
            var resultPaid = await ServiceHelper.Instance.PartnerTransactionService.GetTransactionsAsync(
                statuses.FirstOrDefault(x => x.Name == Status.Paid).Id, partners.FirstOrDefault().Id, 1, 20, TransactionSortOrder.DateDesc);

            var unpaid = resultUnpaid.Results.FirstOrDefault();

            var req = new dto.Files
            {
                Id = unpaid.Id,
                Name = unpaid.Name,
                PartnerId = unpaid.PartnerId,
                Type = unpaid.Type,
                StatusId = unpaid.StatusId,
                PaymentStatusId = statuses.FirstOrDefault(x => x.Name == Status.Paid && x.Type == StatusType.FilePayment).Id,
                TotalAmount = 12.48m,
                CreatedDate = DateTime.UtcNow.AddDays(-12),
                ChangedDate = DateTime.UtcNow,
                PaidDate = DateTime.UtcNow,
                UpdatedBy = respUser.Id
            };

            var fileUpdated = await ServiceHelper.Instance.PartnerTransactionService.UpdateAsync(req);

            //Assertions
            Assert.IsNotNull(iRole, "Role is null");
            Assert.IsNotNull(user, "User is null");
            Assert.IsNotNull(respUser, "User creation failed");
            Assert.IsNotNull(respUser.Id, "User Id not found");

            Assert.IsNotNull(statuses, "Status null");
            Assert.IsTrue(statuses.Count > 0, "Status not found");
            Assert.IsNotNull(partners, "Partner is null");
            Assert.IsTrue(partners.Count > 0, "Reward Partner not found");
            Assert.IsNotNull(files, "Creation of files failed");
            Assert.IsTrue(files.Count > 0, "Files not created");
            Assert.IsNotNull(resultPaid.Results, "Paid transactions not found");
            Assert.IsTrue(resultPaid.Results.Count > 0, "Paid transactions are empty");
            Assert.IsNotNull(resultUnpaid.Results, "Unpaid transactions are null");
            Assert.IsTrue(resultUnpaid.Results.Count > 0, "Unpaid transactions are empty");

            Assert.IsNotNull(unpaid, "No transaction found to update to paid");
            Assert.IsNotNull(req, "Req model is empty");
            Assert.IsNotNull(fileUpdated, "Update entity is null");

            Assert.AreEqual(req.Id, fileUpdated.Id, "Request and response are different");
            Assert.AreEqual(req.PaidDate, fileUpdated.PaidDate, "Paid date differs");
            Assert.AreEqual(req.UpdatedBy, fileUpdated.UpdatedBy, "Updated by differs");
            Assert.AreEqual(req.ChangedDate, fileUpdated.ChangedDate, "ChangedDate differs");
        }
    }
}
