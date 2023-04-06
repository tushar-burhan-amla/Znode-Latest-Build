namespace Znode.Engine.Api.Cache
{
    public interface ISMTPCache
    {
        /// <summary>
        /// Get SMTP details
        /// </summary>
        /// <param name="portalId">int portalId to get smtp</param>
        /// <param name="routeUri">URI to Route.</param>
        /// <param name="routeTemplate">Template to Route.</param>
        /// <returns></returns>
        string GetSMTP(int portalId, string routeUri, string routeTemplate);
    }
}
