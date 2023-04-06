using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IWeightUnitService
    {
        /// <summary>
        /// Gets a list of WeightUnits.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with weightunit list.</param>
        /// <param name="filters">Filters to be applied on weightunit list.</param>
        /// <param name="sorts">Sorting to be applied on weightunit list.</param>
        /// <returns>weightunit list model.</returns>
        WeightUnitListModel GetWeightUnits(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Updates WeightUnit.
        /// </summary>
        /// <param name="weightUnitModel">WeightUnitModel to be updated</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateWeightUnit(WeightUnitModel weightUnitModel);
    }
}
