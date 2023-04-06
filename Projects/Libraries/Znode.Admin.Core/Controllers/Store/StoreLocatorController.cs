using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class StoreLocatorController : BaseController
    {

        #region Private Variables
        private readonly IStoreLocatorAgent _storeLocatorAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly IHelperAgent _helperAgent;
        #endregion

        #region Public Constructors

        /// <summary>
        /// Constructor for the store locator.
        /// </summary>
        public StoreLocatorController(IStoreLocatorAgent storeLocatorAgent, IStoreAgent storeAgent, IHelperAgent helperAgent)
        {
            _storeLocatorAgent = storeLocatorAgent;
            _storeAgent = storeAgent;
            _helperAgent = helperAgent;
        }

        #endregion

        #region Public Methods
        //Get Store List for location.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleStoreLocator", Key = "StoreLocator", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZNodePortalAddress.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZNodePortalAddress.ToString(), model);

            StoreLocatorListViewModel storeLocators = _storeLocatorAgent.GetStoreLocatorList(model);

            //Get the grid model.
            storeLocators.GridModel = FilterHelpers.GetDynamicGridModel(model, storeLocators.StoreLocatorList, GridListType.ZNodePortalAddress.ToString(), string.Empty, null, true, true, storeLocators?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            storeLocators.GridModel.TotalRecordCount = storeLocators.TotalResults;
            return ActionView(storeLocators);
        }

        //Get:create for store locator.
        public virtual ActionResult Create()
            => ActionView(AdminConstants.CreateEdit, _storeLocatorAgent.Create());

        //Post:Create store locator.
        [HttpPost]
        public virtual ActionResult Create(StoreLocatorDataViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                //Save store data.
                viewmodel = _storeLocatorAgent.SaveStore(viewmodel);
                SetNotificationMessage(viewmodel.PortalAddressId > 0
                    ? GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage)
                     : GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
                return viewmodel.PortalAddressId > 0 ? RedirectToAction<StoreLocatorController>(x => x.Update(viewmodel.StoreLocationCode)) :
                     RedirectToAction<StoreLocatorController>(x => x.Create(viewmodel));

            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
                return RedirectToAction<StoreLocatorController>(x => x.Create(viewmodel));
            }
        }

        //Get:Mange store data.
        public virtual ActionResult Edit(int PortalAddressId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEdit, _storeLocatorAgent.GetStoreLocator(PortalAddressId));
        }

        //Get:Mange store data by store location code
        public virtual ActionResult Update(string storeLocationCode)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEdit, _storeLocatorAgent.GetStoreLocator(storeLocationCode));
        }

        //Post:update an existing store data.
        [HttpPost]
        public virtual ActionResult Edit(StoreLocatorDataViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                viewmodel = _storeLocatorAgent.Update(viewmodel);
                SetNotificationMessage(!(viewmodel.HasError)
                ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                 : GetErrorNotificationMessage(Admin_Resources.UpdateError));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateError));
            return RedirectToAction<StoreLocatorController>(x => x.Edit(viewmodel.PortalAddressId));
        }

        //Post:update an existing store data.
        [HttpPost]
        public virtual ActionResult Update(StoreLocatorDataViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                viewmodel = _storeLocatorAgent.Update(viewmodel);
                SetNotificationMessage(!(viewmodel.HasError)
                ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                 : GetErrorNotificationMessage(Admin_Resources.UpdateError));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateError));
            return RedirectToAction<StoreLocatorController>(x => x.Update(viewmodel.StoreLocationCode));
        }

        //Delete an Store by id.
        public virtual JsonResult Delete(string PortalAddressId)
        {
            if (!string.IsNullOrEmpty(PortalAddressId))
            {
                bool status = _storeLocatorAgent.DeleteStoreLocator(PortalAddressId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        //Delete an Store by store Location code.
        public virtual JsonResult DeleteByCode(string storeLocationCode)
        {
            if (!string.IsNullOrEmpty(storeLocationCode))
            {
                bool status = _storeLocatorAgent.DeleteStoreLocator(storeLocationCode, true);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true, true, null);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView("_asideStoreListPanel", storeList);
        }
        #endregion
        //Check store Location Code already exists
        public virtual JsonResult IsStoreLocatorCodeExists(string codeField)
        {
            bool isExist = _helperAgent.IsCodeExists(codeField, CodeFieldService.StoreLocatorService.ToString(), CodeFieldService.IsCodeExists.ToString());
            return Json(new { isExist = !isExist, message = Admin_Resources.ErrorStoreLocationCodeExists }, JsonRequestBehavior.AllowGet);
        }

    }
}