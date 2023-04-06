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
    public class PIMAttributeClient : BaseClient, IPIMAttributeClient
    {
        //Get attribute types list 
        public virtual PIMAttributeTypeListModel GetAttributeTypes(bool isCategory)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.GetAttributeTypes(isCategory);

            //Get response.
            ApiStatus status = new ApiStatus();
            PIMAttributeListResponse response = GetResourceFromEndpoint<PIMAttributeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes list.
            PIMAttributeTypeListModel list = new PIMAttributeTypeListModel { Types = response?.AttributeTypes };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        
        //Get input validations.
        public virtual PIMAttributeInputValidationListModel GetInputValidations(int typeId, int attributeId = 0)
        {
            //Get Endpoint.
            var endpoint = PIMAttributeEndpoint.GetInputValidations(typeId, attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            PIMAttributeListResponse response = GetResourceFromEndpoint<PIMAttributeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            PIMAttributeInputValidationListModel list = new PIMAttributeInputValidationListModel { InputValidations = response?.InputValidations };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Attribute list.
        public virtual PIMAttributeListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetAttributeList(expands, filters, sorts, null, null);

        //Gets paged Attribute  List.
        public virtual PIMAttributeListModel GetAttributeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.GetAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PIMAttributeListResponse response = GetResourceFromEndpoint<PIMAttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeListModel list = new PIMAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Gets Attribute  by ID.
        public virtual PIMAttributeModel GetAttribute(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.GetAttribute(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            PIMAttributeResponse response = GetResourceFromEndpoint<PIMAttributeResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Attribute;
        }

        //Create Attribute 
        public virtual PIMAttributeDataModel CreateAttributeModel(PIMAttributeDataModel attributeModel)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.CreateAttribute();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeDataResponse response = PostResourceToEndpoint<PIMAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(attributeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PIMAttributeDataModel;
        }

        //Save locales.
        public virtual PIMAttributeLocaleListModel SaveLocales(PIMAttributeLocaleListModel model)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.SaveLocales();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeDataResponse response = PostResourceToEndpoint<PIMAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Locales;
        }

        //Save default values.
        public virtual PIMAttributeDefaultValueModel SaveDefaultValues(PIMAttributeDefaultValueModel model)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.SaveDefaultValues();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeDataResponse response = PostResourceToEndpoint<PIMAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.DefaultValues;
        }

        //Delete default value.
        public virtual bool DeleteDefaultValues(int defaultvalueId)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.DeleteDefaultValues(defaultvalueId);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<PIMAttributeResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return deleted;
        }

        //Updates Attribute
        public virtual PIMAttributeDataModel UpdateAttributeModel(PIMAttributeDataModel attributeModel)
        {
            string endpoint = PIMAttributeEndpoint.UpdateAttribute();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeDataResponse response = PutResourceToEndpoint<PIMAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(attributeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PIMAttributeDataModel;
        }

        //Delete Attribute
        public virtual bool DeleteAttributeModel(ParameterModel pimAttributeIds)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.DeleteAttribute();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(pimAttributeIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Delete Attribute
        public virtual PIMFrontPropertiesModel FrontEndProperties(int pimAttributeId)
        {
            string endpoint = PIMAttributeEndpoint.FrontEndProperties(pimAttributeId);

            ApiStatus status = new ApiStatus();
            PIMFrontPropertiesResponse response = GetResourceFromEndpoint<PIMFrontPropertiesResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.FrontEndProperties;
        }

        //Get attribute locales.
        public virtual PIMAttributeLocaleListModel GetAttributeLocale(int attributeId)
        {
            //Get Endpoint.
            var endpoint = PIMAttributeEndpoint.GetAttributeLocale(attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            PIMAttributeLocaleListResponce response = GetResourceFromEndpoint<PIMAttributeLocaleListResponce>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            PIMAttributeLocaleListModel list = new PIMAttributeLocaleListModel { Locales = response?.AttributeLocales };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get default values.
        public virtual PIMAttributeDefaultValueListModel GetDefaultValues(int attributeId)
        {
            //Get Endpoint.
            var endpoint = PIMAttributeEndpoint.GetDefaultValues(attributeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            PIMAttributeLocaleListResponce response = GetResourceFromEndpoint<PIMAttributeLocaleListResponce>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes Input validation list.
            PIMAttributeDefaultValueListModel list = new PIMAttributeDefaultValueListModel { DefaultValues = response?.DefaultValues };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Check attribute Code already exist or not
        public virtual bool IsAttributeCodeExist(string attributeCode, bool isCategory)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.IsAttributeCodeExist(attributeCode, isCategory);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Check Is attribute value is Unique or not.
        public virtual ParameterModel IsAttributeValueUnique(PimAttributeValueParameterModel attributeCodeValues)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeEndpoint.IsAttributeValueUnique();

            //Get response.
            ApiStatus status = new ApiStatus();
            StringResponse response = PostResourceToEndpoint<StringResponse>(endpoint, JsonConvert.SerializeObject(attributeCodeValues), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return new ParameterModel { Ids = response?.Response};
        }

        //Get attribute validation by attribute code.
        public virtual PIMFamilyDetailsModel GetAttributeValidationByCodes(ParameterProductModel model)
        {
            string endpoint = PIMAttributeEndpoint.GetAttributeValidationByCodes();

            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyResponse response = PostResourceToEndpoint<PIMAttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PIMFamilyDetails;
        }
    }
}
