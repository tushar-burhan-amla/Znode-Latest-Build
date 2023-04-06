using MvcSiteMapProvider;
using System;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class ShippingsController : BaseController
    {
        #region Private Variables
        private readonly IShippingAgent _shippingAgent;
        private readonly IProductAgent _productAgent;
        #endregion

        #region Public Constructor
        public ShippingsController(IShippingAgent shippingAgent)
        {
            _shippingAgent = shippingAgent;
        }
        #endregion

        #region Public Methods
        //Action to show create shipping view.
        public virtual ActionResult Create()
        {
            ShippingViewModel shippingViewModel = new ShippingViewModel();
            _shippingAgent.BindDropdownValues(shippingViewModel, false);
            shippingViewModel.CurrencyList = _shippingAgent.GetActiveCurrency();
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, shippingViewModel) : ActionView(AdminConstants.ManageView, shippingViewModel);
        }

        //Action for create shipping.
        [HttpPost]
        public virtual ActionResult Create(ShippingViewModel viewModel)
        {
            _shippingAgent.BindDropdownValues(viewModel, false);

            if (ModelState.IsValid)
            {
                ShippingViewModel shippingViewModel = _shippingAgent.CreateShipping(viewModel);

                if (!shippingViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<ShippingsController>(x => x.Edit(shippingViewModel.ShippingId));
                }
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(shippingViewModel.ErrorMessage));
                    return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, viewModel) : ActionView(AdminConstants.ManageView, viewModel);
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, viewModel) : ActionView(AdminConstants.ManageView, viewModel);
        }

        //Action for show edit view.
        public virtual ActionResult Edit(int? shippingId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            ShippingViewModel shippingViewModel = new ShippingViewModel();
            shippingViewModel = _shippingAgent.GetShippingById(Convert.ToInt32(shippingId));
            shippingViewModel.CurrencyList = _shippingAgent.GetActiveCurrency();
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, shippingViewModel) : ActionView(AdminConstants.ManageView, shippingViewModel);
        }

        //Action for edit shipping.
        [HttpPost]
        public virtual ActionResult Edit(ShippingViewModel model)
        {
            if (ModelState.IsValid)
            {
                ShippingViewModel updatedModel = _shippingAgent.UpdateShipping(model);
                SetNotificationMessage(updatedModel.HasError
                ? GetErrorNotificationMessage(model.ErrorMessage)
                : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<ShippingsController>(x => x.Edit(model.ShippingId));
            }
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, model) : ActionView(AdminConstants.ManageView, model);
        }

        //Action for shippinglist.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleShipping", Key = "Shipping", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeShipping.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeShipping.ToString(), model);
            //Get the list of shippings            
            ShippingListViewModel shippingListViewModel = _shippingAgent.GetShippingList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            shippingListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingListViewModel.ShippingList, GridListType.ZnodeShipping.ToString(), "", null, true, true, shippingListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            shippingListViewModel.GridModel.TotalRecordCount = shippingListViewModel.TotalResults;

            //Returns the shipping list view
            return ActionView(shippingListViewModel);
        }

        // Action for delete shipping by shippingId.
        public virtual JsonResult Delete(string shippingId)
        {
            string message = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(shippingId))
            {
                status = _shippingAgent.DeleteShipping(shippingId, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Action for get state list by country code.
        [HttpGet]
        public virtual JsonResult GetStateListByCountryCode(string countryCode)
             => Json(_shippingAgent.GetStateListByCountryCode(countryCode), JsonRequestBehavior.AllowGet);

        //Action for get city(county) list by statecode.
        [HttpGet]
        public virtual JsonResult GetCityListByStateCode(string stateCode)
             => Json(_shippingAgent.GetCityListByStateCode(stateCode), JsonRequestBehavior.AllowGet);

        //Action for get service list for dropdown.
        [HttpGet]
        public virtual JsonResult BindServiceList(string serviceShippingTypeId)
             => Json(_shippingAgent.GetShippingServiceCodeList(serviceShippingTypeId), JsonRequestBehavior.AllowGet);

        //Checks whether shipping name exists.
        [HttpPost]
        public virtual JsonResult IsShippingNameExist(string shippingName, int shippingId = 0)
            => Json(!_shippingAgent.CheckShippingNameExist(shippingName, shippingId), JsonRequestBehavior.AllowGet);

        #region Shipping SKU 
        //Action for shipping SKUs list.
        public virtual ActionResult ShippingSKUList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int shippingId = 0, int shippingRuleId = 0, string shippingRuleType = null)
          => ActionView(_shippingAgent.GetShippingSKUListViewModel(model, shippingId, shippingRuleId, shippingRuleType));

        //Action for delete shipping SKU(s).
        public virtual JsonResult DeleteShippingSKU(string shippingSKUId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(shippingSKUId))
            {
                status = _shippingAgent.DeleteShippingSKU(shippingSKUId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Shipping Rule

        //Action for shipping list.
        public virtual ActionResult ShippingRuleList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int shippingId = 0, int shippingTypeId = 0)
            => ActionView(_shippingAgent.ShippingRuleList(model, shippingId, shippingTypeId));

        //Action for add shipping rule.
        [HttpGet]
        public virtual ActionResult AddShippingRule(int shippingId)
        {
            ShippingRuleViewModel shippingRuleViewModel = new ShippingRuleViewModel
            {
                ShippingId = shippingId,
                ShippingRuleTypeList = _shippingAgent.GetShippingRuleTypes(shippingId, false),
                CurrencyList = _shippingAgent.GetActiveCurrency(),
                CurrencyId = _shippingAgent.GetShippingById(Convert.ToInt32(shippingId)).CurrencyId
            };
            return ActionView(AdminConstants.ShippingRuleView, shippingRuleViewModel);
        }

        //Action for add shipping rule.
        [HttpPost]
        public virtual JsonResult AddShippingRule(ShippingRuleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (IsNotNull(model))
                    model = _shippingAgent.AddShippingRule(model);
                else
                    model.ErrorMessage = model.ErrorMessage;
                return Json(new { isSuccess = model.ShippingRuleId > 0, Message = (Equals(model, null) || model.HasError) ? model.ErrorMessage : Admin_Resources.RecordCreationSuccessMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { isSuccess = false, issue = model, errors = ModelState.Values.Where(i => i.Errors.Count > 0) });
        }

        [HttpGet]
        public virtual ActionResult EditShippingRule(int shippingRuleId, int shippingId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(_shippingAgent.GetShippingRule(shippingRuleId));
        }
       
        //Action for edit shipping rule.
        [HttpPost]
        public virtual ActionResult EditShippingRule(ShippingRuleViewModel model)
        {
            if (ModelState.IsValid)
                SetNotificationMessage(!Equals(_shippingAgent.UpdateShippingRule(model), null) ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));

            return RedirectToAction<ShippingsController>(x => x.EditShippingRule(model.ShippingRuleId,model.ShippingId));
        }

        //Action for delete shipping rule.
        public virtual JsonResult DeleteShippingRule(string shippingRuleId, int shippingId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(shippingRuleId))
            {
                status = _shippingAgent.DeleteShippingRule(shippingRuleId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion
    }
}