using Newtonsoft.Json;
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
    public class StoreLocatorClient : BaseClient, IStoreLocatorClient
    {
        //Save store data for store location.
        public virtual StoreLocatorDataModel SaveStore(StoreLocatorDataModel storeLocatorModel)
        {
            string endpoint = StoreLocatorEndpoint.SaveStore();

            ApiStatus status = new ApiStatus();
            StoreLocatorResponse response = PostResourceToEndpoint<StoreLocatorResponse>(endpoint, JsonConvert.SerializeObject(storeLocatorModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.storeLocatorModel;
        }

        //Get Store List for location.
        public virtual StoreLocatorListModel GetStoreLocatorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = StoreLocatorEndpoint.GetStoreLocatorList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();

            StoreLocatorListResponse response = GetResourceFromEndpoint<StoreLocatorListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            StoreLocatorListModel list = new StoreLocatorListModel { StoreLocatorList = response?.StoreLocatorList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get store data by id
        public virtual StoreLocatorDataModel GetStoreLocator(int portalAddresId, ExpandCollection expands)
        {
            string endpoint = StoreLocatorEndpoint.GetStoreLocator(portalAddresId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();

            StoreLocatorResponse response = GetResourceFromEndpoint<StoreLocatorResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.storeLocatorModel;
        }

        //Get Store data by store location code
        public virtual StoreLocatorDataModel GetStoreLocator(string storeLocationCode, ExpandCollection expands)
        {
            string endpoint = StoreLocatorEndpoint.GetStoreLocatorByCode(storeLocationCode);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();

            StoreLocatorResponse response = GetResourceFromEndpoint<StoreLocatorResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.storeLocatorModel;
        }

        //Update Existing store data for location.
        public virtual StoreLocatorDataModel Update(StoreLocatorDataModel storeLocatorModel)
        {
            string endpoint = StoreLocatorEndpoint.Manage();

            ApiStatus status = new ApiStatus();
            StoreLocatorResponse response = PutResourceToEndpoint<StoreLocatorResponse>(endpoint, JsonConvert.SerializeObject(storeLocatorModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.storeLocatorModel;
        }

        //Delete an existing store data.
        public virtual bool DeleteStoreLocator(ParameterModel parameterModel, bool isDeleteByCode)
        {
            string endpoint = StoreLocatorEndpoint.DeleteStoreLocator(isDeleteByCode);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
