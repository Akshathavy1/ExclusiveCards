using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class SessionTimeout : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session == null ||
                !context.HttpContext.Session.TryGetValue("ID", out byte[] val))
            {
                context.Result =
                    new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Index"
                    }));
            }
            base.OnActionExecuting(context);
        }
    }
}
