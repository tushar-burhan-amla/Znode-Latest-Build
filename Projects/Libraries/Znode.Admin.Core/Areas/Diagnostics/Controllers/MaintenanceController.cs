using System.Web.Mvc;

using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.Diagnostics.Controllers
{
    public class MaintenanceController : BaseController
    {
        #region Private variables

        private readonly IMaintenanceAgent _maintenanceAgent;

        #endregion

        #region Constructor

        public MaintenanceController(IMaintenanceAgent maintenanceAgent)
        {
            _maintenanceAgent = maintenanceAgent;
        }

        #endregion Constructor

        public virtual ActionResult Index()
        {
            return View();
        }

        // To delete published data of all catalog, store,cms and elastic search.
        public virtual JsonResult PurgeAllPublishedData()
        {
            bool status = _maintenanceAgent.PurgeAllPublishedData();

            return Json(new
            {
                status,
                message = status ?
                Admin_Resources.TextClearPublishDataSuccessfully :
                Admin_Resources.TextClearPublishDataFailed
            },
            JsonRequestBehavior.AllowGet);
        }
    }
}
