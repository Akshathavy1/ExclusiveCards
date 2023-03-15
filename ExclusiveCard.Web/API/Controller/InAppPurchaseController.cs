using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using admin = ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExclusiveCard.WebAdmin.ViewModels;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Enums;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;
using NLog;

namespace ExclusiveCard.WebAdmin.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class InAppPurchaseController : BaseController
    {

        #region Private Members

        //private readonly ILogger _logger;

        private readonly admin.IInAppPurchaseService _inAppPurchaseService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAccountService _customerAccountService;

        #region Apple
        private const string AppleProductionUrl = "https://buy.itunes.apple.com/verifyReceipt";
        private const string AppleTestUrl = "https://sandbox.itunes.apple.com/verifyReceipt";
        //private static HttpClient _client = new HttpClient();
        //private static JsonSerializer _serializer = new JsonSerializer();
        #endregion

        #endregion

        #region Constructor

        public InAppPurchaseController(admin.IInAppPurchaseService inAppPurchaseService, ICustomerService customerService,
            ICustomerAccountService customerAccountService)
        {
            _inAppPurchaseService = inAppPurchaseService;
            _customerService = customerService;
            _customerAccountService = customerAccountService;
        }

        #endregion

        //[HttpPost]
        //[Route("Purchase")]
        //public async Task<IActionResult> AuditPurchase([FromBody] dto.PurchaseResult purchaseResult)
        //{
        //    #region check this is a valid call
        //    if (purchaseResult == null)
        //        return NoContent();

        //    var usertoken = _customerService.GetUserTokenByTokenValue(purchaseResult.UserToken);
        //    if (usertoken == null)
        //        return Unauthorized();
        //    if (string.IsNullOrEmpty(usertoken.Token))
        //        return Unauthorized();

        //    LoginToken loginToken = LoginToken.FromEncryptedString(usertoken.Token);
        //    if (string.IsNullOrEmpty(loginToken?.UserId))
        //        return Unauthorized();

        //    var user = await _customerAccountService.GetUserAsync(loginToken.UserId);
        //    if (user == null || user.UserName == null)
        //    {
        //        return Unauthorized();
        //    }
        //    #endregion

        //    var customer = _customerAccountService.GetCustomer(user.Id);
        //    if (customer == null)
        //    {
        //        return Unauthorized();
        //    }

        //    dto.PurchaseResult purchase = purchaseResult;
        //    purchase.CustomerId = customer.Id;
        //    try
        //    {
        //        purchase.PaymentProvider = (int)Enum.Parse<PaymentProvider>(purchaseResult.Platform);
        //    }
        //    catch
        //    {
        //        //this is not from a supported app platform
        //        return Unauthorized();
        //    }

        //    //Save Purchase Result to audit records now !!!!!!!    
        //    var reuslt = _inAppPurchaseService.ProcessInAppPurchase(purchase);
        //    if (reuslt != 0)
        //        return Ok(JsonResponse<object>.SuccessResponse("Done"));
        //    else
        //        return BadRequest();
        //}


        #region Apple Validation

        [HttpPost]
        [Route("ios")]
        public async Task<HttpResponseMessage> InAppValidateios([FromBody] dto.AppleReceipt receipt)
        {
            if (string.IsNullOrEmpty(receipt.Id) || string.IsNullOrEmpty(receipt.TransactionId) || (receipt.Data == null && receipt.Data2 == null))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            Logger.Info($"IAP receipt: {receipt.Id}, {receipt.TransactionId}");

            var result = await _inAppPurchaseService.PostAppleReceipt(true, receipt);

            //Apple recommends calling production, then falling back to sandbox on an error code
            if (result == null || result.Status == AppleStatus.TestEnvironment)
            {
                Logger.Info("Sandbox purchase, calling test environment...");
                result = await _inAppPurchaseService.PostAppleReceipt(false, receipt);
            }

            if (result?.Status == AppleStatus.Success)
            {
                if (result.Receipt == null)
                {
                    Logger.Error($"IAP {receipt.Id} invalid, no receipt returned!");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                string bundleId = result.Receipt.Property("bundle_id")?.Value.Value<string>();
                if (receipt.BundleId != bundleId)
                {
                    Logger.Error($"IAP {receipt.Id} invalid, bundle id {bundleId} does not match {receipt.BundleId}!");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                var purchases = result.Receipt.Property("in_app")?.Value.Value<JArray>();
                if (purchases == null || purchases.Count == 0)
                {
                    Logger.Error($"IAP {receipt.Id} invalid, no purchases returned!");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                var purchase = purchases.OfType<JObject>()
                    .FirstOrDefault(p => p.Property("product_id")?.Value.Value<string>() == receipt.Id);
                if (purchase == null)
                {
                    Logger.Error($"IAP {receipt.Id} invalid, did not find in list of purchases!");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                purchase = purchases.OfType<JObject>().FirstOrDefault(p =>
                    p.Property("product_id")?.Value.Value<string>() == receipt.Id &&
                    p.Property("transaction_id")?.Value.Value<string>() == receipt.TransactionId);
                if (purchase == null)
                {
                    Logger.Error($"IAP {receipt.Id} invalid, TransactionId did not match!");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //Create Diamond here not in PostAppleReceipt!!!
                var success = await _inAppPurchaseService.ProcessAppleDiamond(result, receipt);
                if (success)
                {
                    Logger.Debug($"IAP Success: {receipt.Id}, {receipt.TransactionId}");
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    Logger.Error($"IAP {receipt.Id}, failed to create Diamond");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                Logger.Error($"IAP {receipt.Id} invalid, status code: {result?.Status}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        #endregion

        #region Android Validation

        [HttpPost]
        [Route("google")]
        public async Task<HttpResponseMessage> InAppValidateGoogle([FromBody] dto.GoogleReceipt receipt)
        {
            //var receipt = await req.Content.ReadAsAsync<dto.GoogleReceipt>();

            if (string.IsNullOrEmpty(receipt.Id) || string.IsNullOrEmpty(receipt.TransactionId) || string.IsNullOrEmpty(receipt.DeveloperPayload))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            Logger.Info($"IAP receipt: {receipt.Id}, {receipt.TransactionId}");

            dto.GoogleResponse googleResponse = null;

            try
            {
              googleResponse = await _inAppPurchaseService.PostGoogleReceipt(receipt);

                if (googleResponse == null)
                {
                    Logger.Error($"IAP {receipt.Id} invalid, no purchase returned from google!");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                if (!googleResponse.PaymentState.HasValue)
                {
                    Logger.Error($"IAP {receipt.Id} invalid, PaymentState has no value - canceled or expired");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception exc)
            {
                Logger.Error($"IAP {receipt.Id} invalid, error reported: {exc.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            //Create Diamond here not in PostGoogleReceipt!!!
            bool success = false;
            if (googleResponse != null)
                success = await _inAppPurchaseService.ProcessGoogleDiamond(googleResponse, receipt);
            if (success)
            {
                Logger.Debug($"IAP Success: {receipt.Id}, {receipt.TransactionId}");
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                Logger.Error($"IAP {receipt.Id}, failed to create Diamond");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

        }

        #endregion
    }
}
