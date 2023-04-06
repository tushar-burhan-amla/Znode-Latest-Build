using System.Web.Mvc;

namespace Znode.Engine.Admin.Controllers
{
    public class ErrorPageController : Controller
    {
        public virtual ActionResult Index()
        {
            return Redirect("/Error");
        }

        public virtual ActionResult PageNotFound()
        {
            return Redirect("/404");
        }

        public virtual ActionResult UnAuthorizedErrorRequest() => View("UnAuthorizedRequest");

    }
}
