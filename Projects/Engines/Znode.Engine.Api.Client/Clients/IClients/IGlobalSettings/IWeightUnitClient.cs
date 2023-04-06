using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IWeightUnitClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of WeightUnits.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with weightunit list.</param>
        /// <param name="filters">Filters to be applied on weightunit list.</param>
        /// <param name="sorts">Sorting to be applied on weightunit list.</param>
        /// <returns>Weight unit list model.</returns>
        WeightUnitListModel GetWeightUnitList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of WeightUnits.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with weightunit list.</param>
        /// <param name="filters">Filters to be applied on weightunit list.</param>
        /// <param name="sorts">Sorting to be applied on weightunit list.</param>
        /// <param name="pageIndex">Start page index of weightunit list.</param>
        /// <param name="pageSize">Page size of weightunit list.</param>
        /// <returns>Weight Unit list model.</returns>
        WeightUnitListModel GetWeightUnitList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update WeightUnits.
        /// </summary>       
        /// <param name="weightUnitModel">WeightUnitModel to be updated.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateWeightUnit(WeightUnitModel weightUnitModel);
    }
}
