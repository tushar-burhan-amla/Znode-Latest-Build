using MvcSiteMapProvider;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class TaxClassController : BaseController
    {
        #region Private Variables

        private readonly ITaxClassAgent _taxClassAgent;

        #endregion Private Variables

        #region Constructor

        public TaxClassController(ITaxClassAgent taxClassAgent, IProductAgent productAgent)
        {
            _taxClassAgent = taxClassAgent;
        }

        #endregion Constructor

        #region Public Methods

        //Get the list of all tax classes.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleTaxClass", Key = "Tax", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeTaxClass.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeTaxClass.ToString(), model);
            //Get the list of tax classes.
            TaxClassListViewModel taxClassList = _taxClassAgent.GetTaxClassList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            taxClassList.GridModel = FilterHelpers.GetDynamicGridModel(model, taxClassList.TaxClassList, GridListType.ZnodeTaxClass.ToString(), string.Empty, null, true, true, taxClassList?.GridModel?.FilterColumn?.ToolMenuList);
            //Set the total record count
            taxClassList.GridModel.TotalRecordCount = taxClassList.TotalResults;
            //Returns the tax Class list view
            return ActionView(taxClassList);
        }

        //Get type method to Create new tax class.
        [HttpGet]
        [MvcSiteMapNode(Title = "Add Tax Class", Key = "AddZnodeTaxClass", Area = "", ParentKey = "Home")]
        public virtual ActionResult Create()
        {
            TaxClassViewModel viewModel = new TaxClassViewModel() { IsActive = true };
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, viewModel) : ActionView(AdminConstants.ManageView, viewModel);
        }

        //Post type method to Create new tax class.
        [HttpPost]
        public virtual ActionResult Create(TaxClassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel = _taxClassAgent.CreateTaxClass(viewModel);

                if (!viewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<TaxClassController>(x => x.Edit(viewModel.TaxClassId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(viewModel.ErrorMessage));
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, viewModel) : ActionView(AdminConstants.ManageView, viewModel);
        }

        //Get type method to Edit tax class.
        [HttpGet]
        [MvcSiteMapNode(Title = "Edit Tax Class", Key = "EditZnodeTaxClass", Area = "", ParentKey = "Home")]
        public virtual ActionResult Edit(int taxClassId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            TaxClassViewModel taxClassViewModel = new TaxClassViewModel();
            taxClassViewModel = _taxClassAgent.GetTaxClass(taxClassId);
            if (IsNotNull(taxClassViewModel))
            {
                taxClassViewModel.TaxClassSKU = new TaxClassSKUViewModel { TaxClassId = taxClassId };
                taxClassViewModel.TaxRule = new TaxRuleViewModel { TaxClassId = taxClassId };
            }
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, taxClassViewModel) : ActionView(AdminConstants.ManageView, taxClassViewModel);
        }

        //Post type method to Edit tax class.
        [HttpPost]
        public virtual ActionResult Edit(TaxClassViewModel taxClassViewModel)
        {
            if (ModelState.IsValid)
            {
                TaxClassViewModel updatedModel = _taxClassAgent.UpdateTaxClass(taxClassViewModel);

                if (!updatedModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<TaxClassController>(x => x.Edit(taxClassViewModel.TaxClassId));
                }

                SetNotificationMessage(GetErrorNotificationMessage(updatedModel.ErrorMessage));
                return RedirectToAction<TaxClassController>(x => x.Edit(taxClassViewModel.TaxClassId));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditPartialView, taxClassViewModel) : ActionView(AdminConstants.ManageView, taxClassViewModel);
        }

        //Delete tax class.
        public virtual JsonResult Delete(string taxClassId)
        {
            string message = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(taxClassId))
            {
                status = _taxClassAgent.DeleteTaxClass(taxClassId, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        #region Tax Class SKU

        //Action for tax SKUs list.
        public virtual ActionResult TaxClassSKUList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int taxClassId, string name) => ActionView(_taxClassAgent.GetTaxClassSKUListViewModel(model, taxClassId, name));

        //Action for add tax class SKU.
        public virtual ActionResult AddTaxClassSKU(int taxClassId = 0, string taxClassSKUs = null)
        {
            TaxClassSKUViewModel taxClassSKUViewModel = new TaxClassSKUViewModel { TaxClassId = taxClassId, SKUs = taxClassSKUs };
            taxClassSKUViewModel = _taxClassAgent.AddTaxClassSKU(taxClassSKUViewModel);

            if (!taxClassSKUViewModel.HasError)
                return Json(new { status = true, message = taxClassSKUViewModel.HasError ? Admin_Resources.ProductFailedAssociatedSuccessMessage : Admin_Resources.ProductAssociatedSuccessMessage }, JsonRequestBehavior.AllowGet);

            return Json(new { status = false, message = taxClassSKUViewModel.HasError ? taxClassSKUViewModel.ErrorMessage : Admin_Resources.ProductAssociatedSuccessMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete tax class SKUs
        public virtual JsonResult DeleteTaxClassSKU(string taxClassSKUId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(taxClassSKUId))
            {
                status = _taxClassAgent.DeleteTaxClassSKU(taxClassSKUId, out message);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion Tax Class SKU

        #region Tax Rule

        //Get tax class rule list.
        public virtual ActionResult TaxRuleList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int taxClassId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeTaxRule.ToString(), model);

            if (taxClassId <= 0)
                return null;
            _taxClassAgent.SetFilters(model.Filters, taxClassId);
            TaxRuleListViewModel taxRuleList = _taxClassAgent.GetTaxRuleList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            taxRuleList.GridModel = FilterHelpers.GetDynamicGridModel(model, taxRuleList?.TaxRuleList, GridListType.ZnodeTaxRule.ToString(), string.Empty, null, true, true, taxRuleList?.GridModel?.FilterColumn?.ToolMenuList);
            taxRuleList.GridModel.TotalRecordCount = taxRuleList.TotalResults;
            return ActionView(taxRuleList);
        }

        //Action for add tax rule.
        [HttpGet]
        public virtual ActionResult AddTaxRule(int taxClassId)
          => ActionView(AdminConstants.TaxRuleView, _taxClassAgent.GetTaxRuleDetails(taxClassId));

        //Action for add tax rule.
        [HttpPost]
        public virtual JsonResult AddTaxRule(TaxRuleViewModel model)
        {
            if (ModelState.IsValid)
                if (IsNotNull(model))
                    model = _taxClassAgent.AddTaxRule(model);
                else
                    model.ErrorMessage = model.ErrorMessage;

            return Json(new { isSuccess = (model.TaxRuleId > 0), Message = (Equals(model, null) || model.HasError) ? model.ErrorMessage : Admin_Resources.RecordCreationSuccessMessage }, JsonRequestBehavior.AllowGet);
        }

        //Action for edit tax rule.
        [HttpGet]
        public virtual ActionResult EditTaxRule(int taxRuleId, int taxClassId = 0)
           => ActionView(AdminConstants.TaxRuleView, _taxClassAgent.GetTaxRule(taxRuleId, taxClassId));

        //Action for edit tax rule.
        [HttpPost]
        public virtual ActionResult EditTaxRule(TaxRuleViewModel model)
        {
            if (ModelState.IsValid)
                model = _taxClassAgent.UpdateTaxRule(model);
            else
                model.ErrorMessage = Admin_Resources.ErrorLoadingdata;

            return Json(new { isSuccess = Equals(model, null) ? false : true, Message = (Equals(model, null) || model.HasError) ? Admin_Resources.UpdateErrorMessage : Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete tax class rule.
        public virtual JsonResult DeleteTaxRule(string taxRuleId, int taxClassId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(taxRuleId))
            {
                status = _taxClassAgent.DeleteTaxRule(taxRuleId, out message);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Action for get state list for dropdown.
        [HttpGet]
        public virtual JsonResult BindStateList(string countryCode)
          => Json(_taxClassAgent.BindStateList(countryCode), JsonRequestBehavior.AllowGet);

        //Action for get county list for dropdown.
        [HttpGet]
        public virtual JsonResult BindCountyList(string stateCode)
        => Json(_taxClassAgent.BindCityList(stateCode), JsonRequestBehavior.AllowGet);

        //Action for get product list.
        public virtual ActionResult ProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedTaxClassProductList.ToString(), model);
            //Get the list of all products.
            ProductDetailsListViewModel productList = _taxClassAgent.GetUnassociatedProductList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Remove tool option.
            productList?.GridModel?.FilterColumn?.ToolMenuList.Clear();
            //Get the grid model.
            GridModel gridModel = GetProdctsXMLGrid(model, productList, GridListType.AssociatedTaxClassProductList);// FilterHelpers.GetDynamicGridModel(model, IsNull(productList?.ProductDetailList) ? new List<ProductDetailsViewModel>() : productList.ProductDetailList, GridListType.AssociatedTaxClassProductList.ToString(), string.Empty, null, true, true, null);

            return PartialView("~/Views/TaxClass/ProductPopupList.cshtml", gridModel);
        }

        #endregion Tax Rule

        #endregion Public Methods

        #region Private Methods

        //Get the grid model.
        public GridModel GetProdctsXMLGrid(FilterCollectionDataModel model, ProductDetailsListViewModel productList, GridListType gridListType)
        {
            GridModel gridModel;
            //Get the grid model.
            if (IsNull(productList?.XmlDataList) || (productList?.XmlDataList?.Count == 0))
            {
                gridModel = GetProductsDynamicGrid(model, productList, gridListType);
            }
            else
            {
                gridModel = FilterHelpers.GetDynamicGridModel(model,
                                                              productList.XmlDataList,
                                                              gridListType.ToString(),
                                                              string.Empty,
                                                              null,
                                                              true,
                                                              true,
                                                              productList?.GridModel?.FilterColumn?.ToolMenuList,
                                                              AttrColumn(productList.AttrubuteColumnName));
            }

            //Set the total record count
            gridModel.TotalRecordCount = productList.TotalResults;
            return gridModel;
        }

        //Get the grid model.
        private GridModel GetProductsDynamicGrid(FilterCollectionDataModel model, ProductDetailsListViewModel productList, GridListType gridListType)
        => FilterHelpers.GetDynamicGridModel(model,
                                             IsNull(productList?.ProductDetailList)
                                                   ? new List<ProductDetailsViewModel>()
                                                   : productList.ProductDetailList,
                                             gridListType.ToString(),
                                             string.Empty,
                                             null,
                                             true,
                                             true,
                                             productList?.GridModel?.FilterColumn?.ToolMenuList);

        #endregion Private Methods
    }
}