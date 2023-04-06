using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;


namespace Znode.Engine.Api.Controllers
{
    public class CatalogHistoryController : BaseController
    {
        #region Private Variables
        private readonly ICatalogHistoryCache _cache;
        private readonly ICatalogHistoryService _service;
        #endregion

        #region Constructor
        public CatalogHistoryController()
        {
            _cache = new CatalogHistoryCache();
            _service = GetService<ICatalogHistoryService>();

        }
        #endregion

        #region Controller Actions
        /// <summary>
        /// Gets the list of catalog history.
        /// </summary>
        /// <returns>List of catalog history.</returns>
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCatalogHistoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CatalogHistoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                CatalogHistoryListResponse data = new CatalogHistoryListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets the catalog history of the specified ID.
        /// </summary>
        /// <param name="id">ID of the catalog history.</param>
        /// <returns>Catalog history for the specified ID.</returns>
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
                //Get catalog history by ID.
                string data = _cache.GetCatalogHistory(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CatalogHistoryResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                CatalogHistoryResponse data = new CatalogHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Creates a catalog history.
        /// </summary>
        /// <param name="model">New catalog history model.</param>
        /// <returns>Newly created catalog history model.</returns>
        [HttpPost]
        public HttpResponseMessage Create([FromBody] CatalogHistoryModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create catalog history.
                CatalogHistoryModel catalogHistory = _service.CreateCatalogHistory(model);
                if (!Equals(catalogHistory, null))
                {
                    response = CreateCreatedResponse(new CatalogHistoryResponse { CatalogHistory = catalogHistory });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(catalogHistory.ID)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (Exception ex)
            {
                CatalogHistoryResponse data = new CatalogHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Updates a catalog history.
        /// </summary>
        /// <param name="model">Catalog history model with new values.</param>
        /// <returns>Updated catalog history model.</returns>
        [HttpPut]
        public HttpResponseMessage Update([FromBody] CatalogHistoryModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update catalog history.
                bool catalogHistory = _service.UpdateCatalogHistory(model);
                response = catalogHistory ? CreateCreatedResponse(new CatalogHistoryResponse { CatalogHistory = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.ID)));

            }
            catch (Exception ex)
            {
                CatalogHistoryResponse data = new CatalogHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Deletes a catalog history.
        /// </summary>
        /// <param name="id">ID of the of the catalog history to be deleted.</param>
        /// <returns>True if catalog history is deleted;False if catalog history is not deleted.</returns>
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response;
            try
            {
                //Delete catalog history.
                bool deleted = _service.DeleteCatalogHistory(id);
                response = deleted ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                CatalogHistoryResponse data = new CatalogHistoryResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}