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
    public class ProductFeedClient : BaseClient, IProductFeedClient
    {
        //Creates Google Site Map for generating XML file
        public virtual ProductFeedModel CreateProductFeed(ProductFeedModel productFeedModel)
        {
            string endpoint = ProductFeedEndpoint.Create();

            ApiStatus status = new ApiStatus();
            ProductFeedResponse response = PostResourceToEndpoint<ProductFeedResponse>(endpoint, JsonConvert.SerializeObject(productFeedModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ProductFeed;
        }

        // Gets the list of product feed.
        public virtual ProductFeedListModel GetProductFeedList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ProductFeedEndpoint.GetProductFeedList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ProductFeedListResponse response = GetResourceFromEndpoint<ProductFeedListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductFeedListModel list = new ProductFeedListModel { ProductFeeds = response?.ProductFeeds };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get product feed by id.
        public virtual ProductFeedModel GetProductFeed(int productFeedId, ExpandCollection expands)
        {
            //Create Endpoint to get the list of all product feed.
            string endpoint = ProductFeedEndpoint.GetProductFeedById(productFeedId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            ProductFeedResponse response = GetResourceFromEndpoint<ProductFeedResponse>(endpoint, status);

            //Check the status of response of product feed list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ProductFeed;
        }

        //Delete product feed.
        public virtual bool DeleteProductFeed(ParameterModel productFeedModel)
        {
            //Create Endpoint to create product feed.
            string endpoint = ProductFeedEndpoint.DeleteProductFeed();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(productFeedModel), status);

            //Check the status of response of product feeds.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update product feed.
        public virtual ProductFeedModel UpdateProductFeed(ProductFeedModel model)
        {
            //Get Endpoint
            string endpoint = ProductFeedEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ProductFeedResponse response = PostResourceToEndpoint<ProductFeedResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ProductFeed;
        }

        //Get product feed master details.
        public virtual ProductFeedModel GetProductFeedMasterDetails()
        {
            //Get Endpoint.
            string endpoint = ProductFeedEndpoint.GetProductFeedMasterDetails();

            //Get response
            ApiStatus status = new ApiStatus();
            ProductFeedResponse response = GetResourceFromEndpoint<ProductFeedResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ProductFeed;
        }

        //Check if file name combination is unique.
        public virtual bool FileNameCombinationAlreadyExist(int localeId, string fileName)
        {
            //Get Endpoint
            string endpoint = ProductFeedEndpoint.FileNameCombinationAlreadyExist(localeId, fileName);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }
}
