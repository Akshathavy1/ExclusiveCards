using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;
using ExclusiveCard.WebAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "RootUser, AdminUser, BackOfficeUser")]
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;


        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ExclusiveUser currentUser = await _userService.GetUserAsync(HttpContext.User);
                var existingRole = await _userService.GetRolesAsync(currentUser);
                HttpContext.Session.SetString("ID", currentUser.Id);
                HttpContext.Session.SetString("User", currentUser.NormalizedUserName);
                HttpContext.Session.SetString("Role", existingRole.FirstOrDefault(x => x != Roles.User.ToString()));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
