using System;
using System.Web.Mvc;
using System.Web.Routing;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public static class SEORouteCollection
    {
        //Map SEO Route.
        public static Route MapSEORoute(this RouteCollection routes, string name, string url, object defaults, object constraints = null)
        {
            if (HelperUtility.IsNull(routes))
                throw new ArgumentNullException("routes");

            if (HelperUtility.IsNull(url))
                throw new ArgumentNullException("url");
            SEOUrlRouteData route = new SEOUrlRouteData(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };
            routes.Add(name, route);
            return route;
        }
    }
}