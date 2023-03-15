using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Website.Helpers;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using NLog;

namespace ExclusiveCard.Website.Controllers
{
    [Route("saml2")]
    public class SSOController : BaseController
    {
        #region Private Members

        private readonly IMemoryCache _cache;
        private readonly ISSOService _ssoService;
        private readonly IConfiguration _configuration;
        private readonly ICustomerAccountService _customerAccountService;
        private readonly ILogger _logger;

        #endregion Private Members

        #region Constructor

        public SSOController(IMemoryCache cache, ISSOService ssoService, IConfiguration configuration,
               IUserService userService, ICustomerAccountService customerAccountService)
        {
            _cache = cache;
            _ssoService = ssoService;
            _configuration = configuration;
            _customerAccountService = customerAccountService;
            _logger = LogManager.GetLogger("databaseLogger");
        }

        #endregion Constructor

        [HttpGet]
        [ActionName("SSORedirect")]
        public async Task<IActionResult> SSORedirect(string userId, string email, int ssoConfigId, string productCode, string merchantName)
        {
            try
            {
                var customer = _customerAccountService.GetCustomer(userId);

                var ssoConfiguration = await _ssoService.GetSSOConfiguration(ssoConfigId);
                var ssoViewModel = new SSOViewModel
                {
                    CustomerId = customer.Id,
                    SSOConfigId = ssoConfigId,
                    OfferUrl = "",
                    ProductCode = productCode,
                    ReturnUrl = "",
                    MerchantName = merchantName,
                    AcsUrl = ssoConfiguration.DestinationUrl
                };

                var ssoResponse = await _ssoService.ProcessSSO(ssoConfigId, customer, email, productCode);
                //var ssoResponse = await _ssoService.ProcessSSO(_configuration[$"SAML2:Issuer{ssoConfiguration.Id}"], metaData, ssoConfiguration?.Url, certificate, false, true, true, attributes);
                ssoViewModel.SamlResponseBase64 = ssoResponse;
                return View("SSORedirect", ssoViewModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SSORedirect was unable to transfer customer");

                //return BadRequest($"SSORedirect: {ex.Message}");
                return View("Error");
            }
        }

        //SSO Return method. Closes the tab instance</returns>
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> SSOReturn()
        {
            await Task.CompletedTask;

            return View("Close");
        }
    }
}