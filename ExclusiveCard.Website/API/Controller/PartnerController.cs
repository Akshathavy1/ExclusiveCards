using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ExclusiveCard.Services.Interfaces;

namespace ExclusiveCard.Website.API.Controller
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : BaseController
    {
        #region PrivateMember

        private readonly IPartnerService _partnerService;
        private readonly ICustomerAccountService _customerAccountService;

        #endregion

        #region Constructor

        public PartnerController(IPartnerService PartnerService, ICustomerAccountService customerAccountService)
        {
            _partnerService = PartnerService;
            _customerAccountService = customerAccountService;
        }

        #endregion

        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                string audience = Request.Host.ToString();
                var partnerLoginResponse = await _partnerService.LoginAsync(userName, password, audience);
                if (partnerLoginResponse == null)
                {
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized);
                }
                else
                {
                    Response.Headers.Add("Authorization", "Bearer: " + partnerLoginResponse);
                    return Ok();
                }
 
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized);
            }
        }


        [Filters.PartnerAuthorise]
        [HttpGet]
        [Route("Customer/SignOn")]
        public async Task<IActionResult> CustomerSignOn(string customerUserName)
        {
            try
            {
                string name = User.Identity.Name;
                string errorMsg = null;

                var result = GetLoginToken(out string token, out string audience);
                if (!result)
                    errorMsg = "Customer SignOn Failed - token not found";

                if (errorMsg == null)
                {
                    //if (!User.Identity.IsAuthenticated)
                    errorMsg = await _partnerService.CustomerSignInAsync(customerUserName, token);
                }

                if (errorMsg == null)
                    return Ok();
                else
                {
                    Logger.Warn(errorMsg);
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized);
            }
        }

        /// <summary>
        /// This method enables a white label partner to redirect a customer to a page on the Exclusive Rewards site
        /// The partner would use the PartnerLogin and CustomerSignIn Async calls to retrieve the token and the customer's authentication cookie
        /// Then they would redirect their customer to this endpoint to load the page
        /// This method validates the token. If valid, the auth cookie is added to the customer's request and then a redirect request is made to the 
        /// supplied URL.
        /// NOTE - this method must be called without the PartnerAuthorise filter.  The bearer token is included as a param instead of in the header
        /// to make it easier to create a redirect on the client side. 
        /// </summary>
        /// <param name="url">Page on the Exclusive Rewards site to redirect to. This must be a relative page</param>
        /// <param name="cookie">The authentication cookie that signs a customer into the site</param>
        /// <param name="token">A JWT authentication token that validates the Partner is logged in and allowed to call the Partner API</param>
        /// <returns>A RedirectResult instance of IActionResult, set with the supplied url.</returns>
        [HttpGet]
        [Route("Customer/Redirect")]
        public IActionResult CustomerRedirect(string url, string cookie, string token)
        {
            // Manually validate the token 
            token = token.StartsWith("Bearer") ? token.Substring(7).TrimStart() : token;
            var result = _partnerService.ValidateLoginToken(token, HttpContext.Request.Host.ToString());

            if (result)
            {
                // Add the cookie into the request
                string name = cookie.Substring(0, cookie.IndexOf('='));
                string value = cookie.Substring(cookie.IndexOf('=') + 1, cookie.IndexOf(';') - cookie.IndexOf('=') - 1);
                var option = new CookieOptions() { SameSite = SameSiteMode.None };
                HttpContext.Response.Cookies.Append(name, value, option);

                // Redirect to the required page
                string redirectUrl = Request.Scheme + "://" + Request.Host.Value + Url.Content("~") + url;
                return new RedirectResult(url);
            }
            else
                return new ForbidResult();
        }

        [Filters.PartnerAuthorise]
        [HttpPost]
        [Route("Customer/CreateAccount")]
        public IActionResult CreateCustomerAccount( [FromBody] CustomerAccountDto customerAccountDto)
        {
            try
            {
                string confirmUrl = Request.Scheme + "://" + Request.Host.Value + Url.Content("~/Account/Confirm");
                _customerAccountService.CreateAccountFromRegistrationCode(customerAccountDto, customerAccountDto.RegistrationCode, confirmUrl);
                return Ok();
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Unable to create customer account");
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            
        }

        [Filters.PartnerAuthorise]
        [HttpGet]
        [Route("Customer/Balances")]
        public async Task<IActionResult> GetCustomerAccountBalances(string customerUserName)
        {
            string errorMsg = null;
            Tuple<string, MembershipCard> customerCard = null;
            CustomerBalances summary = null;

            try
            {
                var result = GetLoginToken(out string token, out string audience);
                if (!result)
                    errorMsg = "Get Customer Balances Failed - token not found";

                if (errorMsg == null)
                {
                    customerCard = await _partnerService.ValidateCustomerAndPartnerAsync(customerUserName, token);
                    errorMsg = customerCard?.Item1;
                }

                if (errorMsg == null)
                {
                    var card = customerCard.Item2;
                    summary = _customerAccountService.GetBalances((int)card.CustomerId, (int)card.PartnerRewardId);
                }

                if (summary != null)
                    return Ok(summary);
                else
                    throw new Exception("GetBalances failed");

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to get customer account balances for customer " + customerUserName + errorMsg ?? string.Empty);
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            
            //TODO: CustBalances 2. Delete old stored proc
            //TODO: CustBalances 3. Refactor MyAccountController to use customerService.GetAccountBalances method
            //TODO: CustBalances 4. Delete the obsolete balance code from the services and data layers.


        }

        #region private methods

        private bool GetLoginToken(out string token, out string audience)
        {
            token = null;
            string bearerToken = String.Empty;
            var request = HttpContext.Request;

            audience = request.Host.ToString();

            if (!request.Headers.ContainsKey("Authorization") || string.IsNullOrEmpty(request.Headers["Authorization"]))
            {
                return false;
            }

            bearerToken = request.Headers["Authorization"];
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        #endregion

    }
}