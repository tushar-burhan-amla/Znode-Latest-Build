using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IStoreExperienceAgent
    {
        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>StoreListViewModel.</returns>
        StoreListViewModel GetStoreExperienceList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);
    }
}
