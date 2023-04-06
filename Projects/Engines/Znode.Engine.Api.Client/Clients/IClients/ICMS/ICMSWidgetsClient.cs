using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ICMSWidgetsClient : IBaseClient
    {
        /// <summary>
        /// Gets CMS Widgets.
        /// </summary>
        /// <param name="filters">Filters for cms widgets.</param>
        /// <param name="sorts">Sorts for for cms widgets.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>CMS Widgets list</returns>
        CMSWidgetsListModel List(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get cms widget by codes
        /// </summary>
        /// <returns>CMSWidgetsListModel</returns>
        CMSWidgetsListModel GetWidgetByCodes(ParameterModel widgetCodes);
    }
}
