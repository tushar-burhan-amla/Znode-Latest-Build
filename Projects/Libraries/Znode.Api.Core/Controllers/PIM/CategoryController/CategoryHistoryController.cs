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
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Controllers
{
    public class CategoryHistoryController : BaseController
    {
        #region Private Variables
        private readonly ICategoryHistoryCache _cache;
        private readonly ICategoryHistoryService _service;
        #endregion

        #region Constructor
        public CategoryHistoryController()
        {
            _cache = new CategoryHistoryCache();
            _service = ZnodeDependencyResolver.GetService<ICategoryHistoryService>();
        }
        #endregion

        #region Controller Actions
        /// <summary>
        /// Gets the list of category history.
        /// </summary>
        /// <returns>List of category history.</returns>
        [ResponseType(typeof(CategoryHistoryListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCategoryHistoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryHistoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                CategoryHistoryListResponse data = new CategoryHistoryListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets the category history of the specified ID.
        /// </summary>
        /// <param name="id">ID of the category history.</param>
        /// <returns>Category history for the specified ID.</returns>
        [ResponseType(typeof(CategoryHistoryResponse))]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
                //Get category history by ID.
                string data = _cache.GetCategoryHistory(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryHistoryResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                CategoryHistoryResponse data = new CategoryHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Creates a category history.
        /// </summary>
        /// <param name="model">New category history model.</param>
        /// <returns>Newly created category history model.</returns>  
        [ResponseType(typeof(CategoryHistoryResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] CategoryHistoryModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create category history.
                CategoryHistoryModel categoryHistory = _service.CreateCategoryHistory(model);
                if (!Equals(categoryHistory, null))
                {
                    response = CreateCreatedResponse(new CategoryHistoryResponse { CategoryHistory = categoryHistory });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(categoryHistory.ID)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                CategoryHistoryResponse data = new CategoryHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Updates a category history.
        /// </summary>
        /// <param name="model">Category history model with new values.</param>
        /// <returns>Updated category history model.</returns>
        [ResponseType(typeof(CategoryHistoryResponse))]
        [HttpPut]
        public HttpResponseMessage Update([FromBody] CategoryHistoryModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update category history.
                bool categoryHistory = _service.UpdateCategoryHistory(model);
                response = categoryHistory ? CreateCreatedResponse(new CategoryHistoryResponse { CategoryHistory = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.ID)));

            }
            catch (Exception ex)
            {
                CategoryHistoryResponse data = new CategoryHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Deletes a category history.
        /// </summary>
        /// <param name="id">ID of the of the category history to be deleted.</param>
        /// <returns>True if category history is deleted;False if category history is not deleted.</returns>
        [ResponseType(typeof(CategoryHistoryResponse))]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response;
            try
            {
                //Delete category history.
                bool deleted = _service.DeleteCategoryHistory(id);
                response = deleted ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                CategoryHistoryResponse data = new CategoryHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}