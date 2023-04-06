using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IOrderStateClient : IBaseClient 
    {
        /// <summary>
        /// Get OrderState List
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="pageIndex">int page index</param>
        /// <param name="pageSize">int pagesize</param>
        /// <returns>List of OrderState</returns>
        OrderStateListModel GetOrderStates(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
