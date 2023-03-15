using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Providers.Email;


namespace ExclusiveCard.WebAdmin.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class MaintenanceController : BaseController
    {
        #region Private Members

        private readonly IMaintenanceService _maintenanceService;

        #endregion
        #region Constructor

        public MaintenanceController(IMaintenanceService maintenanceService){
            _maintenanceService = maintenanceService;
          
        }
        #endregion

        [HttpPost]
        [Route("Daily")]

        public async Task<IActionResult> Daily()
        {
            try
            {
                await _maintenanceService.ExecuteSPRunDailyMaintenanceTasks();
                return Ok(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(JsonResponse<string>.ErrorResponse("Error occurred .please try again later"));
            }
        }


    }
}
