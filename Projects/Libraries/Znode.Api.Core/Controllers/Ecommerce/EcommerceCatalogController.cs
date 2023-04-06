using System;
using System.Collections.Generic;
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

namespace Znode.Engine.Api.Controllers.Ecommerce
{
    public class EcommerceCatalogController : BaseController
    {
        #region Private Variables
        private readonly IEcommerceCatalogService _service;
        private readonly IEcommerceCatalogCache _cache;
        #endregion

        #region Constructor
        public EcommerceCatalogController(IEcommerceCatalogService service)
        {
            _service = service;
            _cache = new EcommerceCatalogCache(_service);
        }
        #endregion

        /// <summary>
        /// Get the list of all Publish Catalogs.
        /// </summary>
        /// <returns>Return publish catalog list.</returns>
        [ResponseType(typeof(PublishCatalogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishCatalogList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPublishCatalogList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PublishCatalogListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get catalogs associated to portal by portalId.
        /// </summary>
        /// <param name="portalId">Portal ID to get associated catalog</param>
        /// <return>Return portal catalog list.</return>
        [ResponseType(typeof(PortalCatalogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedPortalCatalogByPortalId(int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedPortalCatalogByPortalId(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalCatalogListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get catalogs associated with portal.
        /// </summary>
        /// <returns> Returns portal catalog list.</returns>
        [ResponseType(typeof(PortalCatalogListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetAssociatedPortalCatalog([FromBody] ParameterModel filterIds)
        {
            HttpResponseMessage response;

            try
            {
                //Get data from service
                PortalCatalogListModel portalCatalogList = _service.GetAssociatedPortalCatalog(filterIds);
                if (portalCatalogList?.PortalCatalogs?.Count > 0)
                {
                    PortalCatalogListResponse data = new PortalCatalogListResponse { PortalCatalogs = portalCatalogList.PortalCatalogs };
                    response = IsNotNull(data) ? CreateOKResponse(data) : CreateNoContentResponse();
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update Portal Catalog.
        /// </summary>
        /// <param name="portalCatalogModel">portalCatalogModel</param>
        /// <returns>Returns portal catalog.</returns>
        [ResponseType(typeof(PortalCatalogResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdatePortalCatalog([FromBody] PortalCatalogModel portalCatalogModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdatePortalCatalog(portalCatalogModel);
                response = data ? CreateCreatedResponse(new PortalCatalogResponse { PortalCatalog = portalCatalogModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalCatalogResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalCatalogResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Portal Catalog.
        /// </summary>
        /// <param name="portalCatalogId">portalCatalogId</param>
        /// <returns>Returns portal catalog.</returns>
        [ResponseType(typeof(PortalCatalogResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalCatalog(int portalCatalogId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPortalCatalog(portalCatalogId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalCatalogResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalCatalogResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the tree structure for Catalog.
        /// </summary>
        /// <param name="catalogId">ID of catalog to get Catalog tree.</param>
        /// <param name="categoryId">ID of Category to get Catalog tree.</param>
        /// <returns>Return tree structure for catalog.</returns>
        [ResponseType(typeof(CategoryTreeResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCatalogTree(int catalogId, int categoryId)
        {
            HttpResponseMessage response;
            try
            {
                List<CategoryTreeModel> data = _service.GetCatalogTree(catalogId, categoryId);
                response = IsNotNull(data) ? CreateCreatedResponse(new CategoryTreeResponse { CategoryTree = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryTreeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryTreeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Publish Catalog Details
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId to get catalog details</param>
        /// <returns>Return  publish catalog details.</returns>
        [ResponseType(typeof(EcommerceResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishCatalogDetails(int publishCatalogId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPublishCatalogDetails(publishCatalogId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EcommerceResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EcommerceResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Publish Category Details
        /// </summary>
        /// <param name="publishCategoryId">publishCategoryId to get category details</param>
        /// <returns>Return publish category details.</returns>
        [ResponseType(typeof(EcommerceResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishCategoryDetails(int publishCategoryId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPublishCategoryDetails(publishCategoryId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EcommerceResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EcommerceResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Publish Product Details
        /// </summary>
        /// <param name="publishProductId">publishProductId to get product details</param>
        /// <param name="portalId">portalId to get product inventory and pricing details</param> 
        /// <returns>Return ecommerce details.</returns>
        [ResponseType(typeof(EcommerceResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishProductDetails(int publishProductId, int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPublishProductDetails(publishProductId, portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EcommerceResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EcommerceResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }
    }
}
