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
    public class PublishCatalogClient : BaseClient, IPublishCatalogClient
    {
        // Gets the list of Publish Catalogs.
        public virtual PublishCatalogListModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
           => GetPublishCatalogList(expands, filters, sorts, null, null);

        // Gets the list of Publish Catalogs.
        public virtual PublishCatalogListModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishCatalogEndpoint.GetPublishCatalogList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCatalogListResponse response = GetResourceFromEndpoint<PublishCatalogListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCatalogListModel list = new PublishCatalogListModel { PublishCatalogs = response?.PublishCatalogs };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get publish category excluding assigned ids.
        public virtual PublishCatalogListModel GetUnAssignedPublishCatelogList(string assignedIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishCatalogEndpoint.GetUnAssignedPublishCatelogList(assignedIds);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCatalogListResponse response = GetResourceFromEndpoint<PublishCatalogListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCatalogListModel list = new PublishCatalogListModel { PublishCatalogs = response?.PublishCatalogs };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get Publish Catalog.
        public virtual PublishCatalogModel GetPublishCatalog(int publishCatalogId, ExpandCollection expands, int? localeId = null)
        {
            string endpoint = PublishCatalogEndpoint.GetPublishCatalog(publishCatalogId, localeId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            PublishCatalogResponse response = GetResourceFromEndpoint<PublishCatalogResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishCatalog;
        }
    }
}
