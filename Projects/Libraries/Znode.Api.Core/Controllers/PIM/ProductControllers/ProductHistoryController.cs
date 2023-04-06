using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    //Product History Controller.
    public class ProductHistoryController : BaseController
    {
        #region Private Variables
        private readonly IProductHistoryService _service;
        private readonly IProductHistoryCache _cache;
        #endregion

        #region Public Default Constructor
        public ProductHistoryController()
        {
            _service = new ProductHistoryService();
            _cache = new ProductHistoryCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get list of Product history.
        /// </summary>
        /// <returns>Returns list of Product history.</returns>
        [ResponseType(typeof(ProductHistoryListResponse))]
        [HttpGet]
        public HttpResponseMessage GetProductHistoryList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProductHistoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductHistoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductHistoryListResponse data = new ProductHistoryListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets product history of the specified ID.
        /// </summary>
        /// <param name="id">ID of the product history.</param>
        /// <returns>Product history for the specified ID.</returns>
        [ResponseType(typeof(ProductHistoryResponse))]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
                //Get product history by ID.
                string data = _cache.GetProductHistory(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductHistoryResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ProductHistoryResponse data = new ProductHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Creates a product history.
        /// </summary>
        /// <param name="model">New product history model.</param>
        /// <returns>Newly created product history model.</returns>  
        [ResponseType(typeof(ProductHistoryResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] ProductHistoryModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create product history.
                ProductHistoryModel productHistory = _service.CreateProductHistory(model);
                if (!Equals(productHistory, null))
                {
                    response = CreateCreatedResponse(new ProductHistoryResponse { ProductHistory = productHistory });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(productHistory.ProductHistoryId)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                ProductHistoryResponse data = new ProductHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Updates a product history.
        /// </summary>
        /// <param name="model">Product history model with new values.</param>
        /// <returns>Updated product history model.</returns>
        [ResponseType(typeof(ProductHistoryResponse))]
        [HttpPut]
        public HttpResponseMessage Update([FromBody] ProductHistoryModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update product history.
                bool productHistory = _service.UpdateProductHistory(model);
                response = productHistory ? CreateCreatedResponse(new ProductHistoryResponse { ProductHistory = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.ProductHistoryId)));

            }
            catch (Exception ex)
            {
                ProductHistoryResponse data = new ProductHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Deletes a product history.
        /// </summary>
        /// <param name="id">ID of the of the product history to be deleted.</param>
        /// <returns>True if product history is deleted;False if product history is not deleted.</returns>
        [ResponseType(typeof(ProductHistoryResponse))]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response;
            try
            {
                //Delete product history.
                bool deleted = _service.DeleteProductHistory(id);
                response = deleted ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                ProductHistoryResponse data = new ProductHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}