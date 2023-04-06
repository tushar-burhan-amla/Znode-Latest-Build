using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ICityClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of cities.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with city list.</param>
        /// <param name="filters">Filters to be applied on city list.</param>
        /// <param name="sorts">Sorting to be applied on city list.</param>
        /// <returns>City list model.</returns>
        CityListModel GetCityList(FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of cities.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with city list.</param>
        /// <param name="filters">Filters to be applied on city list.</param>
        /// <param name="sorts">Sorting to be applied on city list.</param>
        /// <param name="pageIndex">Start page index of city list.</param>
        /// <param name="pageSize">Page size of city list.</param>
        /// <returns>City list model.</returns>
        CityListModel GetCityList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
