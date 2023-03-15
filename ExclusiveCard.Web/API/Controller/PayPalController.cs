using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExclusiveCard.WebAdmin.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class PayPalController : BaseController
    {
        #region Private Members

        private readonly IPayPalService _payPalService;

        #endregion

        #region Constructor

        public PayPalController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        #endregion

        [HttpPost]
        [Route("IPN")]
        public async Task<IActionResult> IPN()
        {
            try
            {
                Logger.Debug("IPN received.");
                Services.Models.DTOs.IPNContext ipnContext = new Services.Models.DTOs.IPNContext()
                {
                    IPNRequest = Request
                };

                using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII))
                {
                    //ipnContext.RequestBody = reader.ReadToEnd();
                    ipnContext.RequestBody = await reader.ReadToEndAsync();
                }
                Logger.Debug($"IPN Data - {ipnContext.RequestBody}");

                //Fire and forget verification task
                //await Task.Run(() => VerifyTask(ipnContext, paymentNotification));

                //IPNPayPalHelper.VerifyTask(ipnContext); -- comment on 18 Feb 2018 as per new customization requirement

                //Get admin email Id to send out mail when IPN processing fails
                string adminEmail = App.ServiceHelper.Instance.Settings.Value.AdminEmail;

                //Store the IPN received from PayPal
                if (!string.IsNullOrEmpty(ipnContext.RequestBody))
                {
                    await _payPalService.ProcessIPN(ipnContext.RequestBody, adminEmail);
                }
                else
                {
                    Logger.Error($"Request body is empty in {ipnContext}, body {ipnContext.RequestBody}");
                    return NoContent();
                }

                //Reply back a 200 code
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                if (ex.Message == "Transaction already found in customer payment")
                    return Ok(JsonResponse<string>.ErrorResponse("Transaction already found in customer payment"));
                return Ok(JsonResponse<string>.ErrorResponse("Error occurred while transfer file."));
            }
        }
    }
}
