using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPortalProfileService
    {
        /// <summary>
        /// Gets the list of portal profiles.
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="page">int pages</param>
        /// <returns>Portal Profile List</returns>
        PortalProfileListModel GetPortalProfiles(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets a portal portfile according portal profile id.
        /// </summary>
        /// <param name="portalProfileId">int portalProfileId</param>
        /// <param name="expands">Collection of expands</param>
        /// <returns>PortalProfileModel model</returns>
        PortalProfileModel GetPortalProfile(int portalProfileId, NameValueCollection expands);

        /// <summary>
        /// Creates a portal profile.
        /// </summary>
        /// <param name="portalProfileModel">PortalProfileModel model</param>
        /// <returns>Portal Profile</returns>
        PortalProfileModel CreatePortalProfile(PortalProfileModel portalProfileModel);

        /// <summary>
        /// Updates a portal profile.
        /// </summary>
        /// <param name="portalProfileModel">PortalProfileModel model</param>
        /// <returns>return true if PortalProfileModel successfully updated else return false</returns>
        bool UpdatePortalProfile(PortalProfileModel portalProfileModel);

        /// <summary>
        /// Deletes portal profile.
        /// </summary>
        /// <param name="portalProfileId">portal profile Ids for the selected Portal Profile.</param>
        /// <returns>bool isDeleted true/false</returns>
        bool DeletePortalProfile(ParameterModel portalProfileId);
    }
}
