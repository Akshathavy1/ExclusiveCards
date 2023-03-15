using ExclusiveCard.Services.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Db = ExclusiveCard.Data.Models;

namespace ExclusiveCard.Managers
{
    public interface IUserManager
    {
        Task<SignInResult> AdminLoginAsync(string userName, string password);

        Task<UserAccountDetails> CustomerLoginAsync(string userName, string password);

        Task<string> PartnerLoginAsync(string userName, string password, string audience);

        bool ValidatePartnerToken(string token, string audience);

        Task PartnerCustomerSignInAsync(string customerUserName);


        Task LogoutAsync();

        Task<string> GenerateEmailConfirmationTokenAsync(ExclusiveUser user);
        
        Task<string> GenerateEmailConfirmationTokenAsync(Db.ExclusiveUser user);

        Task<IdentityResult> ConfirmEmailTokenAsync(Db.ExclusiveUser user, string token);

        Task<IdentityResult> ConfirmEmailTokenAsync(ExclusiveUser user, string token);

        Task<bool> CheckExistsAsync(string userName);

        Task<string> CreateAsync(ExclusiveUser user, string password = null, string role = "user");

        Task<ExclusiveUser> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal);

        Task<ExclusiveUser> GetUserAsync(string userName);

        string GetClaim(string token, string claimType);
    }
}
