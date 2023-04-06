using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IPortalProfileClient :IBaseClient
    {
        /// <summary>
        /// Gets portal profiles.
        /// </summary>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Portal Profile List</returns>
        PortalProfileListModel GetPortalProfiles(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets Portal Profile by portal profile ID.
        /// </summary>
        /// <param name="portalProfileId">Portal Profile Id.</param>
        /// <param name="expands">Expands collection</param>
        /// <returns>PortalProfileModel</returns>
        PortalProfileModel GetPortalProfile(int portalProfileId, ExpandCollection expands);

        /// <summary>
        /// Creates a portal profile.
        /// </summary>
        /// <param name="model">Portal profile model.</param>
        /// <returns>PortalProfileModel</returns>
        PortalProfileModel CreatePortalProfile(PortalProfileModel model);

        /// <summary>
        /// Updates portal profile.
        /// </summary>
        /// <param name="model">Portal profile model.</param>
        /// <returns>PortalProfileModel.</returns>
        PortalProfileModel UpdatePortalProfile(PortalProfileModel model);

        /// <summary>
        /// Deletes a portal profile.
        /// </summary>
        /// <param name="portalProfileIds">Portal profile ID</param>
        /// <returns>Bool value whether delete is success or not.</returns>
        bool DeletePortalProfile(ParameterModel portalProfileIds);
    }
}
