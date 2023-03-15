using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ExclusiveCard.Services
{
    public class UserService : IUserService
    {
        #region Private Members

        private readonly ILogger _logger;
        private readonly UserManager<ExclusiveUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly Managers.IEmailManager _emailManager;


        #endregion

        #region Constructor

        [Obsolete("This has been replaced by ExclusiveCard.Managers.UserManager and the appropriate methods in CustomerAccountService and PartnerService. Admin service tbc")]
        public UserService(
            UserManager<ExclusiveUser> userManager,
            ICustomerService customerService,
            Managers.IEmailManager emailService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _userManager = userManager;
            _customerService = customerService;
            _emailManager = emailService;
        }

        #endregion

        #region Writes

        public async Task<bool> CreateAsync(ExclusiveUser user, string password, string role)
        {
            bool success = false;
            try
            {
                var createUser = false;
                IdentityResult result = new IdentityResult();
                while (!createUser)
                {
                    try
                    {
                        result = await _userManager.CreateAsync(user, password);
                        createUser = true;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                    }
                }

                if (result.Succeeded)
                {
                    IdentityResult roleResult = new IdentityResult();
                    bool addRole = false;
                    while (!addRole)
                    {
                        try
                        {
                            roleResult = await _userManager.AddToRoleAsync(user, role);
                            addRole = true;
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                        }
                    }

                    if (roleResult.Succeeded)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public async Task<bool> CreateRoleAsync(IdentityRole role)
        {
            await Task.CompletedTask;

            throw new NotImplementedException();

            //bool success = false;
            //try
            //{
            //    IdentityResult result = await _roleManager.CreateAsync(role);
            //    if (result.Succeeded)
            //    {
            //        success = true;
            //    }
            //}
            //catch (Exception)
            //{
            //    success = false;
            //}
            //return success;
        }

        public async Task<byte[]> CreateTokenAsync(ExclusiveUser user)
        {
            byte[] token = null;
            try
            {
                token = await _userManager.CreateSecurityTokenAsync(user);
            }
            catch (Exception)
            {
            }
            return token;
        }

        public async Task<bool> ChangePasswordAsync(ExclusiveUser user, string currentPassword, string newPassword)
        {
            bool success = false;
            try
            {
                IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (result.Succeeded)
                {
                        success = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;

        }

        public async Task<bool> AddNewPassword(ExclusiveUser user, string newPassword)
        {
            bool success = false;
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;

        }

        public async Task<IdentityResult> SetLockoutEndDateAsync(ExclusiveUser user, DateTime date)
        {
            return await _userManager.SetLockoutEndDateAsync(user, date);
        }

        public async Task<bool> SetEmailAsync(ExclusiveUser user, string email)
        {
            bool success;
            try
            {
                IdentityResult result = await _userManager.SetEmailAsync(user, email);
                success = result.Succeeded;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public async Task<bool> SetUserNameAsync(ExclusiveUser user, string username)
        {
            bool success;
            try
            {
                IdentityResult result = await _userManager.SetUserNameAsync(user, username);
                success = result.Succeeded;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public async Task<bool> UpdateUserAsync(ExclusiveUser user)
        {
            bool success;
            try
            {
                IdentityResult result = await _userManager.UpdateAsync(user);
                success = result.Succeeded;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public async Task<bool> UpdateRoleAsync(ExclusiveUser user, string existingRole, string newRole)
        {
            bool success = false;
            IdentityResult remove = await _userManager.RemoveFromRoleAsync(user, existingRole);
            if (remove.Succeeded)
            {
                IdentityResult add = await _userManager.AddToRoleAsync(user, newRole);
                if (add.Succeeded)
                {
                    success = true;
                }
            }
            return success;
        }

        public async Task<bool> DeleteAsync(ExclusiveUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ExclusiveUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ExclusiveUser user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        #endregion

        #region Reads

        public async Task<ExclusiveUser> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<List<string>> GetRolesAsync(ExclusiveUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<string> GetRoleForCurrentUser(System.Security.Claims.ClaimsPrincipal principal)
        {
            ExclusiveUser user = await _userManager.GetUserAsync(principal);
            if (user != null)
            {
                IList<string> rolesList = await _userManager.GetRolesAsync(user);
                return rolesList.FirstOrDefault();
            }
            return null;
        }

        public async Task<ExclusiveUser> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ExclusiveUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ExclusiveUser> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            await Task.CompletedTask;

            throw new NotImplementedException();
            //return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<bool> CheckPasswordAsync(ExclusiveUser user, string password)
        {
            bool result = false;
            try
            {
                 result = await _userManager.CheckPasswordAsync(user, password);
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<List<ExclusiveUser>> GetAllUsers(List<string> roles)
        {
            List<ExclusiveUser> users = new List<ExclusiveUser>();
            foreach (var role in roles)
            {
                users.AddRange(await _userManager.GetUsersInRoleAsync(role));
            }
            return users;
        }

        #endregion

        #region Logic

        public async Task<string> ResetUserPasswordAsync(string userName, string host, string noReplyEmail)
        {
            string result = true.ToString();
            try
            {
                var exclusiveUser = await FindByNameAsync(userName);
                if (exclusiveUser != null && exclusiveUser.Id != "0")
                {
                    //Get Password reset token
                    var token = await GeneratePasswordResetTokenAsync(exclusiveUser);

                    var customer = await _customerService.Get(exclusiveUser.Id);
                    if (customer != null)
                    {
                        var email = new Models.DTOs.Email();
                        string redirectLink = host + "/Account/ResetPassword?customerAuth=" + exclusiveUser.Id + "&token=" + token;
                        email.EmailFrom = noReplyEmail;
                        email.EmailTo = new List<string>();
                        email.EmailTo.Add(userName);
                        email.BodyHtml = $"Dear {customer.Forename} {customer.Surname},<br/><p><We received your request for a new password.</p><p>Click the link below to set a new password for Exclusive Card</p></br><a href='{redirectLink}'>Set New Password</a>";
                        email.BodyPlainText = $"Dear {customer.Forename} {customer.Surname}, We received your request for a new password. Click the link below to set a new password for Exclusive Card.{redirectLink}";
                        email.Subject = "Password Reset Email";
                        string emailsent = await _emailManager.SendEmailAsync(email);
                        result = emailsent;
                    }
                    else if (exclusiveUser.Id != "0" && customer == null)
                    {
                        var email = new Models.DTOs.Email();
                        string redirectLink = host + "/Account/ResetPassword?customerAuth=" + exclusiveUser.Id + "&token=" + token;
                        email.EmailFrom = noReplyEmail;
                        email.EmailTo = new List<string>();
                        email.EmailTo.Add(userName);
                        email.BodyHtml = $"Dear {userName},<br/><p><We received your request for a new password.</p><p>Click the link below to set a new password for Exclusive Card</p></br><a href='{redirectLink}'>Set New Password</a>";
                        email.BodyPlainText = $"Dear {userName}, We received your request for a new password. Click the link below to set a new password for Exclusive Card.{redirectLink}";
                        email.Subject = "Password Reset Email";
                        string emailsent = await _emailManager.SendEmailAsync(email);
                        result = emailsent;
                    }
                    else
                    {
                        result = "We couldn't find an account matching the email address you entered. Please check your email address and try again.";
                    }
                }
                else
                {
                    result = "We couldn't find an account matching the email address you entered. Please check your email address and try again.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                result = false.ToString();
            }

            return result;
        }

        #endregion
    }
}
