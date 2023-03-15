using ExclusiveCard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.Extensions.Options;


namespace ExclusiveCard.WebAdmin.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerReportController : BaseController
    {
        #region Private Members
        
        private readonly ISFTPProvider _sftpProvider;
        private readonly IPartnerService _partnerService;
        private readonly IOptions<TypedAppSettings> _settings;

        #endregion

        #region Constructor

        public PartnerReportController(ISFTPProvider sftpProvider,
            IPartnerService partnerService,
            IOptions<TypedAppSettings> settings)
        {
            _sftpProvider = sftpProvider;
            _partnerService = partnerService;
            _settings = settings;
        }

        #endregion

        [HttpPost]
        [Route("SendPartnerReport")]
        public async Task<IActionResult> SendPartnerReport(int partnerId)
        {
            try
            {
                string response = await _partnerService.SendPartnerReport(partnerId, _settings.Value.AdminEmail,
                    _settings.Value.TAM_FolderIN, _settings.Value.BlobConnectionString, _settings.Value.PartnerContainerName,
                    _settings.Value.Ftp_ServerUri, _settings.Value.Ftp_Username, _settings.Value.Ftp_Password);
                bool success = false;

                bool.TryParse(response, out success);
                return !success
                    ? response == "No data found to create file."
                        ? (IActionResult) NotFound(response)
                        : BadRequest(response)
                    : Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("ProcessPartnerReport")]
        public async Task<IActionResult> ProcessPartnerReport(int partnerId)
        {
            try
            {
                string response = await _partnerService.ProcessPartnerReport(partnerId, _settings.Value.AdminEmail,
                    _settings.Value.TAM_FolderOUT, _settings.Value.BlobConnectionString,
                    _settings.Value.PartnerContainerName, _settings.Value.Blob_Processing, _settings.Value.Blob_Error,
                    _settings.Value.Blob_Processed, _settings.Value.Ftp_ServerUri, _settings.Value.Ftp_Username, _settings.Value.Ftp_Password);
                bool success = false;

                bool.TryParse(response, out success);
                return !success
                    ? (response == "Partner Id not found." || response == "Invalid Partner Id, no record found.")
                        ? (IActionResult)NotFound(response)
                        : BadRequest(response)
                    : Ok();
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("ProcessPartnerPositionFile")]
        public async Task<IActionResult> ProcessPartnerPositionFile()
        {
            try
            {
                string response = await _partnerService.ProcessPartnerPositionFile(_settings.Value.AdminEmail,
                    _settings.Value.BlobConnectionString, _settings.Value.PartnerContainerName,
                    _settings.Value.Blob_Processing, _settings.Value.TAM_FolderPosition, _settings.Value.Blob_Processed,
                    _settings.Value.Blob_Error, _settings.Value.Ftp_ServerUri, _settings.Value.Ftp_Username,
                    _settings.Value.Ftp_Password);
                bool success = false;

                bool.TryParse(response, out success);
                return !success
                    ? (response == "No partner record found with name - TAM." || response == "File Status not found." || response == "No file found to process in the server.")
                        ? (IActionResult)NotFound(response)
                        : BadRequest(response)
                    : Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return BadRequest($"Error occured while processing Tam position file.");
            }
        }

        [HttpPost]
        [Route("SendPartnerWithdrawalReport")]
        public async Task<IActionResult> SendPartnerWithdrawalReport()
        {
            try
            {
                string response = await _partnerService.SendPartnerWithdrawalReport(_settings.Value.AdminEmail,
                    _settings.Value.TAM_FolderIN, _settings.Value.BlobConnectionString, _settings.Value.PartnerContainerName,
                    _settings.Value.Ftp_ServerUri, _settings.Value.Ftp_Username, _settings.Value.Ftp_Password);
                bool success = false;

                bool.TryParse(response, out success);
                return !success
                    ? (response == "TAM dataModels found with no record." || response == "TAM Partner not found." || response == "No record found in Partner Reward Withdrawal with pending status")
                        ? (IActionResult)NotFound(response)
                        : BadRequest(response)
                    : Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("TestConnection")]
        public IActionResult TestConnection()
        {
            try
            {
                //var result = _sftpProvider.TestConnection();
                //if (!result)
                //{
                //    return BadRequest();
                //}
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}