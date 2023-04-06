using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Client
{
    public class QuoteClient : BaseClient, IQuoteClient
    {
        // Gets the list of Quotes.
        public virtual QuoteListModel GetQuoteList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = QuoteEndpoint.List();
            endpoint += BuildEndpointQueryString(null,filters, sorts, pageIndex, pageSize);

            ZnodeLogging.LogMessage($"QuoteClient.GetQuoteList : {endpoint}", "QuoteCreate", System.Diagnostics.TraceLevel.Info);

            //Get response
            ApiStatus status = new ApiStatus();
            QuoteListResponse response = GetResourceFromEndpoint<QuoteListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            QuoteListModel list = new QuoteListModel { Quotes = response?.QuoteList?.Quotes , PortalName = response?.QuoteList?.PortalName};
            if(list?.Quotes?.Count > 0)
            {
                list.MapPagingDataFromResponse(response);
            }

            return list;

        }

        // Create new quote.
        public virtual QuoteCreateModel Create(QuoteCreateModel quoteCreateModel)
        {
            //Get Endpoint.
            string endpoint = QuoteEndpoint.Create();

            //Get response
            ApiStatus status = new ApiStatus();
            CreateQuoteResponse response = PostResourceToEndpoint<CreateQuoteResponse>(endpoint, JsonConvert.SerializeObject(quoteCreateModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Quote;
        }


        //Get quote Receipt details.
        public virtual QuoteResponseModel GetQuoteReceipt(int quoteId)
        {
            string endpoint = QuoteEndpoint.GetQuoteReceipt(quoteId);

            ApiStatus status = new ApiStatus();
            QuoteResponse response = GetResourceFromEndpoint<QuoteResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.QuoteDetails;
        }

        // Get Quote details by Quote Id.
        public virtual QuoteResponseModel GetQuoteById(int omsQuoteId)
        {
            string endpoint = QuoteEndpoint.GetQuoteById(omsQuoteId);

            ApiStatus status = new ApiStatus();
            QuoteDetailResponse response = GetResourceFromEndpoint<QuoteDetailResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.QuoteDetail;
        }


        //  Convert quote To Order
        public virtual OrderModel ConvertQuoteToOrder(ConvertQuoteToOrderModel convertToOrderModel)
        {
            //Get Endpoint.
            string endpoint = QuoteEndpoint.ConvertQuoteToOrder();

            //Get response
            ApiStatus status = new ApiStatus();
            OrderResponse response = PostResourceToEndpoint<OrderResponse>(endpoint, JsonConvert.SerializeObject(convertToOrderModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        // Get Quote LineItems by QuoteId.
        public virtual List<QuoteLineItemModel> GetQuoteLineItemByQuoteId(int omsQuoteId)
        {
            string endpoint = QuoteEndpoint.GetQuoteLineItemByQuoteId(omsQuoteId);

            ApiStatus status = new ApiStatus();
            QuoteLineItemResponse response = GetResourceFromEndpoint<QuoteLineItemResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.QuoteLineItems;
        }

        // Update existing Quote.
        public virtual BooleanModel UpdateQuote(UpdateQuoteModel model)
        {
            //Get Endpoint.
            string endpoint = QuoteEndpoint.UpdateQuote();

            //Get response
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.booleanModel;
        }

        //Get Quote Total
        public virtual string GetQuoteTotal(string quoteNumber)
        {
            string endpoint = QuoteEndpoint.GetQuoteTotal(quoteNumber);

            ApiStatus status = new ApiStatus();
            StringResponse response = GetResourceFromEndpoint<StringResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.Response;
        }

    }
}
