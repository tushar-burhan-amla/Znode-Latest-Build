using Newtonsoft.Json;
using System.Collections.Generic;
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
    public class EcommerceCatalogClient : BaseClient, IEcommerceCatalogClient
    {
        public virtual PublishCatalogListModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of all publish catalogs.
            string endpoint = EcommerceCatalogEndpoint.GetPublishCatalogList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();

            PublishCatalogListResponse response = GetResourceFromEndpoint<PublishCatalogListResponse>(endpoint, status);

            //Check the status of response of publish catalog list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCatalogListModel list = new PublishCatalogListModel { PublishCatalogs = response?.PublishCatalogs };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get portal associated catalogs as per portalId
        public virtual PortalCatalogListModel GetAssociatedPortalCatalogByPortalId(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sorts)
           => GetAssociatedPortalCatalogByPortalId(portalId, expands, filters, sorts, null, null);

        //Get portal associated catalogs as per portalId
        public virtual PortalCatalogListModel GetAssociatedPortalCatalogByPortalId(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = EcommerceCatalogEndpoint.GetAssociatedPortalCatalogByPortalId(portalId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PortalCatalogListResponse response = GetResourceFromEndpoint<PortalCatalogListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalCatalogListModel list = new PortalCatalogListModel { PortalCatalogs = (Equals(response, null)) ? null : response.PortalCatalogs };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get portal associated catalogs
        public virtual PortalCatalogListModel GetAssociatedPortalCatalog(ParameterModel filterIds)
        {
            string endpoint = EcommerceCatalogEndpoint.GetAssociatedPortalCatalog();
            endpoint += BuildEndpointQueryString(null);

            ApiStatus status = new ApiStatus();
            PortalCatalogListResponse response = PostResourceToEndpoint<PortalCatalogListResponse>(endpoint, JsonConvert.SerializeObject(filterIds), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalCatalogListModel list = new PortalCatalogListModel { PortalCatalogs = (Equals(response, null)) ? null : response.PortalCatalogs };

            return list;
        }

        // Updates a Portal Catalog.
        public virtual PortalCatalogModel UpdatePortalCatalog(PortalCatalogModel portalCatalogModel)
        {
            string endpoint = EcommerceCatalogEndpoint.UpdatePortalCatalog();

            ApiStatus status = new ApiStatus();
            PortalCatalogResponse response = PutResourceToEndpoint<PortalCatalogResponse>(endpoint, JsonConvert.SerializeObject(portalCatalogModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PortalCatalog;
        }

        //Get Portal Catalog
        public virtual PortalCatalogModel GetPortalCatalog(int portalCatalogId)
        {
            string endpoint = EcommerceCatalogEndpoint.GetPortalCatalog(portalCatalogId);

            ApiStatus status = new ApiStatus();
            PortalCatalogResponse response = GetResourceFromEndpoint<PortalCatalogResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalCatalog;
        }

        //Get Tree Structure for Catalog.
        public virtual List<CategoryTreeModel> GetCatalogTree(int catalogId, int categoryId)
        {
            string endpoint = EcommerceCatalogEndpoint.GetCatalogTree(catalogId, categoryId);
            ApiStatus status = new ApiStatus();

            CategoryTreeResponse response = GetResourceFromEndpoint<CategoryTreeResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CategoryTree;
        }

        //Get Publish Catalog Details
        public virtual PublishCatalogModel GetPublishCatalogDetails(int publishCatalogId)
        {
            string endpoint = EcommerceCatalogEndpoint.GetPublishCatalogDetails(publishCatalogId);

            ApiStatus status = new ApiStatus();
            EcommerceResponse response = GetResourceFromEndpoint<EcommerceResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishCatalog;
        }

        //Get Publish Category Details
        public virtual PublishCategoryModel GetPublishCategoryDetails(int publishCategoryId)
        {
            string endpoint = EcommerceCatalogEndpoint.GetPublishCategoryDetails(publishCategoryId);

            ApiStatus status = new ApiStatus();
            EcommerceResponse response = GetResourceFromEndpoint<EcommerceResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishCategory;
        }

        //Get Publish Product Details
        public virtual PublishProductModel GetPublishProductDetails(int publishProductId, int portalId)
        {
            string endpoint = EcommerceCatalogEndpoint.GetPublishProductDetails(publishProductId, portalId);

            ApiStatus status = new ApiStatus();
            EcommerceResponse response = GetResourceFromEndpoint<EcommerceResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }
    }
}
