using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PromotionCache : BaseCache, IPromotionCache
    {
        #region Global Variable
        private readonly IPromotionService _service;
        #endregion

        #region Default Constructor
        public PromotionCache(IPromotionService promotionService)
        {
            _service = promotionService;
        }
        #endregion

        #region Public Methods

        #region Promotion

        //Get promotion by promotionId.
        public virtual string GetPromotion(int promotionId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get promotion by promotionId.
                PromotionModel promotion = _service.GetPromotion(promotionId, Expands);
                if (IsNotNull(promotion))
                {
                    PromotionResponse response = new PromotionResponse { Promotion = promotion };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get promotion list.
        public virtual string GetPromotionList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //shipping list
                PromotionListModel promotionList = _service.GetPromotionList(Expands, Filters, Sorts, Page);
                if (promotionList?.PromotionList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    PromotionListResponse response = new PromotionListResponse { PromotionList = promotionList.PromotionList };
                    response.MapPagingDataFromModel(promotionList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        #region Coupon

        public virtual string GetCoupon(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CouponModel coupon = _service.GetCoupon(Filters);
                if (IsNotNull(coupon))
                {
                    CouponResponse response = new CouponResponse { Coupon = coupon };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Coupon List
        public virtual string GetCouponList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //coupon list
                CouponListModel couponList = _service.GetCouponList(Expands, Filters, Sorts, Page);
                if (couponList?.CouponList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    CouponListResponse response = new CouponListResponse { CouponList = couponList.CouponList };
                    response.MapPagingDataFromModel(couponList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Product
        //Get list of Associated or UnAssociated product on the basis of isAssociatedProduct.
        public virtual string GetAssociatedUnAssociatedProductList(int portalId, string productIds, int promotionId, bool isAssociatedProduct, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get associated or unassociated product list on the basis of isAssociatedProduct.
                PublishProductListModel list = _service.GetAssociatedUnAssociatedProductList(portalId, productIds, promotionId, isAssociatedProduct, Expands, Filters, Sorts, Page);
                if (list?.PublishProducts?.Count > 0)
                {
                    //Create response.
                    PublishProductListResponse response = new PublishProductListResponse { PublishProducts = list.PublishProducts };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        #region Category
        //Get list of Associated or UnAssociated Publish Category on the basis of isAssociatedCategory.
        public virtual string GetAssociatedUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId, bool isAssociatedCategory, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get associated or unassociated Publish Category list on the basis of isAssociatedCategory..
                PublishCategoryListModel list = _service.GetAssociatedUnAssociatedCategoryList(portalId, categoryIds, promotionId, isAssociatedCategory, Expands, Filters, Sorts, Page);

                if (list?.PublishCategories?.Count > 0)
                {
                    //Create response.
                    PublishCategoryListResponse response = new PublishCategoryListResponse { PublishCategories = list.PublishCategories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Catalog
        //Get list of Associated or UnAssociated Catalog on the basis of isAssociatedCatalog.
        public virtual string GetAssociatedUnAssociatedCatalogList(int portalId, string catalogIds, int promotionId, bool isAssociatedCatalog, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get associated or unassociated CatalogList on the basis of isAssociatedCatalog.
                PublishCatalogListModel list = _service.GetAssociatedUnAssociatedCatalogList(portalId, catalogIds, promotionId, isAssociatedCatalog, Expands, Filters, Sorts, Page);

                if (list?.PublishCatalogs?.Count > 0)
                {
                    //Create response.
                    PublishCatalogListResponse response = new PublishCatalogListResponse { PublishCatalogs = list.PublishCatalogs };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Brand
        //Get list of Associated or UnAssociated Brand on the basis of isAssociatedBrand.
        public virtual string GetAssociatedUnAssociatedBrandList(string brandIds, int promotionId, bool isAssociatedBrand, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get associated or unassociated Brand list on the basis of isAssociatedBrand.
                BrandListModel list = _service.GetAssociatedUnAssociatedBrandList(brandIds, promotionId, isAssociatedBrand, Expands, Filters, Sorts, Page);
                if (list?.Brands?.Count > 0)
                {
                    //Create response.
                    BrandListResponse response = new BrandListResponse { Brands = list.Brands };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        #region Shipping
        //Get list of Associated or UnAssociated Shipping on the basis of isAssociatedShipping.
        public virtual string GetAssociatedUnAssociatedShippingList(int portalId, string ShippingIds, int promotionId, bool isAssociatedShipping, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get associated or unassociated Shipping list on the basis of isAssociatedShipping.
                ShippingListModel list = _service.GetAssociatedUnAssociatedShippingList(portalId, ShippingIds, promotionId, isAssociatedShipping, Expands, Filters, Sorts, Page);
                if (list?.ShippingList?.Count > 0)
                {
                    //Create response.
                    ShippingListResponse response = new ShippingListResponse { ShippingList = list.ShippingList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
        #endregion
    }
}