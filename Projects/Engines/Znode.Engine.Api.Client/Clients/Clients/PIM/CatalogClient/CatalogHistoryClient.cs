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
    public class CatalogHistoryClient : BaseClient, ICatalogHistoryClient
    {
        //Get catalog history list.
        public virtual CatalogHistoryListModel GetCatalogHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCatalogHistoryList(expands, filters, sorts, null, null);

        //Gets paged catalog history List.
        public virtual CatalogHistoryListModel GetCatalogHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CatalogHistoryEndpoint.GetCatalogHistories();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CatalogHistoryListResponse response = GetResourceFromEndpoint<CatalogHistoryListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CatalogHistoryListModel list = new CatalogHistoryListModel { CatalogHistories = response?.CatalogHistories };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Gets catalog history  by ID.
        public virtual CatalogHistoryModel GetCatalogHistory(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = CatalogHistoryEndpoint.GetCatalogHistory(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            CatalogHistoryResponse response = GetResourceFromEndpoint<CatalogHistoryResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CatalogHistory;
        }

        //Create catalog history 
        public virtual CatalogHistoryModel CreateCatalogHistoryModel(CatalogHistoryModel catalogHistoryModel)
        {
            //Get Endpoint.
            string endpoint = CatalogHistoryEndpoint.CreateCatalogHistory();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CatalogHistoryResponse response = PostResourceToEndpoint<CatalogHistoryResponse>(endpoint, JsonConvert.SerializeObject(catalogHistoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CatalogHistory;
        }

        //Updates catalog history
        public virtual CatalogHistoryModel UpdateCatalogHistoryModel(CatalogHistoryModel catalogHistoryModel)
        {
            string endpoint = CatalogHistoryEndpoint.UpdateCatalogHistory();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CatalogHistoryResponse response = PutResourceToEndpoint<CatalogHistoryResponse>(endpoint, JsonConvert.SerializeObject(catalogHistoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CatalogHistory;
        }

        //Delete catalog history
        public virtual bool DeleteCatalogHistoryModel(int id)
        {
            //Get Endpoint.
            string endpoint = CatalogHistoryEndpoint.DeleteCatalogHistory(id);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<CatalogHistoryResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return deleted;
        }
    }
}
