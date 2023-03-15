using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExclusiveCard.WebAdmin.Controllers
{
    public class ErrorsController : BaseController
    {
        [Route("Error/500")]
        public IActionResult Error500()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                Logger.Error(exceptionFeature.Error);
            }
            ViewBag.ErrorMessage = "Sorry something went wrong on the server";

            return View("Error");
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                Logger.Error(exceptionFeature.Error);
            }

            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry the page you requested could not be found";
                    ViewBag.RouteOfException = statusCodeData.OriginalPath;
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Sorry something went wrong on the server";
                    ViewBag.RouteOfException = statusCodeData.OriginalPath;
                    break;
            }

            return View("Error");
        }
    }
}
