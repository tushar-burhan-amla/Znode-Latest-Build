using System;
using System.Linq;
using Znode.Engine.Admin.Controllers;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Areas.Portal;
using Znode.Libraries.Resources;
using MvcSiteMapProvider;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Engine.Api.Client.Sorts;
using Newtonsoft.Json;

namespace Znode.Admin.Core.Controllers
{
    public class StoreExperienceController : BaseController
    {
        #region Private variables
        private readonly IWebSiteAgent _websiteAgent;
        private readonly IStoreExperienceAgent _storeExperienceAgent;
        private const string _ManagePartialView = "_ManagePartial";
        #endregion

        public StoreExperienceController(IWebSiteAgent websiteAgent, IStoreExperienceAgent storeExperienceAgent)
        {
            _websiteAgent = websiteAgent;
            _storeExperienceAgent = storeExperienceAgent;
        }

        //Get the list of all stores.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleStoreExperienceList", Key = "StoreExperience", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeStoreExperience.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeStoreExperience.ToString(), model);
                    
            //Get the list of all stores.
            StoreListViewModel storeList = _storeExperienceAgent.GetStoreExperienceList(model.Filters, SetDefaultSorts(model.SortCollection), model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeStoreExperience.ToString(), string.Empty, null, true, true, storeList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView(storeList);
        }

        public virtual ActionResult StoreSetting(int portalId, string fileName = "")
        {
            //Get the available WebSite Logo details.
            WebSiteLogoViewModel model = _websiteAgent.GetWebSiteLogoDetails(portalId);
            if (model?.PortalId > 0)
            {
                model.FileName = fileName;
                return Request.IsAjaxRequest() ? ActionView(_ManagePartialView, model) : ActionView(model);
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorPortalNotFound));
            return RedirectToAction<StoreController>(x => x.List(null));
        }

        [HttpPost]
        public virtual ActionResult SaveWebSiteLogo(WebSiteLogoViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool status = _websiteAgent.SaveWebSiteLogo(model);
                string message = status ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage;
                ActionResult action = GotoBackURL();

                if (IsNotNull(action))
                    SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                
                return Json(new { status = status, message = message, Url = Equals(action, null) ? "" : ((RedirectResult)action).Url }, JsonRequestBehavior.AllowGet);
            }
            model.HasError = true;
            return ActionView(_ManagePartialView, model);
        }

        //Publish store CMS Content.
        public virtual JsonResult PublishStoreCMSContent(int portalId, string targetPublishState = null, string publishContent = null)
        {
            if (portalId > 0)
            {
                string errorMessage;
                bool status = _websiteAgent.Publish(portalId, out errorMessage, targetPublishState, publishContent);
                return Json(new { status = status, message = status ? string.Format(Admin_Resources.TextPublishInProgress, "CMS content") : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = PIM_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
        }

        protected virtual SortCollection SetDefaultSorts(SortCollection sorts)
        {
            return HelperMethods.StoreExperienceSort(sorts);
        }
    }
}
