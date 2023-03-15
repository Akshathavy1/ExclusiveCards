using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace ExclusiveCard.Services.Interfaces
{
    public interface IUserService
    {
        #region Writes

        Task<bool> CreateAsync(ExclusiveUser user, string password, string role);

        Task<bool> CreateRoleAsync(IdentityRole role);

        Task<bool> ChangePasswordAsync(ExclusiveUser user, string currentPassword, string newPassword);

        Task<bool> AddNewPassword(ExclusiveUser user, string newPassword);

        Task<IdentityResult> SetLockoutEndDateAsync(ExclusiveUser user, DateTime date);

        Task<bool> SetEmailAsync(ExclusiveUser user, string email);

        Task<bool> SetUserNameAsync(ExclusiveUser user, string username);

        Task<bool> UpdateUserAsync(ExclusiveUser user);

        Task<bool> UpdateRoleAsync(ExclusiveUser user, string existingRole, string newRole);

        Task<bool> DeleteAsync(ExclusiveUser user);

        Task<string> GeneratePasswordResetTokenAsync(ExclusiveUser user);
        Task<IdentityResult> ResetPasswordAsync(ExclusiveUser user, string token, string password);

        #endregion

        #region Reads

        Task<ExclusiveUser> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal);
        Task<List<string>> GetRolesAsync(ExclusiveUser user);
        Task<string> GetRoleForCurrentUser(System.Security.Claims.ClaimsPrincipal principal);
        Task<ExclusiveUser> FindByNameAsync(string userName);
        Task<ExclusiveUser> FindByEmailAsync(string email);
        Task<ExclusiveUser> FindByIdAsync(string id);
        Task<bool> RoleExistsAsync(string roleName);

        Task<bool> CheckPasswordAsync(ExclusiveUser user, string password);

        Task<List<ExclusiveUser>> GetAllUsers(List<string> roles);

        #endregion

        Task<string> ResetUserPasswordAsync(string userName, string host, string noReplyEmail);
    }
}