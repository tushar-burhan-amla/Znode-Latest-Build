using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IStateService
    {
        /// <summary>
        /// Gets a list of states.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with state list.</param>
        /// <param name="filters">Filters to be applied on state list.</param>
        /// <param name="sorts">Sorting to be applied on state list.</param>
        /// <returns>State list model.</returns>
        StateListModel GetStateList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

    }
}
