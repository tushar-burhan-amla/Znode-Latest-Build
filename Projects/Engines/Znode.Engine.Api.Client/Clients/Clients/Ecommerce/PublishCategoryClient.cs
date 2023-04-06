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
    public class PublishCategoryClient : BaseClient, IPublishCategoryClient
    {
        // Gets the list of Publish Categories.
        public virtual PublishCategoryListModel GetPublishCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
              => GetPublishCategoryList(expands, filters, sorts, null, null);

        // Gets the list of Publish Categories.
        public virtual PublishCategoryListModel GetPublishCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishCategoryEndpoint.GetPublishCategoryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCategoryListResponse response = GetResourceFromEndpoint<PublishCategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCategoryListModel list = new PublishCategoryListModel { PublishCategories = response?.PublishCategories };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get the Publish Category.
        public virtual PublishCategoryModel GetPublishCategory(int publishCategoryId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = PublishCategoryEndpoint.GetPublishCategory(publishCategoryId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PublishCategoryResponse response = GetResourceFromEndpoint<PublishCategoryResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishCategory;
        }

        // Get publish category excluding assigned ids.
        public virtual PublishCategoryListModel GetUnAssignedPublishCategoryList(string assignedIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishCategoryEndpoint.GetUnAssignedPublishCategoryList(assignedIds);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCategoryListResponse response = GetResourceFromEndpoint<PublishCategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCategoryListModel list = new PublishCategoryListModel { PublishCategories = response?.PublishCategories };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets the list of Categories.
        public virtual PublishCategoryListModel GetCategoryListForSEO(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SEOSettingEndpoints.GetCategoryListForSEO();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCategoryListResponse response = GetResourceFromEndpoint<PublishCategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishCategoryListModel list = new PublishCategoryListModel { PublishCategories = response?.PublishCategories };
            list.MapPagingDataFromResponse(response);

            return list;
        }
    }
}
