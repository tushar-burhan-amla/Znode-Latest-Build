using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IQuoteService
    {
        /// <summary>
        /// Get Quote List
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>QuoteListModel</returns>
        QuoteListModel GetQuoteList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        
        /// <summary>
        /// Creates a Quote request
        /// </summary>
        /// <param name="quoteCreateModel">QuoteCreateModel</param>
        /// <returns>QuoteCreateModel</returns>
        QuoteCreateModel Create(QuoteCreateModel quoteCreateModel);

        /// <summary>
        /// Get Quote Receipt Details
        /// </summary>
        /// <param name="quoteId">quoteId</param>
        /// <returns>QuoteResponseModel</returns>
        QuoteResponseModel GetQuoteReceipt(int quoteId);

        /// <summary>
        /// Get Quote Details
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <returns>QuoteResponseModel</returns>
        QuoteResponseModel GetQuoteById(int omsQuoteId);

        /// <summary>
        /// Get Quote Details
        /// </summary>
        /// <param name="quoteNumber">quoteNumber</param>
        /// <returns>QuoteResponseModel</returns>
        QuoteResponseModel GetQuoteByQuoteNumber(string quoteNumber);

        /// <summary>
        /// Convert the quote to order.
        /// </summary>
        /// <param name="convertToOrderModel"></param>
        /// <returns>Returns converted order</returns>
        OrderModel ConvertQuoteToOrder(ConvertQuoteToOrderModel convertToOrderModel);

        /// <summary>
        /// Get Quote LineItems by QuoteId.
        /// </summary>
        /// <param name="omsQuoteId">int</param>
        /// <returns>QuoteLineItemModel</returns>
        List<QuoteLineItemModel> GetQuoteLineItems(int omsQuoteId);

        /// <summary>
        /// update existing Quote.
        /// </summary>
        /// <param name="model">UpdateQuoteModel</param>
        /// <returns>bool</returns>
        BooleanModel UpdateQuote(UpdateQuoteModel model);

        /// <summary>
        /// Get Quote Total By quoteNumber
        /// </summary>
        /// <param name="quoteNumber"> quoteNumber </param>
        /// <returns>Quote Total</returns>
        string GetQuoteTotal(string quoteNumber);

        /// <summary>
        ///  To generate unique Quote number on basis of current date.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        string GenerateQuoteNumber(int portalId);

        /// <summary>
        /// Get Quote List
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>CartParameterModel</returns>
        CartParameterModel ToCartParameterModel(int userId, int portalId, int omsQuoteId, int shippingId = 0, int localeId = 0, int omsOrderId = 0);
    }
}
