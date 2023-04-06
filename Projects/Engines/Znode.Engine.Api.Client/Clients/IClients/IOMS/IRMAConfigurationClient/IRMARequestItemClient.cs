using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IRMARequestItemClient : IBaseClient
    {
        /// <summary>
        /// Gets list of RMA request item list.
        /// </summary>       
        /// <param name="expands">Expand collection for RMA request item list.</param>
        /// <param name="filters">Filter for RMA request item list.</param>
        /// <param name="sorts">Sort collection for RMA request item list.</param>
        /// <returns>RMA request item list model.</returns>
        RMARequestItemListModel GetRMARequestItemList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets list of RMA request item list.
        /// </summary>       
        /// <param name="expands">Expand collection for RMA request item list.</param>
        /// <param name="filters">Filter for RMA request item list.</param>
        /// <param name="sorts">Sort collection for RMA request item list.</param>
        /// <param name="pageIndex">Page index for RMA request item list.</param>
        /// <param name="pageSize">Page size for RMA request item list.</param>
        /// <returns>RMA request item list model.</returns>
        RMARequestItemListModel GetRMARequestItemList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get RMA Request Items For Gift Card
        /// </summary>
        /// <param name="orderLineItems"></param>
        /// <returns>RMARequestItemListModel</returns>
        RMARequestItemListModel GetRMARequestItemsForGiftCard(string orderLineItems);

        /// <summary>
        /// Create RMA Request Item
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RMARequestItemModel</returns>
        RMARequestItemModel CreateRMARequestItem(RMARequestItemModel model);
    }
}
