using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Public;
using ExclusiveCard.WebAdmin.Helpers;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAdminUserAccountService _adminUserService;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly IWhiteLabelService _whiteLabelService;

        public AccountController(IAdminUserAccountService adminUserService, IUserService userService, ICustomerAccountService customerAccountService, IWhiteLabelService whiteLabelService)
        {
            _adminUserService = adminUserService;
            _userService = userService;
            _customerAccountService = customerAccountService;
            _whiteLabelService = whiteLabelService;
        }

        [HttpGet]
        [DisplayName("Index")]
        public IActionResult Index()
        {
            ViewBag.Title = "Login";

            return View("Login", new LoginViewModel());
        }

        [HttpPost]
        [DisplayName("Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            try
            {
                if (ModelState.IsValid)
                {
                    SignInResult result = await _adminUserService.Login(model.Username, model.Password);
                    if (result.Succeeded)
                    {
                        return Json(JsonResponse<bool>.SuccessResponse(true)); // RedirectToAction("Index", "Home");
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new
                        {
                            ReturnUrl = returnUrl,
                            //RememberMe = Input.RememberMe
                        });
                    }
                    if (result.IsLockedOut)
                    {
                        //_logger.LogWarning("User account locked out.");
                        return Json(
                            JsonResponse<string>.ErrorResponse(
                                "User account locked out.please try agin after 15 minutes")); //RedirectToPage("./Lockout");
                    }
                }

                // If we got this far, something failed, redisplay form
                return View("Login");
            }
            catch (Exception)
            {
                return Json(JsonResponse<string>.ErrorResponse("Invalid login attempt."));
            }
        }

        [HttpGet]
        [ActionName("LogoutUser")]
        public async Task<IActionResult> LogoutUser(string returnUrl = null)
        {
            await _adminUserService.Logout();

            //_logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return View("Login", new LoginViewModel());
        }

        [Authorize(Roles = "RootUser, AdminUser, BackOfficeUser")]
        [HttpGet]
        [ActionName("ChangePassword")]
        public IActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            return View("ChangePassword", model);
        }

        [Authorize(Roles = "RootUser, AdminUser, BackOfficeUser")]
        [HttpPost]
        [ActionName("SavePassword")]
        public async Task<IActionResult> SavePassword(ChangePasswordViewModel request)
        {
            ExclusiveUser currentUser = await _userService.GetUserAsync(HttpContext.User);

            try
            {
                if (currentUser != null)
                {
                    bool checkpassword = await _userService.CheckPasswordAsync(currentUser, request.OldPassword);
                    if (!checkpassword)
                    {
                        return Json(JsonResponse<string>.ErrorResponse("Old password is wrong."));
                    }
                    await _userService.ChangePasswordAsync(currentUser, request.OldPassword, request.NewPassword);
                }
                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while changing password"));
            }
        }

        [Authorize(Roles = "RootUser, AdminUser, BackOfficeUser")]
        [HttpGet]
        [ActionName("RegisterValidation")]
        public IActionResult RegisterValidation(string code, int id)
        {
            //Method to Validate registrationCode
            try
            {
                var userToken = _customerAccountService.ValidateRegistrationCode(code);
                var whiteLabel = _whiteLabelService.GetSiteSettingsById(id);

                if (userToken != null && whiteLabel != null)
                {
                    string encryptedCode = EncryptionHelper.Encrypt(code);
                    string buildUrl = whiteLabel.URL + "/Account/Register?code=" + encryptedCode;
                    return Json(JsonResponse<object>.SuccessResponse(buildUrl));
                }
                return Json(JsonResponse<string>.ErrorResponse("registration code doesn't currently exist"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("registration code doesn't currently exist."));
            }
        }

        [Authorize(Roles = "RootUser, AdminUser, BackOfficeUser")]
        [HttpGet]
        [ActionName("UpdateRegistrationUrl")]
        public async Task<IActionResult> UpdateRegistrationUrl(string code, int id)
        {
            //Method to Validate registrationCode
            try
            {
                var whiteLabel = _whiteLabelService.GetSiteSettingsById(id);

                if (whiteLabel != null)
                {
                    if (!String.IsNullOrEmpty(code))
                    {
                        var userToken = _customerAccountService.ValidateRegistrationCode(code);
                        if (userToken != null)
                        {
                            string encryptedCode = EncryptionHelper.Encrypt(code);
                            string url = whiteLabel.URL + "/Account/Register?code=" + encryptedCode;
                            whiteLabel.RegistrationUrl = url;
                            var res = await _whiteLabelService.Update(whiteLabel);
                            if (res)
                                return Json(JsonResponse<string>.SuccessResponse("The white label registration URL has been updated"));
                            else
                                return Json(JsonResponse<string>.ErrorResponse("The white label registration URL has not updated."));
                        }
                        else
                        {
                            return Json(JsonResponse<string>.ErrorResponse("Please provide a valid registration code."));
                        }
                    }
                    else
                    {
                        whiteLabel.RegistrationUrl = null;
                        var res = await _whiteLabelService.Update(whiteLabel);
                        if (res)
                            return Json(JsonResponse<string>.SuccessResponse("The white label registration URL has been updated"));
                        else
                            return Json(JsonResponse<string>.ErrorResponse("The white label registration URL has not updated."));
                    }
                }
                return Json(JsonResponse<string>.ErrorResponse("Please provide a valid registration code."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Please provide a valid registration code."));
            }
        }

        [Authorize(Roles = "RootUser, AdminUser, BackOfficeUser")]
        [HttpGet]
        [ActionName("GetRegistrationCode")]
        public IActionResult GetRegistrationCode(int whiteLabelId)
        {
            try
            {
                var whiteLabel = _whiteLabelService.GetSiteSettingsById(whiteLabelId);

                if (whiteLabel != null && whiteLabel.RegistrationUrl != null)
                {
                    string[] words = whiteLabel.RegistrationUrl.Split('=');
                    if (words.Length > 0)
                    {
                        string decryptedCode = EncryptionHelper.Decrypt(words[1]);
                        return Json(JsonResponse<object>.SuccessResponse(decryptedCode));
                    }
                }
                return Json(JsonResponse<string>.ErrorResponse("Registration code does not exist."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                return Json(JsonResponse<string>.ErrorResponse("Registration code does not exist."));
            }
        }

        //private IList<DTOs.WhiteLabelSettings> MapToWhiteLabelSetting(IList<DTOs.WhiteLabelSettings> settings)
        //{
        //    IList<DTOs.WhiteLabelSettings> whiteLabelSetting = new List<DTOs.WhiteLabelSettings>();

        //    foreach (var item in settings)
        //    {
        //        var white = new DTOs.WhiteLabelSettings();
        //        white.Name = item.Name;
        //        white.Id = item.Id;
        //        white.URL = item.URL;
        //        whiteLabelSetting.Add(white);
        //    }
        //    return whiteLabelSetting;
        //}
    }
}