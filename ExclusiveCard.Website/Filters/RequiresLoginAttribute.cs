using System;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ExclusiveCard.Website.Filters
{
    public class RequiresLoginAttribute : ActionFilterAttribute
    {

        private readonly SignInManager<ExclusiveUser> _signInManager;

        public RequiresLoginAttribute(SignInManager<ExclusiveUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task OnActionExecutingAsync(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var tokenDetails = App.ServiceHelper.Instance.CustomerService.GetUserTokenByTokenValue(new Guid(token));
                var user = await App.ServiceHelper.Instance.UserService.FindByIdAsync(tokenDetails.AspNetUserId);
                await _signInManager.SignInAsync(user, false);
            }
        }
    }
}
