using System.Web.Mvc;
using Znode.Engine.Admin.Controllers;

namespace Znode.Engine.Admin.Areas.Diagnostics.Controllers
{
    public class HangfireController : BaseController
    {
        public virtual ActionResult Dashboard() => ActionView("HangfireDashboard");
    }
}
