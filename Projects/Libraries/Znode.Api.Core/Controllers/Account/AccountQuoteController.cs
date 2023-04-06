using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class AccountQuoteController : BaseController
    {
        #region Private readonly Variables
        private readonly IAccountQuoteService _service;
        private readonly IAccountQuoteCache _cache;
        #endregion

        #region Public Constructor
        public AccountQuoteController(IAccountQuoteService service)
        {
            _service = service;
            _cache = new AccountQuoteCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get account quote list.
        /// </summary>        
        /// <returns>Returns account quote list.</returns>
        [ResponseType(typeof(AccountQuoteListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountQuoteList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAccountQuoteList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountQuoteListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Gets an account quote.
        /// </summary>
        /// <returns>return Company Account.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountQuote()
        {
            HttpResponseMessage response;
            try
            {
                //Get account quote details by omsQuoteId.
                string data = _cache.GetAccountQuote(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountQuoteResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create account quote.
        /// </summary>
        /// <param name="shoppingCartModel">model with account quote details.</param>
        /// <returns>Returns created acccount quote.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] ShoppingCartModel shoppingCartModel)
        {
            HttpResponseMessage response;
            try
            {
                AccountQuoteModel accountQuoteModel = _service.Create(shoppingCartModel);

                if (HelperUtility.IsNotNull(accountQuoteModel))
                {
                    response = CreateCreatedResponse(new AccountQuoteResponse { AccountQuote = accountQuoteModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(accountQuoteModel.OmsQuoteId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update account quote.
        /// </summary>
        /// <param name="accountQuoteModel">model contains details to be updated.</param>
        /// <returns>Returns updated Account quote.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateQuoteStatus([FromBody] QuoteStatusModel accountQuoteModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UpdateQuoteStatus(accountQuoteModel) ? CreateOKResponse(new AccountQuoteResponse { QuoteStatus = accountQuoteModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Quote Line Item Quantity.
        /// </summary>
        /// <param name="accountQuoteLineItemModel">model contains details to be updated.</param>
        /// <returns>Returns updated quote line items status.</returns>
        [ResponseType(typeof(OrderLineItemStatusResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateQuoteLineItemQuantity([FromBody] AccountQuoteLineItemModel accountQuoteLineItemModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UpdateQuoteLineItemQuantity(accountQuoteLineItemModel) ? CreateOKResponse(new AccountQuoteLineItemResponse { AccountQuoteLineItem = accountQuoteLineItemModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteLineItemResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteLineItemResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }



        /// <summary>
        /// Update multiple Quote Line Item Quantities.
        /// </summary>
        /// <param name="accountQuoteLineItemModel">model contains details to be updated.</param>
        /// <returns>Returns updated Account quote line item.</returns>
        [ResponseType(typeof(AccountQuoteLineItemListResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateQuoteLineItemQuantities([FromBody] List<AccountQuoteLineItemModel> accountQuoteLineItemModel)
        {
            HttpResponseMessage response;
            try
            {
                QuoteLineItemStatusListModel itemList = _service.UpdateQuoteLineItemQuantities(accountQuoteLineItemModel);
                //Update order line item quantities.
                response = CreateOKResponse(new QuoteLineItemStatusResponse { QuoteLineItemStatusList = itemList });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new QuoteLineItemStatusResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuoteLineItemStatusResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Quote Line Item.
        /// </summary>
        /// <param name="omsQuoteLineItemId">OmsQuoteLineItemId to delete.</param>
        /// <param name="omsParentQuoteLineItemId">omsParentQuoteLineItemId to delete.</param>
        /// <param name="omsQuoteId">omsQuoteId to delete record.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpDelete]
        public virtual HttpResponseMessage DeleteQuoteLineItem(int omsQuoteLineItemId, int omsParentQuoteLineItemId, int omsQuoteId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteQuoteLineItem(omsQuoteLineItemId, omsParentQuoteLineItemId, omsQuoteId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get user approver list.
        /// </summary>        
        /// <returns>Returns user approver list.</returns>
        [ResponseType(typeof(UserApproverListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUserApproverList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUserApproverList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserApproverListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UserApproverListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserApproverListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Method to check if the current user is an approver to any other user and has approvers itself.
        /// </summary>
        /// <param name="userId">Current User Id.</param>
        /// <returns>Returns model containing data about whether it is an approver to the given user id or has its own approvers.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage UserApproverDetails(int userId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.UserApproverDetails(userId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ApproverDetailsResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ApproverDetailsResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update Quote Shipping Address.
        /// </summary>
        /// <param name="updateQuoteShippingAddressModel">model contains details to be updated.</param>
        /// <returns>Returns updated Account quote line item.</returns>
        [ResponseType(typeof(UpdateQuoteShippingAddressResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateQuoteShippingAddress([FromBody] UpdateQuoteShippingAddressModel updateQuoteShippingAddressModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UpdateQuoteShippingAddress(updateQuoteShippingAddressModel) ? CreateOKResponse(new UpdateQuoteShippingAddressResponse { UpdateQuoteShippingAddressModel = updateQuoteShippingAddressModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UpdateQuoteShippingAddressResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UpdateQuoteShippingAddressResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///  Method to check if the current user is tha final approver for the quote.
        /// </summary>
        /// <param name="quoteId">QuoteId.</param>
        /// <returns>Returns true if the user is last approver else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsLastApprover(int quoteId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsLastApprover(quoteId) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get pending payments and pending orders count for showing account menus
        /// </summary>
        /// <returns>returns pending payments count and pending orders count.</returns>
        [ResponseType(typeof(UserDashboardPendingOrdersResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUserDashboardPendingOrderDetailsCount()
        {
            HttpResponseMessage response;
            try
            {
                ///Get pending payments and pending orders count for showing account menus
                string data = _cache.GetUserDashboardPendingOrderDetailsCount(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserDashboardPendingOrdersResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new UserDashboardPendingOrdersResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new UserDashboardPendingOrdersResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserDashboardPendingOrdersResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #region Template
        /// <summary>
        /// Create template.
        /// </summary>
        /// <param name="shoppingCartModel">Shopping cart model to create template.</param>
        /// <returns>Returns Account template model.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateTemplate([FromBody] AccountTemplateModel shoppingCartModel)
        {
            HttpResponseMessage response;
            try
            {
                AccountTemplateModel accountTemplateModel = _service.CreateTemplate(shoppingCartModel);

                if (HelperUtility.IsNotNull(accountTemplateModel))
                {
                    response = CreateCreatedResponse(new AccountQuoteResponse { AccountTemplate = accountTemplateModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(accountTemplateModel.OmsTemplateId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get account quote list.
        /// </summary>        
        /// <returns>Returns template list.</returns>
        [ResponseType(typeof(AccountTemplateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTemplateList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTemplateList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountTemplateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountTemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete template.
        /// </summary>
        /// <param name="omsTemplateId">OMS Template Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteTemplate([FromBody] ParameterModel omsTemplateId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteTemplate(omsTemplateId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete cart item.
        /// </summary>
        /// <param name="accountTemplateModel">Account Template Model</param>
        /// <returns>if deleted successfully returns true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteCartItem([FromBody] AccountTemplateModel accountTemplateModel)
        {
            HttpResponseMessage response;

            try
            {
                response = _service.DeleteCartItem(accountTemplateModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Gets an account template.
        /// </summary>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <returns>return account template model.</returns>
        [ResponseType(typeof(AccountQuoteResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountTemplate(int omsTemplateId)
        {
            HttpResponseMessage response;
            try
            {
                //Gets an account template.
                string data = _cache.GetAccountTemplate(omsTemplateId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountQuoteResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountQuoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get billing account number.
        /// </summary>
        /// <returns>Returns billing account number.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBillingAccountNumber(int userId)
        {
            HttpResponseMessage response;
            try
            {
                string billingAccountNumber = _service.GetUsersAdditionalAttributes(userId);
                response = CreateOKResponse(new StringResponse { Response = billingAccountNumber });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        
        #endregion
        #endregion
    }
}
