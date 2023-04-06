using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IRMAReturnAgent
    {
        /// <summary>
        /// Get order return list
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="portalId">portalId</param>
        /// <param name="portalName">portalName</param>
        /// <returns>Returns list of returns.</returns>
        RMAReturnListViewModel GetReturnList(FilterCollectionDataModel model, int portalId = 0, string portalName = null);

        /// <summary>
        /// Manage Return
        /// </summary>
        /// <param name="returnNumber">returnNumber</param>
        /// <returns>return RMAReturnViewModel</returns>
        RMAReturnViewModel ManageReturn(string returnNumber);

        /// <summary>
        /// Get List of Return States
        /// </summary>
        /// <param name="returnStatus">returnStatus</param>
        /// <returns>return RMAReturnStatusList</returns>
        RMAReturnStatusList GetReturnStatusList(string returnStatus);

        /// <summary>
        /// Submit order return details.
        /// </summary>
        /// <param name="returnNumber">returnNumber</param>
        /// <param name="notes">notes</param>
        /// <returns>Returns RMAReturnViewModel.</returns>
        RMAReturnViewModel SubmitOrderReturn(string returnNumber, string notes);

        /// <summary>
        /// Update Order Return Line Item
        /// </summary>
        /// <param name="orderReturnLineItemModel">orderReturnLineItemModel</param>
        /// <param name="returnNumber">returnNumber</param>
        /// <returns>Returns RMAReturnViewModel.</returns>
        RMAReturnViewModel UpdateOrderReturnLineItem(RMAReturnLineItemViewModel orderReturnLineItemModel, string returnNumber);

        /// <summary>
        /// Update Order Return Status
        /// </summary>
        /// <param name="returnStatusCode">returnStatusCode</param>
        /// <param name="returnNumber">returnNumber</param>
        /// <returns>Returns RMAReturnViewModel.</returns>
        RMAReturnViewModel UpdateOrderReturnStatus(int returnStatusCode, string returnNumber);

        /// <summary>
        /// Print order return receipt by return number
        /// </summary>
        /// <param name="returnNumber">returnNumber</param>
        /// <returns>RMAReturnViewModel containing return details</returns>
        RMAReturnViewModel PrintReturnReceipt(string returnNumber);

        /// <summary>
        /// Check if the order is eligible for return
        /// </summary>
        /// <param name="orderNumber">orderNumber</param>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns true if the order is eligible for return</returns>
        bool IsOrderEligibleForReturn(string orderNumber, int userId, int portalId);


        /// <summary>
        ///  Get order details by order number for return.
        /// </summary>
        /// <param name="orderNumber">orderNumber</param>
        /// <param name="userId">userId</param>
        /// <returns>Returns RMAReturnOrderDetailViewModel model.</returns>
        RMAReturnOrderDetailViewModel GetOrderDetailsForReturn(string orderNumber, int userId);

        /// <summary>
        /// Perform calculations for an order return line item.
        /// </summary>
        /// <param name="returnCalculateViewModel">returnCalculateViewModel</param>
        /// <returns>Returns RMAReturnCalculateViewModel.</returns>
        RMAReturnCalculateViewModel CalculateOrderReturn(RMAReturnCalculateViewModel returnCalculateViewModel);
        
        /// <summary>
        /// Submit order return.
        /// </summary>
        /// <param name="returnViewModel">returnViewModel</param>
        /// <returns>Returns RMAReturnViewModel.</returns>
        RMAReturnViewModel SubmitCreateReturn(RMAReturnViewModel returnViewModel);

        /// <summary>
        /// Get order return details by return number
        /// </summary>
        /// <param name="returnNumber">returnNumber</param>
        /// <param name="isReturnDetailsReceipt">isReturnDetailsReceipt</param>
        /// <returns>RMAReturnViewModel containing return details</returns>
        RMAReturnViewModel GetReturnDetails(string returnNumber, bool isReturnDetailsReceipt = true);

        /// <summary>
        /// Validate Return Item to create return.
        /// </summary>
        /// <param name="returnViewModel">returnViewModel</param>
        /// <returns>Returns RMAReturnViewModel.</returns>
        RMAReturnViewModel IsValidReturnItems(RMAReturnViewModel returnViewModel);
    }
}
