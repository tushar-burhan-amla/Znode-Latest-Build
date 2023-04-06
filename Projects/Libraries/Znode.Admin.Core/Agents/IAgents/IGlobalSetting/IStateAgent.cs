using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IStateAgent
    {
        // <summary>
        /// Get the list of all States.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns> State List View Model</returns>
        StateListViewModel GetStateList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get active states as list of SelectListItem.
        /// </summary>
        /// <param name="stateCode">Selected State Code</param>
        /// <returns>List of SelectListItem</returns>
        List<SelectListItem> GetActiveStateList(FilterCollection filters, string stateCode = "");

    }
}