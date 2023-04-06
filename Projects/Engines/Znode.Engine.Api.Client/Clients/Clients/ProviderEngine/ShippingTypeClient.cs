using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class ShippingTypeClient : BaseClient, IShippingTypeClient
    {
        public virtual ShippingTypeListModel GetShippingTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of shipping type.
            string endpoint = ShippingTypeEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ShippingTypeListResponse response = GetResourceFromEndpoint<ShippingTypeListResponse>(endpoint, status);

            //check the status of response of type shipping type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ShippingTypeListModel list = new ShippingTypeListModel { ShippingTypeList = response?.ShippingTypeList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual ShippingTypeModel GetShippingType(int shippingTypeId)
        {
            //Create Endpoint to get a shipping type.
            string endpoint = ShippingTypeEndpoint.Get(shippingTypeId);

            ApiStatus status = new ApiStatus();
            ShippingTypeResponse response = GetResourceFromEndpoint<ShippingTypeResponse>(endpoint, status);

            //check the status of response of type shipping type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ShippingType;
        }

        public virtual ShippingTypeModel CreateShippingType(ShippingTypeModel shippingTypeModel)
        {
            //Create Endpoint to create new shipping type.
            string endpoint = ShippingTypeEndpoint.Create();

            ApiStatus status = new ApiStatus();
            ShippingTypeResponse response = PostResourceToEndpoint<ShippingTypeResponse>(endpoint, JsonConvert.SerializeObject(shippingTypeModel), status);

            //check the status of response of type shipping type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ShippingType;
        }

        public virtual ShippingTypeModel UpdateShippingType(ShippingTypeModel shippingTypeModel)
        {
            //Create Endpoint to update shipping type.
            string endpoint = ShippingTypeEndpoint.Update();

            ApiStatus status = new ApiStatus();
            ShippingTypeResponse response = PutResourceToEndpoint<ShippingTypeResponse>(endpoint, JsonConvert.SerializeObject(shippingTypeModel), status);

            //check the status of response of type shipping type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.ShippingType;
        }

        public virtual bool DeleteShippingType(ParameterModel entityIds)
        {
            //Create Endpoint to delete shipping type.
            string endpoint = ShippingTypeEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(entityIds), status);

            //check the status of response of type shipping type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual ShippingTypeListModel GetAllShippingTypesNotInDatabase()
        {
            //Create Endpoint to get the list of shipping type which are not in database.
            string endpoint = ShippingTypeEndpoint.GetAllShippingTypesNotInDatabase();

            ApiStatus status = new ApiStatus();
            ShippingTypeListResponse response = GetResourceFromEndpoint<ShippingTypeListResponse>(endpoint, status);

            //check the status of response of type shipping type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ShippingTypeListModel list = new ShippingTypeListModel { ShippingTypeList = response?.ShippingTypeList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual bool BulkEnableDisableShippingTypes(ParameterModel entityIds, bool isEnable)
        {
            //Create Endpoint to get the list of tax rule type which are not in database.
            string endpoint = ShippingTypeEndpoint.BulkEnableDisableShippingTypes(isEnable);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(entityIds), status);

            //check the status of response of type tax rule type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
