using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Website.Models;
using ExclusiveCard.Website.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerAccountService _customerAccountService;

        public UserController(
            ICustomerService customerService,
            ICustomerAccountService customerAccountService)
        {
            _customerService = customerService;
            _customerAccountService = customerAccountService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var response = await _customerAccountService.Login(loginRequest.UserName, loginRequest.Password);
            if (response.Id > 0)
            {
                HttpRequestMessage httpRequestMessage = HttpContext.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
                string remoteIp = GetUserIp(httpRequestMessage);
                long loginTimeoutMinutes = (long)TimeSpan.FromDays(90).TotalMinutes;
                LoginResponse userLogin = await GetUserLoginViewModel(loginRequest.UserName, loginRequest.Password, loginTimeoutMinutes, remoteIp, loginRequest.AppId);
                return Ok(userLogin);
            }
            else
                return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login/Validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateLoginRequest validateLoginRequest)
        {
            var usertoken =  _customerService.GetUserTokenByTokenValue(validateLoginRequest.Token);
            
            if (usertoken == null)
                return Unauthorized();

            if (string.IsNullOrEmpty(usertoken.Token)) 
                return Unauthorized();

            LoginToken loginToken = LoginToken.FromEncryptedString(usertoken.Token);
            if (string.IsNullOrEmpty(loginToken?.UserId))
                return Unauthorized();

            var user = await _customerAccountService.GetUserAsync(loginToken.UserId);
            if (!string.Equals(validateLoginRequest.UserName, user?.UserName,
                    StringComparison.CurrentCultureIgnoreCase)
                || loginToken.ExpiresTimestamp < DateTime.UtcNow.Ticks)
            {
                return Unauthorized();
            }

            var customer =  _customerAccountService.GetCustomer(user.Id);
            var summary = _customerAccountService.GetAccountSummary(customer.Id);
            
            UserValidation userVal = new UserValidation
            {
                AppId = validateLoginRequest.AppId,
                CustomerName = $"{customer?.Forename} {customer?.Surname}"
            };
            
            if (summary.IsDiamondCustomer)
            {
                userVal.CardNumber = summary.CardNumber;
                userVal.ExpiryDate = TimeZoneInfo.ConvertTimeFromUtc(summary.CardExpiryDate, TimeZoneInfo.Local).ToString("dd MMM yyyy");
            }

            return Ok(userVal);
        }

        /// <summary>
        /// This method is being called to validate the registration code provided by the user while Joining Exclusive, 
        /// Since it is a ajax call, we need to have this method in public access specifier
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("RegisterValidation")]
        public IActionResult RegisterValidation(string code)
        {
            //Method to Validate registrationCode
            try
            {
                dto.UserToken userToken = _customerAccountService.ValidateRegistrationCode(code);
                if (userToken != null)
                {
                    //return Json(JsonResponse<object>.SuccessResponse(userToken));
                    return Ok(JsonResponse<object>.SuccessResponse(userToken));
                }
                //TODO:  Localisation of error messages
                //return Json(JsonResponse<string>.ErrorResponse("We couldn't find an account matching the registration code you entered. Please check your registration code and try again."));
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //return Json(JsonResponse<string>.ErrorResponse("Error while validate register. Please try again."));
                return Unauthorized();
            }
        }


        private string GetUserIp(HttpRequestMessage request)
		{
			if (request != null && request.Properties.ContainsKey("MS_HttpContext"))
			{
				dynamic ctx = request.Properties["MS_HttpContext"];
				if (ctx != null)
				{
					return ctx.Request.UserHostAddress;
				}
			}

			return null;
		}

		private async Task<LoginResponse> GetUserLoginViewModel(string userName, string password, long loginTimeOutMinutes, string remoteIp, string appId)
		{
			DateTime expiryDate = DateTime.UtcNow.AddMinutes(loginTimeOutMinutes);

			// userId is username
			LoginToken token = new LoginToken()
			{
				UserId = userName,
				Password = password,
				ExpiresTimestamp = expiryDate.Ticks,
				RemoteIp = remoteIp
			};
			string authToken = token.ToEncryptedString();
			
            var user = await _customerAccountService.GetUserAsync(userName);
			Guid tokenGuid = Guid.NewGuid();
			try
			{
				await _customerService.AddLoginToken(new dto.LoginUserToken()
				{
					AspNetUserId = user.Id,
					Token = authToken,
					TokenValue = tokenGuid
				});
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
			return new LoginResponse
			{
				Token = tokenGuid,
				Timestamp = expiryDate,
				UserName = userName,
				AppId = appId
			};
		}
	}
}