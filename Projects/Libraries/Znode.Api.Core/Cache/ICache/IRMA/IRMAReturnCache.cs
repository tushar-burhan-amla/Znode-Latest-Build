namespace Znode.Engine.Api.Cache
{
    public interface IRMAReturnCache
    {
        /// <summary>
        ///  Get order details by order number for return.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="orderNumber">orderNumber</param>
        /// <param name="isFromAdmin">isFromAdmin</param>
        /// <returns>Get order details in string format by serializing it.</returns>
        string GetOrderDetailsForReturn(int userId, string orderNumber, bool isFromAdmin = false);

        /// <summary>
        /// Get the list of all returns.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of returns in string format by serializing it.</returns>
        string GetReturnList(string routeUri, string routeTemplate);

        /// <summary>
        ///  Get order return details by return number
        /// </summary>
        /// <param name="rmaReturnNumber">Return Number</param>
        /// <returns>Return details in string format by serializing it.</returns>
        string GetReturnDetails(string rmaReturnNumber);

        /// <summary>
        /// Get Return Status List
        /// </summary>
        /// <param name="routeUri">URI to Route</param>
        /// <param name="routeTemplate">Template to Route</param>
        /// <returns>return in string format</returns>
        string GetReturnStatusList(string routeUri, string routeTemplate);

        /// <summary>
        ///  Get order return details for admin by return number
        /// </summary>
        /// <param name="rmaReturnNumber">Return Number</param>
        /// <returns>Return details in string format by serializing it.</returns>
        string GetReturnDetailsForAdmin(string rmaReturnNumber);
    }
}