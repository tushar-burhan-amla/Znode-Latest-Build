using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class CategoryController : BaseController
    {
        #region Private Variables
        private readonly ICategoryService _service;
        private readonly ICategoryCache _cache;
        #endregion

        #region Default Constructor
        public CategoryController(ICategoryService service)
        {
            _service = service;
            _cache = new CategoryCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of categories.
        /// </summary>
        /// <returns>It will return list of categories.</returns>
        [ResponseType(typeof(CategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCategories(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets a category.
        /// </summary>
        /// <param name="categoryId">ID of category to be retrieved.</param>
        /// <param name="familyId">Family Id.</param>
        /// <param name="localeId">Active locale Id</param>
        /// <returns>Return category based on categoryId.</returns>
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int categoryId, int familyId, int localeId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCategory(categoryId, familyId, localeId, RouteUri, RouteTemplate);
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
        /// Method to create category.
        /// </summary>
        /// <param name="model">Category model.</param>
        /// <returns>Returns created category.</returns>
        [ResponseType(typeof(CategoryResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]CategoryValuesListModel model)
        {
            HttpResponseMessage response;
            try
            {
                var category = _service.CreateCategory(model);
                if (category?.PimCategoryId > 0)
                {
                    response = CreateCreatedResponse(new CategoryResponse { CategoryValues = category });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(category.PimCategoryId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to update category.
        /// </summary>
        /// <param name="model">Category model to be updated.</param>
        /// <returns>Returns updated category model.</returns>
        [ResponseType(typeof(CategoryResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]CategoryValuesListModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool category = _service.UpdateCategory(model);
                response = category ? CreateCreatedResponse(new CategoryResponse { CategoryValues = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.PimCategoryId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Method to delete category.
        /// </summary>
        /// <param name="categoryIds">IDs of category to be deleted.</param>
        /// <returns>Return status as per delete operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel categoryIds)
        {
            HttpResponseMessage response;
            try
            {
                var deleted = _service.DeleteCategory(categoryIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Delete Category Product
        /// </summary>
        /// <param name="PimCategoryProductId">PimCategoryProductId</param>
        /// <returns>Return true if deleted successfully else fail.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage DeleteCategoryProduct(ParameterModel PimCategoryProductId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteCategoryAssociatedProducts(PimCategoryProductId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Associated Categories To Product
        /// </summary>
        /// <param name="pimCategoryId"></param>
        /// <returns>ParameterModel</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage DeleteAssociatedCategoriesToProduct(ParameterModel pimCategoryId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteAssociatedCategoriesToProduct(pimCategoryId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate Category Product
        /// </summary>
        /// <param name="listModel">listModel</param>
        /// <returns>Return list of product depend on associate category.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AssociateCategoryProduct([FromBody] List<CategoryProductModel> listModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateCategoryProduct(listModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Associate Categories to  Product
        /// </summary>
        /// <param name="listModel">listModel</param>
        /// <returns>Return list of product depend on associate category.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AssociateCategoriesToProduct([FromBody] List<CategoryProductModel> listModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateCategoriesToProduct(listModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get associated or unassociated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProducts">associatedProducts</param>
        /// <returns></returns>
        [ResponseType(typeof(CategoryProductListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedUnAssociatedCategoryProducts(categoryId, associatedProducts, RouteUri, RouteTemplate);
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
        /// Get associated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProducts">Associated product status</param>
        /// <returns>Return associated products</returns>
        [ResponseType(typeof(CategoryProductListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAssociatedCategoryProducts(int categoryId, bool associatedProducts)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedCategoryProducts(categoryId, associatedProducts, RouteUri, RouteTemplate);
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
        /// Get the Categories associated to Product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="associatedProducts"></param>
        /// <returns></returns>
        [ResponseType(typeof(CategoryProductListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAssociatedCategoriesToProducts(int productId, bool associatedProducts)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedCategoriesToProduct(productId, associatedProducts, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CategoryProductListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        #region Publish Category
        /// <summary>
        /// Method to publish category
        /// </summary>
        /// <param name="categoryIds">ID of category to be published.</param>
        /// <returns>Returns status as per published operation.</returns>
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Publish(ParameterModel categoryIds)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel published = _service.Publish(categoryIds);
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

        /// <summary>
        /// Update product details associated to category.
        /// </summary>
        /// <param name="categoryProductModel">CategoryProductModel with product details</param>
        /// <returns>Return True or False.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCategoryProductDetail(CategoryProductModel categoryProductModel)
        {
            HttpResponseMessage response;
            try
            {
                bool updated = _service.UpdateCategoryProductDetail(categoryProductModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = updated });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

    }
}
