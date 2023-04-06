using MvcSiteMapProvider;
using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Admin.Areas.PIM.Controllers
{
    public class BrandController : BaseController
    {
        #region Private Variables
        private readonly IBrandAgent _brandAgent;
        private readonly IProductAgent _productAgent;
        private readonly ILocaleAgent _localeAgent;
        #endregion

        #region Constructor

        public BrandController(IBrandAgent brandAgent, IProductAgent productAgent, ILocaleAgent localeAgent)
        {
            _brandAgent = brandAgent;
            _productAgent = productAgent;
            _localeAgent = localeAgent;
        }

        #endregion

        #region Brand Settings

        //Get brand list.
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleBrand", Key = "Brand", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeBrandDetails.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeBrandDetails.ToString(), model);

            BrandListViewModel brandList = _brandAgent.GetBrandList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            brandList.GridModel = FilterHelpers.GetDynamicGridModel(model, brandList?.Brands, GridListType.ZnodeBrandDetails.ToString(), string.Empty, null, true, true, brandList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            brandList.GridModel.TotalRecordCount = brandList.TotalResults;
            return ActionView(brandList);
        }

        [HttpGet]
        public virtual ActionResult Create()
        => ActionView(AdminConstants.CreateEdit, _brandAgent.GetBrandViewModel());


        //POST: Create brand details.
        [HttpPost]
        public virtual ActionResult Create(BrandViewModel brandViewModel)
        {
            if (ModelState.IsValid)
            {
                brandViewModel = _brandAgent.CreateBrand(brandViewModel);
                if (HelperUtility.IsNotNull(brandViewModel))
                {
                    if (brandViewModel.HasError)
                    {
                        SetNotificationMessage(GetErrorNotificationMessage(brandViewModel.ErrorMessage));
                        return ActionView(AdminConstants.CreateEdit, brandViewModel);
                    }
                    SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.CreateMessage));
                    return RedirectToAction<BrandController>(x => x.Edit(brandViewModel.BrandId, Convert.ToInt32(DefaultSettingHelper.DefaultLocale)));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorFailedToCreate));
            return ActionView(AdminConstants.CreateEdit, new BrandViewModel());
        }

        //Get:for Edit brand.
        public virtual ActionResult Edit(int BrandId, int localeId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEdit, _brandAgent.GetBrand(BrandId, localeId));
        }
        //POST: Edit brand details.
        [HttpPost]
        public virtual ActionResult Edit(BrandViewModel brandViewModel)
        {
            if (ModelState.IsValid)
            {
                brandViewModel = _brandAgent.UpdateBrand(brandViewModel);
                if (!brandViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.UpdateMessage));
                    return RedirectToAction<BrandController>(x => x.Edit(brandViewModel.BrandId, brandViewModel.LocaleId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.UpdateErrorMessage));
            return ActionView(AdminConstants.CreateEdit, brandViewModel);
        }

        //Delete brand. 
        public virtual JsonResult Delete(string brandId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(brandId))
            {
                status = _brandAgent.DeleteBrand(brandId, out message);

                if (status)
                    message = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Check brand name already exists or not.
        [HttpPost]
        public virtual JsonResult IsBrandNameExist(string BrandCode, int brandId = 0)
        => Json(!_brandAgent.CheckBrandNameExist(BrandCode, brandId), JsonRequestBehavior.AllowGet);

        //Action for brand product list.
        public virtual ActionResult AssociatedProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int brandId = 0, int localeId = 0, string brandCode = null, string brandName = null)
        => ActionView("_AssociatedProducts", _brandAgent.AssociatedProductList(model, brandId, brandCode, localeId, Server.UrlDecode(brandName)));

        //Action for get product list. 
        public virtual ActionResult UnAssociatedProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int localeId = 0, string brandCode = null)
            => ActionView("ProductPopupList", _brandAgent.UnAssociatedProductList(model, brandCode, localeId));

        // Action for associate brand product
        public virtual JsonResult AssociateBrandProducts(string brandCode, string productIds)
        {
            string errorMessage = PIM_Resources.ErrorAssociateBrandProducts;
            bool status = false;
            if (IsNotNull(productIds) && IsNotNull(brandCode))
            {
                status = _brandAgent.AssociateBrandProduct(brandCode, productIds);
                errorMessage = status ? PIM_Resources.SuccessAssociateBrandProducts : PIM_Resources.ErrorAssociateBrandProducts;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete associated products.
        public virtual ActionResult UnAssociateBrandProducts(string pimProductId, string attributeValue)
        {
            string errorMessage = PIM_Resources.ErrorUnAssociateBrandProducts;
            bool status = false;
            if (IsNotNull(pimProductId))
            {
                status = _brandAgent.UnAssociateBrandProduct(attributeValue, pimProductId);
                errorMessage = status ? PIM_Resources.ProductUnassociatedSuccessfully : PIM_Resources.Unabletounassociateproduct;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Active/Inactive Brands
        public virtual ActionResult ActiveInactiveBrand(string brandIds, bool isActive)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(brandIds))
            {
                status = _brandAgent.ActiveInactiveBrand(brandIds, isActive);
                message = status ? (status && isActive) ? Admin_Resources.SuccessMessageStatusActive : Admin_Resources.SuccessMessageStatusInactive : Admin_Resources.ErrorMessageFailedStatus;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Check brand SEO friendly page name exist or not. 
        public virtual JsonResult IsBrandSEOFriendlyPageNameExist(string seoFriendlyPageName, int seoDetailsId = 0)
       => Json(!_brandAgent.CheckBrandSEOFriendlyPageNameExist(seoFriendlyPageName, seoDetailsId), JsonRequestBehavior.AllowGet);

        public virtual ActionResult AssociatedStoreList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int brandId = 0, int localeId = 0, string brandCode = null, string brandName = null)
        => ActionView("_AssociatedStores", _brandAgent.AssociatedStoreList(model, brandId, brandCode, localeId, Server.UrlDecode(brandName)));


        //Action for get product list. 
        public virtual ActionResult UnAssociatedStoreList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int localeId = 0, string brandCode = null)
            => ActionView("StorePopupList", _brandAgent.UnAssociatedStoreList(model, brandCode, localeId));

        // Action for associate brand portal
        public virtual JsonResult AssociateBrandPortals(int brandId, string portalIds)
        {
            string errorMessage = PIM_Resources.ErrorAssociateBrandProducts;
            bool status = false;
            if (IsNotNull(brandId) && IsNotNull(portalIds))
            {
                status = _brandAgent.AssociateBrandPortal(brandId, portalIds);
                errorMessage = status ? Admin_Resources.SuccessAssociateBrandStore : PIM_Resources.ErrorAssociateBrandProducts;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete associated portals.
        public virtual JsonResult UnAssociateBrandPortals(string portalId, int brandId)
        {
            string errorMessage = PIM_Resources.ErrorUnAssociateBrandProducts;
            bool status = false;
            if (!string.IsNullOrEmpty(portalId))
            {
                status = _brandAgent.UnAssociateBrandPortal(brandId, portalId);
                errorMessage = status ? PIM_Resources.ProductUnassociatedSuccessfully : PIM_Resources.Unabletounassociateproduct;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete associated portals.
        public virtual JsonResult CheckUniqueBrandCode(string code)
            => Json(new { result = _brandAgent.CheckUniqueBrandCode(code) }, JsonRequestBehavior.AllowGet);

        //Get Brand Name by brand code.
        public virtual JsonResult GetBrandName(string code,int localeId)
           => Json(new { result = _brandAgent.GetBrandName(code, localeId) }, JsonRequestBehavior.AllowGet);
        #endregion
    }
}