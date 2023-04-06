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
    public class CustomerReviewClient : BaseClient, ICustomerReviewClient
    {
        // Gets the list of Customer Reviews.
        public virtual CustomerReviewListModel GetCustomerReviewList(string localeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CustomerReviewEndpoint.GetCustomerReviewList(localeId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CustomerReviewListResponse response = GetResourceFromEndpoint<CustomerReviewListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CustomerReviewListModel list = new CustomerReviewListModel { CustomerReviewList = response?.CustomerReviewList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Customer Review on the basis of cutomer review id.
        public virtual CustomerReviewModel GetCustomerReview(int customerReviewId, string localeId)
        {
            string endpoint = CustomerReviewEndpoint.GetCustomerReview(customerReviewId, localeId);

            ApiStatus status = new ApiStatus();
            CustomerReviewResponse response = GetResourceFromEndpoint<CustomerReviewResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CustomerReview;
        }

        //Update Customer Review.
        public virtual CustomerReviewModel UpdateCustomerReview(CustomerReviewModel customerReviewModel)
        {
            string endpoint = CustomerReviewEndpoint.UpdateCustomerReview();

            ApiStatus status = new ApiStatus();
            CustomerReviewResponse response = PutResourceToEndpoint<CustomerReviewResponse>(endpoint, JsonConvert.SerializeObject(customerReviewModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CustomerReview;
        }

        //Delete Customer Review.
        public virtual bool DeleteCustomerReview(ParameterModel customerReviewId)
        {
            string endpoint = CustomerReviewEndpoint.DeleteCustomerReview();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(customerReviewId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Change customer review status.
        public virtual bool BulkStatusChange(ParameterModel customerReviewId, string statusId)
        {
            string endpoint = CustomerReviewEndpoint.BulkStatusChange(statusId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(customerReviewId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //create Customer Review.
        public virtual CustomerReviewModel CreateCustomerReview(CustomerReviewModel customerReviewModel)
        {
            string endpoint = CustomerReviewEndpoint.CreateCustomerReview();

            ApiStatus status = new ApiStatus();
            CustomerReviewResponse response = PostResourceToEndpoint<CustomerReviewResponse>(endpoint, JsonConvert.SerializeObject(customerReviewModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CustomerReview;
        }
    }
}
