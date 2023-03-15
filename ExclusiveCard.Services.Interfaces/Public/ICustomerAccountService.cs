using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using dto = ExclusiveCard.Services.Models.DTOs;
using Db = ExclusiveCard.Data.Models;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface ICustomerAccountService
    {
        // Account Registration 
        dto.UserToken ValidateRegistrationCode(string code);

        dto.UserToken CreateAccountFromPendingToken(dto.CustomerAccountDto customerAccount, string confirmUrl,  bool login, dto.MembershipPlan plan = null);

        dto.UserToken CreateAccountFromRegistrationCode(dto.CustomerAccountDto customerAccount, string registrationCode, string confirmUrl);


        // Login services

        //TODO:  Work out why Login needs to return full customer name and address details
        Task<dto.UserAccountDetails> Login(string userName, string password);

        Task Logout(string returnUrl = null);

        Task ForgotPassword(string userName);



        Task<string> GenerateEmailConfirmationTokenAsync(Db.ExclusiveUser user);

        Task<IdentityResult> ConfirmEmailTokenAsync(Db.ExclusiveUser user, string token);

        // Get Data

        Task<dto.ExclusiveUser> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal);

        Task<dto.ExclusiveUser> GetUserAsync(string userName);

        dto.CustomerBalances GetBalances(int customerId, int partnerRewardId);

        dto.CustomerAccountSummary GetAccountSummary(string userId);

        dto.CustomerAccountSummary GetAccountSummary(int customerId);

        dto.MembershipPlan GetDefaultDiamondPlan();

        dto.MembershipPlan GetMembershipPlan(int planId);

        dto.Customer GetCustomer(string userId);

        //Hack
        dto.BankDetail GetBankDetail(int bankDetailId);
        dto.BankDetail CreateBankDetail(dto.BankDetail bankDetail);
        dto.BankDetail UpdateBankDetail(dto.BankDetail bankDetail);
        dto.CustomerBankDetail GetCustomerBankDetail(int customerId, int bankDetailId = 0);
        dto.CustomerBankDetail CreateCustomerBankDetail(dto.CustomerBankDetail customerBankDetail);
        dto.CustomerBankDetail UpdateCustomerBankDetail(dto.CustomerBankDetail customerBankDetail);
        //Hack

        dto.CustomerBalances GetBenefactorDeposits(int customerId);

        dto.MembershipPlan GetTalkSportRegistrationCode(int whiteLabelId, int membershipPlanTypeId);

        // Update Data

        dto.Customer UpdateCustomerSettings(dto.Customer customer);
    }
}
