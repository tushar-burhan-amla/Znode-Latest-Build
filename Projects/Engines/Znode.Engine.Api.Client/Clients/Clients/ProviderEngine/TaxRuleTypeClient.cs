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
    public class TaxRuleTypeClient : BaseClient, ITaxRuleTypeClient
    {
        public virtual TaxRuleTypeListModel GetTaxRuleTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of tax rule type.
            string endpoint = TaxRuleTypeEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TaxRuleTypeListResponse response = GetResourceFromEndpoint<TaxRuleTypeListResponse>(endpoint, status);

            //check the status of response of type tax rule type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TaxRuleTypeListModel list = new TaxRuleTypeListModel { TaxRuleTypes = response?.TaxRuleTypes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual TaxRuleTypeModel GetTaxRuleType(int taxRuleTypeId)
        {
            //Create Endpoint to get a tax rule type.
            string endpoint = TaxRuleTypeEndpoint.Get(taxRuleTypeId);

            ApiStatus status = new ApiStatus();
            TaxRuleTypeResponse response = GetResourceFromEndpoint<TaxRuleTypeResponse>(endpoint, status);

            //check the status of response of type tax rule type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.TaxRuleType;
        }

        public virtual TaxRuleTypeModel CreateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel)
        {
            //Create Endpoint to create new tax rule type.
            string endpoint = TaxRuleTypeEndpoint.Create();

            ApiStatus status = new ApiStatus();
            TaxRuleTypeResponse response = PostResourceToEndpoint<TaxRuleTypeResponse>(endpoint, JsonConvert.SerializeObject(taxRuleTypeModel), status);

            //check the status of response of type tax rule type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.TaxRuleType;
        }

        public virtual TaxRuleTypeModel UpdateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel)
        {
            //Create Endpoint to update tax rule type.
            string endpoint = TaxRuleTypeEndpoint.Update();

            ApiStatus status = new ApiStatus();
            TaxRuleTypeResponse response = PutResourceToEndpoint<TaxRuleTypeResponse>(endpoint, JsonConvert.SerializeObject(taxRuleTypeModel), status);

            //check the status of response of type tax rule type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.TaxRuleType;
        }

        public virtual bool DeleteTaxRuleType(ParameterModel taxRuleTypeIds)
        {
            //Create Endpoint to delete tax rule type.
            string endpoint = TaxRuleTypeEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(taxRuleTypeIds), status);

            //check the status of response of type tax rule type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual TaxRuleTypeListModel GetAllTaxRuleTypesNotInDatabase()
        {
            //Create Endpoint to get the list of tax rule type which are not in database.
            string endpoint = TaxRuleTypeEndpoint.GetAllTaxRuleTypesNotInDatabase();

            ApiStatus status = new ApiStatus();
            TaxRuleTypeListResponse response = GetResourceFromEndpoint<TaxRuleTypeListResponse>(endpoint, status);

            //check the status of response of type tax rule type.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TaxRuleTypeListModel list = new TaxRuleTypeListModel { TaxRuleTypes = response?.TaxRuleTypes };
            list.MapPagingDataFromResponse(response);
            return list;
        }
        
        public virtual bool BulkEnableDisableTaxRuleTypes(ParameterModel taxRuleTypeIds, bool isEnable)
        {
            //Create Endpoint to get the list of tax rule type which are not in database.
            string endpoint = TaxRuleTypeEndpoint.BulkEnableDisableTaxRuleTypes(isEnable);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(taxRuleTypeIds), status);

            //check the status of response of type tax rule type.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
