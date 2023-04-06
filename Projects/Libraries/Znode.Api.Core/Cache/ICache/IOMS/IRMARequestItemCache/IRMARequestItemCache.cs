namespace Znode.Engine.Api.Cache
{
    public interface IRMARequestItemCache
    {
        /// <summary>
        /// Get list of RMA request list item.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>       
        /// <returns>List of RMA request items.</returns>
        string GetRMARequestItems(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of RMA request list item.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <param name="orderLineItems">Comma seperated order line items.</param>
        /// <returns>RMA Request item string data.</returns>
        string GetRMARequestItemsForGiftCard(string routeUri, string routeTemplate, string orderLineItems);
    }
}