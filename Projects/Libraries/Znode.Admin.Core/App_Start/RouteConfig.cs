using System.Web.Mvc;
using System.Web.Routing;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("fonts/{*pathInfo}");

            if (ZnodeAdminSettings.DisableRoutesForStaticFile)
            {
                routes.IgnoreRoute("Data/{*pathInfo}");
                routes.IgnoreRoute("Content/css");
                routes.IgnoreRoute("MediaFolder/{*pathInfo}");
                routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });
            }
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
