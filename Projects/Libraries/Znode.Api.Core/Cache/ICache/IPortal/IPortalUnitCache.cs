namespace Znode.Engine.Api.Cache
{
    public interface IPortalUnitCache 
    {
        /// <summary>
        /// Get PortalUnit details.
        /// </summary>
        /// <param name="portalId">int portalId to get PortalUnit.</param>
        /// <param name="routeUri">URI to Route.</param>
        /// <param name="routeTemplate">Template to Route.</param>
        /// <returns></returns>
        string GetPortalUnit(int portalId, string routeUri, string routeTemplate);
    }
}
