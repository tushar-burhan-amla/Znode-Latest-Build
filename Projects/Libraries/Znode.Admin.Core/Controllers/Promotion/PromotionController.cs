using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class PromotionController : BaseController
    {
        #region Private Variables
        private readonly  IPromotionAgent _promotionAgent;
        private readonly string storeListAsidePanelPopup = "_AsideStorelistPanelPopup";
        #endregion

        #region Public Constructor
        public PromotionController(IPromotionAgent promotionAgent)
        {
            _promotionAgent = promotionAgent;
        }
        #endregion

        #region Promotion

        //Action for promotionlist.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TextPromotion", Key = "Promotion", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePromotion.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePromotion.ToString(), model);
            //Get the list of promotions            
            PromotionListViewModel promotionListViewModel = _promotionAgent.GetPromotionList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);

            //Get the grid model
            promotionListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, promotionListViewModel.PromotionList, GridListType.ZnodePromotion.ToString(), "", null, true, true, promotionListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            promotionListViewModel.GridModel.TotalRecordCount = promotionListViewModel.TotalResults;

            //Returns the shipping list view
            return ActionView(promotionListViewModel);
        }

        //GET: Action to show create promotion view.
        public virtual ActionResult Create()
        {
            PromotionViewModel promotionViewModel = new PromotionViewModel();
            _promotionAgent.BindDropdownValues(promotionViewModel);
            promotionViewModel.StartDate = DateTime.Now;
            promotionViewModel.EndDate = DateTime.Now.AddDays(30);
            promotionViewModel.CouponList = new CouponListViewModel() { CouponList = new List<CouponViewModel>() };
            promotionViewModel.ShowAllStoreCheckbox = true;//_promotionAgent.ShowAllStoreCheckbox();
            return ActionView(AdminConstants.CreateEdit, promotionViewModel);
        }

        //POST: Action to create promotion.
        [HttpPost]
        public virtual ActionResult Create(PromotionViewModel viewModel, [ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            _promotionAgent.BindDropdownValues(viewModel);
            viewModel = _promotionAgent.CreatePromotion(viewModel, model);

            if (!viewModel.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<PromotionController>(x => x.Edit(null, viewModel.PromotionId));
            }

            _promotionAgent.BindDropdownValues(viewModel);
            viewModel.StartDate = DateTime.Now;
            viewModel.EndDate = DateTime.Now.AddDays(30);
            viewModel.CouponList = new CouponListViewModel() { CouponList = new List<CouponViewModel>() };
            SetNotificationMessage(GetErrorNotificationMessage(viewModel.ErrorMessage));
            return ActionView(AdminConstants.CreateEdit, viewModel);
        }

        //Get promotion attribute on changing discount type
        public virtual ActionResult GetPromotionDiscountAttribute(string discountName)
        {
            PIMFamilyDetailsViewModel attributeFamilyDetails = _promotionAgent.GetPromotionAttribute(discountName);
            return ActionView("_CreatePromotionAttribute", attributeFamilyDetails);
        }

        //GET: Action to show edit view.
        public virtual ActionResult Edit([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int? promotionId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            PromotionViewModel promotionViewModel = new PromotionViewModel();
            promotionViewModel = _promotionAgent.GetPromotionById(Convert.ToInt32(promotionId));
            _promotionAgent.BindDropdownValues(promotionViewModel);
            promotionViewModel.CouponList = new CouponListViewModel();
            promotionViewModel.CouponList = _promotionAgent.GetCouponList(promotionId.GetValueOrDefault(), model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);
            promotionViewModel.ShowAllStoreCheckbox = _promotionAgent.ShowAllStoreCheckbox();
            return ActionView(AdminConstants.CreateEdit, promotionViewModel);
        }

        //POST: Action to update promotion.
        [HttpPost]
        public virtual ActionResult Edit(PromotionViewModel viewModel, [ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            ModelState["StartDate"].Errors.Clear();
            PromotionViewModel updatedModel = _promotionAgent.UpdatePromotion(viewModel, model);

            SetNotificationMessage(updatedModel.HasError
            ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
            : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            return RedirectToAction<PromotionController>(x => x.Edit(null, viewModel.PromotionId));
        }

        // Action to delete promotion by promotionId.
        public virtual JsonResult Delete(string promotionId)
        {
            string message = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(promotionId))
            {
                status = _promotionAgent.DeletePromotion(promotionId, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Bind the Catalog List as per Store selected.
        /// </summary>
        /// <param name="storeIds">Store Ids</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public virtual JsonResult CatalogListByStoreId(int storeIds)
       => Json(_promotionAgent.GetPublishedCatalogList(storeIds), JsonRequestBehavior.AllowGet);


        /// <summary>
        /// Bind the Category List as per Store selected.
        /// </summary>
        /// <param name="storeIds">Store Ids</param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public virtual JsonResult BindCategoryList(int storeIds)
       => Json(_promotionAgent.GetPublishedCategoryList(storeIds), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public virtual JsonResult ProfileListByStoreId(int storeIds)
     => Json(_promotionAgent.ProfileListByStorId(storeIds), JsonRequestBehavior.AllowGet);

        [HttpGet]
        public virtual JsonResult CategoryListByStoreId(int storeIds)
    => Json(_promotionAgent.GetPublishedCategoryList(storeIds), JsonRequestBehavior.AllowGet);

        //Check Promotion code already exists or not. 
        [HttpPost]
        public virtual JsonResult IsPromotionNameExist(string PromoCode, int promotionId = 0)
          => Json(!_promotionAgent.CheckPromotionCodeExist(PromoCode, promotionId), JsonRequestBehavior.AllowGet);

        //Check Promotion coupon code already exists or not. 
        [HttpPost]
        public virtual JsonResult CheckCouponCodeExist(string couponCode, int promotionId = 0)
          => Json(!_promotionAgent.CheckCouponCodeExist(couponCode, promotionId), JsonRequestBehavior.AllowGet);

        //This method is used to download Promotion data in csv format.
        public virtual ActionResult ExportPromotionData()
        {
            //Get the list of promotions            
            PromotionListViewModel promotionListViewModel = _promotionAgent.GetPromotionList(null, null, null, null, 1000000000, true);
            if (promotionListViewModel?.PromotionExportList.Count > 0)
            {
                DownloadHelper downloadHelper = new DownloadHelper();
                downloadHelper.ExportDownload(promotionListViewModel?.PromotionExportList, Convert.ToInt32(FileTypes.CSV).ToString(), Response, null, $"{"Promotion"}{AdminConstants.CSV}");
            }
            else
                SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.ExportNoRecordFound, NotificationType.error));

            return RedirectToAction<PromotionController>(x => x.List(null));
        }

        //This method is used to download Promotion coupon data in csv format.
        public virtual ActionResult ExportPromotionCouponData(int promotionId)
        {
            //Get the list of promotions            
            PromotionViewModel promotionViewModel = new PromotionViewModel();
            promotionViewModel = _promotionAgent.GetPromotionById(promotionId);
            _promotionAgent.BindDropdownValues(promotionViewModel);
            promotionViewModel.CouponList = new CouponListViewModel();
            promotionViewModel.CouponList = _promotionAgent.GetCouponList(promotionId, null, new FilterCollection(), null, null, 1000000, true);

            if (promotionViewModel?.CouponList?.CouponExportList.Count > 0)
            {
                DownloadHelper downloadHelper = new DownloadHelper();
                downloadHelper.ExportDownload(promotionViewModel?.CouponList?.CouponExportList, Convert.ToInt32(FileTypes.CSV).ToString(), Response, null, $"{"CouponFor_" + promotionViewModel.Name}{AdminConstants.CSV}");
            }
            else
                SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.ExportNoRecordFound, NotificationType.error));

            return RedirectToAction<PromotionController>(x => x.List(null));
        }

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);
            StoreListViewModel storeList = _promotionAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView(storeListAsidePanelPopup, storeList);
        }

        #region Catelog
        //Get List of un-associated published Catelogs. 
        public virtual ActionResult GetPublishedCatelogs([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, int promotionId, string assignedIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.PublishedCatelogList.ToString(), model);
            PortalCatalogListViewModel catelogList = _promotionAgent.GetPublishedCatelogList(storeId, promotionId, assignedIds, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            catelogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catelogList.PortalCatalogs, GridListType.PublishedCatelogList.ToString(), string.Empty, null, true);

            //Set the total record count
            catelogList.GridModel.TotalRecordCount = catelogList.TotalResults;
            return ActionView("_PublishedCatelogList", catelogList);
        }

        //Get list of associated catelog list by categoryIds.
        public virtual ActionResult GetAssociatedCatelog([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, string catelogIds, int promotionId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedCatelogList.ToString(), model);
            PortalCatalogListViewModel catelogList = _promotionAgent.GetAssociatedUnAssociatedCatelogList(storeId, catelogIds, promotionId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            catelogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catelogList.PortalCatalogs, GridListType.AssociatedCatelogList.ToString(), string.Empty, null, true, true, catelogList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            catelogList.GridModel.TotalRecordCount = catelogList.TotalResults;
            return ActionView("_AssociatedCatelogList", catelogList);
        }

        //Associate catelog to already created promotion. 
        [HttpGet]
        public virtual JsonResult AssociateCatalogToPromotion(int storeId, string associatedCatelogIds, int promotionId)
            => Json(_promotionAgent.AssociateCatalogToPromotion(storeId, associatedCatelogIds, promotionId), JsonRequestBehavior.AllowGet);

        //Removes a Catalog type association entry from promotion.
        public virtual JsonResult UnAssociateCatalogs(string publishCatalogId, int promotionId = 0)
        {
            if (!string.IsNullOrEmpty(publishCatalogId) && promotionId > 0)
            {
                bool status = _promotionAgent.UnAssociateCatalog(publishCatalogId, promotionId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Category
        //Get List of un-associated published Categories. 
        public virtual ActionResult GetPublishedCategories([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, int promotionId, string assignedIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.PublishedCategoryList.ToString(), model);
            CategoryListViewModel categoryList = _promotionAgent.GetPublishedCategoryList(storeId, promotionId, assignedIds, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, categoryList.Categories, GridListType.PublishedCategoryList.ToString(), string.Empty, null, true);

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return ActionView("_PublishedCategoryList", categoryList);
        }

        //Get list of associated category list by categoryIds.
        public virtual ActionResult GetAssociatedCategory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, string categoryIds, int promotionId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedCategoryList.ToString(), model);
            CategoryListViewModel categoryList = _promotionAgent.GetAssociatedUnAssociatedCategoryList(storeId, categoryIds, promotionId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, categoryList.Categories, GridListType.AssociatedCategoryList.ToString(), string.Empty, null, true, true, categoryList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return ActionView("_AssociatedCategoryList", categoryList);
        }

        //Associate category to already created promotion. 
        [HttpGet]
        public virtual JsonResult AssociateCategoryToPromotion(int storeId, string associatedCategoryIds, int promotionId)
            => Json(_promotionAgent.AssociateCategoryToPromotion(storeId, associatedCategoryIds, promotionId), JsonRequestBehavior.AllowGet);

        //Removes a Category type association entry from promotion.
        public virtual JsonResult UnAssociateCategories(string publishCategoryId, int promotionId = 0)
        {
            if (!string.IsNullOrEmpty(publishCategoryId) && promotionId > 0)
            {
                bool status = _promotionAgent.UnAssociateCategory(publishCategoryId, promotionId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Product
        //Get List of associated published products. 
        public virtual ActionResult GetPublishedProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, int promotionId, string assignedIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.PublishedProductList.ToString(), model);
            PublishProductsListViewModel productList = _promotionAgent.GetPublishedProductList(storeId, promotionId, assignedIds, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            productList.GridModel = FilterHelpers.GetDynamicGridModel(model, productList.PublishProductsList, GridListType.PublishedProductList.ToString(), string.Empty, null, true);

            //Set the total record count
            productList.GridModel.TotalRecordCount = productList.TotalResults;
            return ActionView("_PublishedProductList", productList);
        }

        //Get list of associated product list by productIds.
        public virtual ActionResult GetAssociatedProduct([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, string productIds, int promotionId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedProductsList.ToString(), model);
            PublishProductsListViewModel productList = _promotionAgent.GetAssociatedUnAssociatedProductList(storeId, productIds, promotionId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            productList.GridModel = FilterHelpers.GetDynamicGridModel(model, productList.PublishProductsList, GridListType.AssociatedProductsList.ToString(), string.Empty, null, true, true, productList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            productList.GridModel.TotalRecordCount = productList.TotalResults;
            return ActionView("_AssociatedProductList", productList);
        }

        //Associate product to already created promotion. 
        [HttpGet]
        public virtual JsonResult AssociateProductToPromotion(int storeId, string associatedProductIds, int promotionId, string discountTypeName)
            => Json(_promotionAgent.AssociateProductToPromotion(storeId, associatedProductIds, promotionId, discountTypeName), JsonRequestBehavior.AllowGet);

        //Removes a product type association entry from promotion.
        public virtual JsonResult UnAssociateProducts(string publishProductId, int promotionId = 0)
        {
            if (!string.IsNullOrEmpty(publishProductId) && promotionId > 0)
            {
                bool status = _promotionAgent.UnAssociateProduct(publishProductId, promotionId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Brand
        //Get List of associated Brands. 
        public virtual ActionResult GetPromotionBrands([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int promotionId, string assignedIds)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePromotionBrandDetails.ToString(), model);
            BrandListViewModel brandList = _promotionAgent.GetAssociatedUnAssociatedBrandList(assignedIds, promotionId, false, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            brandList.GridModel = FilterHelpers.GetDynamicGridModel(model, brandList.Brands, GridListType.ZnodePromotionBrandDetails.ToString(), string.Empty, null, true);

            //Set the total record count
            brandList.GridModel.TotalRecordCount = brandList.TotalResults;
            return ActionView("_PromotionBrandList", brandList);
        }

        //Get list of associated brand list by brandIds.
        public virtual ActionResult GetAssociatedBrands([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, string assignedIds, int promotionId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePromotionAssociatedBrandDetails.ToString(), model);
            BrandListViewModel brandList = _promotionAgent.GetAssociatedUnAssociatedBrandList(assignedIds, promotionId, true, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            brandList.GridModel = FilterHelpers.GetDynamicGridModel(model, brandList.Brands, GridListType.ZnodePromotionAssociatedBrandDetails.ToString(), string.Empty, null, true, true, brandList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            brandList.GridModel.TotalRecordCount = brandList.TotalResults;
            return ActionView("_AssociatedBrandList", brandList);
        }

        //Associate brand to already created promotion. 
        [HttpGet]
        public virtual JsonResult AssociateBrandToPromotion(string associatedBrandIds, int promotionId)
            => Json(_promotionAgent.AssociateBrandToPromotion(associatedBrandIds, promotionId), JsonRequestBehavior.AllowGet);

        //Removes a brand type association entry from promotion.
        public virtual JsonResult UnAssociateBrands(string BrandId, int promotionId = 0)
        {
            if (!string.IsNullOrEmpty(BrandId) && promotionId > 0)
            {
                bool status = _promotionAgent.UnAssociateBrand(BrandId, promotionId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Shipping
        //Get Associated shipping list for portal.
        public virtual ActionResult GetPromotionShippingList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, string assignedIds, int promotionId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePromotionShippingDetails.ToString(), model);
            //Get portal shipping list.
            ShippingListViewModel shippingListViewModel = _promotionAgent.GetAssociatedUnAssociatedShippingList(storeId, assignedIds, promotionId, false, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            shippingListViewModel.PortalId = storeId;
            //Get the grid model.
            shippingListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingListViewModel?.ShippingList, GridListType.ZnodePromotionShippingDetails.ToString(), string.Empty, null, true, true, shippingListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            shippingListViewModel.GridModel.TotalRecordCount = shippingListViewModel.TotalResults;

            return ActionView("_PromotionShippingList", shippingListViewModel);
        }

        //Get list of associated Shipping list by ShippingIds.
        public virtual ActionResult GetAssociatedShippings([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int storeId, string assignedIds, int promotionId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePromotionAssociatedShippingDetails.ToString(), model);
            ShippingListViewModel shippingList = _promotionAgent.GetAssociatedUnAssociatedShippingList(storeId, assignedIds, promotionId, true, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            shippingList.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingList.ShippingList, GridListType.ZnodePromotionAssociatedShippingDetails.ToString(), string.Empty, null, true, true, shippingList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            shippingList.GridModel.TotalRecordCount = shippingList.TotalResults;
            return ActionView("_AssociatedShippingList", shippingList);
        }

        //Associate Shipping to already created promotion. 
        [HttpGet]
        public virtual JsonResult AssociateShippingToPromotion(string associatedShippingIds, int promotionId)
            => Json(_promotionAgent.AssociateShippingToPromotion(associatedShippingIds, promotionId), JsonRequestBehavior.AllowGet);

        //Removes a Shipping type association entry from promotion.
        public virtual JsonResult UnAssociateShippings(string ShippingId, int promotionId = 0)
        {
            if (!string.IsNullOrEmpty(ShippingId) && promotionId > 0)
            {
                bool status = _promotionAgent.UnAssociateShipping(ShippingId, promotionId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }
        #endregion Shipping
        #endregion
    }
}