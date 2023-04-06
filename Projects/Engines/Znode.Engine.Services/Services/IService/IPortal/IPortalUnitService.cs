using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IPortalUnitService
    {
        /// <summary>
        ///  Get PortalUnit by portal Id.
        /// </summary>
        /// <param name="portalId">int portalId to get the PortalUnit.</param>
        /// <returns>PortalUnitModel</returns>
        PortalUnitModel GetPortalUnit(int portalId);

        /// <summary>
        /// Update PortalUnit associated to portal.
        /// </summary>
        /// <param name="portalUnitModel">PortalUnitModel to update data.</param>
        /// <returns>returns true if PortalUnit updated else returns false.</returns>
        bool CreateUpdatePortalUnit(PortalUnitModel portalUnitModel);
    }
}
