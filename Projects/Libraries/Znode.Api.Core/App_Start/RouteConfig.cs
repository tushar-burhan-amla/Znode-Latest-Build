using System.Web.Mvc;
using System.Web.Routing;
using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Engine.Api
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            if (ZnodeApiSettings.DisableRoutesForStaticFile)
            {
                routes.IgnoreRoute("Data/{*pathInfo}");
            }

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
