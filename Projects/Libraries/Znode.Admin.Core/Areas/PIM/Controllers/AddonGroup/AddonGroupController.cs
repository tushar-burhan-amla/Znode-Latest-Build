using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Areas.PIM.Controllers
{
    public class AddonGroupController : BaseController
    {
        #region Private Variables

        private readonly IAddonGroupAgent _addonGroupAgent;

        #endregion Private Variables

        #region Add-on Group

        public AddonGroupController(IAddonGroupAgent addonGroupAgent)
        {
            _addonGroupAgent = addonGroupAgent;
        }

        //GET: Action to show create new addon group view.
        [HttpGet]
        public virtual ActionResult CreateAddonGroup()
        {
            AddonGroupViewModel addonGroup = new AddonGroupViewModel();
            addonGroup.PimAddonGroupLocales = new List<AddonGroupLocaleViewModel>();
            addonGroup.PimAddonGroupLocales.Add(new AddonGroupLocaleViewModel());
            addonGroup.Locale = LocaleModelMap.ToLocaleListItem(DefaultSettingHelper.GetActiveLocaleList());
            addonGroup.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            return (Request.IsAjaxRequest()) ? PartialView("_CreateEditAddonGroup", addonGroup) : ActionView("CreateEditAddonGroup", addonGroup);
        }

        //GET: Action to create new addon group.
        [HttpPost]
        public virtual ActionResult CreateAddonGroup(AddonGroupViewModel model)
        {
            if (IsNotNull(model))
            {
                string errorMessage = string.Empty;
                model = _addonGroupAgent.CreateAddonGroup(model);
                if (IsNotNull(model) && model.PimAddonGroupId > 0)
                    return ReturnCreateAddonGroupResult(model, true, true);
            }
            model.Locale = LocaleModelMap.ToLocaleListItem(DefaultSettingHelper.GetActiveLocaleList());
            return ReturnCreateAddonGroupResult(model, false, false);
        }

        //GET: Action to show edit view.
        [HttpGet]
        public virtual ActionResult EditAddonGroup(int pimAddOnGroupId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            AddonGroupViewModel addonGroup = new AddonGroupViewModel();
            addonGroup = _addonGroupAgent.GetAddonGroup(pimAddOnGroupId);
            addonGroup.AddonGroupName = addonGroup?.PimAddonGroupLocales?.FirstOrDefault()?.AddonGroupName;
            return View("CreateEditAddonGroup", addonGroup);
        }

        //POST: Action to update addon group.
        [HttpPost]
        public virtual ActionResult EditAddonGroup(AddonGroupViewModel model)
        {
            if (IsNotNull(model) && ModelState.IsValid)
            {
                model = _addonGroupAgent.UpdateAddonGroup(model);
                model.Locale = LocaleModelMap.ToLocaleListItem(DefaultSettingHelper.GetActiveLocaleList());
                if (IsNotNull(model) && !model.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.UpdateMessage));
                    return RedirectToAction<AddonGroupController>(x => x.EditAddonGroup(model.PimAddonGroupId));
                }
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                return View("CreateEditAddonGroup", model);
            }
            SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorAddonGroupUpdate));
            return View("CreateEditAddonGroup", model);
        }

        //Action for addon group list.
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,LabelAddAddonGroup", Key = "AddOnGroup", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            AddonGroupListViewModel addonGroupList = new AddonGroupListViewModel();
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.View_GetPimAddonGroups.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_GetPimAddonGroups.ToString(), model);
            //Get the list of all addon groups.
            addonGroupList = _addonGroupAgent.GetAddonGroupList(model.Filters, model.SortCollection, model.Expands, model.Page, model.RecordPerPage);

            //Get the grid model.
            addonGroupList.GridModel = FilterHelpers.GetDynamicGridModel(model, addonGroupList?.AddonGroups, GridListType.View_GetPimAddonGroups.ToString(), string.Empty, null, true, true, addonGroupList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            addonGroupList.GridModel.TotalRecordCount = addonGroupList.TotalResults;

            //Return addon group list view.
            return ActionView("AddonGroupList", addonGroupList);
        }

        // Action to delete addon group by pimAddOnGroupId.
        public virtual ActionResult DeleteAddonGroup(string pimAddOnGroupId)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(pimAddOnGroupId))
            {
                bool isAddonGroupDeleted = _addonGroupAgent.DeleteAddonGroup(pimAddOnGroupId, out errorMessage);
                return Json(new { status = isAddonGroupDeleted, message = isAddonGroupDeleted ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        #endregion Add-on Group

        //

        //Action to associate products with addon groups.
        [HttpPost]
        public virtual ActionResult AssociateAddonGroupProducts(ParentProductAssociationModel model)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(model.AssociatedIds))
            {
                bool isAddonGroupProductAssociated = _addonGroupAgent.AssociateAddonGroupProduct(model.ParentId, model.AssociatedIds);
                return Json(new { status = isAddonGroupProductAssociated, message = isAddonGroupProductAssociated ? "Products associated to addon group successfully." : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Please select products to be associated." }, JsonRequestBehavior.AllowGet);
        }

        //Associated addon group products.
        public virtual ActionResult GetAssociatedProducts(int addonGroupId, string addonGroupName, int localeId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            AddonGroupViewModel addonGroupViewModel = new AddonGroupViewModel();
            addonGroupViewModel.PimAddonGroupId = addonGroupId;
            addonGroupViewModel.AddonGroupName = addonGroupName;
            addonGroupViewModel.LocaleId = localeId;

            ProductDetailsListViewModel associatedProduct = new ProductDetailsListViewModel();

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePimAddonGroupProduct.ToString(), model);

            //Get the list of all addon groups.
            associatedProduct = _addonGroupAgent.GetAssociatedAddonGroupProduct(addonGroupId, localeId, model.SortCollection, model.Expands, model.Filters, model.Page, model.RecordPerPage);

            //Get the grid model.
            associatedProduct.GridModel = GetProdctsXMLGrid(model, associatedProduct, GridListType.ZnodePimAddonGroupProduct);

            //assign associated addon group product in addon group view model.
            addonGroupViewModel.AssociatedChildProducts = associatedProduct;

            //Return addon group list view.
            return ActionView("AssociateProducts", addonGroupViewModel);
        }

        public virtual ActionResult GetUnassociatedProducts(int addonGroupId, int localeId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedProducts.ToString(), model);

            ProductDetailsListViewModel associatedProduct = _addonGroupAgent.GetUnassociatedAddonGroupProduct(addonGroupId, localeId, model.SortCollection, model.Expands, model.Filters, model.Page, model.RecordPerPage);
            associatedProduct?.AttrubuteColumnName?.Remove(AdminConstants.ProductImage);
            //Remove tool option.
            associatedProduct?.GridModel?.FilterColumn?.ToolMenuList.Clear();
            //Get the grid model.
            GridModel gridModel = GetProdctsXMLGrid(model, associatedProduct, GridListType.UnassociatedProducts);

            //Set the total record count
            gridModel.TotalRecordCount = associatedProduct.TotalResults;
            associatedProduct.GridModel = gridModel;
            associatedProduct.AddonGroupId = addonGroupId;
            associatedProduct.LocaleId = localeId;

            return PartialView("~/Areas/PIM/Views/AddonGroup/ProductPopupList.cshtml", associatedProduct);
        }

        public virtual ActionResult DeleteAddonGroupProducts(string addonGroupProductId)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(addonGroupProductId))
            {
                bool isAddonGroupProductAssociationDeleted = _addonGroupAgent.DeleteAddonGroupProducts(addonGroupProductId, out errorMessage);
                return Json(new { status = isAddonGroupProductAssociationDeleted, message = isAddonGroupProductAssociationDeleted ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = "Please select atleast one associated product to be deleted." }, JsonRequestBehavior.AllowGet);
        }

        //method to return create addon group result
        private ActionResult ReturnCreateAddonGroupResult(AddonGroupViewModel model, bool isSuccess, bool isRedirect)
        {
            string statusMessage = isSuccess ? PIM_Resources.CreateMessage : (string.IsNullOrEmpty(model.ErrorMessage) ? PIM_Resources.ErrorAddonGroupCreated : model.ErrorMessage);

            if (Request.IsAjaxRequest())
                return Json(new { status = isSuccess, message = statusMessage }, JsonRequestBehavior.AllowGet);

            SetNotificationMessage(isSuccess ? GetSuccessNotificationMessage(statusMessage) : GetErrorNotificationMessage(statusMessage));
            return isRedirect ? RedirectToAction<AddonGroupController>(x => x.EditAddonGroup(model.PimAddonGroupId)) : View("CreateEditAddonGroup", model);
        }

        #region Private Methods

        //Get the grid model.
        private GridModel GetProdctsXMLGrid(FilterCollectionDataModel model, ProductDetailsListViewModel productList, GridListType gridListType)
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