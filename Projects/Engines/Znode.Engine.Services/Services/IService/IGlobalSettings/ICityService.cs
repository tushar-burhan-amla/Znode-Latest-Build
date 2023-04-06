using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICityService
    {
        /// <summary>
        /// Gets a list of cities.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with city list.</param>
        /// <param name="filters">Filters to be applied on city list.</param>
        /// <param name="sorts">Sorting to be applied on city list.</param>
        /// <returns>City list model.</returns>
        CityListModel GetCityList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
