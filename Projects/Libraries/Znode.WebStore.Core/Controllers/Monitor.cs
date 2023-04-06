using System.Web.Mvc;

namespace Znode.Engine.WebStore.Controllers
{
    /// <summary>
    /// This is the controller used only to monitor whether the site is running or not.
    /// </summary>
    public class MonitorController : Controller
    {
        /// <summary>
        /// Checks if the site is running or not.  It will show only text with time.
        /// </summary>
        /// <returns>Returns the view with the current date</returns>
        public ActionResult Index()
        {
            Response.Write($"TEST OK - {System.DateTime.Now.ToString()}");
            return View();
        }
    }
}