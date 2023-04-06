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
    public class PromotionTypeClient : BaseClient, IPromotionTypeClient
    {
        public virtual PromotionTypeListModel GetPromotionTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of Promotion Type type.
            string endpoint = PromotionTypeEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PromotionTypeListResponse response = GetResourceFromEndpoint<PromotionTypeListResponse>(endpoint, status);

            //check the status of response of type Promotion Type type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PromotionTypeListModel list = new PromotionTypeListModel { PromotionTypes = response?.PromotionTypeList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual PromotionTypeModel GetPromotionType(int promotionTypeId)
        {
            //Create Endpoint to get a Promotion Type type.
            string endpoint = PromotionTypeEndpoint.Get(promotionTypeId);

            ApiStatus status = new ApiStatus();
            PromotionTypeResponse response = GetResourceFromEndpoint<PromotionTypeResponse>(endpoint, status);

            //check the status of response of type Promotion Type type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PromotionType;
        }

        public virtual PromotionTypeModel CreatePromotionType(PromotionTypeModel promotionTypeModel)
        {
            //Create Endpoint to create new Promotion Type type.
            string endpoint = PromotionTypeEndpoint.Create();

            ApiStatus status = new ApiStatus();
            PromotionTypeResponse response = PostResourceToEndpoint<PromotionTypeResponse>(endpoint, JsonConvert.SerializeObject(promotionTypeModel), status);

            //check the status of response of type Promotion Type type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PromotionType;
        }

        public virtual PromotionTypeModel UpdatePromotionType(PromotionTypeModel promotionTypeModel)
        {
            //Create Endpoint to update Promotion Type type.
            string endpoint = PromotionTypeEndpoint.Update();

            ApiStatus status = new ApiStatus();
            PromotionTypeResponse response = PutResourceToEndpoint<PromotionTypeResponse>(endpoint, JsonConvert.SerializeObject(promotionTypeModel), status);

            //check the status of response of type Promotion Type type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.PromotionType;
        }

        public virtual bool DeletePromotionType(ParameterModel entityIds)
        {
            //Create Endpoint to delete Promotion Type type.
            string endpoint = PromotionTypeEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(entityIds), status);

            //check the status of response of type Promotion Type type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual PromotionTypeListModel GetAllPromotionTypesNotInDatabase()
        {
            //Create Endpoint to get the list of Promotion Type type which are not in database.
            string endpoint = PromotionTypeEndpoint.GetAllPromotionTypesNotInDatabase();

            ApiStatus status = new ApiStatus();
            PromotionTypeListResponse response = GetResourceFromEndpoint<PromotionTypeListResponse>(endpoint, status);

            //check the status of response of type Promotion Type type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PromotionTypeListModel list = new PromotionTypeListModel { PromotionTypes = response?.PromotionTypeList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual bool BulkEnableDisablePromotionTypes(ParameterModel entityIds, bool isEnable)
        {
            //Create Endpoint to get the list of tax rule type which are not in database.
            string endpoint = PromotionTypeEndpoint.BulkEnableDisablePromotionTypes(isEnable);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(entityIds), status);

            //check the status of response of type tax rule type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
