using System.Collections.Generic;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface IQuoteAgent
    {
        /// <summary>
        /// Create Quote
        /// </summary>
        /// <param name="quoteCreateViewModel"></param>
        /// <returns>QuoteCreateViewModel</returns>
        QuoteCreateViewModel Create(QuoteCreateViewModel quoteCreateViewModel);

        /// <summary>
        /// Get Quote Receipt by quoteId
        /// </summary>
        /// <param name="omsOrderId"></param>
        /// <returns>QuoteResponseViewModel</returns>
        QuoteResponseViewModel GetQuoteReceipt(int omsQuoteId);

        /// <summary>
        /// Get Quote List
        /// </summary>
        /// <returns>List of Quotes</returns>
        List<QuoteViewModel> GetQuoteList();

        /// <summary>
        /// Get Quote Details
        /// </summary>
        /// <param name="omsQuoteId"></param>
        /// <returns></returns>
        QuoteResponseViewModel GetQuote(int omsQuoteId);

        /// <summary>
        /// Convert quote to order.
        /// </summary>
        /// <param name="convertToOrderModel"></param>
        /// <returns>OrdersViewModel</returns>
        OrdersViewModel ConvertQuoteToOrder(ConvertQuoteToOrderViewModel convertToOrderModel);


        /// <summary>
        /// Get Paged Quote List
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sortCollection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordPerPage"></param>
        /// <returns>Quote List</returns>
        QuoteListViewModel GetQuoteList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Set payment details for paypal payment process.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="paymentSettingId"></param>
        /// <param name="paymentCode"></param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        ConvertQuoteToOrderViewModel SetPayPalToken(string token, int paymentSettingId, string paymentCode, int quoteId);

        /// <summary>
        /// Set payment details for amazon payment process.
        /// </summary>
        /// <param name="amazonOrderReferenceId"></param>
        /// <param name="paymentType"></param>
        /// <param name="paymentSettingId"></param>
        /// <param name="paymentCode"></param>
        /// <param name="quoteId"></param>
        /// <param name="captureId"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        ConvertQuoteToOrderViewModel SetAmazonPayDetails(string amazonOrderReferenceId, string paymentType, int paymentSettingId, string paymentCode, int quoteId, string captureId, string orderNumber );
    }
}
