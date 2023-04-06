using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IRMAReturnClient : IBaseClient
    {
        /// <summary>
        /// Get order details by order number for return.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="orderNumber">orderNumber</param>
        /// <param name="isFromAdmin">isFromAdmin</param>
        /// <returns>OrderModel</returns>
        OrderModel GetOrderDetailsForReturn(int userId, string orderNumber, bool isFromAdmin = false);

        /// <summary>
        /// Get the list of all returns.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>RMAReturnListModel.</returns>
        RMAReturnListModel GetReturnList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Check if order is eligible for return
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="orderNumber">orderNumber</param>
        /// <returns>Returns true if the order is eligible for return</returns>
        bool IsOrderEligibleForReturn(int userId, int portalId, string orderNumber);

        /// <summary>
        /// Get order return details by return number
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <param name="expands">Expand Collection</param>
        /// <returns>Return details for return number</returns>
        RMAReturnModel GetReturnDetails(string returnNumber, ExpandCollection expands = null);

        /// <summary>
        /// Insert or update order return details.
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        RMAReturnModel SaveOrderReturn(RMAReturnModel returnModel);

        /// <summary>
        /// Delete order return on the basis of return number.
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <param name="userId">userId</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        bool DeleteOrderReturn(string returnNumber, int userId);

        /// <summary>
        /// Submit order return.
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        RMAReturnModel SubmitOrderReturn(RMAReturnModel returnModel);

        /// <summary>
        /// Validate order lineitems for create return
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        RMAReturnModel IsValidReturnItems(RMAReturnModel returnModel);

        /// <summary>
        /// Perform calculations for an order return line item.
        /// </summary>
        /// <param name="returnCalculateModel">returnCalculateModel</param>
        /// <returns>Returns RMAReturnCalculateModel.</returns>
        RMAReturnCalculateModel CalculateOrderReturn(RMAReturnCalculateModel returnCalculateModel);

        /// <summary>
        /// Get Return Status List
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="pageIndex">int page index</param>
        /// <param name="pageSize">int pagesize</param>
        /// <returns>List of Return Status</returns>
        RMAReturnStateListModel GetReturnStatusList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get order return details for admin by return number
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <returns>Return details for return number</returns>
        RMAReturnModel GetReturnDetailsForAdmin(string returnNumber);

        /// <summary>
        /// To create return history
        /// </summary>
        /// <param name="returnHistoryModel">List of RMAReturnHistoryModel</param>
        /// <returns>Returns true if return history created successfully</returns>
        bool CreateReturnHistory(List<RMAReturnHistoryModel> returnHistoryModel);

        /// <summary>
        /// Save notes for return.
        /// </summary>
        /// <param name="rmaReturnNotesModel">RMAReturnNotesModel</param>
        /// <returns>Returns true if return notes saved successfully</returns>
        bool SaveReturnNotes(RMAReturnNotesModel rmaReturnNotesModel);
    }
}
