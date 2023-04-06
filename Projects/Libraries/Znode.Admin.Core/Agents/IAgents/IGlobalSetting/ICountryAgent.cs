using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface ICountryAgent
    {
        // <summary>
        /// Get the list of all Countries.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns> Country List View Model</returns>
        CountryListViewModel GetCountries(ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Method to Update Country in database.
        /// </summary>
        /// <param name="model">DefaultGlobalConfigViewModel model</param>
        /// <returns>Returns true/false</returns>
        bool UpdateCountry(DefaultGlobalConfigViewModel model, out string message);

        /// <summary>
        /// Get active country as list of SelectListItem.
        /// </summary>
        /// <returns>List of SelectListItem</returns>
        List<SelectListItem> GetActiveCountryList();
    }
}