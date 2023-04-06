namespace Znode.Engine.Api.Cache
{
    public interface IRMARequestCache
    {

        /// <summary>
        /// Gets RMA Request list data.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetRMARequests(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets RMA Request.
        /// </summary>
        /// <param name="rmaRequestId">RMA request ID.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetRMARequest(int rmaRequestId, string routeUri, string routeTemplate);

        /// <summary>
        /// To Get Order RMA Display Flag by Order Id.
        /// </summary>
        /// <param name="omsOrderDetailsId">int OMS order details Id</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns true if Order RMA  is 1 else returns false.</returns>
        string GetOrderRMAFlag(int omsOrderDetailsId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the issued gift cards for a particular RMA request ID.
        /// </summary>
        /// <param name="rmaRequestId">RMA Request ID for which issued gift cards will be fetched.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>String data for issued gift cards.</returns>
        string GetIssuedGiftCards(int rmaRequestId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of service requests according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetServiceRequestReport(string routeUri, string routeTemplate);
    }
}
