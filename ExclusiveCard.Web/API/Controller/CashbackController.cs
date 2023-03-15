using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;


namespace ExclusiveCard.WebAdmin.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class CashbackController : BaseController
    {
        #region Private Members

        private readonly IOptions<TypedAppSettings> _settings;
        private readonly ICashbackService _cashbackService;

        #endregion

        #region Constructor

        public CashbackController(IOptions<TypedAppSettings> settings,
            ICashbackService cashbackService)
        {
            _settings = settings;
            _cashbackService = cashbackService;
        }

        #endregion

        [HttpGet]
        [Route("GetTransactionReport")]
        public IActionResult GetTransactionReport(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                int transactionReport =  _cashbackService.GetTransactionReport(dateFrom, dateTo, _settings.Value.StrackrAPI_id, _settings.Value.StrackrAPI_key);
                return Ok();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("MigrateStagingData")]
        public async Task<IActionResult> MigrateCashbackTransactions()
        {
            try
            {
                string response = await _cashbackService.MigrateCashbackTransactions(_settings.Value.AdminEmail,
                    _settings.Value.CashbackConfirmedInDays);

                bool success = false;

                bool.TryParse(response, out success);

                if (success)
                {
                    Ok();
                }
                else
                {
                    BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return Ok(JsonResponse<Object>.SuccessResponse(true));
        }
    }
}