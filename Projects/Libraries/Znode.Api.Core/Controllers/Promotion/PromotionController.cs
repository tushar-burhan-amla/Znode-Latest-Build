using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class PromotionController : BaseController
    {
        #region Private Variables
        private readonly IPromotionService _service;
        private readonly IPromotionCache _cache;
        #endregion

        #region Default Constructor
        public PromotionController(IPromotionService service)
        {
            _service = service;
            _cache = new PromotionCache(_service);
        }
        #endregion

        #region Public Methods

        #region Promotion

        /// <summary>
        /// Gets a promotion.
        /// </summary>
        /// <param name="promotionId">The Id of the promotion.</param>
        /// <returns>Returns promotion.</returns>
        [ResponseType(typeof(PromotionResponse))]
        [HttpGet]
        public HttpResponseMessage Get(int promotionId)
        {
            HttpResponseMessage response;
            try
            {
                //Get promotion by promotionId.
                string data = _cache.GetPromotion(promotionId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PromotionResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                PromotionResponse data = new PromotionResponse { ErrorCode = ex.ErrorCode, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                PromotionResponse data = new PromotionResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets list of promotion.
        /// </summary>
        /// <returns>Returns promotion list.</returns>
        [ResponseType(typeof(PromotionListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get Promotions.
                string data = _cache.GetPromotionList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PromotionListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                PromotionListResponse data = new PromotionListResponse { ErrorCode = ex.ErrorCode, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                PromotionListResponse data = new PromotionListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Method to create promotion.
        /// </summary>
        /// <param name="model">Promotion model.</param>
        /// <returns>Returns created promotion.</returns>
        [ResponseType(typeof(PromotionResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Create([FromBody] PromotionModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create promotion.
                PromotionModel promotion = _service.CreatePromotion(model);
                if (!Equals(promotion, null))
                {
                    response = CreateCreatedResponse(new PromotionResponse { Promotion = promotion });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(promotion.PromotionId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                PromotionResponse data = new PromotionResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                PromotionResponse data = new PromotionResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Method to update promotion.
        /// </summary>
        /// <param name="model">Promotion model.</param>
        /// <returns>Returns updated promotion.</returns>
        [ResponseType(typeof(PromotionResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Update([FromBody] PromotionModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update promotion.
                bool IsUpdated = _service.UpdatePromotion(model);
                if (IsUpdated)
                {
                    response = CreateCreatedResponse(new PromotionResponse { Promotion = model });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.PromotionId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                PromotionResponse data = new PromotionResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                PromotionResponse data = new PromotionResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Delete promotion by promotionId
        /// </summary>
        /// <param name="promotionId">Id of promotion</param>
        /// <returns>Returns true/false as per delete operation.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Delete([FromBody] ParameterModel promotionId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete promotion.
                bool deleted = _service.DeletePromotion(promotionId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of published categories.
        /// </summary>
        /// <returns>List of published categories.</returns>   

        [ResponseType(typeof(CategoryListResponse))]
        [HttpPost]
        public HttpResponseMessage GetPublishedCategoryList([FromBody] ParameterModel filterIds)
        {
            HttpResponseMessage response;

            try
            {
                //Get data from service
                CategoryListModel publishedCategoryList = _service.GetPublishedCategoryList(filterIds);
                if (publishedCategoryList?.Categories?.Count > 0)
                {
                    CategoryListResponse data = new CategoryListResponse { Categories = publishedCategoryList.Categories };
                    response = IsNotNull(data) ? CreateOKResponse(data) : CreateNoContentResponse();
                }
                else
                    response = CreateNoContentResponse();
            }

            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///  Associate catelog to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage AssociateCatalogToPromotion(AssociatedParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool associated = _service.AssociateCatalogToPromotion(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = associated });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        ///  Associate category to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage AssociateCategoryToPromotion(AssociatedParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool associated = _service.AssociateCategoryToPromotion(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = associated });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Coupon

        /// <summary>
        /// Gets a coupon.
        /// </summary>
        /// <returns>Returns coupon.</returns>
        [ResponseType(typeof(CouponResponse))]
        [HttpGet]
        public HttpResponseMessage GetCoupon()
        {
            HttpResponseMessage response;
            try
            {
                //Get coupon.
                string data = _cache.GetCoupon(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CouponResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                CouponResponse data = new CouponResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets List of Coupons.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(CouponListResponse))]
        [HttpGet]
        public HttpResponseMessage GetCouponList()
        {
            HttpResponseMessage response;
            try
            {
                //Get Promotions.
                string data = _cache.GetCouponList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CouponListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                CouponListResponse data = new CouponListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get promotion attribute on changing discount type
        /// </summary>
        /// <param name="discountName">Discount Type Name</param>
        /// <returns></returns>
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpGet]
        public HttpResponseMessage GetPromotionAttribute(string discountName)
        {
            HttpResponseMessage response;

            try
            {
                PIMFamilyDetailsModel pimFamilyDetailsModel = _service.GetPromotionAttribute(discountName);
                response = (!Equals(pimFamilyDetailsModel, null) && pimFamilyDetailsModel?.Attributes?.Count > 0)
                        ? CreateOKResponse(new PIMAttributeFamilyResponse { PIMFamilyDetails = pimFamilyDetailsModel }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                PIMAttributeFamilyResponse data = new PIMAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        #endregion

        #region Product

        /// <summary>
        /// Gets list of published products.
        /// </summary>
        /// <returns>List of published products.</returns> 
        [ResponseType(typeof(ProductDetailsListResponse))]
        [HttpPost]
        public HttpResponseMessage GetPublishedProductList([FromBody] ParameterModel filterIds)
        {
            HttpResponseMessage response;

            try
            {
                //Get data from service
                ProductDetailsListModel publishedProductList = _service.GetPublishedProductList(filterIds);
                if (publishedProductList?.ProductDetailList?.Count > 0)
                {
                    ProductDetailsListResponse data = new ProductDetailsListResponse { ProductDetailList = publishedProductList.ProductDetailList };
                    response = IsNotNull(data) ? CreateOKResponse(data) : CreateNoContentResponse();
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ProductDetailsListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductDetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        ///  Associate product to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage AssociateProductToPromotion(AssociatedParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool associated = _service.AssociateProductToPromotion(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = associated });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of associated or unassociated products on the basis of isAssociatedProduct .
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpPost]
        public HttpResponseMessage GetAssociatedUnAssociatedProductList(PromotionModel model, bool isAssociatedProduct)
        {
            HttpResponseMessage response;

            try
            {
                int portalId = !string.IsNullOrEmpty(model.PortalId.ToString()) ? Convert.ToInt32(model.PortalId) : 0;
                string data = _cache.GetAssociatedUnAssociatedProductList(portalId, model.AssociatedProductIds, model.PromotionId, isAssociatedProduct, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Removes a product type association entry from promotion.
        /// </summary>
        /// <param name="publishProductIds">IDs of product type association to be deleted.</param>
        /// <param name="promotionId">>promotionId for delete record of product Associate to promotion.</param>
        /// <returns>True if product type association is removed;False if removal of product type association failed.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage UnAssociateProduct(ParameterModel publishProductIds, int promotionId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UnAssociateProduct(publishProductIds, promotionId);
                response = deleted ? CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion

        #region Category
        /// <summary>
        /// Gets associated or unassociated list of Category on the basis of isAssociatedCategory.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PublishCategoryListResponse))]
        [HttpPost]
        public HttpResponseMessage GetAssociatedUnAssociatedCategoryList(PromotionModel model, bool isAssociatedCategory)
        {
            HttpResponseMessage response;

            try
            {
                int portalId = !string.IsNullOrEmpty(model.PortalId.ToString()) ? Convert.ToInt32(model.PortalId) : 0;
                string data = _cache.GetAssociatedUnAssociatedCategoryList(portalId, model.AssociatedCategoryIds, model.PromotionId, isAssociatedCategory, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Removes a Category type association entry from promotion.
        /// </summary>
        /// <param name="publishCategoryIds">IDs of Category type association to be deleted.</param>
        /// <param name="promotionId">>promotionId for delete record of Category Associate to promotion.</param>
        /// <returns>True if Category type association is removed;False if removal of Category type association failed.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage UnAssociateCategory(ParameterModel publishCategoryIds, int promotionId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UnAssociateCategory(publishCategoryIds, promotionId);
                response = deleted ? CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Removes a Catalog type association entry from promotion.
        /// </summary>
        /// <param name="publishCatalogIds">IDs of Catalog type association to be deleted.</param>
        /// <param name="promotionId">>promotionId for delete record of Catalog Associate to promotion.</param>
        /// <returns>True if Catalog type association is removed;False if removal of Catalog type association failed.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage UnAssociateCatalog(ParameterModel publishCatalogIds, int promotionId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UnAssociateCatalog(publishCatalogIds, promotionId);
                response = deleted ? CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion

        #region Catalog
        /// <summary>
        /// Gets list of associated or unassociated Catalogs  on the basis of isAssociatedCatalog.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PublishCatalogListResponse))]
        [HttpPost]
        public HttpResponseMessage GetAssociatedUnAssociatedCatalogList(PromotionModel model, bool isAssociatedCatalog)
        {
            HttpResponseMessage response;

            try
            {
                int portalId = !string.IsNullOrEmpty(model.PortalId.ToString()) ? Convert.ToInt32(model.PortalId) : 0; 
                string data = _cache.GetAssociatedUnAssociatedCatalogList(portalId, model.AssociatedCatelogIds, model.PromotionId, isAssociatedCatalog, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCatalogListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion

        #region Brand

        /// <summary>
        ///  Associate Brand to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage AssociateBrandToPromotion(AssociatedParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool associated = _service.AssociateBrandToPromotion(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = associated });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of associated or unassociated Brand on the basis of isAssociatedBrand .
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(BrandListResponse))]
        [HttpPost]
        public HttpResponseMessage GetAssociatedUnAssociatedBrandList(PromotionModel model, bool isAssociatedBrand)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedUnAssociatedBrandList(model.AssociatedBrandIds, model.PromotionId, isAssociatedBrand, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BrandListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BrandListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Removes a Brand type association entry from promotion.
        /// </summary>
        /// <param name="brandIds">IDs of Brand type association to be deleted.</param>
        /// <param name="promotionId">>promotionId for delete record of Brand Associate to promotion.</param>
        /// <returns>True if Brand type association is removed;False if removal of Brand type association failed.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage UnAssociateBrand(ParameterModel brandIds, int promotionId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UnAssociateBrand(brandIds, promotionId);
                response = deleted ? CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion

        #region Shipping

        /// <summary>
        ///  Associate Shipping to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage AssociateShippingToPromotion(AssociatedParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool associated = _service.AssociateShippingToPromotion(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = associated });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of associated or unassociated Shipping on the basis of isAssociatedShipping .
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ShippingListResponse))]
        [HttpPost]
        public HttpResponseMessage GetAssociatedUnAssociatedShippingList(PromotionModel model, bool isAssociatedShipping)
        {
            HttpResponseMessage response;

            try
            {
                int portalId = !string.IsNullOrEmpty(model.PortalId.ToString()) ? Convert.ToInt32(model.PortalId) : 0;
                string data = _cache.GetAssociatedUnAssociatedShippingList(portalId, model.AssociatedShippingIds, model.PromotionId, isAssociatedShipping, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShippingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Removes a Shipping type association entry from promotion.
        /// </summary>
        /// <param name="ShippingIds">IDs of Shipping type association to be deleted.</param>
        /// <param name="promotionId">>promotionId for delete record of Shipping Associate to promotion.</param>
        /// <returns>True if Shipping type association is removed;False if removal of Shipping type association failed.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage UnAssociateShipping(ParameterModel ShippingIds, int promotionId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UnAssociateShipping(ShippingIds, promotionId);
                response = deleted ? CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion
        #endregion
    }
}
