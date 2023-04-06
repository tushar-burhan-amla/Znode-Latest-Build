using System.Web.Mvc;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ZnodeLicenseManager licenseMgr = new ZnodeLicenseManager();
            licenseMgr.Validate();

            if (Equals(licenseMgr.LicenseType, ZnodeLicenseType.Invalid))
            {
                return RedirectToAction("Index", "Activate", new { area = "Activate" });
            }

            if (!Equals(HttpContext.ApplicationInstance.Context.AllErrors, null) && HttpContext.ApplicationInstance.Context.AllErrors.Length > 0)
            {
                HttpContext.ApplicationInstance.Context.Response.StatusCode = 400;
                return new EmptyResult();
            }              
            return View();
        }       
    }
}
