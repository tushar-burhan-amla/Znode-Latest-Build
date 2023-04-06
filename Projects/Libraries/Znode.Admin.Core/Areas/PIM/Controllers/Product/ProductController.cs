using MvcSiteMapProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Areas.PIM.Controllers
{
    public class ProductsController : BaseController
    {
        #region Private Variables

        private readonly IProductAgent _productAgent;
        private INavigationAgent _navigationAgent;
        private readonly ICategoryAgent _categoryAgent;
        private readonly IPriceAgent _priceAgent;

        #endregion Private Variables

        #region Constructor

        public ProductsController(IProductAgent productAgent, INavigationAgent navigationAgent, ICategoryAgent categoryAgent, IPriceAgent priceAgent)
        {
            _productAgent = productAgent;
            _navigationAgent = navigationAgent;
            _categoryAgent = categoryAgent;
            _priceAgent = priceAgent;
        }

        #endregion Constructor

        //Get the index view for PIM
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources, TitlePIM", Key = "PIM", Area = "PIM", ParentKey = "Home")]
        public virtual ActionResult Index() => View();

        //Get list of product

        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,LabelProductList", Key = "MainProductList", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int pimCatalogId = 0, string catalogName = null)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.View_ManageProductList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ManageProductList.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Remove catalog filter flag in filters.
            RemoveCatalogFilterValue(model, pimCatalogId);

            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetProductList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, pimCatalogId, catalogName);

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.View_ManageProductList);

            return ActionView(productList);
        }

        public virtual ActionResult ProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ManageProductList.ToString(), model);

            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetProductList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model.
            GridModel gridModel = GetProductsDynamicGrid(model, productList, GridListType.View_ManageProductList);

            //Set the total record count
            gridModel.TotalRecordCount = productList.TotalResults;

            return PartialView("_ProductPopupList", gridModel);
        }

        //Get attributes with group to create new product.
        [HttpGet]
        public virtual ActionResult Create(int familyId = 0, int catalogId = 0, int categoryId = 0, int categoryHierarchyId = 0)
        {
            ActionResult action = GotoBackURL();
            if (IsNotNull(action))
                return action;

            PIMFamilyDetailsViewModel attributeFamilyDetails = _productAgent.GetAttributeFamilyDetails(familyId);
            if (!Equals(attributeFamilyDetails, null))
            {
                attributeFamilyDetails.PimCatalogId = catalogId;
                attributeFamilyDetails.CategoryId = categoryId;
                attributeFamilyDetails.PimCategoryHierarchyId = categoryHierarchyId;
                return ActionView(AdminConstants.CreateEdit, attributeFamilyDetails);
            }

            return RedirectToAction<ProductsController>(x => x.List(null, 0, null));
        }

        //Create new product.
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            string errorMessage = string.Empty;
            string selectedtab = Request.QueryString["selectedtab"];
            ProductViewModel createdProduct = _productAgent.CreateProduct(model, out errorMessage);
            SetNotificationMessage(createdProduct.ProductId > 0
                ? (createdProduct.ActionMode == AdminConstants.Create ? GetSuccessNotificationMessage(PIM_Resources.CreateMessage) : GetSuccessNotificationMessage(PIM_Resources.UpdateMessage))
                : GetErrorNotificationMessage(errorMessage));

            if (createdProduct.ProductId < 1)
                return RedirectToAction<ProductsController>(x => x.Create(0, 0, 0, 0));

            // If request is from catalog.
            if (createdProduct.PimCatalogId > 0)
                return RedirectToAction<CatalogController>(x => x.Manage(null, createdProduct.PimCatalogId.GetValueOrDefault(), createdProduct.PimCategoryId.GetValueOrDefault(), null, -1, true));

            return RedirectToAction<ProductsController>(x => x.Edit(createdProduct.ProductId, selectedtab));
        }

        //Edit product
        [HttpGet]
        public virtual ActionResult Edit(int PimProductId, string selectedtab = null)
        {
            ActionResult action = GotoBackURL();
            if (IsNotNull(action))
                return action;

            PIMFamilyDetailsViewModel productAttributes = _productAgent.GetProduct(PimProductId, false);

            if (IsNotNull(productAttributes))
            {
                productAttributes.ProductId = PimProductId;
                return ActionView(AdminConstants.CreateEditProduct, productAttributes);
            }

            SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.ErrorLoadingdata));
            return RedirectToAction<ProductsController>(x => x.List(null, 0, null));
        }

        //Copy product
        [HttpGet]
        public virtual ActionResult Copy(int PimProductId)
        {
            PIMFamilyDetailsViewModel productAttributes = _productAgent.GetProduct(PimProductId, true);

            if (IsNotNull(productAttributes))
            {
                return ActionView(AdminConstants.CreateEdit, productAttributes);
            }

            SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.ErrorLoadingdata));
            return RedirectToAction<ProductsController>(x => x.List(null, 0, null));
        }

        //Delete product
        public virtual ActionResult Delete(string PimProductId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(PimProductId))
            {
                status = _productAgent.DeleteProduct(PimProductId);
                message = status ? Admin_Resources.DeleteMessage : PIM_Resources.ErrorFailedToDeleteProduct;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }


        public virtual ActionResult EditAssignLinkProducts(int pimLinkProductDetailId, string data)
        {
            LinkProductDetailViewModel linkProductDetailViewModel = JsonConvert.DeserializeObject<LinkProductDetailViewModel[]>(data)[0];

            bool status = false;
            string message = string.Empty;
            if (ModelState.IsValid)
                status = _productAgent.EditAssignLinkProducts(
                    new LinkProductDetailViewModel
                    {
                        PimLinkProductDetailId = pimLinkProductDetailId,

                        DisplayOrder = linkProductDetailViewModel.DisplayOrder
                    });

            message = status ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage;
            return Json(new
            {
                status = status,
                message = message,
                cmsWidgetProductId = linkProductDetailViewModel.PimLinkProductDetailId
            }, JsonRequestBehavior.AllowGet);

        }


        //Get attributes with group to create new product on family change.
        [HttpGet]
        public virtual ActionResult GetAttributes(int familyId, int productId, int localeId = 0)
        {
            PIMFamilyDetailsViewModel attributeFamilyDetails = null;
            if (productId > 0)
            {
                attributeFamilyDetails = _productAgent.GetProductAttributes(productId, familyId);
                attributeFamilyDetails.ProductId = productId;
            }
            else
                attributeFamilyDetails = _productAgent.GetAttributeFamilyDetails(familyId);

            if (IsNotNull(attributeFamilyDetails))
                return ActionView(AdminConstants.CreateEdit, attributeFamilyDetails);

            SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.ErrorLoadingdata));
            return RedirectToAction<CategoryController>(x => x.List(null, 0, null));
        }

        //Get configure attribute list for configure product
        [HttpGet]
        public virtual ActionResult GetConfigureAttributeList(int familyId, int productId)
        {
            PIMFamilyDetailsViewModel attributeFamilyDetails = null;
            attributeFamilyDetails = _productAgent.GetConfigureAttribute(familyId, productId);
            return Json(new { status = attributeFamilyDetails?.Attributes.Count > 0 ? "true" : "false", data = attributeFamilyDetails?.Attributes }, JsonRequestBehavior.AllowGet);
        }

        //Get similar combination product id while associating to configure product.
        public virtual ActionResult GetSimilarCombination(int productId) => Json(new { combinationProductIds = _productAgent.GetSimilarCombination(productId) }, JsonRequestBehavior.AllowGet);

        //Activate/De-Activate product in bulk
        public virtual ActionResult ActivateDeactivateProducts(string productIds, bool isActive)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(productIds))
            {
                status = _productAgent.ActivateDeactivateProducts(productIds, isActive);
                message = status ? (status && isActive) ? Admin_Resources.SuccessMessageStatusActive : Admin_Resources.SuccessMessageStatusInactive : Admin_Resources.ErrorMessageFailedStatus;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult ProductCategoryList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedCategoriesToCatalog.ToString(), model);
            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);
            CategoryListViewModel categoryList = _categoryAgent.GetCategoryList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            categoryList.AttrubuteColumnName?.Remove(ZnodeConstant.CategoryImage);

            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, IsNull(categoryList?.XmlDataList) ? new List<dynamic>() : categoryList.XmlDataList, GridListType.UnAssociatedCategoriesToCatalog.ToString(), string.Empty, null, true, true, categoryList?.GridModel?.FilterColumn?.ToolMenuList, AttrColumn(categoryList.AttrubuteColumnName));

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return ActionView(AdminConstants.UnAssociatedCategoriesView, categoryList);
        }

        /// <summary>
        /// Get the List of Associated categories to Products
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ActionResult GetAssociatedProductCategories([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int productId)
        {
            //return the list of associated categories with product.

            //CategoryListViewModel categoryList = _categoryAgent
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedCategoriesToCatalog.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);
            CategoryListViewModel categoryList = _categoryAgent.GetCategoryList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            categoryList.AttrubuteColumnName?.Remove(ZnodeConstant.CategoryImage);

            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, IsNull(categoryList?.XmlDataList) ? new List<dynamic>() : categoryList.XmlDataList, GridListType.UnAssociatedCategoriesToCatalog.ToString(), string.Empty, null, true, true, categoryList?.GridModel?.FilterColumn?.ToolMenuList, AttrColumn(categoryList.AttrubuteColumnName));

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return ActionView(AdminConstants.associatedCategoriesToProducts, categoryList);
        }

        #region Associate Add-ons

        //Get associated addon groups by parentProductId.
        public virtual ActionResult GetAssociatedAddonGroups([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int parentProductId)
        {
            AddonGroupListViewModel associatedAddonGroup = _productAgent.GetAssociatedAddonGroup(parentProductId, model.Filters, model.SortCollection, model.Expands, model.Page, model.RecordPerPage);
            associatedAddonGroup.ParentProductId = parentProductId;

            //Get the grid model.
            foreach (AddonGroupViewModel addonGroup in associatedAddonGroup.AddonGroups)
            {
                //Assign default view filter and sorting if exists for the first request.
                FilterHelpers.GetDefaultView(GridListType.View_ManageProductAddonList.ToString(), model);
                addonGroup.AssociatedChildProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, addonGroup.AssociatedChildProducts.ProductDetailList, GridListType.View_ManageProductAddonList.ToString(), string.Empty, null, true);

                //Set the total record count
                addonGroup.AssociatedChildProducts.GridModel.TotalRecordCount = addonGroup.AssociatedChildProducts.TotalResults;
            }

            return ActionView("_AddonList", associatedAddonGroup);
        }

        //Associate addon group to parent product
        public virtual ActionResult AssociateAddonGroup(ParentProductAssociationModel model)
        {
            bool isAddonGroupAssociated = false;
            if (IsNotNull(model) && model.ParentId > 0 && !string.IsNullOrEmpty(model.AssociatedIds))
                isAddonGroupAssociated = _productAgent.AssociateAddonGroups(model.ParentId, model.AssociatedIds);

            return RedirectToAction<ProductsController>(x => x.GetAssociatedAddonGroups(null, model.ParentId));
        }

        //Delete addon product by addonproductId and ParentproductId
        public virtual ActionResult DeleteAddonProduct(int addonProductId, int parentProductId)
        {
            bool isAddonUnassociated = false;
            if (addonProductId > 0)
            {
                isAddonUnassociated = _productAgent.DeleteAddonProduct(addonProductId, parentProductId);
            }
            return RedirectToAction<ProductsController>(x => x.GetAssociatedAddonGroups(null, parentProductId));
        }

        //Get unassociated addon group by parentproductId
        public virtual ActionResult GetUnassociatedAddonGroups([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int parentProductId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociateAddonGroupList.ToString(), model);
            //Get the list of all products.
            AddonGroupListViewModel addonGroups = _productAgent.GetUnassociatedAddonGroups(parentProductId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            addonGroups.GridModel = FilterHelpers.GetDynamicGridModel(model, addonGroups.AddonGroups, GridListType.UnassociateAddonGroupList.ToString(), string.Empty, null, true);

            //Set the total record count.
            addonGroups.GridModel.TotalRecordCount = addonGroups.TotalResults;
            addonGroups.ParentProductId = parentProductId;

            return PartialView("_UnassociatedAddonGroups", addonGroups);
        }

        //Associate addon product to parent product
        public virtual JsonResult AssociatedAddonProduct(ParentProductAssociationModel model)
        {
            bool isAddonProductAssociated = _productAgent.AssociateAddonProduct(model.ParentId, model.AssociatedIds, model.DisplayOrder, model?.IsDefault, model.PimProductId);
            return Json(new { status = isAddonProductAssociated, message = isAddonProductAssociated ? Admin_Resources.AssignSuccessful : Admin_Resources.ErrorAssignProducts }, JsonRequestBehavior.AllowGet);
        }

        //Update Addon Display Order
        public virtual JsonResult UpdateAddonDisplayOrder(int pimAddonProductDetailId, string data)
        {
            string message = string.Empty;
            if (ModelState.IsValid && IsNotNull(data))
            {
                AddonProductDetailViewModel model = _productAgent.UpdateAddonDisplayOrder(pimAddonProductDetailId, data);

                if (!model.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = model.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Unassociate addon product
        public virtual ActionResult UnassociateAddonProducts(string pimAddonProductDetailId, int productId = 0)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(pimAddonProductDetailId))
            {
                bool isAddonProductUnassociated = _productAgent.DeleteAddonProductDetail(pimAddonProductDetailId, productId);
                return Json(new { status = isAddonProductUnassociated, message = isAddonProductUnassociated ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Update addon product association.
        public virtual JsonResult UpdateProductAddonAssociation(AddOnProductViewModel addonProductViewModel)
        {
            addonProductViewModel = _productAgent.UpdateProductAddonAssociation(addonProductViewModel);
            return Json(new { status = !addonProductViewModel.HasError, message = !addonProductViewModel.HasError ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion Associate Add-ons

        #region Assign Link products

        //Get list of assigned link products.
        public virtual ActionResult AssignedLinkProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int parentProductId, int attributeId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ManageLinkProductList.ToString(), model);

            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetAssignedLinkProducts(parentProductId, attributeId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            productList.ParentProductId = parentProductId;
            productList.AttributeId = attributeId;

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.View_ManageLinkProductList);

            ViewBag.ParentProductId = parentProductId;
            ViewBag.AttributeId = attributeId;
            productList.GridModel.FrontObjectName = productList.GridModel.FrontObjectName + attributeId.ToString();
            return ActionView("_LinkProductList", productList);
        }

        //Assign product to parent product as link product.
        public virtual JsonResult AssignLinkProduct(ParentProductAssociationModel model)
        {
            bool isLinkAssociated = _productAgent.AssignLinkProducts(model.ParentId, model.AttributeId, model.AssociatedIds);
            return Json(new { status = isLinkAssociated, message = isLinkAssociated ? Admin_Resources.AssignSuccessful : PIM_Resources.ErrorAssignedLinkProducts }, JsonRequestBehavior.AllowGet);
        }

        //Removes relation of child product from parent product as link products.
        public virtual ActionResult UnAssignLinkProducts(string pimLinkProductDetailId)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(pimLinkProductDetailId))
            {
                bool isLinkProductUnassigned = _productAgent.UnassignLinkProducts(pimLinkProductDetailId, out errorMessage);
                return Json(new { status = isLinkProductUnassigned, message = isLinkProductUnassigned ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion Assign Link products

        #region Product Type

        //Get list of assigned products to parent product.
        public virtual ActionResult GetAssociatedProducts([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int parentProductId, int attributeId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ManageProductTypeList_GroupProduct.ToString(), model);
            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetAssociatedProducts(parentProductId, attributeId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            productList.ParentProductId = parentProductId;
            productList.AttributeId = attributeId;

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.View_ManageProductTypeList_GroupProduct);

            return ActionView("_AssociatedProductList", productList);
        }

        //Get list of assigned bundle products to parent product.
        public virtual ActionResult GetAssociatedBundleProducts([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int parentProductId, int attributeId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ManageProductTypeList_BundleProduct.ToString(), model);
            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetAssociatedProducts(parentProductId, attributeId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            productList.ParentProductId = parentProductId;
            productList.AttributeId = attributeId;

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.View_ManageProductTypeList_BundleProduct);
            productList.ProductType = ZnodeConstant.BundleProduct;
            return ActionView("_AssociatedProductList", productList);
        }

        //Get list of assigned products to parent product.
        public virtual ActionResult GetAssociatedConfigureProducts([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int parentProductId, string associatedAttributeIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ManageProductTypeList.ToString(), model);
            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetAssociatedUnAssociatedConfigureProducts(parentProductId, string.Empty, associatedAttributeIds, false, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            SessionHelper.SaveDataInSession<List<dynamic>>("ProductList", productList?.ProductDetailListDynamic?.ToList());

            SessionHelper.SaveDataInSession<List<dynamic>>("AssociatedProductList", productList?.ProductDetailListDynamic?.ToList());
            if (productList?.ProductDetailListDynamic?.Count > 0)
                foreach (var item in productList.ProductDetailListDynamic)
                    item.IsNonEditableRow = string.Empty;

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.View_ManageProductTypeList);

            //Adding columns at runtime into the grid.
            if (productList.NewAttributeList?.Count > 0)
            {
                foreach (var item in productList.NewAttributeList)
                    productList.GridModel.WebGridColumn.Add(new System.Web.Helpers.WebGridColumn { ColumnName = item.AttributeCode, Header = item.AttributeCode, CanSort = true });
            }

            //Set the total record count
            productList.GridModel.TotalRecordCount = productList.TotalResults;

            return ActionView("_AssociatedConfigureProductList", productList);
        }

        //Assign product to parent product.
        public virtual JsonResult AssociateProducts(AssociatedProductViewModel model)
        {
            bool status = _productAgent.AssociateProducts(model.AssociatedProductIds, model.ParentProductId, model.AttributeId);
            return Json(new { status = status, message = status ? Admin_Resources.ProductAssociatedSuccessMessage : Admin_Resources.ErrorInProductAssociation }, JsonRequestBehavior.AllowGet);
        }

        //Get Product list which are not assigned to parent product.
        public virtual JsonResult UnassociateProducts(string pimProductTypeAssociationId, int productId = 0)
        {
            if (!string.IsNullOrEmpty(pimProductTypeAssociationId))
            {
                bool status = _productAgent.UnassociatedProduct(pimProductTypeAssociationId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Get list of Products to be associated.
        public virtual ActionResult GetProductsToBeAssociated([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, string productIds, string productType)
        {
            if (productType != ZnodeConstant.BundleProduct)
            {
                //Assign default view filter and sorting if exists for the first request.
                FilterHelpers.GetDefaultView(GridListType.View_ManageProductTypeListForToBeAssociated.ToString(), model);
            }
            else
            {
                FilterHelpers.GetDefaultView(GridListType.View_ManageProductTypeList_BundleProduct.ToString(), model);
            }

            ProductDetailsListViewModel productList = _productAgent.GetProductsToBeAssociated(productIds, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Assign values for AssociatedProductIds to list model
            productList.AssociatedProductIds = productIds;

            //Get the grid model.
            productList.GridModel = productType != ZnodeConstant.BundleProduct? GetProdctsXMLGrid(model, productList, GridListType.View_ManageProductTypeListForToBeAssociated) : GetProdctsXMLGrid(model, productList, GridListType.View_ManageProductTypeList_BundleProduct);

            return ActionView("_ProductsToBeAssociate", productList);
        }

        //Get list of Products to be associated.
        public virtual ActionResult GetConfigureProductsToBeAssociated([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, string associatedProductIds, string associatedAttributeIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_ManageProductTypeList.ToString(), model);
            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetAssociatedUnAssociatedConfigureProducts(0, associatedProductIds, associatedAttributeIds, false, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            SessionHelper.SaveDataInSession<List<dynamic>>("ProductList", productList?.ProductDetailListDynamic?.ToList());

            if (productList?.ProductDetailListDynamic?.Count > 0)
                foreach (var item in productList.ProductDetailListDynamic)
                    item.IsNonEditableRow = string.Empty;

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.View_ManageProductTypeList);
            //Adding columns at runtime into the grid.
            if (productList.NewAttributeList?.Count > 0)
            {
                foreach (var item in productList.NewAttributeList)
                    productList.GridModel.WebGridColumn.Add(new System.Web.Helpers.WebGridColumn { ColumnName = item.AttributeCode, Header = item.AttributeCode });
            }

            return ActionView("_ConfigureProductsToBeAssociate", productList);
        }

        #endregion Product Type

        #region Custom Field

        //Create new Custom field to Product
        [HttpGet]
        public virtual PartialViewResult AddCustomField(int productId)
        {
            CustomFieldViewModel customFieldViewModel = new CustomFieldViewModel();
            customFieldViewModel = _productAgent.GetLocales();
            customFieldViewModel.ProductId = productId;
            return PartialView(AdminConstants.AddCustomFieldView, customFieldViewModel);
        }

        //Add new custom field.
        [HttpPost]
        public virtual ActionResult AddCustomField(CustomFieldViewModel customFieldViewModel)
        {
            customFieldViewModel = ModelState.IsValid ? _productAgent.AddCustomField(customFieldViewModel) : _productAgent.GetErrorViewModel(customFieldViewModel, PIM_Resources.ErrorLoadingdata);

            if (customFieldViewModel.HasError)
                return PartialView(customFieldViewModel);
            else
                return Json(new
                {
                    isSuccess = customFieldViewModel.CustomFieldId < 1 ? false : true,
                    Message = ((customFieldViewModel.CustomFieldId < 1) || customFieldViewModel.HasError) ? customFieldViewModel.ErrorMessage : PIM_Resources.CreateMessage
                }, JsonRequestBehavior.AllowGet);
        }

        //Get list of all CustomFields associated with product
        public virtual ActionResult CustomFields([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int productId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePimCustomField.ToString(), model);
            //Get the list of all CustomFields.
            CustomFieldListViewModel customFieldList = _productAgent.GetCustomFields(productId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            customFieldList.ProductId = productId;
            //Get the grid model.
            customFieldList.GridModel = FilterHelpers.GetDynamicGridModel(model, customFieldList.CustomFields, GridListType.ZnodePimCustomField.ToString(), string.Empty, null, true, true, customFieldList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            customFieldList.GridModel.TotalRecordCount = customFieldList.TotalResults;

            return ActionView(AdminConstants.CustomFieldListView, customFieldList);
        }

        //Get data for edit custom field.
        [HttpGet]
        public virtual ActionResult EditCustomField(int customFieldId, int productId)
        {
            if (productId > 0 && customFieldId > 0)
            {
                CustomFieldViewModel customField = _productAgent.GetCustomField(customFieldId, null);

                if (Equals(customField, null))
                {
                    return RedirectToAction<ProductsController>(x => x.CustomFields(null, 0));
                }
                return PartialView(AdminConstants.AddCustomFieldView, customField);
            }
            return View(GridListType.View_ManageProductList);
        }

        //Post data for edit custom field.
        [HttpPost]
        public virtual ActionResult EditCustomField(CustomFieldViewModel customFieldViewModel)
        {
            if (ModelState.IsValid)
                customFieldViewModel = _productAgent.UpdateCustomField(customFieldViewModel);
            else
                customFieldViewModel.ErrorMessage = PIM_Resources.ErrorLoadingdata;

            return Json(new
            {
                isSuccess = (!Equals(customFieldViewModel, null) && !customFieldViewModel.HasError),
                Message = (Equals(customFieldViewModel, null) || customFieldViewModel.HasError) ? PIM_Resources.UpdateErrorMessage : PIM_Resources.UpdateMessage
            }, JsonRequestBehavior.AllowGet);
        }

        //Delete custom field
        public virtual ActionResult DeleteCustomField(string customFieldId, int productId)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(customFieldId))
            {
                bool isDeleted = _productAgent.DeleteCustomField(customFieldId, out errorMessage);
                errorMessage = string.IsNullOrEmpty(errorMessage) ? Admin_Resources.ErrorFailedToDelete : errorMessage;
                return Json(new { status = isDeleted, message = isDeleted ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get assigned personalized attributes
        public virtual ActionResult GetAssignedPersonalizedAttributes(int productId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.listTypeSessionKey);
            if (productId > 0)
            {
                _productAgent.SetFilters(model.Filters, productId);
                PIMProductAttributeValuesListViewModel listViewModel = _productAgent.GetAssignedPersonalizedAttributes(model.Filters, model.SortCollection, null, model.Page, model.RecordPerPage);
                listViewModel.PimProductId = productId;
                listViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                TempData[AdminConstants.ProductId] = productId;
                return PartialView("_AssignedPersonalizedAttributes", listViewModel);
            }
            return null;
        }

        //Get un-assigned personalized attributes
        public virtual ActionResult GetUnAssignedPersonalizedAttributes(int productId)
        {
            PIMAttributeValueListViewModel listViewModel = new PIMAttributeValueListViewModel();
            listViewModel.baseDropDownList = _productAgent.GetUnAssignedPersonalizedAttributes(productId);
            listViewModel.PimProductId = productId;
            return PartialView("_UnassignedPersonalizedAttributes", listViewModel);
        }

        //Assign personalized attributes
        public virtual ActionResult AssignPersonalizedAttributes(string id, bool flag, int entityId = 0)
        {
            bool status = false;
            string message = string.Empty;
            //Entity id  is refer to pim product id as this is generic code
            int pimProductId = entityId > 0 ? entityId : Convert.ToInt32(TempData[AdminConstants.ProductId]);
            if (flag) //Assigns attribute.
            {
                if (id?.Length > 0 && pimProductId > 0)
                    status = _productAgent.AssignPersonalizedAttributes(id, pimProductId, out message);
            }
            else //Unassign attributes.
            {
                if (pimProductId > 0)
                    status = _productAgent.UnassignPersonalizeAttributes(id, pimProductId, out message);
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        #region Edit associated Products prototype

        public virtual JsonResult UpdateAssociatedProducts(int pimProductTypeAssociationId, int relatedProductId, string data, int pimProductId = 0, int productId = 0)
        {
            //In configurable product, productId is being used
            if (productId > 0 && pimProductId == 0)
                pimProductId = productId;
        
            ProductTypeAssociationViewModel model = JsonConvert.DeserializeObject<ProductTypeAssociationViewModel[]>(data)[0];

            model = new ProductTypeAssociationViewModel { PimProductTypeAssociationId = pimProductTypeAssociationId, PimProductId = pimProductId, PimParentProductId = relatedProductId, DisplayOrder = model.DisplayOrder, IsDefault = model.IsDefault , BundleQuantity = model.BundleQuantity};

            bool status = _productAgent.UpdateAssociatedProduct(model);

            string message = status ? PIM_Resources.UpdateMessage : relatedProductId > 0 ? PIM_Resources.UpdateErrorMessage : PIM_Resources.ErrorSaveTheProductFirst;
            return Json(new { status = status, message = message, PimProductId = model.PimProductId, PimAttributeId = model.PimAttributeId }, JsonRequestBehavior.AllowGet);
        }

        #endregion Edit associated Products prototype

        #endregion Custom Field

        //Get un-associated product list for Group/Bundle product.
        public virtual ActionResult GetUnassociatedProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int parentProductId, int listType, int? addonProductId, string associatedProductIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedProducts.ToString(), model);
            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetUnassociatedProducts(parentProductId, associatedProductIds, addonProductId.GetValueOrDefault(), listType, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            productList.AttributeId = SessionHelper.GetDataFromSession<int>("linkAttributeId");
            productList.AssociatedProductIds = !string.IsNullOrEmpty(associatedProductIds) ? associatedProductIds : "0";

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.UnassociatedProducts);

            return PartialView("_UnassociatedProducts", productList);
        }

        //Get un-associated product list for configure product.
        public virtual ActionResult GetUnassociatedConfigureProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int parentProductId, string attributeIds, string associatedProductIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnassociatedProductsDynamic.ToString(), model);
            //Get the list of all products.
            ProductDetailsListViewModel productList = _productAgent.GetAssociatedUnAssociatedConfigureProducts(parentProductId, associatedProductIds, attributeIds, true, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            SessionHelper.SaveDataInSession<List<dynamic>>("ProductList", productList?.ProductDetailListDynamic?.ToList());

            //Get the grid model.
            productList.GridModel = GetProdctsXMLGrid(model, productList, GridListType.UnassociatedProductsDynamic);
            //Adding columns at runtime into the grid.
            if (productList.NewAttributeList?.Count > 0)
            {
                foreach (var item in productList.NewAttributeList)
                    productList.GridModel.WebGridColumn.Add(new System.Web.Helpers.WebGridColumn { ColumnName = item.AttributeCode, Header = item.AttributeCode });
            }
            // Removing tools option in grid section for configurable product inside assosiated product popup 
            if (productList.GridModel?.FilterColumn?.ToolMenuList != null)
            {
                productList.GridModel.FilterColumn.ToolMenuList = null;
            }
           
            return PartialView("_UnassociatedConfigureProducts", productList);
        }

        #region Product Update Import
        //Get product update dialog.
        [HttpGet]
        public virtual ActionResult UpdateProducts()
        {
            ImportViewModel viewModel = new ImportViewModel();
            return PartialView(AdminConstants.UpdateProductPartialView, viewModel);
        }

        //Download product template.
        public virtual JsonResult DownloadProductTemplate()
       => Json(new { data = _productAgent.getFileContent(), AdminConstants.ContentTypeCSV, fileName = string.Concat(AdminConstants.ProductUpdate, AdminConstants.CSV) }, JsonRequestBehavior.AllowGet);

        //This method will upload the file and process the uploaded data for import.
        [HttpPost]
        public virtual ActionResult UpdateProducts(ImportViewModel importModel)
        {
            string statusMessage = string.Empty;
            if (_productAgent.ImportProductUpdateData(importModel, out statusMessage))
                SetNotificationMessage(GetSuccessNotificationMessage(statusMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(statusMessage));

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Publish Product

        //Draft product and then publish product
        public virtual ActionResult UpdateAndPublishProduct([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            string errorMessage = string.Empty;
            string revisionType = Convert.ToString(model.GetValue("revisionType"));

            ProductViewModel createdProduct = _productAgent.CreateProduct(model, out errorMessage);

            if (createdProduct.ProductId > 0)
                SetNotificationMessage(_productAgent.PublishProduct(Convert.ToString(createdProduct.ProductId), revisionType, out errorMessage) ?
                    GetSuccessNotificationMessage(PIM_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(errorMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(errorMessage));

            return RedirectToAction<ProductsController>(x => x.Edit(createdProduct.ProductId, null));
        }

        public virtual ActionResult PublishProduct(int pimProductId, string revisionType)
        {
            if (pimProductId > 0)
            {
                string errorMessage = string.Empty;
                bool status = false;

                status = _productAgent.PublishProduct(Convert.ToString(pimProductId), revisionType, out errorMessage);

                return Json(new { status = status, message = status ? PIM_Resources.TextPublishedSuccessfully : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = PIM_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
        }

        #endregion Publish Product

        #region Product SKU list for Autocomplete feature

        //Get product sku list for attribute code as SKU and its attribute value.
        [HttpGet]
        public virtual JsonResult GetProductListBySKU(string attributeValue)
           => Json(_productAgent.GetSkuProductListBySKU(attributeValue, null), JsonRequestBehavior.AllowGet);

        #endregion Product SKU list for Autocomplete feature

        #region Product association with Price
        /// <summary>
        /// This method is used to render the price tab in add/edit product page.
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult GetProductPriceBySku(ProductPriceListSKUDataViewModel productPriceListSKUDataViewModel)
        {
            PriceSKUViewModel priceSKUViewModel = _priceAgent.GetProductPricingDetailsBySku(productPriceListSKUDataViewModel);
            ProductPriceSKUDataViewModel productpriceSKUDataViewModel = new ProductPriceSKUDataViewModel()
            {
                PriceList = _priceAgent.GetAllActivePriceList(),
                PriceSKU = priceSKUViewModel.PriceId != null ? priceSKUViewModel : new PriceSKUViewModel() { SKU = productPriceListSKUDataViewModel.Sku },
                PriceTier = new PriceTierViewModel { PriceListId = priceSKUViewModel?.PriceListId },
                PriceListId = priceSKUViewModel.PriceListId == null ? productPriceListSKUDataViewModel.ProductpriceListId : priceSKUViewModel.PriceListId,
                ProductId = productPriceListSKUDataViewModel.PimProductId,
                baseDropDownList= _priceAgent.GetCustomColumnList()
        };
            return ActionView(AdminConstants.ProductPricePartialView, productpriceSKUDataViewModel);
        }

        /// <summary>
        /// This method is used to add/update the product price
        /// </summary>
        /// <param name="priceSKUDataViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult AddProductPriceBySku(PriceSKUDataViewModel priceSKUDataViewModel)
        {
            int? productId = priceSKUDataViewModel?.PriceSKU?.ProductId;
            if (ModelState.IsValid)
            {
                priceSKUDataViewModel.PriceSKU.PriceListId = priceSKUDataViewModel.PriceListId;
                priceSKUDataViewModel = priceSKUDataViewModel.PriceId == null || priceSKUDataViewModel.PriceId == 0 ? _priceAgent.AddSKUPrice(priceSKUDataViewModel) : _priceAgent.UpdateSKUPrice(priceSKUDataViewModel);
                if (!priceSKUDataViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage((priceSKUDataViewModel.PriceId == null ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage)));
                }
                else if (priceSKUDataViewModel.PriceSKU.PriceId > 0)
                {
                    SetNotificationMessage(GetErrorNotificationMessage(priceSKUDataViewModel.ErrorMessage));
                }
                return Redirect($"{AdminConstants.ProductEditUrl}{"?selectedtab="}{AdminConstants.PriceTab}{"&PimProductId="}{Convert.ToString(productId)}{"&priceListId="}{Convert.ToString(priceSKUDataViewModel.PriceListId)}");
            }
            SetNotificationMessage(GetErrorNotificationMessage(priceSKUDataViewModel.ErrorMessage));
            return Redirect($"{AdminConstants.ProductEditUrl}{"?selectedtab="}{AdminConstants.PriceTab}{"&PimProductId="}{Convert.ToString(productId)}{"&priceListId="}{Convert.ToString(priceSKUDataViewModel.PriceListId)}");
        }

        /// <summary>
        /// This method is used to delete the tier price of product.
        /// </summary>
        /// <param name="priceTierId"></param>
        /// <returns></returns>
        public virtual JsonResult DeleteTierPrice(int priceTierId)
        {
            bool status = false;
            if (priceTierId > 0)
            {
                status = _priceAgent.DeleteTierPrice(priceTierId);
            }
            return Json(new { HasNoError = status, Message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        // Update display order of Configurable Products.
        public virtual JsonResult UpdateAssociatedConfigurableProducts(int pimProductTypeAssociationId, int relatedProductId, bool IsDefault, string data, int productId = 0)
        {
            ProductTypeAssociationViewModel productTypeAssociationViewModel = JsonConvert.DeserializeObject<ProductTypeAssociationViewModel[]>(data)[0];
            productTypeAssociationViewModel.IsDefault = IsDefault;
            productTypeAssociationViewModel.PimProductId = productId;
            productTypeAssociationViewModel.PimProductTypeAssociationId = pimProductTypeAssociationId;
            productTypeAssociationViewModel.PimParentProductId = relatedProductId;

            bool status = _productAgent.UpdateAssociatedProduct(productTypeAssociationViewModel);

            string message = status ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage;
            return Json(new { status = status, message = message, PimProductId = productTypeAssociationViewModel.PimProductId, PimAttributeId = productTypeAssociationViewModel.PimAttributeId }, JsonRequestBehavior.AllowGet);
        }
        #endregion

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
        {
            if (IsNull(productList?.ProductDetailList) || productList?.ProductDetailList?.Count == 0)

                return FilterHelpers.GetDynamicGridModel(model,
                                                       productList.ProductDetailListDynamic,
                                                       gridListType.ToString(),
                                                       string.Empty,
                                                       null,
                                                       true,
                                                       true,
                                                       productList?.GridModel?.FilterColumn?.ToolMenuList);
            else
                return FilterHelpers.GetDynamicGridModel(model,
                                                    productList.ProductDetailList,
                                                    gridListType.ToString(),
                                                    string.Empty,
                                                    null,
                                                    true,
                                                    true,
                                                    productList?.GridModel?.FilterColumn?.ToolMenuList);
        }

        //Remove catalog filter flag in FilterCollectionDataModel
        private void RemoveCatalogFilterValue(FilterCollectionDataModel model, int pimCatalogId)
        {
            model.Filters?.RemoveAll(x => string.Equals(x.FilterName, AdminConstants.IsCatalogFilter, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion Private Methods
    }
}