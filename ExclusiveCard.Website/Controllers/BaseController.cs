using Microsoft.AspNetCore.Mvc;

namespace ExclusiveCard.Website.Controllers
{
    public class BaseController : Controller
    {       
        public readonly NLog.ILogger Logger;

        public BaseController()
        {
            Logger = NLog.LogManager.GetLogger(GetType().FullName);
        }
    }
}