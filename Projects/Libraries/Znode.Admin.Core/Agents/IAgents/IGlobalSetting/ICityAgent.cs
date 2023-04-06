using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ICityAgent
    {
        // <summary>
        /// Get the list of all Cities.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns> City List View Model</returns>
        CityListViewModel GetCityList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get active cities as list of SelectListItem.
        /// </summary>
        /// <param name="countyCode">Selected City Code</param>
        /// <returns>List of SelectListItem</returns>
        List<SelectListItem> GetActiveCityList(FilterCollection filters, string countyCode);
    }
}