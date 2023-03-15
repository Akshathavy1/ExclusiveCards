using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IAdminUserAccountService
    {
        Task<dto.UserToken> ValidateRegistrationCode(string code);
        Task<dto.UserToken> CreateAccount(dto.UserAccountDetails details, Guid? pendingRegistrationToken=null);
        Task<SignInResult> Login(string userName, string password);
        Task Logout(string returnUrl = null);
        Task ForgotPassword(string userName);
    }
}
