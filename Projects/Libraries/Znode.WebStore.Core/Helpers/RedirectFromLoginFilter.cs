using System;
using System.Web.Mvc;
using Znode.Engine.WebStore.Controllers;
using Znode.Libraries.ECommerce.Utilities;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore
{
    public class RedirectFromLogin : ActionFilterAttribute, IActionFilter
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (BaseController)filterContext.Controller;
            if (IsRedirectFromLogin(filterContext) && IsLoginRequired())
                filterContext.Result = controller.RedirectToAction(ZnodeWebstoreSettings.HomeAction, ZnodeWebstoreSettings.HomeController);
            base.OnActionExecuting(filterContext);
        }

        private static bool IsLoginRequired()
        {
            return Convert.ToBoolean(GetService<IAuthenticationHelper>().IsAuthorizationMandatory());
        }

        public bool IsRedirectFromLogin(ActionExecutingContext filterContext)
        {
            return filterContext.HttpContext.Request.UrlReferrer?.AbsolutePath.Equals(ZnodeWebstoreSettings.LoginPageUrl) ?? false;
        }
    }
}
