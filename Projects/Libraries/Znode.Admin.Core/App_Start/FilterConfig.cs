using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(GetService<IAuthenticationHelper>());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidateInputAttribute(false));
        }
    }
}
