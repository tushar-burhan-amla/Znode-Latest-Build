using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class PromotionClient : BaseClient, IPromotionClient
    {
        #region Promotion

        //Get promotion by promotion Id
        public virtual PromotionModel GetPromotion(int promotionId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetPromotionById(promotionId);
            endpoint += BuildEndpointQueryString(expands);

            //Get response.
            ApiStatus status = new ApiStatus();
            PromotionResponse response = GetResourceFromEndpoint<PromotionResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Promotion;
        }

        //Get promotion list 
        public virtual PromotionListModel GetPromotionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetPromotionList(expands, filters, sorts, null, null);

        //Get promotion list 
        public virtual PromotionListModel GetPromotionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetPromotionList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            PromotionListResponse response = GetResourceFromEndpoint<PromotionListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Promotion list.
            PromotionListModel promotionList = new PromotionListModel { PromotionList = response?.PromotionList };
            promotionList.MapPagingDataFromResponse(response);

            return promotionList;
        }


        //Create promotion 
        public virtual PromotionModel CreatePromotion(PromotionModel model)
        {
            //Get Endpoint
            string endpoint = PromotionEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PromotionResponse response = PostResourceToEndpoint<PromotionResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Promotion;
        }

        //Update promotion 
        public virtual PromotionModel UpdatePromotion(PromotionModel model)
        {
            //Get Endpoint
            string endpoint = PromotionEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PromotionResponse response = PostResourceToEndpoint<PromotionResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            //Check and set status
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Promotion;
        }

        //Delete promotion by promotionId
        public virtual bool DeletePromotion(ParameterModel promotionId)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(promotionId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get Published Categories
        public virtual CategoryListModel GetPublishedCategories(ParameterModel filterIds)
        {
            string endpoint = PromotionEndpoint.GetPublishedCategories();
            endpoint += BuildEndpointQueryString(null);

            ApiStatus status = new ApiStatus();
            CategoryListResponse response = PostResourceToEndpoint<CategoryListResponse>(endpoint, JsonConvert.SerializeObject(filterIds), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryListModel list = new CategoryListModel { Categories = response?.Categories };

            return list;
        }

        //Get Published Products
        public virtual ProductDetailsListModel GetPublishedProducts(ParameterModel filterIds)
        {
            string endpoint = PromotionEndpoint.GetPublishedProducts();
            endpoint += BuildEndpointQueryString(null);

            ApiStatus status = new ApiStatus();
            ProductDetailsListResponse response = PostResourceToEndpoint<ProductDetailsListResponse>(endpoint, JsonConvert.SerializeObject(filterIds), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductDetailsListModel list = new ProductDetailsListModel { ProductDetailList = response?.ProductDetailList };

            return list;
        }
        
        #endregion

        #region Coupon

        //Get coupon on the basis of filter collection
        public virtual CouponModel GetCoupon(FilterCollection filters)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetCoupon();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            //Get response.
            ApiStatus status = new ApiStatus();
            CouponResponse response = GetResourceFromEndpoint<CouponResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Coupon;
        }


        //Get Coupon List
        public virtual CouponListModel GetCouponList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetCouponList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            CouponListResponse response = GetResourceFromEndpoint<CouponListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Promotion list.
            CouponListModel couponList = new CouponListModel { CouponList = response?.CouponList };
            couponList.MapPagingDataFromResponse(response);

            return couponList;
        }

        //Get promotion attribute on changing discount type
        public virtual PIMFamilyDetailsModel GetPromotionAttribute(string discountName)
        {
            string endpoint = PromotionEndpoint.GetPromotionAttribute(discountName);

            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyResponse response = GetResourceFromEndpoint<PIMAttributeFamilyResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PIMFamilyDetails;
        }

        //Associate catalog to already created promotion. 
        public virtual bool AssociateCatalogToPromotion(int portalId, string associateCatelogIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.AssociateCatalogToPromotion();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new AssociatedParameterModel() { PortalId = portalId, AssociateIds = associateCatelogIds, PromotionId = promotionId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Associate category to already created promotion. 
        public virtual bool AssociateCategoryToPromotion(int portalId, string associateCategoryIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.AssociateCategoryToPromotion();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new AssociatedParameterModel() { PortalId = portalId, AssociateIds = associateCategoryIds, PromotionId = promotionId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Associate product to already created promotion. 
        public virtual bool AssociateProductToPromotion(int portalId, string associateProductIds, int promotionId, string discountTypeName)
        {
            string endpoint = PromotionEndpoint.AssociateProductToPromotion();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new AssociatedParameterModel() { PortalId = portalId, AssociateIds = associateProductIds, PromotionId = promotionId, DiscountTypeName = discountTypeName }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        // Gets the list of associated or UnAssociated Products on the basis of isAssociatedProduct.
        public virtual PublishProductListModel GetAssociatedUnAssociatedProductList(PromotionModel promotionModel, bool isAssociatedProduct, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetAssociatedUnAssociatedProductList(isAssociatedProduct);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishProductListResponse response = PostResourceToEndpoint<PublishProductListResponse>(endpoint, JsonConvert.SerializeObject(promotionModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishProductListModel list = new PublishProductListModel { PublishProducts = response?.PublishProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get associated or UnAssociated category List on basis of isAssociatedCategory
        public virtual PublishCategoryListModel GetAssociatedUnAssociatedCategoryList(PromotionModel promotionModel, bool isAssociatedCategory, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetAssociatedUnAssociatedCategoryList(isAssociatedCategory);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCategoryListResponse response = PostResourceToEndpoint<PublishCategoryListResponse>(endpoint, JsonConvert.SerializeObject(promotionModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCategoryListModel list = new PublishCategoryListModel { PublishCategories = response?.PublishCategories };
            list.MapPagingDataFromResponse(response);

            return list;
        }


        // Gets the list of associated or UnAssociated Catalogs  on the basis of isAssociatedCatalog.
        public virtual PublishCatalogListModel GetAssociatedUnAssociatedCatalogList(PromotionModel promotionModel, bool isAssociatedCatalog, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetAssociatedUnAssociatedCatalogList(isAssociatedCatalog);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCatalogListResponse response = PostResourceToEndpoint<PublishCatalogListResponse>(endpoint, JsonConvert.SerializeObject(promotionModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCatalogListModel list = new PublishCatalogListModel { PublishCatalogs = response?.PublishCatalogs };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Removes a product type association entry from promotion.
        public virtual bool UnAssociateProduct(ParameterModel publishProductIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.UnAssociateProduct(promotionId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(publishProductIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Removes a Category type association entry from promotion.
        public virtual bool UnAssociateCategory(ParameterModel publishCategoryIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.UnAssociateCategory(promotionId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(publishCategoryIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Removes a Catalog type association entry from promotion.
        public virtual bool UnAssociateCatalog(ParameterModel publishCatalogIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.UnAssociateCatalog(promotionId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(publishCatalogIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Associate brand to already created promotion. 
        public virtual bool AssociateBrandToPromotion(string associateBrandIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.AssociateBrandToPromotion();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new AssociatedParameterModel() {AssociateIds = associateBrandIds, PromotionId = promotionId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        // Gets the list of associated or UnAssociated Brands on the basis of isAssociatedBrand.
        public virtual BrandListModel GetAssociatedUnAssociatedBrandList(PromotionModel promotionModel, bool isAssociatedBrand, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetAssociatedUnAssociatedBrandList(isAssociatedBrand);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            BrandListResponse response = PostResourceToEndpoint<BrandListResponse>(endpoint, JsonConvert.SerializeObject(promotionModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BrandListModel list = new BrandListModel { Brands = response?.Brands };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Removes a Brand type association entry from promotion.
        public virtual bool UnAssociateBrand(ParameterModel brandIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.UnAssociateBrand(promotionId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(brandIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Associate Shipping to already created promotion. 
        public virtual bool AssociateShippingToPromotion(string associateShippingIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.AssociateShippingToPromotion();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new AssociatedParameterModel() { AssociateIds = associateShippingIds, PromotionId = promotionId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        // Gets the list of associated or UnAssociated Shippings on the basis of isAssociatedShipping.
        public virtual ShippingListModel GetAssociatedUnAssociatedShippingList(PromotionModel promotionModel, bool isAssociatedShipping, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PromotionEndpoint.GetAssociatedUnAssociatedShippingList(isAssociatedShipping);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ShippingListResponse response = PostResourceToEndpoint<ShippingListResponse>(endpoint, JsonConvert.SerializeObject(promotionModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ShippingListModel list = new ShippingListModel { ShippingList = response?.ShippingList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Removes a Shipping type association entry from promotion.
        public virtual bool UnAssociateShipping(ParameterModel shippingIds, int promotionId)
        {
            string endpoint = PromotionEndpoint.UnAssociateShipping(promotionId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(shippingIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion
    }
}
