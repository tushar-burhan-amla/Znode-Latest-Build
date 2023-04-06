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
    public class AttributesClient : BaseClient, IAttributesClient
    {
        //Get attribute types list 
        public virtual AttributesTypeListModel GetAttributeTypes()
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.GetAttributeTypeList();

            //Get response.
            ApiStatus status = new ApiStatus();
            AttributeListResponse response = GetResourceFromEndpoint<AttributeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attribute type list.
            AttributesTypeListModel list = new AttributesTypeListModel { Types = response?.AttributeTypes };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual AttributesInputValidationListModel GetInputValidations(int typeId, int attributeId = 0)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.GetInputValidations(typeId, attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            AttributeListResponse response = GetResourceFromEndpoint<AttributeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            AttributesInputValidationListModel list = new AttributesInputValidationListModel { InputValidations = response?.InputValidations };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Attribute list.
        public virtual AttributesListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetAttributeList(expands, filters, sorts, null, null);

        //Gets paged Attribute  List.
        public virtual AttributesListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.GetAttributeList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            AttributeListResponse response = GetResourceFromEndpoint<AttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributesListModel list = new AttributesListModel { Attributes = response?.Attributes };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Gets Attribute  by ID.
        public virtual AttributesDataModel GetAttribute(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.GetAttribute(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();

            AttributesResponses response = GetResourceFromEndpoint<AttributesResponses>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Attribute;
        }

        //Create Attribute 
        public virtual AttributesDataModel CreateAttributeModel(AttributesDataModel attributeModel)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.Create();

            ApiStatus status = new ApiStatus();

            //Get Serialize object as a response.
            AttributesResponses response = PostResourceToEndpoint<AttributesResponses>(endpoint, JsonConvert.SerializeObject(attributeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Attribute;
        }

        //Save attribute locale value
        public virtual AttributesLocaleListModel SaveLocales(AttributesLocaleListModel model)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.SaveLocales();

            ApiStatus status = new ApiStatus();
            AttributesDataResponse response = PostResourceToEndpoint<AttributesDataResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Locales;
        }

        //Save Attribute Default Value
        public virtual AttributesDefaultValueModel SaveDefaultValues(AttributesDefaultValueModel model)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.SaveDefaultValues();

            ApiStatus status = new ApiStatus();
            AttributesDataResponse response = PostResourceToEndpoint<AttributesDataResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.DefaultValues;
        }

        //Delete Attribute Default value by Defaultvalue id
        public virtual bool DeleteDefaultValues(int defaultvalueId)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.DeleteDefaultValues(defaultvalueId);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<AttributesResponses>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return deleted;
        }

        //Updates Attribute
        public virtual AttributesDataModel UpdateAttributeModel(AttributesDataModel attributeModel)
        {
            string endpoint = AttributesEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            AttributesResponses response = PostResourceToEndpoint<AttributesResponses>(endpoint, JsonConvert.SerializeObject(attributeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.Attribute;
        }

        //Delete Attribute
        public virtual bool DeleteAttributeModel(ParameterModel pimAttributeIds)
        {
            //Get Endpoint.
            string endpoint = AttributesEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(pimAttributeIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get AttributesLocaleList by attribute id
        public virtual AttributesLocaleListModel GetAttributeLocale(int attributeId)
        {
            //Get Endpoint.
            var endpoint = AttributesEndpoint.GetAttributeLocale(attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            AttributesLocaleListResponse response = GetResourceFromEndpoint<AttributesLocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            AttributesLocaleListModel list = new AttributesLocaleListModel { Locales = response?.AttributeLocales };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get DefaultValues list by attribute id
        public virtual AttributesDefaultValueListModel GetDefaultValues(int attributeId)
        {
            //Get Endpoint.
            var endpoint = AttributesEndpoint.GetDefaultValues(attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            AttributesLocaleListResponse response = GetResourceFromEndpoint<AttributesLocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            AttributesDefaultValueListModel list = new AttributesDefaultValueListModel { DefaultValues = response?.DefaultValues };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Check attribute Code already exist or not
        public virtual bool IsAttributeCodeExist(string attributeCode)
        {
            //Get Endpoint.
            var endpoint = AttributesEndpoint.IsAttributeCodeExist(attributeCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }
}
