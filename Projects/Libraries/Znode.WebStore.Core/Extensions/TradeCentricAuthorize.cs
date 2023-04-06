using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Znode.WebStore.Core.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TradeCentricAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool isAuthenticated = filterContext.HttpContext.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "", action = "", area = "" }));
            }
        }
    }
}
