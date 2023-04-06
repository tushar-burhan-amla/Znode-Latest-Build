using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Controllers
{
    public class ImportController : BaseController
    {
        #region Private Read-only members
        private readonly IImportAgent _importAgent;
        private const string logDetailsView = "_ImportLogDetails";
        private const string UserLogDetailsView = "_UserImportLogDetails";
        #endregion

        #region Public Constructor        
        public ImportController(IImportAgent importAgent)
        {
            _importAgent = importAgent;
        }
        #endregion

        //Show logs details
        [Authorize]
        public virtual ActionResult ShowLogDetails(int importProcessLogId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model = null)
        {
            ImportLogsListViewModel importLogs = _importAgent.GetImportLogDetails(importProcessLogId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, importLogs?.LogsList, WebStoreEnum.ZnodeImportLogDetails.ToString(), string.Empty, null, true, true, importLogs?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count.
            importLogs.GridModel.TotalRecordCount = importLogs.TotalResults;
            return ActionView(logDetailsView, importLogs);
        }

        //Delete logs.
        [Authorize]
        public virtual JsonResult DeleteLogs(string importProcessLogId)
        {
            string message = string.Empty;
            bool isDeleted = _importAgent.DeleteLog(importProcessLogId);
            message = isDeleted ? WebStore_Resources.DeleteMessage : WebStore_Resources.DeleteFailMessage;
            return Json(new { status = isDeleted, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Show user logs details.
        public virtual ActionResult ShowUserLogDetails(int importProcessLogId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            ImportLogsListViewModel importLogs = _importAgent.GetImportLogDetails(importProcessLogId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            importLogs.GridModel = FilterHelpers.GetDynamicGridModel(model, importLogs?.LogsList, WebStoreEnum.ZnodeImportLogDetails.ToString(), string.Empty, null, true, true, importLogs?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            importLogs.GridModel.TotalRecordCount = importLogs.TotalResults;
            return ActionView(UserLogDetailsView, importLogs);
        }
    }
}
