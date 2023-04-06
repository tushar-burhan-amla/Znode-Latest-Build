using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers.Reports
{
    public class MyReportsController : BaseController
    {
        #region Private ReadOnly members
        private readonly IReportAgent _reportAgent;
        #endregion

        #region Public Constructor
        public MyReportsController(IReportAgent reportAgent)
        {
            _reportAgent = reportAgent;
        }
        #endregion

        #region Public Methods
        public virtual ActionResult Index(string reportType = "")
        {
            if (!string.IsNullOrEmpty(reportType))
            {
                //Set Report Details based on the Report Type
                ReportViewModel reportViewModel = _reportAgent.SetReportType(reportType);
                return View(reportViewModel);
            }
            return RedirectToAction<MyReportsController>(x => x.Index(ReportTypeEnum.Sales.ToString()));
        }


        //Get the Report Details based on the Report Path.        
        public virtual ActionResult GetReport(string reportPath, string reportName, bool isDynamicReport = false)
        {
            if (!string.IsNullOrEmpty(reportPath))
            {
                ReportViewModel reportSettingModel = _reportAgent.SetReportControlSetting(reportPath, isDynamicReport);
                reportSettingModel.Name = reportName;
                reportSettingModel.isDynamicReport = isDynamicReport;
                return ActionView("ViewReport", reportSettingModel);
            }
            else
                return RedirectToAction<MyReportsController>(x => x.Index(ReportTypeEnum.Sales.ToString()));
        }

        //Get Dynamic report.
        [HttpGet]
        public virtual ActionResult DynamicReport()
        {
            DynamicReportViewModel viewModel = _reportAgent.BindViewModel();
            if (!viewModel.IsImportCompleted)
                SetNotificationMessage(GetInfoNotificationMessage(Admin_Resources.ImportProcessRunning));

            return View("DynamicReport", viewModel);
        }

        //Action for create dynamic report.
        public virtual ActionResult DynamicReport(DynamicReportViewModel model)
        {
            string errorMessage = string.Empty;
            return Json(new { data = _reportAgent.GenerateDynamicReport(model, out errorMessage), errorMessage = errorMessage, reportName = model.ReportName, reportPath = ZnodeAdminSettings.ReportServerDynamicReportFolderName }, JsonRequestBehavior.AllowGet);
        }

        //Edit report.
        public virtual ActionResult EditReport(int CustomReportTemplateId = 0)
        => View("DynamicReport", _reportAgent.GetCustomReport(CustomReportTemplateId));
        

        //Get filter row.
        public virtual ActionResult Filter(string reportType = "", string filterName = "", string operatorName = "", string filterValue = "") => PartialView("~/Views/MyReports/_FilterRow.cshtml", _reportAgent.GetFilterRow(reportType, filterName, operatorName, filterValue));

        //Delete Get Link Widget Configuration.
        public virtual JsonResult DeleteTableRowData(string areaMappingId)
        {
            string message = "success";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get the Attributes and Filters
        public virtual ActionResult GetExportData(string dynamicReportType)
         => Json(new { data = _reportAgent.GetExportData(dynamicReportType) }, JsonRequestBehavior.AllowGet);

        //Get column list.
        public virtual ActionResult GetColumnList(int reportId = 0, string dynamicReportType = "")
       => Json(new { data = _reportAgent.GetColumnList(reportId, dynamicReportType) }, JsonRequestBehavior.AllowGet);

        //Get operator list.
        public virtual ActionResult GetOperators(string reportType, string filterName)
        {
            string dataType = string.Empty;
            return Json(new { data = _reportAgent.GetOperators(reportType, filterName, out dataType), dataType = dataType }, JsonRequestBehavior.AllowGet);
        }

        //Get the list of Dynamic reports.
        public virtual ActionResult list([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeDynamicReport.ToString(), model);
            ReportListViewModel dynamicReportList = _reportAgent.GetDynamicReportList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            dynamicReportList.GridModel = FilterHelpers.GetDynamicGridModel(model, dynamicReportList?.Reports, GridListType.ZnodeDynamicReport.ToString(), string.Empty, null, true, true, dynamicReportList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            dynamicReportList.GridModel.TotalRecordCount = dynamicReportList.TotalResults;
            return ActionView(dynamicReportList);
        }

        //Delete dynamic report with the help of name.  The name has been bounded in 'text' and hence the parameter in this method is kept as text only.
        public virtual ActionResult DeleteReport(string customReportTemplateId)
        {
            string errorMsg = string.Empty;
            bool isDeleted = _reportAgent.DeleteDynamicReport(customReportTemplateId, out errorMsg);
            string message = isDeleted ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;

            return Json(new { status = isDeleted, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get Report view.
        public virtual ActionResult GetReportView(string reportType, int selectedValue = 0) => PartialView(_reportAgent.GetReportViewName(reportType), _reportAgent.GetReportView(reportType, selectedValue));

        #endregion
    }

}

