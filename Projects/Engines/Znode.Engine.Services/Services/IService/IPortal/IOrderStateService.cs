using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IOrderStateService
    {
        /// <summary>
        /// Get List of OrderStates
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="page">int page</param>
        /// <returns>List of OrderStates</returns>
        OrderStateListModel GetOrderStates(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
