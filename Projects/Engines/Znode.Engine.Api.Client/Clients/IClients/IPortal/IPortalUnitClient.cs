using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IPortalUnitClient : IBaseClient
    {
        /// <summary>
        /// Get PortalUnit by portalId
        /// </summary>
        /// <param name="portalId">int portalId to get PortalUnit.</param>
        /// <returns>PortalUnit Model</returns>
        PortalUnitModel GetPortalUnit(int portalId);

        /// <summary>
        /// Create/Update PortalUnit.
        /// </summary>
        /// <param name="portalUnitModel">PortalUnit to update PortalUnit.</param>
        /// <returns>Updated PortalUnit.</returns>
        PortalUnitModel CreateUpdatePortalUnit(PortalUnitModel portalUnitModel);
    }
}
