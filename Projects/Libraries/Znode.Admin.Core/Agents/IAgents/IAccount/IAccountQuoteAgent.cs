using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IAccountQuoteAgent
    {
        /// <summary>
        /// Get Account Quote List.
        /// </summary>
        /// <param name="filters">Filter Collection Parameter</param>
        /// <param name="sortCollection">Sort Collection Parameter</param>
        /// <param name="pageIndex">Start Index of Page</param>
        /// <param name="recordPerPage">Records Per Page.</param>
        /// <param name="accountId">accountId</param>
        /// <param name="userId">userId</param>
        /// <returns>Return Account Quote List in AccountQuoteListViewModel model.</returns>
        AccountQuoteListViewModel GetAccountQuoteList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int accountId = 0, int userId = 0, bool isPendingPayment = false);

        /// <summary>
        /// Gets an Account Quote by omsQuoteId.
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId to get Account Quote detail.</param>
        /// <param name="updatePageType">Current updated page is redirect to list page on the basis of updatePageType</param>
        /// <returns>Account Quote Details.</returns>
        AccountQuoteViewModel GetAccountQuote(int omsQuoteId, string updatePageType);

        /// <summary>
        /// Update the Account Quote Details.
        /// </summary>
        /// <param name="accountQuoteViewModel">Model of Type AccountQuoteViewModel.</param>
        /// <returns>Return AccountQuoteViewModel.</returns>
        bool UpdateQuoteStatus(AccountQuoteViewModel accountQuoteViewModel);

        /// <summary>
        /// Update quote status.
        /// </summary>
        /// <param name="quoteId">Quote ids to update status.</param>
        /// <param name="status">Status to change.</param>
        /// <param name="orderStatus">Order status to be updated.</param>
        /// <param name="message">set error message.</param>
        /// <param name="isPendingPaymentStatus">Pending Payment Status</param>
        /// <returns>Returns true if quote status updates successfully else false.</returns>
        bool UpdateQuoteStatus(string quoteId, int status, bool isPendingPaymentStatus, string orderStatus, ref string message);

        /// <summary>
        /// Convert quote to order.
        /// </summary>
        /// <param name="omsQuoteId">OMSQuoteId</param>
        /// <returns>Returns CreateOrderViewModel</returns>
        CreateOrderViewModel ConvertToOrder(int omsQuoteId);

        /// <summary>
        /// Convert To Order
        /// </summary>
        /// <param name="accountQuoteViewModel">accountQuoteViewModel</param>
        /// <returns>OrderViewModel</returns>
        OrderViewModel ConvertToOrder(AccountQuoteViewModel accountQuoteViewModel);
    }
}
