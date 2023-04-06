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
    public class AccountQuoteClient : BaseClient, IAccountQuoteClient
    {
        //Get Account Quote List.
        public virtual AccountQuoteListModel GetAccountQuoteList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountQuoteEndpoint.GetAccountQuoteList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AccountQuoteListResponse response = GetResourceFromEndpoint<AccountQuoteListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AccountQuoteListModel list = new AccountQuoteListModel();
            if (HelperUtility.IsNotNull(response?.AccountQuotes))
            {
                list.AccountQuotes = response.AccountQuotes.AccountQuotes;
                list.CustomerName = response.AccountQuotes.CustomerName;
                list.AccountName = response.AccountQuotes.AccountName;
                list.HasParentAccounts = response.AccountQuotes.HasParentAccounts;
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Account Quote Details by omsQuoteId.
        public virtual AccountQuoteModel GetAccountQuote(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = AccountQuoteEndpoint.GetAccountQuote();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = GetResourceFromEndpoint<AccountQuoteResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.AccountQuote;
        }

        //Create Account Quote.
        public virtual AccountQuoteModel Create(ShoppingCartModel shoppingCartModel)
        {
            string endpoint = AccountQuoteEndpoint.Create();

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = PostResourceToEndpoint<AccountQuoteResponse>(endpoint, JsonConvert.SerializeObject(shoppingCartModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountQuote;
        }

        //Update Account Quote.
        public virtual QuoteStatusModel UpdateQuoteStatus(QuoteStatusModel accountQuoteModel)
        {
            string endpoint = AccountQuoteEndpoint.UpdateQuoteStatus();

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = PutResourceToEndpoint<AccountQuoteResponse>(endpoint, JsonConvert.SerializeObject(accountQuoteModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.QuoteStatus;
        }

        //Method to check if the current user is an approver to any other user and has approvers itself.
        public virtual ApproverDetailsModel UserApproverDetails(int userId)
        {
            //Get Endpoint.
            string endpoint = AccountQuoteEndpoint.UserApproverDetails(userId);

            //Get response.
            ApiStatus status = new ApiStatus();
            ApproverDetailsResponse response = GetResourceFromEndpoint<ApproverDetailsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ApproverDetails;
        }

        //Get billing account number.
        public virtual string GetBillingAccountNumber(int userId)
        {
            string endpoint = AccountQuoteEndpoint.GetBillingAccountNumber(userId);
            ApiStatus status = new ApiStatus();
            StringResponse response = GetResourceFromEndpoint<StringResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.Response;
        }

        //Method to check if the current user is tha final approver for the quote.
        public virtual bool IsLastApprover(int quoteId)
        {
            //Get Endpoint.
            string endpoint = AccountQuoteEndpoint.IsLastApprover(quoteId);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Update Account Quote Line Item Quantity.
        public virtual AccountQuoteLineItemModel UpdateQuoteLineItemQuantity(AccountQuoteLineItemModel accountQuoteLineItemModel)
        {
            string endpoint = AccountQuoteEndpoint.UpdateQuoteLineItemQuantity();

            ApiStatus status = new ApiStatus();
            AccountQuoteLineItemResponse response = PutResourceToEndpoint<AccountQuoteLineItemResponse>(endpoint, JsonConvert.SerializeObject(accountQuoteLineItemModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AccountQuoteLineItem;
        }

        //Delete the Account Quote Line Item by omsQuoteLineItemId.
        public virtual bool DeleteQuoteLineItem(int omsQuoteLineItemId, int omsQuoteId)
        {
            string endpoint = AccountQuoteEndpoint.DeleteQuoteLineItem(omsQuoteLineItemId, omsQuoteId);
            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return deleted;
        }

        //Get the user approver list.
        public virtual UserApproverListModel GetUserApproverList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountQuoteEndpoint.GetUserApproverList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            UserApproverListResponse response = GetResourceFromEndpoint<UserApproverListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            UserApproverListModel userApproverList = new UserApproverListModel { UserApprovers = response?.UserApproverList?.UserApprovers };
            userApproverList.MapPagingDataFromResponse(response);

            return userApproverList;
        }

        //Get pending payments and pending orders count for showing account menus
        public virtual UserDashboardPendingOrdersModel GetUserDashboardPendingOrderDetailsCount(FilterCollection filters)
        {
            //Get Endpoint.
            string endpoint = AccountQuoteEndpoint.UserDashboardPendingOrderDetailsCount();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserDashboardPendingOrdersResponse response = GetResourceFromEndpoint<UserDashboardPendingOrdersResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PendingOrderDetailsCount;
        }

        #region Template
        //Create account template.
        public virtual AccountTemplateModel CreateTemplate(AccountTemplateModel shoppingCartModel)
        {
            string endpoint = AccountQuoteEndpoint.CreateTemplate();

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = PostResourceToEndpoint<AccountQuoteResponse>(endpoint, JsonConvert.SerializeObject(shoppingCartModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountTemplate;
        }

        //Get template List.
        public virtual AccountTemplateListModel GetTemplateList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountQuoteEndpoint.GetTemplateList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AccountTemplateListResponse response = GetResourceFromEndpoint<AccountTemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AccountTemplateListModel list = new AccountTemplateListModel { AccountTemplates = response?.AccountTemplates.AccountTemplates };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Delete cart item list.
        public virtual bool DeleteCartItem(AccountTemplateModel accountTemplateModel)
        {
            string endpoint = AccountQuoteEndpoint.DeleteCartItem();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(accountTemplateModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Delete template.
        public virtual bool DeleteTemplate(ParameterModel omsTemplateId)
        {
            string endpoint = AccountQuoteEndpoint.DeleteTemplate();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(omsTemplateId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get Account template details by omsTemplateId.
        public virtual AccountTemplateModel GetAccountTemplate(int omsTemplateId, ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = AccountQuoteEndpoint.GetAccountTemplate(omsTemplateId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AccountQuoteResponse response = GetResourceFromEndpoint<AccountQuoteResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.AccountTemplate;
        }

        // Create new order.
        public virtual OrderModel ConvertToOrder(AccountQuoteModel accountQuoteModel)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.ConvertToOrder();

            //Get response
            ApiStatus status = new ApiStatus();
            OrderResponse response = PostResourceToEndpoint<OrderResponse>(endpoint, JsonConvert.SerializeObject(accountQuoteModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }
        #endregion
    }
}
