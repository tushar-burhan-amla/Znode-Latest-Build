using System.Web.Mvc;
using Znode.Engine.WebStore.Controllers;

namespace Znode.Custom.WebStore.Controllers
{
    public class CustomTestController : BaseController
    {
         [Route("Home/About")]
        public ActionResult CustomAction()
        {
            return View("CustomView");
        }
    }
}
