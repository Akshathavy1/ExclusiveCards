using Microsoft.AspNetCore.Mvc;
using NLog;

namespace ExclusiveCard.Website.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public readonly ILogger Logger;

        public BaseController()
        {
            Logger = LogManager.GetLogger(GetType().FullName);
        }
    }
}