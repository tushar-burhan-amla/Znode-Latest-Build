using System.Web.Mvc;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.WebStore
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(GetService<IAuthenticationHelper>());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
