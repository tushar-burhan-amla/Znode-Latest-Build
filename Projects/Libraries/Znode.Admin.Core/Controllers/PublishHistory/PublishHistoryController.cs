using MvcSiteMapProvider;
using System;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class PublishHistoryController : BaseController
    {
        #region Private ReadOnly members
        private readonly IPublishHistoryAgent _publishHistoryAgent;
        #endregion

        #region Public Constructor
        public PublishHistoryController(IPublishHistoryAgent publishHistoryAgent)
        {
            _publishHistoryAgent = publishHistoryAgent;
        }
        #endregion

        #region Public Methods

        //Get the index view for Admin Landing Page
        public virtual ActionResult Index() => ActionView();

        public virtual ActionResult List(string publishState, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePublishHistory.ToString(), model);

            PublishHistoryListViewModel publishHistoryListViewModel = _publishHistoryAgent.GetPublishHistoryList(publishState, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            publishHistoryListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, publishHistoryListViewModel?.PublishHistoryList, GridListType.ZnodePublishHistory.ToString(), string.Empty, null, true, true, publishHistoryListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            publishHistoryListViewModel.GridModel.TotalRecordCount = publishHistoryListViewModel.TotalResults;

            return ActionView(publishHistoryListViewModel);
        }

        public virtual JsonResult DeletePublishHistory(int versionId)
        {
            bool result = _publishHistoryAgent.DeleteProductLog(versionId);

            return Json(new { message = result ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDelete, status = result}, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}