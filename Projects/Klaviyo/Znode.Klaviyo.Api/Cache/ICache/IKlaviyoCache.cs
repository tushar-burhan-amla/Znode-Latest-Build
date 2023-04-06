namespace Znode.Engine.Api.klaviyo.Cache
{
    public interface IKlaviyoCache
    {
        /// <summary>
        /// Get Klaviyo details
        /// </summary>
        /// <param name="portalId">int portalId to get klaviyo</param>
        /// <param name="routeUri">URI to Route.</param>
        /// <param name="routeTemplate">Template to Route.</param>
        /// <returns> String</returns>
        string GetKlaviyo(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Provider List details
        /// </summary>
        /// <param name="portalId">int portalId to get klaviyo</param>
        /// <param name="routeUri">URI to Route.</param>
        /// <param name="routeTemplate">Template to Route.</param>
        /// <returns> String</returns>
        string GetEmailProviderList(string routeUri, string routeTemplate);
    }
}
