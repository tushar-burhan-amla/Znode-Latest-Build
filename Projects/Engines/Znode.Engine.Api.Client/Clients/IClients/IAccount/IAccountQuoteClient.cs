using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IAccountQuoteClient : IBaseClient
    {
        /// <summary>
        /// Get Account Quote List.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns account quote list.</returns>
        AccountQuoteListModel GetAccountQuoteList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Account Quote by omsQuoteId.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters contains omsQuoteId.</param>
        /// <returns>Return Account Quote Details in AccountQuoteModel Format.</returns>
        AccountQuoteModel GetAccountQuote(ExpandCollection expands, FilterCollection filters);
        /// <summary>
        /// Create Account Quote.
        /// </summary>
        /// <param name="shoppingCartModel">model quote details to be created.</param>
        /// <returns>AccountQuoteModel</returns>
        AccountQuoteModel Create(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Update Account Quote. 
        /// </summary>
        /// <param name="accountQuoteModel">model quote details to be updated.</param>
        /// <returns>Returns true if updated successfully.</returns>
        QuoteStatusModel UpdateQuoteStatus(QuoteStatusModel accountQuoteModel);

        /// <summary>
        /// Method to check if the current user is an approver to any other user and has approvers itself.
        /// </summary>
        /// <param name="userId">Current UserId.</param>
        /// <returns>Returns model containing data about whether the user is an approver to someone and has approvers itself.</returns>
        ApproverDetailsModel UserApproverDetails(int userId);

        /// <summary>
        /// Method to check if the current user is tha final approver for the quote.
        /// </summary>
        /// <param name="quoteId">Quote Id.</param>
        /// <returns>Returns true if the user is last approver else returns false.</returns>
        bool IsLastApprover(int quoteId);

        /// <summary>
        /// Update Account Quote Line Item Quantity.
        /// </summary>
        /// <param name="accountQuoteLineItemModel">accountQuoteLineItemModel contains data to update.</param>
        /// <returns>accountQuoteLineItemModel</returns>
        AccountQuoteLineItemModel UpdateQuoteLineItemQuantity(AccountQuoteLineItemModel accountQuoteLineItemModel);

        /// <summary>
        /// Delete the Account Quote Line Item by omsQuoteLineItemId.
        /// </summary>
        /// <param name="omsQuoteLineItemId">omsQuoteLineItemId to delete record.</param>
        /// <param name="omsQuoteId">omsQuoteId to delete record.</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        bool DeleteQuoteLineItem(int omsQuoteLineItemId, int omsQuoteId);

        /// <summary>
        /// Get the list of user approvers with there level and order status details.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of user approvers.</returns>
        UserApproverListModel GetUserApproverList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get billing account number.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>Returns billing account number.</returns>
        string GetBillingAccountNumber(int userId);
       
        #region Template
        /// <summary>
        /// Create template.
        /// </summary>
        /// <param name="shoppingCartModel">ShoppingCartModel.</param>
        /// <returns>Returns AccountTemplateModel.</returns>
        AccountTemplateModel CreateTemplate(AccountTemplateModel shoppingCartModel);

        /// <summary>
        /// Get template List.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns template list.</returns>
        AccountTemplateListModel GetTemplateList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete template.
        /// </summary>
        /// <param name="omsTemplateId">OMS Template Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteTemplate(ParameterModel omsTemplateId);

        /// <summary>
        /// Delete cart items.
        /// </summary>
        /// <param name="model">Model containing omsTemplateId and multiple omsTemplateLineItemIds.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteCartItem(AccountTemplateModel accountTemplateModel);

        /// <summary>
        /// Get account template model.
        /// </summary>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <param name="expands">Expands</param>
        /// <param name="filters">filters</param>
        /// <returns>Returns AccountTemplateModel</returns>
        AccountTemplateModel GetAccountTemplate(int omsTemplateId, ExpandCollection expands, FilterCollection filters = null);
        #endregion

        /// <summary>
        /// Convert To Order
        /// </summary>
        /// <param name="accountQuoteModel">accountQuoteModel</param>
        /// <returns>OrderModel</returns>
        OrderModel ConvertToOrder(AccountQuoteModel accountQuoteModel);
    }
}
