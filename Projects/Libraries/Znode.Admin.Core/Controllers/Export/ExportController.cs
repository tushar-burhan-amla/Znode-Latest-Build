using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.Admin.Core.ViewModels;
using System.Net;
using System;

namespace Znode.Admin.Core.Controllers
{
    public class ExportController : BaseController
    {
        #region Private Variables
        private readonly IExportAgent _exportAgent;
        #endregion

        #region Constructor
        public ExportController(IExportAgent exportAgent)
        {
            _exportAgent = exportAgent;
        }
        #endregion

        #region Public Method

        //For Import Data in CSV,Excel and PDF
        public virtual ActionResult Export(string exportFileTypeId, string Type, int localId, int pimCatalogId = 0, string catalogName = null, int paramId = 0, int? pageIndex = null, int? pageSize = null)
        {
            string fileType = "";
            FilterCollection filters = SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);
            SortCollection sort = SessionHelper.GetDataFromSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey);
            if (exportFileTypeId == "1")
            {
                fileType = "Excel";
            }
            else if (exportFileTypeId == "2")
            {
                fileType = "CSV";
            }
            else
            {
                fileType = "PDF";
            }
            
                ExportResponseMessageModel exportResponseMessage = _exportAgent.GetExportLogList(fileType, Type, filters, sort, pageIndex, pageSize, SessionHelper.GetDataFromSession<int>(DynamicGridConstants.FolderId),
                                                                                localId, pimCatalogId, catalogName, paramId);
                if (Equals(exportResponseMessage.Message, PIM_Resources.ErrorPublishCatalog))
                    return Json(new { Message = exportResponseMessage.Message, HasError = true }, JsonRequestBehavior.AllowGet);
                return Json(new { Message = exportResponseMessage.Message, HasError = exportResponseMessage.HasError }, JsonRequestBehavior.AllowGet);
        }
    

        #endregion
        //Form Submission Export
        public virtual ActionResult ExportFormSubmission(string exportType)
        {
            FilterCollection filters = SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);
            SortCollection sort = SessionHelper.GetDataFromSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey);
            ExportResponseMessageModel exportResponseMessage = _exportAgent.GetExportFormSubmissionList(exportType, filters, sort, null, null,
             SessionHelper.GetDataFromSession<int>(DynamicGridConstants.FolderId));
            if (Equals(exportResponseMessage.Message, PIM_Resources.ErrorPublishCatalog))
                return Json(new { Message = exportResponseMessage.Message, HasError = true  }, JsonRequestBehavior.AllowGet);
            return Json(new { Message = exportResponseMessage.Message, HasError = exportResponseMessage.HasError }, JsonRequestBehavior.AllowGet);
        }
        //Show Export logs list
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeExportProcessLog.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeExportProcessLog.ToString(), model);
            ExportProcessLogsListViewModel exportProcessLogs = _exportAgent.GetExportLogs(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model.
            exportProcessLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, exportProcessLogs?.ProcessLogs, GridListType.ZnodeExportProcessLog.ToString(), string.Empty, null, true, true, exportProcessLogs?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            exportProcessLogs.GridModel.TotalRecordCount = exportProcessLogs.TotalResults;
            return ActionView(exportProcessLogs);
        }

        //Method to download export files by passing export id.
        public virtual ActionResult DownloadExportFile(string tableName)
        {
            string filePath = _exportAgent.DownloadExportFile(tableName);
            if (string.Equals(filePath, Admin_Resources.ExportDownloadErrorMessage, StringComparison.InvariantCultureIgnoreCase))
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ExportDownloadErrorMessage));
                return RedirectToAction<ExportController>(x => x.List(null));
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                byte[] bytes = _exportAgent.GetZipFile(filePath);
                return File(bytes, "application/zip", $"{ tableName}.zip");
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorDownloadTheme));
            return RedirectToAction<ExportController>(x => x.List(null));
        }

        //Delete Export logs
        public virtual JsonResult DeleteLogs(string exportProcessLogId)
        {
            string message = string.Empty;
            bool isDeleted = _exportAgent.DeleteLog(exportProcessLogId);
            message = isDeleted ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;
            return Json(new { status = isDeleted, message = message }, JsonRequestBehavior.AllowGet);
        }

        #region Private Method
        /// <summary>
        /// Get Export Data By list
        /// </summary>
        /// <param name="exportFileTypeId">ExportTypeId - 1-Excel ,2-CSV</param>
        /// <param name="exportList">ExportList</param>
        /// <returns>ExportContent</returns>
        protected string GetExportData(string exportFileTypeId, List<dynamic> exportList)
            => new DownloadHelper().ExportDownload(exportFileTypeId, _exportAgent.CreateDataSource(exportList), null, null, null, false);


        #endregion
     
    }
}
