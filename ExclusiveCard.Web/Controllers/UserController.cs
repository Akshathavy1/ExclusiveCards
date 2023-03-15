using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "RootUser, AdminUser")]
    public class UserController : BaseController
    {
        #region Private Members

        private readonly IUserService _userService;

        #endregion

        #region Constructor

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ManageUserViewModel model = new ManageUserViewModel();
                List<SelectListItem> roles = (from object eVal in Enum.GetValues(typeof(Roles))
                    select new SelectListItem { Text = Enum.GetName(typeof(Roles), eVal), Value = eVal.ToString() }).ToList();

                List<string> roleList =
                    (from role in roles
                        where role.Text == Roles.AdminUser.ToString() || role.Text == Roles.BackOfficeUser.ToString()
                        select role.Text).ToList();

                List<ExclusiveUser> users = await _userService.GetAllUsers(roleList);
                if (users != null && users.Count > 0)
                {
                    foreach (ExclusiveUser user in users)
                    {
                        var rolelst = await _userService.GetRolesAsync(user);
                        var userAvailable = rolelst.FirstOrDefault(x => x != Roles.User.ToString());
                        model.UserSummaries.Add(new UserSummary
                        {
                            Id = user.Id,
                            Username = user.UserName,
                            Email = user.Email,
                            Role = userAvailable == Roles.AdminUser.ToString() ? Keys.Keys.AdminUser : Keys.Keys.BackOfficeUser,
                            RoleId = (int)Enum.Parse(typeof(Roles), userAvailable)
                        });
                    }
                }
                return View("Index", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("GetUsers")]
        public async Task<IActionResult> GetUsers(string username, string email)
        {
            try
            {
                List<UserSummary> model = new List<UserSummary>();
                
                List<SelectListItem> roles = (from object eVal in Enum.GetValues(typeof(Roles))
                                              select new SelectListItem { Text = Enum.GetName(typeof(Roles), eVal), Value = eVal.ToString() }).ToList();

                List<string> roleList =
                (from role in roles
                 where role.Text == Roles.AdminUser.ToString() || role.Text == Roles.BackOfficeUser.ToString()
                 select role.Text).ToList();

                List<ExclusiveUser> users = await _userService.GetAllUsers(roleList);

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
                {
                    users = users.Where(x =>
                        x.Email == email &&
                        x.UserName == username).ToList();
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    users = users.Where(x =>
                        x.UserName.Contains(username)).ToList();
                }
                else if (!string.IsNullOrEmpty(email))
                {
                    users = users.Where(x =>
                        x.Email.Contains(email)).ToList();
                }

                if (users != null && users.Count > 0)
                {
                    foreach (ExclusiveUser user in users)
                    {
                        List<string> rolelst = await _userService.GetRolesAsync(user);
                        string roleAvailable = rolelst.FirstOrDefault(x => x.Contains(Roles.AdminUser.ToString()) || x.Contains(Roles.BackOfficeUser.ToString()));
                        UserSummary summary = new UserSummary
                        {
                            Id = user.Id,
                            Username = user.UserName,
                            Email = user.Email,
                            Role = rolelst.Any(x => x == Roles.AdminUser.ToString())
                                ? Keys.Keys.AdminUser
                                : Keys.Keys.BackOfficeUser,
                            RoleId = (int)Enum.Parse(typeof(Roles), roleAvailable)
                        };
                        model.Add(summary);
                    }
                }
                return PartialView("_users", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while retreiving users. Please try again."));
            }
        }

        [HttpGet]
        [ActionName("Add")]
        public IActionResult Add()
        {
            try
            {
                UserViewModel model = new UserViewModel();
                ViewBag.FormTitle = "Add User";
                return View("User", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(Guid userId)
        {
            try
            {
                UserViewModel model = new UserViewModel();
                
                ExclusiveUser user = await _userService.FindByIdAsync(userId.ToString());
                List<string> rolelst = await _userService.GetRolesAsync(user);
                if (user != null)
                {
                    model.Id = user.Id;
                    model.Email = user.Email;
                    model.ConfirmEmail = user.Email;
                    model.Username = user.UserName;
                    model.RoleId = rolelst.FirstOrDefault(x => x != Roles.User.ToString());
                    model.LockoutDate = user.LockoutEnd;
                }
                ViewBag.FormTitle = "Edit User";
                return View("User", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("SaveUser")]
        public async Task<IActionResult> SaveUser(UserViewModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    StringBuilder sbErrors = new StringBuilder();

                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            sbErrors.Append(error.ErrorMessage);
                            sbErrors.Append("<br/>");
                        }
                    }
                    return Json(JsonResponse<string>.ErrorResponse(sbErrors.ToString()));
                }

                ExclusiveUser iUser = await _userService.FindByEmailAsync(user.Email);
                if (iUser != null && string.IsNullOrEmpty(user.Id))
                {
                    return Json(JsonResponse<string>.ErrorResponse("User already exists with this email address."));
                }

                bool result = false;
                //await UserHelper.SaveUser(_userService, user);
                if (!string.IsNullOrEmpty(user.Id) && iUser != null)
                {
                    iUser = await _userService.FindByIdAsync(user.Id);
                    if (iUser != null)
                    {
                        bool emailUpdated = true;
                        bool usernameUpdated = true;
                        bool updateRole = true;
                        bool passwordUpdated = true;
                        if (!Equals(iUser.Email, user.Email))
                        {
                            emailUpdated = await _userService.SetEmailAsync(iUser, user.Email);
                        }
                        if (!Equals(iUser.UserName, user.Username))
                        {
                            usernameUpdated = await _userService.SetUserNameAsync(iUser, user.Username);
                        }
                        var rolelst = await _userService.GetRolesAsync(iUser);

                        if (!rolelst.Contains(user.RoleId))
                        {
                            updateRole = await _userService.UpdateRoleAsync(iUser, rolelst.FirstOrDefault(x => x != Roles.User.ToString()), user.RoleId);
                        }

                        if (!string.IsNullOrEmpty(user.Password) && !string.IsNullOrEmpty(user.ConfirmPassword))
                        {
                            passwordUpdated = await _userService.AddNewPassword(iUser, user.Password);
                        }

                        if (emailUpdated && usernameUpdated && updateRole && passwordUpdated)
                        {
                            result = true;
                        }
                    }
                }
                else
                {
                    iUser = new ExclusiveUser
                    {
                        Email = user.Email,
                        UserName = user.Username,
                        LockoutEnabled = true
                    };
                    string role = user.RoleId;

                    result = await _userService.CreateAsync(iUser, user.Password, role);
                }
                if (result)
                {
                    return Json(JsonResponse<bool>.SuccessResponse(true));
                }
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while saving user. Please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while saving user. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("DisableUser")]
        public async Task<IActionResult> DisableUser(UserViewModel user)
        {
            try
            {
                ExclusiveUser iUser = await _userService.FindByIdAsync(user.Id);
                IdentityResult result = await _userService.SetLockoutEndDateAsync(iUser, DateTime.UtcNow);
                return result.Succeeded
                    ? Json(JsonResponse<bool>.SuccessResponse(true))
                    : Json(JsonResponse<string>.ErrorResponse("Error disabling user. Please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error disabling user. Please try again."));
            }
        }
    }
}
