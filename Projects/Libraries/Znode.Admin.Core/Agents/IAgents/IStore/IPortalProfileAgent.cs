using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IPortalProfileAgent
    {
        /// <summary>
        /// Get all the Portal Profiles from specified portal id
        /// </summary>
        /// <param name="portalId">portal id to get the associated Portal Profile.</param>
        /// <param name="filters">filter list across Portal Profile.</param>
        /// <param name="sortCollection">sort collection for Portal Profile.</param>
        /// <param name="pageIndex">pageIndex for Portal Profile record. </param>
        /// <param name="recordPerPage">paging Portal Profile record per page.</param>
        /// <returns></returns>
        PortalProfileListViewModel GetPortalProfiles(int portalId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Create New Portal Profile.
        /// </summary>
        /// <param name="portalProfileViewModel">PortalProfile ViewModel.</param>
        /// <returns>Returns PortalProfile ViewModel.</returns>
        PortalProfileViewModel CreatePortalProfile(PortalProfileViewModel portalProfileViewModel);

        /// <summary>
        /// Get Existing PortalProfile by portalProfile id.
        /// </summary>
        /// <param name="portalProfileId">portalProfile id to get the PortalProfile.</param>
        /// <returns>Returns PortalProfileViewModel</returns>
        PortalProfileViewModel GetPortalProfile(int portalProfileId, ExpandCollection expands = null);

        /// <summary>
        /// Update Existing PortalProfile.
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="portalProfileId">portalProfileId</param>
        /// <param name="data">json data </param>
        /// <returns>Returns PortalProfileViewModel.</returns>
        PortalProfileViewModel UpdatePortalProfile(int portalId, int portalProfileId, string data);

        /// <summary>
        /// Delete Existing PortalProfile by portalProfile Ids.
        /// </summary>
        /// <param name="portalProfileIds">portalProfile Ids of portalProfile to be deleted.</param>
        /// <returns>Returns true if PortalProfile Deleted else returns false.</returns>
        bool DeletePortalProfile(string portalProfileId);

        /// <summary>
        /// Get Profile List by PortalId.
        /// </summary>
        /// <param name="portalId">int PortalId</param>
        /// <param name="expands"></param>
        /// <param name="filters">filter list across Profile.</param>
        /// <param name="sorts">sort collection for Profile.</param>
        /// <param name="pageIndex">pageIndex for Portal Profile record.</param>
        /// <param name="pageSize">paging Portal Profile record per page</param>
        /// <param name="portalPortalId">int portalpPortalId to update the existing profiles.</param>
        /// <returns>Profile list.</returns>
        List<SelectListItem> GetProfileList(int portalId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int portalProfileId = 0);
    }
}
