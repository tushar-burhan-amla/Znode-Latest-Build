
namespace Znode.Engine.Api.Cache
{
    public interface IQuoteCache
    {
        /// <summary>
        /// Get the list of all Quotes.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of Quotes in string format by serializing it.</returns>
        string GetQuoteList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Quote Details
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Quotes details in string format by serializing it.</returns>
        string GetQuoteReceipt(int quoteId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get quote details by quote id.
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>quote details</returns>
        string GetQuoteById(int omsQuoteId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get quote details by quote number.
        /// </summary>
        /// <param name="quoteNumber">quoteNumber</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>quote details</returns>
        string GetQuoteByQuoteNumber(string quoteNumber, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Quote LineItems by QuoteId.
        /// </summary>
        /// <param name="omsQuoteId">int.</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of Quotes LineItems in string format by serializing it.</returns>
        string GetQuoteLineItems(int omsQuoteId, string routeUri, string routeTemplate);
    }
}
