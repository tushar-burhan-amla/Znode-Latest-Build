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
    public class TaxClassClient : BaseClient, ITaxClassClient
    {
        #region Tax Class

        // Get the list of tax class.
        public virtual TaxClassListModel GetTaxClassList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of tax class.
            string endpoint = TaxClassEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TaxClassListResponse response = GetResourceFromEndpoint<TaxClassListResponse>(endpoint, status);

            //check the status of response of type tax class.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TaxClassListModel list = new TaxClassListModel { TaxClassList = response?.TaxClasses };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        // Get a tax class.
        public virtual TaxClassModel GetTaxClass(int taxClassId)
        {
            //Create Endpoint to get a tax class.
            string endpoint = TaxClassEndpoint.Get(taxClassId);

            ApiStatus status = new ApiStatus();
            TaxClassResponse response = GetResourceFromEndpoint<TaxClassResponse>(endpoint, status);

            //check the status of response of type tax class.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.TaxClass;
        }

        // Create a new tax class.
        public virtual TaxClassModel CreateTaxClass(TaxClassModel taxClassModel)
        {
            //Create Endpoint to create new tax class.
            string endpoint = TaxClassEndpoint.Create();

            ApiStatus status = new ApiStatus();
            TaxClassResponse response = PostResourceToEndpoint<TaxClassResponse>(endpoint, JsonConvert.SerializeObject(taxClassModel), status);

            //check the status of response of type tax class.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.TaxClass;
        }

        // Update a Tax Class.
        public virtual TaxClassModel UpdateTaxClass(TaxClassModel taxClassModel)
        {
            //Create Endpoint to update tax class.
            string endpoint = TaxClassEndpoint.Update();

            ApiStatus status = new ApiStatus();
            TaxClassResponse response = PutResourceToEndpoint<TaxClassResponse>(endpoint, JsonConvert.SerializeObject(taxClassModel), status);

            //check the status of response of type tax class.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.TaxClass;
        }

        // Delete a Tax Class.
        public virtual bool DeleteTaxClass(ParameterModel taxClassIds)
        {
            //Create Endpoint to delete tax class.
            string endpoint = TaxClassEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(taxClassIds), status);

            //check the status of response of type tax class.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion Tax Class

        #region Tax Class SKU

        // Get Tax Class SKU list.
        public virtual TaxClassSKUListModel GetTaxClassSKUList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {           
            string endpoint = TaxClassEndpoint.TaxClassSKUList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TaxClassSKUListResponse response = GetResourceFromEndpoint<TaxClassSKUListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TaxClassSKUListModel taxClassSKUListModel = new TaxClassSKUListModel { TaxClassSKUList = response?.TaxClassSKUList };
            taxClassSKUListModel.MapPagingDataFromResponse(response);

            return taxClassSKUListModel;
        }

        // Add Tax Class SKU.
        public virtual TaxClassSKUModel AddTaxClassSKU(TaxClassSKUModel taxClassSKUModel)
        {
            string endpoint = TaxClassEndpoint.AddTaxClassSKU();

            ApiStatus status = new ApiStatus();
            TaxClassSKUResponse response = PostResourceToEndpoint<TaxClassSKUResponse>(endpoint, JsonConvert.SerializeObject(taxClassSKUModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.TaxClassSKU;
        }

        // Delete existing Tax Class SKU.
        public virtual bool DeleteTaxClassSKU(string taxClassSKUId)
        {
            string endpoint = TaxClassEndpoint.DeleteTaxClassSKU();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = taxClassSKUId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Get unassociated product list.
        public virtual ProductDetailsListModel GetUnassociatedProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = TaxClassEndpoint.GetUnassociatedProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductListResponse response = GetResourceFromEndpoint<ProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductDetailsListModel list = GetProductDetailsListModelByResponse(response);

            return list;
        }

        #endregion Tax Class SKU

        #region Tax Rule

        // Get Tax Rule list.
        public virtual TaxRuleListModel GetTaxRuleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = TaxClassEndpoint.TaxRuleList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TaxRuleListResponse response = GetResourceFromEndpoint<TaxRuleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TaxRuleListModel taxRuleListModel = new TaxRuleListModel { TaxRuleList = response?.TaxRuleList };
            taxRuleListModel.MapPagingDataFromResponse(response);

            return taxRuleListModel;
        }

        // Get Tax Rule by TaxRuleId.
        public virtual TaxRuleModel GetTaxRule(int taxRuleId)
        {
            string endpoint = TaxClassEndpoint.GetTaxRule(taxRuleId);

            ApiStatus status = new ApiStatus();
            TaxRuleResponse response = GetResourceFromEndpoint<TaxRuleResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.TaxRule;
        }

        // Add Tax Rule.
        public virtual TaxRuleModel AddTaxRule(TaxRuleModel taxRuleModel)
        {
            string endpoint = TaxClassEndpoint.AddTaxRule();

            ApiStatus status = new ApiStatus();
            TaxRuleResponse response = PostResourceToEndpoint<TaxRuleResponse>(endpoint, JsonConvert.SerializeObject(taxRuleModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.TaxRule;
        }

        // Update Tax Rule.
        public virtual TaxRuleModel UpdateTaxRule(TaxRuleModel taxRuleModel)
        {
            string endpoint = TaxClassEndpoint.UpdateTaxRule();

            ApiStatus status = new ApiStatus();
            TaxRuleResponse response = PutResourceToEndpoint<TaxRuleResponse>(endpoint, JsonConvert.SerializeObject(taxRuleModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.TaxRule;
        }

        // Delete existing Tax Rule.
        public virtual bool DeleteTaxRule(string taxRuleId)
        {
            string endpoint = TaxClassEndpoint.DeleteTaxRule();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = taxRuleId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion Tax Rule

        //Gives product detail list model by product list response
        public ProductDetailsListModel GetProductDetailsListModelByResponse(ProductListResponse response)
        {
            ProductDetailsListModel list = new ProductDetailsListModel
            {
                ProductDetailList = response?.ProductDetails,
                Locale = response?.Locale,
                AttributeColumnName = response?.AttrubuteColumnName,
                XmlDataList = response?.XmlDataList
            };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        #region Avalara 
        //Test the avalara connection.
        public virtual string TestAvalaraConnection(TaxPortalModel taxPortalModel)
        {
            string endpoint = TaxClassEndpoint.TestAvalaraConnection();

            ApiStatus status = new ApiStatus();
            StringResponse response = PostResourceToEndpoint<StringResponse>(endpoint, JsonConvert.SerializeObject(taxPortalModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Response;
        }
        #endregion
    }
}