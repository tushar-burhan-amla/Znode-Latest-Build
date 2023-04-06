using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IStateClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of states.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with state list.</param>
        /// <param name="filters">Filters to be applied on state list.</param>
        /// <param name="sorts">Sorting to be applied on state list.</param>
        /// <returns>State list model.</returns>
        StateListModel GetStateList(FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of states.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with state list.</param>
        /// <param name="filters">Filters to be applied on state list.</param>
        /// <param name="sorts">Sorting to be applied on state list.</param>
        /// <param name="pageIndex">Start page index of state list.</param>
        /// <param name="pageSize">Page size of state list.</param>
        /// <returns>State list model.</returns>
        StateListModel GetStateList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
