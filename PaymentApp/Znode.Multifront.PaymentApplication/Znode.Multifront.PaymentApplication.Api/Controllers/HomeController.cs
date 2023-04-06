using System.Web.Mvc;
using Znode.Multifront.PaymentFramework.Bussiness;

namespace Znode.Multifront.PaymentApplication.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ZnodeLicenseManager licenseMgr = new ZnodeLicenseManager();
            licenseMgr.Validate();

            if (Equals(licenseMgr.LicenseType, ZnodeLicenseType.Invalid))
            {
                return RedirectToAction("Index", "Activate");
            }

            if (!Equals(HttpContext.ApplicationInstance.Context.AllErrors, null) && HttpContext.ApplicationInstance.Context.AllErrors.Length > 0)
                return new EmptyResult();
            return View();
        }
    }
}
