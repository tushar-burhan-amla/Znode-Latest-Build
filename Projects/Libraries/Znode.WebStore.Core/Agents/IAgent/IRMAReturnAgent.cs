using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface IRMAReturnAgent
    {

        /// <summary>
        ///  Get order details by order number for return.
        /// </summary>
        /// <param name="orderNumber">orderNumber</param>
        /// <returns>Returns RMAReturnOrderDetailViewModel model.</returns>
        RMAReturnOrderDetailViewModel GetOrderDetailsForReturn(string orderNumber = null);

        /// <summary>
        /// Get order return list for a user
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sortCollection">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns list of returns.</returns>
        RMAReturnListViewModel GetReturnList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Check if the order is eligible for return
        /// </summary>
        /// <param name="orderNumber">orderNumber</param>
        /// <returns>Returns true if the order is eligible for return</returns>
        bool IsOrderEligibleForReturn(string orderNumber);

        /// <summary>
        /// Get order return details by return number
        /// </summary>
        /// <param name="returnNumber">returnNumber</param>
        /// <param name="isReturnDetailsReceipt">isReturnDetailsReceipt</param>
        /// <returns>RMAReturnViewModel containing return details</returns>
        RMAReturnViewModel GetReturnDetails(string returnNumber, bool isReturnDetailsReceipt = true);

        /// <summary>
        /// Insert or update order return details.
        /// </summary>
        /// <param name="returnViewModel">returnViewModel</param>
        /// <returns>Returns RMAReturnViewModel.</returns>
        RMAReturnViewModel SaveOrderReturn(RMAReturnViewModel returnViewModel);

        /// <summary>
        /// Delete order return on the basis of return number.
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        bool DeleteOrderReturn(string returnNumber);

        /// <summary>
        /// Submit order return.
        /// </summary>
        /// <param name="returnViewModel">returnViewModel</param>
        /// <returns>Returns RMAReturnViewModel.</returns>
        RMAReturnViewModel SubmitOrderReturn(RMAReturnViewModel returnViewModel);

        /// <summary>
        /// Perform calculations for an order return line item.
        /// </summary>
        /// <param name="returnCalculateViewModel">returnCalculateViewModel</param>
        /// <returns>Returns RMAReturnCalculateViewModel.</returns>
        RMAReturnCalculateViewModel CalculateOrderReturn(RMAReturnCalculateViewModel returnCalculateViewModel);

        /// <summary>
        /// Manage order return
        /// </summary>
        /// <param name="returnNumber">returnNumber</param>
        /// <returns>Returns RMAReturnOrderDetailViewModel model.</returns>
        RMAReturnOrderDetailViewModel ManageOrderReturn(string returnNumber = null);

        /// <summary>
        /// Validate Guest user return.
        /// </summary>
        /// <param name="orderNumber">order number</param>
        /// <returns></returns>
        UserViewModel ValidateGuestUserReturn(string orderNumber);
    }
}