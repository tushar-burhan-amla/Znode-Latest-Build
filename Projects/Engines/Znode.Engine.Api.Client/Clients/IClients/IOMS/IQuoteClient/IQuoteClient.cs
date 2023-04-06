using System.Collections.Generic;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IQuoteClient : IBaseClient
    {
        /// <summary>
        /// Get the list of Quotes.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>QuoteListModel.</returns>
        QuoteListModel GetQuoteList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create Quote
        /// </summary>
        /// <param name="quoteCreateModel"></param>
        /// <returns>quoteCreateModel</returns>
        QuoteCreateModel Create(QuoteCreateModel quoteCreateModel);

        /// <summary>
        /// Get Quote Receipt Details
        /// </summary>
        /// <param name="quoteId">quoteId</param>
        /// <returns>QuoteResponseModel</returns>
        QuoteResponseModel GetQuoteReceipt(int quoteId);

        /// <summary>
        /// Get Quote details by Quote id.
        /// </summary>
        /// <param name="omsQuoteId">Quote id</param>
        /// <returns>QuoteResponseModel</returns>
        QuoteResponseModel GetQuoteById(int omsQuoteId);

        /// <summary>
        /// Convert Quote To Order 
        /// </summary>
        /// <param name="convertToOrderModel"></param>
        /// <returns></returns>
        OrderModel ConvertQuoteToOrder(ConvertQuoteToOrderModel convertToOrderModel);

        /// <summary>
        /// Get Quote LineItems by QuoteId.
        /// </summary>
        /// <param name="omsQuoteId">int</param>
        /// <returns>List<QuoteLineItemModel></returns>
        List<QuoteLineItemModel> GetQuoteLineItemByQuoteId(int omsQuoteId);

        /// <summary>
        /// Update existing Quote.
        /// </summary>
        /// <param name="model">UpdateQuoteModel</param>
        /// <returns>UpdateQuoteModel</returns>
        BooleanModel UpdateQuote(UpdateQuoteModel model);

        /// <summary>
        /// Get Quote Total
        /// </summary>
        /// <param name="quoteNumber">quote Number</param>
        /// <returns>Quote Total</returns>
        string GetQuoteTotal(string quoteNumber);
    }
}
