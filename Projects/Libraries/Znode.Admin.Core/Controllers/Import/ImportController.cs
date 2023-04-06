using System;
using System.Diagnostics;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using System.IO;

namespace Znode.Engine.Admin.Controllers
{
    public class ImportController : BaseController
    {
        #region Private Variables
        private readonly IImportAgent _importAgent;
        private const string logStatusView = "_ImportLogStatus";
        private const string logDetailsView = "_ImportLogDetails";
        #endregion

        #region Public Constructor
        public ImportController(IImportAgent importAgent)
        {
            _importAgent = importAgent;
        }
        #endregion

        #region Public Methods

        //This method will fetch the list of import types and bind it on view.
        [HttpGet]
        public virtual ActionResult Index() => View(AdminConstants.Create, _importAgent.BindViewModel());

        //This method will will upload the file and process the uploaded data for import.
        [HttpPost]
        public virtual ActionResult Index(ImportViewModel importModel)
        {
            string message = importModel.IsPartialPage ? Admin_Resources.LinkViewImportLogs : "";
            bool exportStatus = _importAgent.CheckExportProcess();
            if (exportStatus)
            {
                SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorPublishCatalog));
                return Json(new { status = exportStatus }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                bool status= _importAgent.ImportData(importModel);
                if (status)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.ImportProcessInitiated + message));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ImportProcessFailed + message));

                //Assign the values back to model
                importModel = _importAgent.BindViewModel();
                return Json(new { status = status }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get all pricing list and code for pricing.
        [AllowAnonymous]
        public virtual ActionResult GetPricingList()
            => Json(new { pricingList = _importAgent.GetPricingList() }, JsonRequestBehavior.AllowGet);

        //Get all catalog list and code for catalog.
        [AllowAnonymous]
        public virtual ActionResult GetCatalogList()
            => Json(new { catalogList = _importAgent.GetCatalogList() }, JsonRequestBehavior.AllowGet);

        //Get all country list.
        [AllowAnonymous]
        public virtual ActionResult GetCountryList()
           => Json(new { countryList = _importAgent.GetCountryList() }, JsonRequestBehavior.AllowGet);

        //Get all Portal list
        [AllowAnonymous]
        public virtual ActionResult GetportalList() => Json(new { portalList = _importAgent.GetPortalList()},JsonRequestBehavior.AllowGet);

        //Action to get import template list for dropdown.
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult BindTemplateList(int importHeadId, int familyId, int promotionTypeId = 0)
          => Json(new { templatenamelist = _importAgent.GetImportTemplateList(importHeadId, familyId, promotionTypeId) }, JsonRequestBehavior.AllowGet);

        //Action to get associated template list for selected template.
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult GetAssociatedTemplateList(int templateId, int importHeadId, int familyId, int promotionTypeId = 0)
          => Json(new { templateMappingList = _importAgent.GetImportTemplateMappingList(templateId, importHeadId, familyId, promotionTypeId) }, JsonRequestBehavior.AllowGet);

        //Get all families for Product import
        [AllowAnonymous]
        public virtual ActionResult GetAllFamilies(bool isCategory)
            => Json(new { productFamilies = _importAgent.GetAllFamilies(isCategory) }, JsonRequestBehavior.AllowGet);

        //Get all promotion type list and code for promotions.
        public virtual ActionResult GetPromotionTypeList()
            => Json(new { promotionTypeList = _importAgent.GetPromotionTypeList() }, JsonRequestBehavior.AllowGet);

        //Action to get saved CSV data.
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult GetCsvData(ImportViewModel model)
        {
            string fileName = string.Empty;
            string csvHeaders = _importAgent.GetCsvHeaders(model.FilePath, out fileName);
            return Json(new { Csvlist = csvHeaders, UpdateFileName = fileName }, JsonRequestBehavior.AllowGet);
        }

        // Download the template
        [HttpPost]
        public virtual ActionResult DownloadTemplate(int downloadImportHeadId, string downloadImportName, int downloadImportFamilyId, int downloadImportPromotionTypeId = 0)
        {
            _importAgent.DownloadTemplate(downloadImportHeadId, downloadImportName, downloadImportFamilyId, downloadImportPromotionTypeId, Response);
            if(Request.IsAjaxRequest())
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            else
                return new EmptyResult();
        }

        //Show import logs list
        public virtual ActionResult list([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeImportProcessLog.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeImportProcessLog.ToString(), model);
            ImportProcessLogsListViewModel importProcessLogs = _importAgent.GetImportLogs(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importProcessLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, importProcessLogs?.ProcessLogs, GridListType.ZnodeImportProcessLog.ToString(), string.Empty, null, true, true, importProcessLogs?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            importProcessLogs.GridModel.TotalRecordCount = importProcessLogs.TotalResults;
            return ActionView(importProcessLogs);
        }

        //Show logs status
        public virtual ActionResult ShowLogStatus(int importProcessLogId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeImportProcessLogStatus.ToString(), model);
            ImportProcessLogsListViewModel importProcessLogs = _importAgent.GetImportLogStatus(importProcessLogId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importProcessLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, importProcessLogs?.ProcessLogs, GridListType.ZnodeImportProcessLogStatus.ToString(), string.Empty, null, true, true, importProcessLogs?.GridModel?.FilterColumn?.ToolMenuList);

            return ActionView(logStatusView, importProcessLogs);
        }

        //Show logs details
        public virtual ActionResult ShowLogDetails(int importProcessLogId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeImportLogDetails.ToString(), model);
            ImportLogsListViewModel importLogs = _importAgent.GetImportLogDetails(importProcessLogId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, importLogs?.LogsList, GridListType.ZnodeImportLogDetails.ToString(), string.Empty, null, true, true, importLogs?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            importLogs.GridModel.TotalRecordCount = importLogs.TotalResults;
            return ActionView(logDetailsView, importLogs);
        }

        [HttpPost]
        public virtual ActionResult DownloadPDF(string importProcessLogId,int pageIndex, int pageSize)
        {
            string errorMessage = string.Empty;
            bool status = false;
            byte[] pdfBytes;
            try
            {
                if (string.IsNullOrEmpty(importProcessLogId))
                    return Json(new { success = status });
                string htmlContent = RenderRazorViewToString("_ImportLogDetailsDownloadView", _importAgent.DownloadImportLogDetails(Convert.ToInt32(importProcessLogId), null, null, null, pageIndex, pageSize));
                NReco.PdfGenerator.HtmlToPdfConverter htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                using (MemoryStream Stream = new MemoryStream(pdfBytes))
                {
                    // set HTTP response headers
                    HttpContext.Response.Clear();
                    HttpContext.Response.AddHeader("Content-Type", "application/pdf");
                    HttpContext.Response.AddHeader("Cache-Control", "max-age=0");
                    HttpContext.Response.AddHeader("Accept-Ranges", "none");

                    HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=ImportErrorLogDetails" + ".pdf");

                    // send the generated PDF
                    Stream.WriteTo(Response.OutputStream);
                    Stream.Close();
                    HttpContext.Response.Flush();
                    HttpContext.Response.End();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                errorMessage = ex.Message;
                throw;
            }

            //return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, "ImportLogDetail_" + DateTime.Now.ToString("MMddyyyyhhmmss") + ".pdf");
            return Json(new { success = status });
        }

        //Delete logs 
        public virtual JsonResult DeleteLogs(string importProcessLogId)
        {
            string message = string.Empty;
            bool isDeleted = _importAgent.DeleteLog(importProcessLogId);
            message = isDeleted ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;
            return Json(new { status = isDeleted, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Update Mappings
        [HttpPost]
        public virtual ActionResult UpdateMappings(ImportViewModel importModel)
        {
            if (importModel.TemplateId.Equals(0))
                return Json(new { status = false, message = Admin_Resources.ImportNoTemplateSelectedLabel}, JsonRequestBehavior.AllowGet);

            string message = string.Empty;
            bool isUpdated = _importAgent.UpdateMappings(importModel);
            message = isUpdated ? Admin_Resources.ImportMappingUpdateMessage : Admin_Resources.ImportMappingUpdateErrorMessage;
            return Json(new { status = isUpdated, message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //Get custom import template list. It will not return the system defined import template.
        public virtual ActionResult ManageCustomImportTemplateList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeImportTemplate.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeImportTemplate.ToString(), model);
            ImportManageTemplateMappingListViewModel importTemplateResponse = _importAgent.GetCustomImportTemplateList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importTemplateResponse.GridModel = FilterHelpers.GetDynamicGridModel(model, importTemplateResponse?.ImportTemplate, GridListType.ZnodeImportTemplate.ToString(), string.Empty, null, true, true, importTemplateResponse?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            importTemplateResponse.GridModel.TotalRecordCount = importTemplateResponse.TotalResults;
            return ActionView("_ManageImportTemplate", importTemplateResponse);
        }

        //Deletes the custom import templates. It will not delete the system defined import templates.
        public virtual JsonResult DeleteImportTemplate(string importTemplateId)
        {
            string message = string.Empty;
            bool isDeleted = _importAgent.DeleteImportTemplate(importTemplateId);
            message = isDeleted ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;
            return Json(new { status = isDeleted, message = message }, JsonRequestBehavior.AllowGet);
        }
    }
}