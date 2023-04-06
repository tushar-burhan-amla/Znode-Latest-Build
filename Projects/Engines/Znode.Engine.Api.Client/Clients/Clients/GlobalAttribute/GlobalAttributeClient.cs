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
    public class GlobalAttributeClient : BaseClient, IGlobalAttributeClient
    {
        //Create global attribute. 
        public virtual GlobalAttributeDataModel CreateAttributeModel(GlobalAttributeDataModel attributeModel)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.CreateAttribute();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeDataResponse response = PostResourceToEndpoint<GlobalAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(attributeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.GlobalAttributeDataModel;
        }

        //Save locales.
        public virtual GlobalAttributeLocaleListModel SaveLocales(GlobalAttributeLocaleListModel model)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.SaveLocales();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeDataResponse response = PostResourceToEndpoint<GlobalAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Locales;
        }

        //Save default values.
        public virtual GlobalAttributeDefaultValueModel SaveDefaultValues(GlobalAttributeDefaultValueModel model)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.SaveDefaultValues();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeDataResponse response = PostResourceToEndpoint<GlobalAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.DefaultValues;
        }

        //Get input validations.
        public virtual GlobalAttributeInputValidationListModel GetInputValidations(int typeId, int attributeId = 0)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.GetInputValidations(typeId, attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeListResponse response = GetResourceFromEndpoint<GlobalAttributeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            GlobalAttributeInputValidationListModel list = new GlobalAttributeInputValidationListModel { InputValidations = response?.InputValidations };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get paged Attribute List.
        public virtual GlobalAttributeListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.GetAttributeList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            GlobalAttributeListResponse response = GetResourceFromEndpoint<GlobalAttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeListModel list = new GlobalAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get global attribute by ID.
        public virtual GlobalAttributeModel GetAttribute(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.GetAttribute(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            GlobalAttributeResponse response = GetResourceFromEndpoint<GlobalAttributeResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Attribute;
        }

        //Update global attribute.
        public virtual GlobalAttributeDataModel UpdateAttributeModel(GlobalAttributeDataModel attributeModel)
        {
            string endpoint = GlobalAttributeEndpoint.UpdateAttribute();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeDataResponse response = PutResourceToEndpoint<GlobalAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(attributeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.GlobalAttributeDataModel;
        }

        //Delete global attribute(s).
        public virtual bool DeleteAttributeModel(ParameterModel globalAttributeIds)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.DeleteAttribute();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalAttributeIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get attribute locales details.
        public virtual GlobalAttributeLocaleListModel GetAttributeLocale(int attributeId)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.GetAttributeLocale(attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeLocaleListResponse response = GetResourceFromEndpoint<GlobalAttributeLocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            GlobalAttributeLocaleListModel list = new GlobalAttributeLocaleListModel { Locales = response?.AttributeLocales };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get default values.
        public virtual GlobalAttributeDefaultValueListModel GetDefaultValues(int attributeId)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.GetDefaultValues(attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeLocaleListResponse response = GetResourceFromEndpoint<GlobalAttributeLocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            GlobalAttributeDefaultValueListModel list = new GlobalAttributeDefaultValueListModel { DefaultValues = response?.DefaultValues };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Check attribute code already exist or not.
        public virtual bool IsAttributeCodeExist(string attributeCode)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.IsAttributeCodeExist(attributeCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Delete default value.
        public virtual bool DeleteDefaultValues(int defaultValueId)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.DeleteDefaultValues(defaultValueId);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<GlobalAttributeResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return deleted;
        }

        //Check is attribute value is unique or not.
        public virtual ParameterModel IsAttributeValueUnique(GlobalAttributeValueParameterModel attributeCodeValues)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.IsAttributeValueUnique();

            //Get response.
            ApiStatus status = new ApiStatus();
            StringResponse response = PostResourceToEndpoint<StringResponse>(endpoint, JsonConvert.SerializeObject(attributeCodeValues), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return new ParameterModel { Ids = response?.Response };
        }
    }
}