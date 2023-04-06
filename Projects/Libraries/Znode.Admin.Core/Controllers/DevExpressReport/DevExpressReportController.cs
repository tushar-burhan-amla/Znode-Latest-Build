using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class DevExpressReportController : BaseController
    {
        #region Private Variables
        private readonly IDevExpressReportAgent _devExpressReportAgent;
        #endregion

        #region Constructor
        public DevExpressReportController(IDevExpressReportAgent devExpressReportAgent) { _devExpressReportAgent = devExpressReportAgent; }
        #endregion

        #region public methods
        public ActionResult Index(int? reportCategoryId) => View(_devExpressReportAgent.GetReportCategories(reportCategoryId));

        //Render report by report code  
        public ActionResult GetReport(string reportCode, string reportName) => View("DevExpressReportViewer", _devExpressReportAgent.GenerateReport(reportCode, reportName));

        //To save report layout.
        [HttpPost]
        public JsonResult SaveReportLayout(ReportViewModel reportModel)
        {
            reportModel.UserId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
            _devExpressReportAgent.SaveReportIntoDatabase(reportModel);
            return Json(new { status = true, message = Admin_Resources.ZnodeReportSaveMessage }, JsonRequestBehavior.AllowGet);
        }

        //To get saved report layouts.
        [HttpPost]
        public JsonResult LoadSavedReportLayout(ReportViewModel reportModel)
        {
            reportModel.UserId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
            var data = _devExpressReportAgent.LoadSavedReportLayout(reportModel);
            return Json(new { status = true, message = Admin_Resources.ZnodeReportSaveMessage, data = data }, JsonRequestBehavior.AllowGet);
        }

        //Delete saved report layout by report view Id.
        [HttpPost]
        public JsonResult DeleteSavedReportLayout(int reportViewId)
        {
            _devExpressReportAgent.DeleteSavedReportLayout(reportViewId);
            return Json(new { status = true, message = Admin_Resources.ZnodeReportSaveMessage }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetPartial(string reportCode)
        {
            return PartialView(string.Format("_{0}", reportCode));
        }

        #endregion


    }
}
