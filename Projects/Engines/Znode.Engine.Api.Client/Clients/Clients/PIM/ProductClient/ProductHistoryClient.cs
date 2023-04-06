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
    //Product History Client.
    public class ProductHistoryClient : BaseClient, IProductHistoryClient
    {
        #region Public Methods
        //Get product history list.
        public virtual ProductHistoryListModel GetProductHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetProductHistoryList(expands, filters, sorts, null, null);

        //Get product history list.
        public virtual ProductHistoryListModel GetProductHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ProductHistoryEndpoint.GetProductHistoryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductHistoryListResponse response = GetResourceFromEndpoint<ProductHistoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductHistoryListModel list = new ProductHistoryListModel { ProductHistoryList = response?.ProductHistoryList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get product history  by ID.
        public virtual ProductHistoryModel GetProductHistory(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = ProductHistoryEndpoint.GetProductHistory(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            ProductHistoryResponse response = GetResourceFromEndpoint<ProductHistoryResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ProductHistory;
        }

        //Create product history. 
        public virtual ProductHistoryModel CreateProductHistoryModel(ProductHistoryModel productHistoryModel)
        {
            //Get Endpoint.
            string endpoint = ProductHistoryEndpoint.CreateProductHistory();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ProductHistoryResponse response = PostResourceToEndpoint<ProductHistoryResponse>(endpoint, JsonConvert.SerializeObject(productHistoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ProductHistory;
        }

        //Update product history.
        public virtual ProductHistoryModel UpdateProductHistoryModel(ProductHistoryModel productHistoryModel)
        {
            string endpoint = ProductHistoryEndpoint.UpdateProductHistory();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ProductHistoryResponse response = PutResourceToEndpoint<ProductHistoryResponse>(endpoint, JsonConvert.SerializeObject(productHistoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ProductHistory;
        }

        //Delete product history.
        public virtual bool DeleteProductHistoryModel(int id)
        {
            //Get Endpoint.
            string endpoint = ProductHistoryEndpoint.DeleteProductHistory(id);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<ProductHistoryResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return deleted;
        } 
        #endregion
    }
}
