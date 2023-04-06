using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using System.Collections.Generic;
using System.Web;
using System;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Controllers
{
    public class SEOController : BaseController
    {
        #region Private variables
        private readonly ISEOSettingAgent _seoSettingAgent;
        private readonly IUrlRedirectAgent _urlRedirectAgent;
        private readonly IStoreAgent _storeAgent;
        private const string createEditUrlRedirectView = "CreateEditUrlRedirect";
        private const string _productListView = "_productList";
        private const string _categoryListView = "_categoryList";
        private const string _contentPageListView = "_contentPageListView";
        private const string PublishedProductListView = "_PublishedProductList";
        private const string PublishedCategoryListView = "_PublishedCategoryList";
        private const string ContentPageList = "_ContentPageList";
        private const string urlRedirectListView = "_urlRedirectList";

        #endregion

        #region Constructor
        public SEOController(ISEOSettingAgent seoSettingAgent, IUrlRedirectAgent urlRedirectAgent, IStoreAgent storeAgent)
        {
            _seoSettingAgent = seoSettingAgent;
            _urlRedirectAgent = urlRedirectAgent;
            _storeAgent = storeAgent;
        }
        #endregion

        #region Default SEO Settings
        [HttpGet]
        public virtual ActionResult SEOSetting(int portalId = 0)
        {
            PortalSEOSettingViewModel model = _seoSettingAgent.GetPortalSEOSetting(portalId);
            return (Request.IsAjaxRequest()) ? PartialView("_PortalSEOSetting", model) : ActionView("SeoSettings", model);
        }

        [HttpPost]
        public virtual ActionResult SaveSEOSetting(PortalSEOSettingViewModel model)
        {
            PortalSEOSettingViewModel portalSetting = new PortalSEOSettingViewModel();
            if (ModelState.IsValid)
            {
                if (model?.CMSPortalSEOSettingId > 0)
                {
                    portalSetting = _seoSettingAgent.UpdatePortalSEOSetting(model);
                    SetNotificationMessage(model.HasError ? GetErrorNotificationMessage(Admin_Resources.ErrorUpdatePortalSEO) : GetSuccessNotificationMessage(Admin_Resources.SuccessPortalSEOUpdated));
                }
                else
                {
                    portalSetting = _seoSettingAgent.CreatePortalSEOSetting(model);
                    SetNotificationMessage(portalSetting.CMSPortalSEOSettingId > 0 ? GetSuccessNotificationMessage(Admin_Resources.SuccessPortalSEOUpdated) : GetErrorNotificationMessage(Admin_Resources.ErrorPortalSEOSettingCreated));
                }
            }
            return RedirectToAction<SEOController>(x => x.SEOSetting(model.PortalId));
        }

        public virtual ActionResult GetPublishedProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int PortalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SEOProductsDetails.ToString(), model);
            ProductDetailsListViewModel productList = _seoSettingAgent.GetPublishedProducts(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, PortalId);
            productList.PortalList = _seoSettingAgent.GetPortalList();
            productList.SEOTypeId = (int)SEOType.Product;
            //Get the grid model
            productList.GridModel = FilterHelpers.GetDynamicGridModel(model, productList.ProductDetailList, GridListType.SEOProductsDetails.ToString(), string.Empty, null, true);

            //Set the total record count
            productList.GridModel.TotalRecordCount = productList.TotalResults;

            return (Request.IsAjaxRequest()) ? PartialView(_productListView, productList) : ActionView(PublishedProductListView, productList);
        }

        public virtual ActionResult GetProductsForSEO([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int PortalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SEOProductsDetails.ToString(), model);
            ProductDetailsListViewModel productList = _seoSettingAgent.GetProductsForSEO(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, PortalId);
            productList.PortalList = _seoSettingAgent.GetPortalList();
            productList.SEOTypeId = (int)SEOType.Product;
            //Get the grid model
            productList.GridModel = FilterHelpers.GetDynamicGridModel(model, productList.ProductDetailList, GridListType.SEOProductsDetails.ToString(), string.Empty, null, true);

            //Set the total record count
            productList.GridModel.TotalRecordCount = productList.TotalResults;

            return (Request.IsAjaxRequest()) ? PartialView(_productListView, productList) : ActionView(PublishedProductListView, productList);
        }

        [Obsolete]
        public virtual ActionResult GetPublishedCategories([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int PortalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SEOCategoryDetails.ToString(), model);

            CategoryListViewModel categoryList = _seoSettingAgent.GetPublishedCategories(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, PortalId);
            categoryList.PortalList = _seoSettingAgent.GetPortalList();
            categoryList.SEOTypeId = (int)SEOType.Category;
            categoryList.Categories.ForEach(x => x.PortalId = categoryList.PortalId);

            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, categoryList.Categories, GridListType.SEOCategoryDetails.ToString(), string.Empty, null, true);

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return (Request.IsAjaxRequest()) ? PartialView(_categoryListView, categoryList) : ActionView(PublishedCategoryListView, categoryList);
        }

        public virtual ActionResult GetCategoriesForSEO([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int PortalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SEOCategoryDetails.ToString(), model);

            CategoryListViewModel categoryList = _seoSettingAgent.GetCategoriesForSEO(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, PortalId);
            categoryList.PortalList = _seoSettingAgent.GetPortalList();
            categoryList.SEOTypeId = (int)SEOType.Category;
            categoryList.Categories.ForEach(x => x.PortalId = categoryList.PortalId);

            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, categoryList.Categories, GridListType.SEOCategoryDetails.ToString(), string.Empty, null, true);

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return (Request.IsAjaxRequest()) ? PartialView(_categoryListView, categoryList) : ActionView(PublishedCategoryListView, categoryList);
        }

        public virtual ActionResult GetContentPages([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int PortalId = 0)
        {
            List<SelectListItem> PortalList = _seoSettingAgent.GetPortalList();
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SEOContentPages.ToString(), model);

            //Set Filter for PortalID.
            model.Filters = _seoSettingAgent.SetFilter(model, PortalId, PortalList);

            //Get content page list details.
            ContentPageListViewModel contentPageList = _seoSettingAgent.GetContentPageDetails(model, PortalId, PortalList);
            contentPageList.SEOTypeId = (int)SEOType.ContentPage;

            //Get the grid model.
            contentPageList.GridModel = FilterHelpers.GetDynamicGridModel(model, contentPageList.ContentPageList, GridListType.SEOContentPages.ToString(), string.Empty, null, true);

            //Set the total record count
            contentPageList.GridModel.TotalRecordCount = contentPageList.TotalResults;
            return (Request.IsAjaxRequest()) ? PartialView(_contentPageListView, contentPageList) : ActionView(ContentPageList, contentPageList);
        }

        [HttpGet]
        public virtual ActionResult SEODetails(int seoTypeId, string seoCode = "", int localeId = 0, int portalId = 0, int pimProductId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            SEODetailsViewModel model = _seoSettingAgent.GetSEODetails(seoCode, seoTypeId, localeId, portalId);
            model.SEOCode = seoCode;
            model.PimProductId = pimProductId;
            model.LocaleId = model.LocaleId == 0 ? (Convert.ToInt32(DefaultSettingHelper.DefaultLocale)) : model.LocaleId;
            return Request.IsAjaxRequest() ? PartialView("_SEODetails", model) : ActionView("SEODetails", model);
        }

        [HttpGet]
        public virtual ActionResult SEODetailsOfProduct(int seoTypeId, string seoCode, int localeId = 0, int portalId = 0, int ItemId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return SEODetails(seoTypeId, seoCode, localeId, portalId, ItemId); //To do replace a with seoCode when implemented
        }
        [HttpGet]
        public virtual ActionResult SEODetailsOfCategory(int seoTypeId, string seoCode, int localeId = 0, int portalId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return SEODetails(seoTypeId, seoCode, localeId, portalId);//To do replace a with seoCode when implemented
        }

        [HttpGet]
        public virtual ActionResult SEODetailsOfUrlRedirect(int cmsUrlRedirectId) => EditUrlRedirect(cmsUrlRedirectId);

        [HttpPost]
        public virtual ActionResult SEODetails(SEODetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model?.CMSSEODetailId > 0)
                {
                    model = _seoSettingAgent.UpdateSEODetails(model);
                    if (model.HasError)
                    {
                        SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                    }
                    else
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SuccessEditSEODetail));
                }
                else
                {
                    model = _seoSettingAgent.CreateSEODetails(model);
                    if (model.HasError)
                    {
                        SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                    }
                    else
                        SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SuccessEditSEODetail));
                }
            }
            switch (model.SEOTypeName)
            {
                case AdminConstants.Product:
                    return RedirectToAction<SEOController>(x => x.SEODetailsOfProduct(model.CMSSEOTypeId, model.SEOCode, model.LocaleId, model.PortalId, Convert.ToInt32(model.PimProductId)));
                case AdminConstants.Category:
                    return RedirectToAction<SEOController>(x => x.SEODetailsOfCategory(model.CMSSEOTypeId, model.SEOCode, model.LocaleId, model.PortalId));
                default:
                    return RedirectToAction<SEOController>(x => x.SEODetails(model.CMSSEOTypeId, model.SEOCode, model.LocaleId, model.PortalId, 0));
            }
        }

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);

            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView("_asideStoreListPopupPanel", storeList);
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        public virtual JsonResult Publish(string seoCode = "", int portalId = 0, int localeId = 0, int seoTypeId = 0)
        {

            if (!string.IsNullOrEmpty(seoCode))
            {
                string errorMessage;
                bool status = _seoSettingAgent.Publish(seoCode, portalId, localeId, seoTypeId, out errorMessage);
                return Json(new { status = status, message = status ? PIM_Resources.TextPublishedSuccessfully : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = Admin_Resources.ErrorDataRequired }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult PublishWithPreview(string seoCode, int portalId = 0, int localeId = 0, int seoTypeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            if (!string.IsNullOrEmpty(seoCode))
            {
                string errorMessage;
                bool status = _seoSettingAgent.Publish(seoCode, portalId, localeId, seoTypeId, out errorMessage, targetPublishState, takeFromDraftFirst);
                return Json(new { status = status, message = status ? PIM_Resources.TextPublishedSuccessfully : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = Admin_Resources.ErrorDataRequired }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult UpdateAndPublishSeo(SEODetailsViewModel model)
        {
            string errorMessage = string.Empty;
            string targetPublishState = model.TargetPublishState;
            bool takeFromDraftFirst = model.TakeFromDraftFirst;
            if (model?.CMSSEODetailId == 0)
                model = _seoSettingAgent.CreateSEODetails(model);
            else
            {
                if (HelperUtility.IsNotNull(model.SEOUrl))
                    model = _seoSettingAgent.UpdateSEODetails(model);
            }
            if (model.HasError)
            {
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                return GetSEOTypeName(model);
            }

            if (HelperUtility.IsNotNull(model.SEOCode) && HelperUtility.IsNotNull(model.SEOUrl))
            {
                bool status = _seoSettingAgent.Publish(model.SEOCode, model.PortalId, model.LocaleId, model.CMSSEOTypeId, out errorMessage, targetPublishState, takeFromDraftFirst);
                SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(errorMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorDataRequired));
            return GetSEOTypeName(model);
        }

        public virtual ActionResult GetSEOTypeName(SEODetailsViewModel model)
        {
            switch (model.SEOTypeName)
            {
                case AdminConstants.Product:
                    return RedirectToAction<SEOController>(x => x.GetProductsForSEO(null, model.PortalId));
                case AdminConstants.Category:
                    return RedirectToAction<SEOController>(x => x.GetCategoriesForSEO(null, model.PortalId));
                case AdminConstants.ContentPage:
                    return RedirectToAction<SEOController>(x => x.GetContentPages(null, model.PortalId));
                default:
                    return RedirectToAction<SEOController>(x => x.SEODetails(model.CMSSEOTypeId, model.SEOCode, model.LocaleId, model.PortalId, 0));
            }
        }

        public virtual ActionResult GetDefaultSEODetails(int seoTypeId, int itemId = 0, int localeId = 0, int portalId = 0, string seoCode = null)
        {
            SEODetailsViewModel model = _seoSettingAgent.GetDefaultSEODetails(seoTypeId, localeId, portalId, seoCode, itemId);
            return ActionView((seoTypeId == 1) ? AdminConstants.productSEO : AdminConstants.categorySEO, model);
        }

        #endregion

        #region Url Redirect
        //Method return a list view to display list of Static pages.
        public virtual ActionResult UrlRedirectList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSUrlRedirect.ToString(), model);
            UrlRedirectListViewModel urlRedirectList = _urlRedirectAgent.GetUrlRedirectList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId);

            //Get the grid model.
            urlRedirectList.GridModel = FilterHelpers.GetDynamicGridModel(model, urlRedirectList?.UrlRedirects, GridListType.ZnodeCMSUrlRedirect.ToString(), string.Empty, null, true, true, urlRedirectList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            urlRedirectList.GridModel.TotalRecordCount = urlRedirectList.TotalResults;
            return (Request.IsAjaxRequest()) ? PartialView(urlRedirectListView, urlRedirectList) : ActionView(urlRedirectList);
        }

        //Get:Create Url Redirect.
        [HttpGet]
        public virtual ActionResult CreateUrlRedirect(int portalId = 0)
            => ActionView(createEditUrlRedirectView, new UrlRedirectViewModel() { PortalId = portalId, StoreName = _storeAgent.GetStore(portalId)?.StoreName });

        [HttpPost]
        public virtual ActionResult CreateUrlRedirect(UrlRedirectViewModel urlRedirectViewModel)
        {
            if (ModelState.IsValid)
            {
                urlRedirectViewModel = _urlRedirectAgent.CreateUrlRedirect(urlRedirectViewModel);
                if (urlRedirectViewModel?.CMSUrlRedirectId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<SEOController>(x => x.EditUrlRedirect(urlRedirectViewModel.CMSUrlRedirectId, urlRedirectViewModel.PortalId));
                }
            }

            if (string.IsNullOrEmpty(urlRedirectViewModel?.ErrorMessage))
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
            else
                SetNotificationMessage(GetErrorNotificationMessage(urlRedirectViewModel.ErrorMessage));
            return ActionView(createEditUrlRedirectView, urlRedirectViewModel);
        }

        //Get: Edit Url Redirect
        [HttpGet]
        public virtual ActionResult EditUrlRedirect(int cmsUrlRedirectId, int portalId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return Redirect(HttpUtility.UrlDecode("/SEO/UrlRedirectList?portalId=" + portalId));

            return ActionView(createEditUrlRedirectView, _urlRedirectAgent.GetUrlRedirect(cmsUrlRedirectId));
        }

        [HttpPost]
        public virtual ActionResult EditUrlRedirect(UrlRedirectViewModel urlRedirectViewModel)
        {
            if (ModelState.IsValid)
            {
                urlRedirectViewModel = _urlRedirectAgent.UpdateUrlRedirect(urlRedirectViewModel);
                if (!urlRedirectViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<SEOController>(x => x.EditUrlRedirect(urlRedirectViewModel.CMSUrlRedirectId, urlRedirectViewModel.PortalId));
                }
            }

            if (string.IsNullOrEmpty(urlRedirectViewModel?.ErrorMessage))
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(urlRedirectViewModel.ErrorMessage));
            return ActionView(createEditUrlRedirectView, urlRedirectViewModel);
        }

        //Method to delete Url Redirect
        public virtual JsonResult DeleteUrlRedirect(string cmsUrlRedirectId)
        {
            if (!string.IsNullOrEmpty(cmsUrlRedirectId))
            {
                bool status = _urlRedirectAgent.DeleteUrlRedirect(cmsUrlRedirectId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Product SEO 
        // Get the Product SEO details for PIM edit.
        [Obsolete]
        public virtual ActionResult ProductSEODetails(int seoTypeId, int itemId = 0, int localeId = 0, int portalId = 0)
        {
            SEODetailsViewModel model = _seoSettingAgent.GetProductSEODetails(itemId, seoTypeId, localeId, portalId);
            return ActionView(AdminConstants.productSEO, model);
        }

        // Get the Product SEO details for PIM edit.
        public virtual ActionResult GetSEODetailsBySEOCode(int seoTypeId, int localeId = 0, int portalId = 0, string seoCode = "")
        {
            SEODetailsViewModel model = _seoSettingAgent.GetSEODetailsBySEOCode(seoTypeId, localeId, portalId, seoCode);
            return ActionView(AdminConstants.productSEO, model);
        }

        // Save update Product SEO details for PIM edit.
        [HttpPost]
        [Obsolete]
        public virtual JsonResult ProductSEODetails(SEODetailsViewModel model)
        {
            string _message = string.Empty;
            if (ModelState.IsValid)
            {
                model.IsAllStore = (model.PortalId == 0); //That mean all stores are selected.
                bool isCreate = (model.IsAllStore || model?.CMSSEODetailId == 0);
                model.CMSSEOTypeId = (int)SEOType.Product;
                model = isCreate ? _seoSettingAgent.CreateSEODetails(model) : _seoSettingAgent.UpdateSEODetails(model);
                _message = model.HasError ? model.ErrorMessage : (isCreate ? Admin_Resources.SuccessCreateSEODetail : Admin_Resources.SuccessEditSEODetail);
            }

            return Json(new { status = !model.HasError, message = _message, cmsseodetailId = model?.CMSSEODetailId }, JsonRequestBehavior.AllowGet);
        }

        // Save update Product SEO details for PIM edit.
        [HttpPost]
        public virtual JsonResult GetSEODetailsBySEOCode(SEODetailsViewModel model)
        {
            string _message = string.Empty;
            if (ModelState.IsValid)
            {
                model.IsAllStore = (model.PortalId == 0); //That mean all stores are selected.
                bool isCreate = (model.IsAllStore || model?.CMSSEODetailId == 0);
                model.CMSSEOTypeId = (int)SEOType.Product;
                model = isCreate ? _seoSettingAgent.CreateSEODetails(model) : _seoSettingAgent.UpdateSEODetails(model);
                _message = model.HasError ? model.ErrorMessage : (isCreate ? Admin_Resources.SuccessCreateSEODetail : Admin_Resources.SuccessEditSEODetail);
            }

            return Json(new { status = !model.HasError, message = _message, cmsseodetailId = model?.CMSSEODetailId }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Category SEO 
        // Get the Category SEO details.
        public virtual ActionResult CategorySEODetails(int seoTypeId, int itemId = 0, int localeId = 0, int portalId = 0, string seoCode = null)
        {
            SEODetailsViewModel model = _seoSettingAgent.GetSEODetailsBySEOCode(seoTypeId, localeId, portalId, seoCode);
            return ActionView(AdminConstants.categorySEO, model);
        }

        // Save update Product SEO details for PIM edit.
        [HttpPost]
        public virtual JsonResult CategorySEODetails(SEODetailsViewModel model)
        {
            string _message = string.Empty;
            if (ModelState.IsValid)
            {
                model.IsAllStore = (model.PortalId == 0); //That mean all stores are selected.
                bool isCreate = (model.IsAllStore || model?.CMSSEODetailId == 0);
                model.CMSSEOTypeId = (int)SEOType.Category;

                model = isCreate ? _seoSettingAgent.CreateSEODetails(model) : _seoSettingAgent.UpdateSEODetails(model);
                _message = model.HasError ? model.ErrorMessage : (isCreate ? Admin_Resources.SuccessCreateSEODetail : Admin_Resources.SuccessEditSEODetail);
            }

            return Json(new { status = !model.HasError, message = _message, cmsseodetailId = model?.CMSSEODetailId }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Seo
        /// <summary>
        /// Delete Seo Detail by using seoTypeId, portalId and seoCode.
        /// </summary>
        /// <param name="seoTypeId">seoType Id</param>
        /// <param name="portalId">portal Id</param>
        /// /// <param name="seoCode">seoCode</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>.
        public virtual ActionResult DeleteSeoDetail(int seoTypeId=0, int portalId=0, string seoCode = "")
        {
            string message = string.Empty;
            bool status = false;

              status = _seoSettingAgent.DeleteSeo(seoTypeId, portalId, seoCode);
              message = status ? Admin_Resources.DeleteSeoDetail : Admin_Resources.ErrorDeleteSeo;
            
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}