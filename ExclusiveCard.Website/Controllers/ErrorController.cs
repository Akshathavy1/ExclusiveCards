using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System;

namespace ExclusiveCard.Website.Controllers
{
    public class ErrorController : BaseController
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            try
            {
                var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
                Logger.Error(exception);
            }
            catch (Exception)
            { }

            return View("Error");
        }
                
    }
}
