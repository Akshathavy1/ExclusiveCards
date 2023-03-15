using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Managers;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExclusiveCard.Services.Admin
{
    public class AdminUserAccountService : IAdminUserAccountService
    {
        #region Private fields and constructor

        private readonly IUserManager _userManager;

        public AdminUserAccountService(IUserManager usrManager)
        {
            _userManager = usrManager;
        }




        #endregion


        public async Task<UserToken> CreateAccount(UserAccountDetails details, Guid? pendingRegistrationToken = null)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task ForgotPassword(string userName)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        //TODO:  Replace with Customer object
        public async Task<SignInResult> Login(string userName, string password)
        {
            var result = await _userManager.AdminLoginAsync(userName, password);
            return result;

        }

        public async Task Logout(string returnUrl = null)
        {
            await _userManager.LogoutAsync();
        }

        public async Task<UserToken> ValidateRegistrationCode(string code)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
