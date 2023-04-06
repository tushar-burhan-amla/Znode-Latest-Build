using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICMSWidgetsService
    {
        /// <summary>
        /// Get CMS Widgets list.
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>Returns list of CMS Widgets.</returns>
        CMSWidgetsListModel List(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Widget details by Widget codes.
        /// </summary>
        /// <param name="codes">Get widget by codes.</param>
        /// <returns>CMS Widget list Model</returns>
        CMSWidgetsListModel GetWidgetByCodes(ParameterModel widgetCodes);
    }
}
