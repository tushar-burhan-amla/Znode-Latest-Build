using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Controllers
{
    public class CMSSearchConfigurationController : BaseController
    {
        private readonly ISearchConfigurationAgent _searchConfigurationAgent;

        public CMSSearchConfigurationController(ISearchConfigurationAgent searchConfigurationAgent)
        {
            _searchConfigurationAgent = searchConfigurationAgent;
        }

        #region Public Methods
        //Get the data of CMS index of portal
        [HttpGet]
        public virtual ActionResult CreateIndex(int portalId = 0, string storeName = "")
        {
            FilterCollectionDataModel model = new FilterCollectionDataModel { Filters = new FilterCollection() };

            //Get and Set Filters from Cookies if exists.
            if (portalId == 0)
                FilterHelpers.GetFiltersFromCookies(GridListType.ZnodeCmsPageSearchIndexMonitor.ToString(), model);

            CMSPortalContentPageIndexViewModel cmsPortalContentPageIndexData = _searchConfigurationAgent.GetCmsPageIndexData(model.Filters, portalId);

            cmsPortalContentPageIndexData.StoreName = string.IsNullOrEmpty(cmsPortalContentPageIndexData?.StoreName) ? storeName : cmsPortalContentPageIndexData?.StoreName;

            return (Request.IsAjaxRequest()) ? PartialView("_CreateIndex", cmsPortalContentPageIndexData) : ActionView("CmsPageSearchSetting", cmsPortalContentPageIndexData);
        }

        //Insert and create the CMS index of portal
        [HttpPost]
        public virtual ActionResult InsertCreateIndexData(CMSPortalContentPageIndexViewModel cmsPortalIndexModel)
        {
            if (ModelState.IsValid)
            {
                cmsPortalIndexModel = _searchConfigurationAgent.InsertCreateCmsPageIndexData(cmsPortalIndexModel);

                if (cmsPortalIndexModel.HasError && cmsPortalIndexModel.IsDisabledCMSPageResults)
                    SetNotificationMessage(GetErrorNotificationMessage(cmsPortalIndexModel.ErrorMessage));

                return ActionView("_CreateIndex", cmsPortalIndexModel);
            }
            cmsPortalIndexModel.HasError = true;
            return ActionView("_CreateIndex", cmsPortalIndexModel);
        }

        //Get CMS page search index monitor data
        public virtual ActionResult GetCmsPageSearchIndexMonitor(int cmsSearchIndexId, int portalId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            if(model.Filters?.Count > 0)
                FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCmsPageSearchIndexMonitor.ToString(), model);
            else
                FilterHelpers.GetFiltersFromCookies(GridListType.ZnodeCmsPageSearchIndexMonitor.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCmsPageSearchIndexMonitor.ToString(), model);

            CMSSearchIndexMonitorListViewModel cmsSearchIndexMonitorList = _searchConfigurationAgent.GetCmsPageSearchIndexMonitorList(cmsSearchIndexId, portalId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            cmsSearchIndexMonitorList.GridModel = FilterHelpers.GetDynamicGridModel(model, cmsSearchIndexMonitorList.CMSSearchIndexMonitorList, GridListType.ZnodeCmsPageSearchIndexMonitor.ToString(), string.Empty, null, true, true, cmsSearchIndexMonitorList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            cmsSearchIndexMonitorList.GridModel.TotalRecordCount = cmsSearchIndexMonitorList.TotalResults;
            return ActionView("_CmsPageSearchIndexMonitorList", cmsSearchIndexMonitorList);
        }
        #endregion
    }
}
