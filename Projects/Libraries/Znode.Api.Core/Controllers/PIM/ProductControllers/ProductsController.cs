using System;
using System.Diagnostics;
using System.Linq;
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

namespace Znode.Engine.Api.Controllers
{
    public class ProductsController : BaseController
    {
        #region Private Variables
        private readonly IProductService _service;
        private readonly IProductCache _cache;
        private readonly IPublishProductService _publishProductService;
        #endregion

        #region public default constructor
        public ProductsController(IProductService service, IPublishProductService publishProductService)
        {
            _service = service;
            _publishProductService = publishProductService;
            _cache = new ProductCache(_service);
        }
        #endregion

        #region Product
        /// <summary>
        /// To get Product by Product id.
        /// </summary>
        /// <param name="model">PIMGetProductModel</param>
        /// <returns>Return product based on product Id. </returns>   
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetProduct(PIMGetProductModel model)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetProduct(model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeFamilyResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PIMAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get List of Product.
        /// </summary>
        /// <returns>Return list of product.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProductList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProducts(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get List of Product.
        /// </summary>
        /// <returns>Return list of product.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBrandProductList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetBrandProducts(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get List of Product.
        /// </summary>
        /// <param name="productIds">Product IDs.</param>
        /// <returns>Product List.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProductList(string productIds)
        {
            HttpResponseMessage response;
            try
            {
                ParameterModel parameter = new ParameterModel { Ids = productIds };
                string data = _cache.GetProducts(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get List of Product.
        /// </summary>
        /// <param name="productIds">Product IDs.</param>
        /// <returns>Product List.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetProductsToBeAssociated(ParameterModel productIds)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProducts(productIds, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        /// <summary>
        /// Gets list of products to be associated with parent Product.
        /// </summary>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="associatedProductIds">Product IDs to be associated.</param>
        /// <param name="associatedAttributeIds">associatedAttributeIds to be associated.</param>
        /// <param name="pimProductIdsIn">pimProductIdsIn to be associated or not.</param>
        /// <returns></returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedUnAssociatedConfigureProducts(parentProductId, associatedProductIds, associatedAttributeIds, pimProductIdsIn, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create/Update product.
        /// </summary>
        /// <param name="model"> Product model</param>
        /// <returns>Return product model with newly created product Id.</returns>
        [ResponseType(typeof(ProductResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateProduct([FromBody] ProductModel model)
        {
            HttpResponseMessage response;

            try
            {
                var product = _service.CreateProduct(model);
                if (!Equals(product, null))
                {
                    response = CreateCreatedResponse(new ProductResponse { Product = product });

                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(product.ProductId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ProductResponse data = new ProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound, ErrorDetailList = ex.ErrorDetailList };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                ProductResponse data = new ProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound};
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Delete an Exiting Product.
        /// </summary>
        /// <param name="productIds">productids</param>
        /// <returns>Return status based on delete operation.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteProduct(ParameterModel productIds)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteProduct(productIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get associated or unassociated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProduts">Set true for associatedProduts and false for unAssociatedProduts</param>
        /// <returns></returns>
        [ResponseType(typeof(CategoryProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedUnAssociatedCategoryProducts(categoryId, associatedProduts, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CategoryProductListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }


        /// <summary>
        ///  Get Product Attributes and Family as per selected Family.
        /// </summary>
        /// <param name="pimFamilyModel">PIMFamily Model</param>
        /// <returns>Return Product Attributes and Family as per selected Family.</returns>
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetProductFamilyDetails([FromBody] PIMFamilyModel pimFamilyModel)
        {
            HttpResponseMessage response;

            try
            {
                PIMFamilyDetailsModel pimFamilyDetailsModel = _service.GetProductFamilyDetails(pimFamilyModel);
                response = (!Equals(pimFamilyDetailsModel, null) && pimFamilyDetailsModel?.Attributes?.Count > 0 && pimFamilyDetailsModel?.Groups?.Count > 0)
                        ? CreateOKResponse(new PIMAttributeFamilyResponse { PIMFamilyDetails = pimFamilyDetailsModel }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeFamilyResponse data = new PIMAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get all configure attributes associated with familyID.
        /// </summary>
        /// <param name="pimFamilyModel"></param>
        /// <returns>configure Product List</returns>
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetConfigureAttributes([FromBody] PIMFamilyModel pimFamilyModel)
        {
            HttpResponseMessage response;
            try
            {
                PIMFamilyDetailsModel pimFamilyDetailsModel = _service.GetConfigureAttributes(pimFamilyModel);
                response = (pimFamilyDetailsModel?.Attributes?.Count > 0)
                        ? CreateOKResponse(new PIMAttributeFamilyResponse { PIMFamilyDetails = pimFamilyDetailsModel }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PIMAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Activate/Deactivate products in bulk
        /// </summary>
        /// <param name="model">ActivateDeactivateProductsModel</param>
        /// <returns>True/False</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage ActivateDeactivateProducts(ActivateDeactivateProductsModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.ActivateDeactivateProducts(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Update Product Attribute Code Value.
        /// </summary>
        /// <param name="model">Update attribute code value.</param>
        /// <returns>Returns response with status.</returns>
        [ResponseType(typeof(ProductAttributeCodeValueListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateProductAttributeValue(AttributeCodeValueModel model)
        {
            HttpResponseMessage response;
            try
            {
                ProductAttributeCodeValueListModel attributeListModel = _service.UpdateProductAttributeValue(model);
               
                response = CreateOKResponse(new ProductAttributeCodeValueListResponse { AttributeCodeValueList = attributeListModel.AttributeCodeValueList, Status = attributeListModel.Status });
            }
            catch(ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ProductAttributeCodeValueListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex.ToString(), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductAttributeCodeValueListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ProcessingFailed });
                ZnodeLogging.LogMessage(ex.ToString(), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion

        #region Personalize Attribute
        /// <summary>
        /// Get Assigned Personalized Attribute List.
        /// </summary>
        /// <returns>Return list of assigned personalized attributes</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssignedPersonalizedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssignedPersonalizedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Unassigned Personalized Attribute List.
        /// </summary>
        /// <returns>Return list of unassigned personalized attributes</returns>
        [ResponseType(typeof(PIMAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassignedPersonalizedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnassignedPersonalizedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeListResponse data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Assign personalized attributes.
        /// </summary>
        /// <param name="model">PIM attribute value list model.</param>
        /// <returns>Returns the inserted records.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssignPersonalizedAttributes([FromBody] PIMAttributeValueListModel model)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.AssignedPersonalizedAttribute(model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Unassign personalized attributes.
        /// </summary>
        /// <param name="model">Parameter model for attributes to be unassigned.</param>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <returns>True/false response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnassignPersonalizedAttributes([FromBody] ParameterModel model, int parentProductId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.UnassignPersonalizedAttributes(model, parentProductId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #endregion

        #region Product Type

        /// <summary>
        /// Gets list of products associated with parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="attributeId">Attribute Id.</param>
        /// <returns>List of ProductDetailsModel.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedProducts(int parentProductId, int attributeId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedProducts(parentProductId, attributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets list of products those are not associated with parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="associatedProductIds">Already associated product Ids.</param>
        /// <returns>List of ProductDetailsModel.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetUnassociatedProducts(int parentProductId, ParameterModel associatedProductIds)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnassociatedProducts(parentProductId, associatedProductIds.Ids, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Adds product type association entries.
        /// </summary>
        /// <param name="model">ProductTypeAssociationList model to be added.</param>
        /// <returns>Http response containing boolean value whether products are associated or not.</returns>
        [ResponseType(typeof(ProductTypeAssociationListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateProduct([FromBody] ProductTypeAssociationListModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool isProductAssociated = _service.AssociateProduct(model);
                response = isProductAssociated ? CreateCreatedResponse(new ProductTypeAssociationListResponse { AssociatedProductList = model.AssociatedProducts }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ProductTypeAssociationListResponse data = new ProductTypeAssociationListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ProductTypeAssociationListResponse data = new ProductTypeAssociationListResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Removes a product type association entry.
        /// </summary>
        /// <param name="productTypeAssociationId">ID of the product type association to be deleted.</param>
        /// <returns>Boolean value True/False in response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnassociateProduct(ParameterModel productTypeAssociationId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UnassociateProduct(productTypeAssociationId);
                response = deleted ? CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
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

        #region Associate Addons
        /// <summary>
        /// Associates an add-on group to a product.
        /// </summary>
        /// <param name="model">Addon product list model.</param>
        /// <returns>True if addon group is associated, False if addon group fails to associate.</returns>
        [ResponseType(typeof(AddonProductListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAddon([FromBody] AddonProductListModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool isAddonAssociated = _service.AssociateAddon(model);
                if (isAddonAssociated)
                {
                    response = CreateCreatedResponse(new AddonProductListResponse { AddonProducts = model.AddonProducts });
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                AddonProductListResponse data = new AddonProductListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                AddonProductListResponse data = new AddonProductListResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Deletes association of product and add-on group.
        /// </summary>
        /// <param name="addonProductIds">Addon product IDs to be deleted.</param>
        /// <returns>True if addon product is deleted;False if addon product fails to delete.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAssociatedAddons([FromBody]ParameterModel addonProductIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteAssociatedAddons(addonProductIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of associated addon group with associated child products as add-ons.
        /// </summary>
        /// <param name="parentProductId">Parent Product ID.</param>
        /// <returns>List of associated addon group along with child products associated as addons.</returns>
        [ResponseType(typeof(AddonGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedAddonDetails(int parentProductId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedAddonDetails(parentProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddonGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                AddonGroupListResponse data = new AddonGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Creates association of associated addon group and child product as add-on to parent product.
        /// </summary>
        /// <param name="addonProductDetails">Addon Product detail ID.</param>
        /// <returns>True if addon product details is created;False if addon product details fails to create.</returns>
        [ResponseType(typeof(AddOnProductDetailListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateAddonProductDetail([FromBody]AddOnProductDetailListModel addonProductDetails)
        {
            HttpResponseMessage response;

            try
            {
                bool isAddonProductAssociated = _service.CreateAddonProductDetail(addonProductDetails);
                if (isAddonProductAssociated)
                {
                    response = CreateCreatedResponse(new AddOnProductDetailListResponse { AddOnProductDetails = addonProductDetails.AddOnProductDetailList });
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                AddOnProductDetailListResponse data = new AddOnProductDetailListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                AddOnProductDetailListResponse data = new AddOnProductDetailListResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Deletes association of associated addon group and child product as add-on to parent product.
        /// </summary>
        /// <param name="addonProductDetailIds">Addon Product detail IDs to be deleted.</param>
        /// <returns>True if addon product details is deleted;False if addon product details fails to delete.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAddonProductDetails([FromBody]ParameterModel addonProductDetailIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteAddonProductDetails(addonProductDetailIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassociated addon groups.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <returns>List of unassociated add-on groups.</returns>
        [ResponseType(typeof(AddonGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassociatedAddonGroups(int parentProductId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnassociatedAddonGroups(parentProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddonGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                AddonGroupListResponse data = new AddonGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassociated child products as add-ons.
        /// </summary>
        /// <param name="addonProductId">Addon Product ID according to which child product list will be retrieved.</param>
        /// <returns>Unassociated child products as add-ons</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassociatedAddonProducts(int addonProductId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnassociatedAddonProducts(addonProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update addon product association.
        /// </summary>
        /// <param name="addonProductModel">Add-on Product association model.</param>
        /// <returns>Updated Add-on product model.</returns>
        [ResponseType(typeof(AddonProductResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateProductAddonAssociation(AddOnProductModel addonProductModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isProductAddonAssociationUpdated = _service.UpdateProductAddonAssociation(addonProductModel);
                response = isProductAddonAssociationUpdated ? CreateCreatedResponse(new AddonProductResponse { AddonProduct = addonProductModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                AddonProductResponse data = new AddonProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                AddonProductResponse data = new AddonProductResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Update Addon Display Order.
        /// </summary>
        /// <param name="addOnProductDetailModel">Addon product model containing display order and other values.</param>
        /// <returns>Updated Add-on product model.</returns>
        [ResponseType(typeof(AddonDisplayOrderResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAddonDisplayOrder(AddOnProductDetailModel addOnProductDetailModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isProductAddonAssociationUpdated = _service.UpdateAddonDisplayOrder(addOnProductDetailModel);
                response = isProductAddonAssociationUpdated ? CreateCreatedResponse(new AddonDisplayOrderResponse { AddonProductDisplayOrderResponse = addOnProductDetailModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                AddonDisplayOrderResponse data = new AddonDisplayOrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                AddonDisplayOrderResponse data = new AddonDisplayOrderResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }
        #endregion

        #region Link Product

        /// <summary>
        /// Gets list of products which are associated as Link products to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link attribute Id.</param>
        /// <returns>List of Product model.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedLinkProducts(int parentProductId, int linkAttributeId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedLinkProducts(parentProductId, linkAttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets list of list of products which are not associated as link products to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link Attribute ID.</param>
        /// <returns>List of Product model.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassociatedLinkProducts(int parentProductId, int linkAttributeId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnassociatedLinkProducts(parentProductId, linkAttributeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Adds a product link association entry.
        /// </summary>
        /// <param name="model">Link Product details model to be added.</param>
        /// <returns>Added Link Product model in response .</returns>
        [ResponseType(typeof(LinkProductDetailListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssignLinkProducts([FromBody] LinkProductDetailListModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool isLinkProductAssigned = _service.AssignLinkProduct(model);
                if (isLinkProductAssigned)
                {
                    response = CreateCreatedResponse(new LinkProductDetailListResponse { LinkProducts = model.LinkProductDetailList });
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                LinkProductDetailListResponse data = new LinkProductDetailListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                LinkProductDetailListResponse data = new LinkProductDetailListResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Removes a product Link association entry.
        /// </summary>
        /// <param name="linkProductDetailId">ID of the link product details to be deleted.</param>
        /// <returns>Boolean value True/False in response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnassignLinkProducts(ParameterModel linkProductDetailId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.UnassignLinkProduct(linkProductDetailId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
       

        [ResponseType(typeof(LinkProductDetailResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAssignLinkProducts([FromBody] LinkProductDetailModel linkProductDetailModel)
        {

            HttpResponseMessage response=null;
            try
            {
                //Update Product.
                response = _service.UpdateAssignLinkProducts(linkProductDetailModel) ? CreateOKResponse(new LinkProductDetailResponse { LinkProductDetail = linkProductDetailModel, ErrorCode = 0, HasError = false }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(linkProductDetailModel.PimLinkProductDetailId)));
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        #region Custom Field
        /// <summary>
        /// Method to create Custom Field.
        /// </summary>
        /// <param name="customFieldModel">CustomFieldModel model.</param>
        /// <returns>Returns created profile.</returns>
        [ResponseType(typeof(CustomFieldResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage AddCustomField([FromBody] CustomFieldModel customFieldModel)
        {
            HttpResponseMessage response;
            try
            {
                //Create custom field.
                CustomFieldModel customField = _service.AddCustomField(customFieldModel);
                if (!Equals(customField, null))
                {
                    response = CreateCreatedResponse(new CustomFieldResponse { CustomField = customField });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(customField.CustomFieldId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                CustomFieldResponse data = new CustomFieldResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                CustomFieldResponse data = new CustomFieldResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AlreadyExist };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// To get Custom Field by Custom Field id.
        /// </summary>
        /// <param name="customFieldId">customField Id</param>
        /// <returns>Custom field model</returns>  
        [ResponseType(typeof(CustomFieldResponse))]
        [HttpGet]
        public HttpResponseMessage GetCustomField(int customFieldId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCustomField(customFieldId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CustomFieldResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                CustomFieldResponse data = new CustomFieldResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get List of CustomField.
        /// </summary>
        /// <returns> It will return list of customfield. </returns>
        [ResponseType(typeof(CustomFieldListResponse))]
        [HttpGet]
        public HttpResponseMessage GetCustomFieldList()

        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCustomFields(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CustomFieldListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                CustomFieldListResponse data = new CustomFieldListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update an Exiting CustomField.
        /// </summary>
        /// <param name="customFieldModel">CustomFieldModel customFieldModel.</param>
        /// <returns>Updated CustomField model.</returns>
        [ResponseType(typeof(CustomFieldResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateCustomField([FromBody] CustomFieldModel customFieldModel)
        {
            HttpResponseMessage response;

            try
            {
                bool customField = _service.UpdateCustomField(customFieldModel);
                response = customField ? CreateCreatedResponse(new CustomFieldResponse { CustomField = customFieldModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                CustomFieldResponse data = new CustomFieldResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                CustomFieldResponse data = new CustomFieldResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Delete an Exiting CustomField.
        /// </summary>
        /// <param name="customFieldId">customFieldId to delete custom field</param>
        /// <returns>Retrun status as per delete operation.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage DeleteCustomField(ParameterModel customFieldId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteCustomField(customFieldId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion

        /// <summary>
        /// Get associated product by pimProductTypeAssociationId.
        /// </summary>
        /// <param name="pimProductTypeAssociationId">Pim Product Type Association Id</param>
        /// <returns>Return associated product by pimProductTypeAssociationId. </returns>
        [ResponseType(typeof(ProductTypeAssociationResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedProduct(int pimProductTypeAssociationId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedProduct(pimProductTypeAssociationId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductTypeAssociationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductTypeAssociationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update associated product.
        /// </summary>
        /// <param name="productTypeAssociationModel">ProductTypeAssociationModel</param>
        /// <returns>Return as per update operation.</returns>
        [ResponseType(typeof(ProductTypeAssociationResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateAssociatedProduct([FromBody] ProductTypeAssociationModel productTypeAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedProduct(productTypeAssociationModel);
                response = data ? CreateCreatedResponse(new ProductTypeAssociationResponse { ProductTypeAssociation = productTypeAssociationModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ProductTypeAssociationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductTypeAssociationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #region Publish Product.
        /// <summary>
        /// Method to publish product.
        /// </summary>
        /// <param name="productIds">ID of product to be published.</param>
        /// <returns>Returns status as per published operation.</returns>
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Publish(ParameterModel productIds)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel published = _service.Publish(productIds);
                response = !Equals(published, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = published }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Product SKU list for Autocomplete feature
        /// <summary>
        /// Get product sku list by attribute code as SKU and attribute value. 
        /// </summary>
        /// <param name="attributeValue">Attribute value to get in list.</param>
        /// <returns>Returns product sku list.</returns>
        [ResponseType(typeof(PIMProductAttributeValuesListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProductSKUsByAttributeCode(string attributeValue)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetProductSKUsByAttributeCode(attributeValue, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMProductAttributeValuesListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PIMProductAttributeValuesListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }
        #endregion

        #region Product Update Import

        /// <summary>
        /// This method will fetch and process the product update data from uploaded file.
        /// </summary>
        /// <param name="model">Improt Model</param>
        /// <returns>Returns true if successfull else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage ImportProductUpdateData(ImportModel model)
        {
            HttpResponseMessage response;
            response = CreateNoContentResponse();
            try
            {
                int processStarted = _service.ProcessProductUpdateData(model);
                response = processStarted > 0 ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = "Import process failed.", ErrorCode = ErrorCodes.ProcessingFailed });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ImportError });
            }
            return response;
        }

        /// <summary>
        /// Get active products for recent viewed products API
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="catalogId"></param>
        /// <param name="localeId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetActiveProducts(string parentIds, int catalogId, int localeId, int versionId)
        {
            HttpResponseMessage response;
            try
            {
                var recentViewProductModel = _publishProductService.GetActiveProducts(parentIds.Split(',').Select(int.Parse).ToList(), catalogId, localeId, versionId);
                response = CreateOKResponse(new RecentViewProductResponse
                {
                    RecentViewProductModelCollection = recentViewProductModel
                });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ImportError });
            }

            return response;
        }
        #endregion        
    }
}