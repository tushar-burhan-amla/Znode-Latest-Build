using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class CategoryHistoryClient : BaseClient
    {
        //Get category history list.
        public virtual CategoryHistoryListModel GetCategoryHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCategoryHistoryList(expands, filters, sorts, null, null);

        //Gets paged category history List.
        public virtual CategoryHistoryListModel GetCategoryHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CategoryHistoryEndpoint.GetCategoryHistories();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CategoryHistoryListResponse response = GetResourceFromEndpoint<CategoryHistoryListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryHistoryListModel list = new CategoryHistoryListModel { CategoryHistories = response?.CategoryHistories };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Gets category history  by ID.
        public virtual CategoryHistoryModel GetCategoryHistory(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = CategoryHistoryEndpoint.GetCategoryHistory(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            CategoryHistoryResponse response = GetResourceFromEndpoint<CategoryHistoryResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CategoryHistory;
        }

        //Create category history 
        public virtual CategoryHistoryModel CreateCategoryHistoryModel(CategoryHistoryModel categoryHistoryModel)
        {
            //Get Endpoint.
            string endpoint = CategoryHistoryEndpoint.CreateCategoryHistory();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CategoryHistoryResponse response = PostResourceToEndpoint<CategoryHistoryResponse>(endpoint, JsonConvert.SerializeObject(categoryHistoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CategoryHistory;
        }

        //Updates category history
        public virtual CategoryHistoryModel UpdateCategoryHistoryModel(CategoryHistoryModel categoryHistoryModel)
        {
            string endpoint = CategoryHistoryEndpoint.UpdateCategoryHistory();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CategoryHistoryResponse response = PutResourceToEndpoint<CategoryHistoryResponse>(endpoint, JsonConvert.SerializeObject(categoryHistoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CategoryHistory;
        }

        //Delete category history
        public virtual bool DeleteCategoryHistoryModel(int id)
        {
            //Get Endpoint.
            string endpoint = CategoryHistoryEndpoint.DeleteCategoryHistory(id);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<CategoryHistoryResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return deleted;
        }
    }
}
