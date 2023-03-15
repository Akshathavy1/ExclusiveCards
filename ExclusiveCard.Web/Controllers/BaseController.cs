using Microsoft.AspNetCore.Mvc;
using NLog;

namespace ExclusiveCard.WebAdmin.Controllers
{
    public class BaseController : Controller
    {
        public readonly ILogger Logger;

        public BaseController()
        {
            Logger = LogManager.GetLogger(GetType().FullName);
        }
    }
}
