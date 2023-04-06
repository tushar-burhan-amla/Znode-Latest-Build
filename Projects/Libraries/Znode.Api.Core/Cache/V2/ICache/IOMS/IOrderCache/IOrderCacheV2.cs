namespace Znode.Engine.Api.Cache
{
    public interface IOrderCacheV2 : IOrderCache
    {
        /// <summary>
        /// Get the list of all orders.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of order in string format by serializing it.</returns>
        string GetOrderListV2(string routeUri, string routeTemplate);

        /// <summary>
        /// Get order details by order number.
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>order details</returns>
        string GetOrderByOrderNumber(string orderNumber, string routeUri, string routeTemplate);
    }
}
