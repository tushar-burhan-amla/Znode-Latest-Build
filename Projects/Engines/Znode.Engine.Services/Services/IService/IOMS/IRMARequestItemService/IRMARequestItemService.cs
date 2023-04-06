using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IRMARequestItemService
    {
        /// <summary>
        /// Creates an RMA request item.
        /// </summary>
        /// <param name="rmaRequestItemModel">RMA request item model.</param>
        /// <returns>RMARequestItemModel</returns>
        RMARequestItemModel CreateRMAItemRequest(RMARequestItemModel rmaRequestItemModel);

        /// <summary>
        /// Get list of RMA request items.
        /// </summary>        
        /// <param name="expands">Expands for RMA list items.</param>
        /// <param name="filters">Filter collection for RMA item list.</param>
        /// <param name="sorts">Sorting of RMA list.</param>
        /// <param name="page">Paging for RMA item list.</param>
        /// <returns>RMA request item list.</returns>
        RMARequestItemListModel GetRMARequestItemList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        ///  Get list of RMA request items.
        /// </summary>
        /// <param name="orderLineItemList">Order line items.</param>
        /// <returns>Request item model.</returns>
        RMARequestItemListModel GetRMARequestItemsForGiftCard(string orderLineItemList);

    }
}
