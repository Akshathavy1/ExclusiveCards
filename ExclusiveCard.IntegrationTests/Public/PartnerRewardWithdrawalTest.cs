using ExclusiveCard.Enums;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using data = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.IntegrationTests.Public
{
    public class PartnerRewardWithdrawalTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AssertWithdrawalData()
        {
            var status = await ServiceHelper.Instance.StatusService.GetAll();
            var partner = await ServiceHelper.Instance.PartnerService.GetAllAsync((int)PartnerType.RewardPartner);
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
                PartnerId = partner.FirstOrDefault().Id,
                CreatedDate = current,
                LatestValue = 26m,
                ValueDate = DateTime.UtcNow.AddDays(-1)
            };
            dto.PartnerRewards resPartnerRewards = await ServiceHelper.Instance.PartnerRewardService.AddAsync(reward);
            //Create Membership card
            dto.MembershipCard membershipCard = Common.Common.BuildMembershipCardModel();
            membershipCard.PartnerRewardId = resPartnerRewards.Id;
            dto.MembershipCard membershipCardAdded = await Common.Customer.CreatMembershipCard(respCustomer.Id, 4, 1.ToString(), membershipCard);

            //Create Bank details
            dto.BankDetail bankDetail = Common.Common.BuildBankDetail();
            dto.BankDetail respBankDetail = await ServiceHelper.Instance.BankDetailService.Add(bankDetail);

            //Create Customer bank details
            dto.CustomerBankDetail customerBankDetail =
                Common.Common.BuildCustomerBankDetail(respCustomer.Id, respBankDetail.Id);
            dto.CustomerBankDetail respCustomerBankDetail =
                await ServiceHelper.Instance.CustomerBankDetailService.Add(customerBankDetail);

            //Call withdrawal data
            var data =
                await ServiceHelper.Instance.PartnerRewardWithdrawalService.GetWithdrawalDataForRequest(
                    membershipCardAdded.Id);

            //Assertions
            Assert.IsNotNull(status, "Status not found");
            Assert.IsTrue(status.Count > 0, "Status empty");
            Assert.IsNotNull(partner, "Partner not found");
            Assert.IsTrue(partner.Count > 0, "Partner is empty");
            Assert.IsNotNull(roleResult, "Role not found");
            Assert.IsNotNull(respUser, "User not found");
            Assert.IsNotNull(respCustomer, "Customer not found");
            Assert.IsNotNull(resPartnerRewards, "Partner Reward not found");
            Assert.IsNotNull(membershipCardAdded, "Membership card not found");
            Assert.IsNotNull(respBankDetail, "Bank details not found");
            Assert.IsNotNull(respCustomerBankDetail, "Customer bank details not found");
            Assert.IsNotNull(data, "Withdrawal data not found");
            Assert.IsFalse(data.RequestExists, "Request exists is true");
            Assert.IsTrue(data.CustomerId > 0, "Customer Id is null");
            Assert.IsTrue(data.PartnerRewardId > 0, "Partner Reward is null");
            Assert.IsTrue(data.BankDetailId > 0, "Bank detail is null");
            Assert.IsNotNull(data.Name, "Account name is null");
            Assert.IsNotNull(data.AccountNumber, "Account number is null");
            Assert.IsNotNull(data.SortCode, "Sort code is null");
            Assert.IsTrue(data.AvailableFund > 0m, "Available fund is null");
        }
    }
}
