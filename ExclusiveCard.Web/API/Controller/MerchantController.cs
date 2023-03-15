using System;
using System.Threading.Tasks;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.Helpers;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExclusiveCard.WebAdmin.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class MerchantController : BaseController
    {
        #region PrivateMember

        private readonly IMerchantService _merchantService;
        private readonly IMerchantImageService _merchantImageService;
        private readonly IAffiliateMappingRuleService _affiliateMappingRuleService;
        private readonly IAffiliateMappingService _affiliateMappingService;
        private readonly IAzureStorageProvider _azureStorageProvider;

        #endregion

        #region Constructor

        public MerchantController(IMerchantService merchantService, IMerchantImageService merchantImageService, 
            IAffiliateMappingRuleService affiliateMappingRuleService, IAffiliateMappingService affiliateMappingService,
            IAzureStorageProvider azureStorageProvider)
        {
            _merchantService = merchantService;
            _merchantImageService = merchantImageService;
            _affiliateMappingRuleService = affiliateMappingRuleService;
            _affiliateMappingService = affiliateMappingService;
            _azureStorageProvider = azureStorageProvider;
        }

        #endregion

        // GET: api/<controller>
        [HttpGet]
        [Route("ImportMerchantImage")]
        public async Task<IActionResult> ImportMerchantImage()
        {
            //Import Merchant Image from StrackerAPI Json File
            try
            {
                CommonHelper.Initialize();
                bool response = await MerchantHelper.GetJsonAndMapToImportMerchantImage(_merchantService, _merchantImageService, _affiliateMappingRuleService, _affiliateMappingService, _azureStorageProvider);
                if (response)
                {
                    return Ok(JsonResponse<Object>.SuccessResponse(true));
                }
                return Ok(JsonResponse<Object>.ErrorResponse("Merchant json file name already exists."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Ok(JsonResponse<Object>.ErrorResponse("Merchant image upload failed"));
            }
        }
    }
}
