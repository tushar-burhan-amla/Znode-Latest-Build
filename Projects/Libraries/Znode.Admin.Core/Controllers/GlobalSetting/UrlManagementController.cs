using Newtonsoft.Json;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using System.Linq;
using System;

namespace Znode.Engine.Admin.Controllers
{
    public class UrlManagementController : BaseController
    {
        #region Private Variable
        private readonly IUrlManagementAgent _urlManagementAgent;
        private readonly IStoreAgent _storeAgent;
        #endregion Private Variable

        #region Constructor
        public UrlManagementController(IUrlManagementAgent urlManagementAgent, IStoreAgent storeAgent)
        {
            _urlManagementAgent = urlManagementAgent;
            _storeAgent = storeAgent;
        }

        #endregion Constructor


        #region PublicMethods

        // List admin/api url
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeAdminAPIDomain.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeAdminAPIDomain.ToString(), model);
            DomainListViewModel domainList = _urlManagementAgent.GetDomainList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            var applicationTypes = _urlManagementAgent.GetAdminAPIApplicationTypes();
            domainList.Domains?.ForEach(item => { item.ApplicationTypeList = applicationTypes; });
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, domainList.Domains, GridListType.ZnodeAdminAPIDomain.ToString(), string.Empty, null, true, true, domainList?.GridModel?.FilterColumn?.ToolMenuList);
            gridModel.TotalRecordCount = domainList.TotalResults;
            domainList.GridModel = gridModel;
            return ActionView(domainList);
        }

        [HttpGet]
        public virtual ActionResult CreateUrl()
        {
            StoreListViewModel storeList = _storeAgent.GetStoreList();
            int portalId= Convert.ToInt32(storeList?.StoreList?.FirstOrDefault()?.PortalId ); // Get first or default portal
            return ActionView(AdminConstants.CreateEditUrlView, new DomainViewModel { PortalId = portalId, ApplicationTypeList = _urlManagementAgent.GetAdminAPIApplicationTypes() });
        }

        // This method will create admin/api url
        [HttpPost]
        public virtual ActionResult CreateUrl(DomainViewModel domainViewModel)
        {
            if (ModelState.IsValid)
            {
                domainViewModel = _urlManagementAgent.CreateDomainUrl(domainViewModel);

                if (!domainViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<UrlManagementController>(x => x.List(null));
                }
            }
            domainViewModel.ApplicationTypeList = _urlManagementAgent.GetAdminAPIApplicationTypes();
            SetNotificationMessage(GetErrorNotificationMessage(domainViewModel.ErrorMessage));
            return View(AdminConstants.CreateEditUrlView, domainViewModel);
        }

        // Edit admin/api url.
        public virtual ActionResult EditUrl(int portalId, int domainId, bool status, string data)
        {
            DomainViewModel domainViewModel = JsonConvert.DeserializeObject<DomainViewModel[]>(data)[0];
            bool exists = _urlManagementAgent.CheckDomainNameExist(domainViewModel.DomainName, domainId);

            if (exists)
                return Json(new { status = false, message = PIM_Resources.DomainNameAlreadyExists }, JsonRequestBehavior.AllowGet);

            if (ModelState.IsValid)
            {
                domainViewModel = _urlManagementAgent.UpdateDomainUrl(domainViewModel);
                domainViewModel.ApplicationTypeList = _urlManagementAgent.GetAdminAPIApplicationTypes();
                if (!domainViewModel.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        // This method will enable or disable the domain.
        public virtual ActionResult EnableDisableDomain(string DomainId, bool IsActive)
        {
            if (!string.IsNullOrEmpty(DomainId))
            {
                string message = string.Empty;
                bool status = _urlManagementAgent.EnableDisableDomain(DomainId, IsActive, out message);
                if (status && IsActive)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.EnableMessage));
                else if (status && !IsActive)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DisableMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return RedirectToAction<UrlManagementController>(x => x.List(null));
        }
        
        // This method will delete the url.
        [HttpGet]
        public virtual JsonResult DeleteUrl(string domainId)
        {
            if (!string.IsNullOrEmpty(domainId))
            {
                string message = string.Empty;

                bool status = _urlManagementAgent.DeleteDomainUrl(domainId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion PublicMethods
    }
}
