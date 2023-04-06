using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAccountQuoteService
    {
        /// <summary>
        /// Get list for Account Quote.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Account Quote list.</param>
        /// <param name="filters">Filters to be applied on Account Quote list.</param>
        /// <param name="sorts">Sorting to be applied on Account Quote list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of Account Quote.</returns>
        AccountQuoteListModel GetAccountQuoteList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Account Quote on the basis of omsQuoteId.
        /// </summary>
        ///<param name="filters">Filters to be applied on Account Quote.</param>
        ///<param name="expands">Expands to be retrieved along with Account Quote.</param>
        /// <returns>Returns AccountQuoteModel.</returns>
        AccountQuoteModel GetAccountQuote(FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Create Account Quote.
        /// </summary>
        /// <param name="shoppingCartModel">shoppingCartModel to create account quote.</param>
        /// <returns>Returns created account quote.</returns>
        AccountQuoteModel Create(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Update Account Quote.
        /// </summary>
        /// <param name="accountQuoteModel">model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateQuoteStatus(QuoteStatusModel accountQuoteModel);

        /// <summary>
        /// Update Account Quote.
        /// </summary>
        /// <param name="omsOrderStateId">omsOrderStateId.</param>
        /// <param name="omsQuoteIds">omsQuoteIds.</param>
        /// <param name="ExceptStatus">ExceptStatus.</param>
        /// <param name="isUpdated">isUpdated.</param>
        /// <returns>Returns list of AccountQuoteModel</returns>
        IList<AccountQuoteModel> UpdateQuoteStatus(int omsOrderStateId, string omsQuoteIds, string ExceptStatus, out int isUpdated);

        /// <summary>
        /// Get all the approver's comments saved against the quote.
        /// </summary>
        /// <param name="quoteId">quoteId.</param>
        /// <returns>Returns list of QuoteApprovalModel</returns>
        List<QuoteApprovalModel> GetApproverComments(int quoteId);

        /// <summary>
        /// Method to check if the current user is an approver to any other user and has approvers itself.
        /// </summary>
        /// <param name="userId">Current User Id.</param>
        /// <returns>Returns model containing data about whether it is an approver to the given user id or has its own approvers.</returns>
        ApproverDetailsModel UserApproverDetails(int userId);

        /// <summary>
        /// Update Quote Line Item Quantity.
        /// </summary>
        /// <param name="accountQuoteLineItemModel">model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateQuoteLineItemQuantity(AccountQuoteLineItemModel accountQuoteLineItemModel);

        /// <summary>
        /// Update multiple quote line item quantities.
        /// </summary>
        /// <param name="accountQuoteLineItemModel">Model containing multiple line items.</param>
        /// <returns>Returns a status model showing whose quantities failed to update.</returns>
        QuoteLineItemStatusListModel UpdateQuoteLineItemQuantities(List<AccountQuoteLineItemModel> accountQuoteLineItemModel);

        /// <summary>
        /// Delete Account Delete Quote Line Item.
        /// </summary>
        /// <param name="omsQuoteLineItemId">omsQuoteLineItemId to delete.</param>
        /// <param name="omsParentQuoteLineItemId">omsParentQuoteLineItemId to delete.</param>
        /// <param name="omsQuoteId">omsQuoteId to delete.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteQuoteLineItem(int omsQuoteLineItemId, int omsParentQuoteLineItemId, int omsQuoteId);
      
        /// <summary>
        /// Update Quote Shipping Address.
        /// </summary>
        /// <param name="updateQuoteShippingAddressModel">update quote shipping address</param>
        /// <returns>>Returns true if model updated successfully else return false.</returns>
        bool UpdateQuoteShippingAddress(UpdateQuoteShippingAddressModel updateQuoteShippingAddressModel);


        /// <summary>
        /// Get the list of user approvers with there level and order status details.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of user approvers.</returns>
        UserApproverListModel GetUserApproverList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Method to check if the current user is tha final approver for the quote.
        /// </summary>
        /// <param name="quoteId">Quote Id.</param>
        /// <returns>Returns true if the user is last approver else returns false.</returns>
        bool IsLastApprover(int quoteId);

        /// <summary>
        /// Get billing account number.
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns>Returns billing account number.</returns>
        string GetUsersAdditionalAttributes(int userId);

        /// <summary>
        /// Get pending payments and pending orders count for showing account menus
        /// </summary>
        /// <param name="filters">filters.</param>
        /// <returns>Returns model containing data about pending payments and pending orders count.</returns>
        UserDashboardPendingOrdersModel GetUserDashboardPendingOrderDetailsCount(FilterCollection filters);

        #region Template
        /// <summary>
        /// Create template.
        /// </summary>
        /// <param name="shoppingCartModel">Shopping cart model to create template.</param>
        /// <returns>Returns Account template model.</returns>
        AccountTemplateModel CreateTemplate(AccountTemplateModel shoppingCartModel);

        /// <summary>
        /// Edit Saved Cart.
        /// </summary>
        /// <param name="shoppingCartModel">Shopping cart model to Edit Save Cart Items.</param>
        /// <returns>Returns Status</returns>
        bool EditSaveCart(AccountTemplateModel shoppingCartModel);

        /// <summary>
        /// Get Template Data
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <returns>Account Template Model</returns>
        AccountTemplateModel GetTemplate(int templateId, NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// Get the account template.
        /// </summary>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters</param>
        /// <returns>Returns accountTemplateModel.</returns>
        AccountTemplateModel GetAccountTemplate(int templateId, NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// Add Product To Cart For SaveCart
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// <param name="userId">userId</param>
        /// <returns>Status</returns>
        bool AddProductToCartForSaveCart(int omsTemplateId, int userId, int portalId);

        /// <summary>
        /// Edit Save Cart Name
        /// </summary>
        /// <param name="templateName">templateName</param>
        /// <param name="templateid">templateid</param>
        /// <returns>Status</returns>
        bool EditSaveCartName(string templateName, int templateId);

        /// <summary>
        /// Get list template.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with template list.</param>
        /// <param name="filters">Filters to be applied on template list.</param>
        /// <param name="sorts">Sorting to be applied on template list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of template.</returns>
        AccountTemplateListModel GetTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete template.
        /// </summary>
        /// <param name="omsTemplateIds">OMS Template Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteTemplate(ParameterModel omsTemplateIds);

        /// <summary>
        /// Delete cart items.
        /// </summary>
        /// <param name="model">Model containing omsTemplateId and multiple OmsTemplateLineItemIds.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteCartItem(AccountTemplateModel accountTemplateModel);

        /// <summary>
        /// Send updated quote status to user
        /// </summary>
        /// <param name="status">Quoute status</param>
        /// <param name="portalId">Store ID</param>
        /// <param name="accountQuoteList">Quote details</param>
        /// <param name="localeId">Language ID</param>
        void SendQuoteStatusEmailToUser(string status, int portalId, List<AccountQuoteModel> accountQuoteList, int localeId);
        #endregion

        #region Save for Later

        /// <summary>
        /// get saved cart for later.
        /// </summary>
        /// <param name="userId">LoggedIn userId</param>
        /// <param name="templateType">Template type</param>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <returns>AccountTemplateModel</returns>
        AccountTemplateModel GetCartForLater(int userId, string templateType, NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// Delete all line items for later
        /// </summary>
        /// <param name="omsTemplateId">omsTemplateId</param>
        /// <param name="isFromSavedCart">isFromSavedCart</param>
        /// <returns>Bool value True/False</returns>
        bool DeleteAllCartitemsForLater(int omsTemplateId, bool isFromSavedCart = false);

        #endregion
    }
}
