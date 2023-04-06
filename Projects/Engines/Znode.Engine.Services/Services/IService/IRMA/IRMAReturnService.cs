using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IRMAReturnService
    {
        /// <summary>
        /// Get Return eligible order number list for Return Items
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns RMAReturnOrderNumberList model.</returns>
        RMAReturnOrderNumberListModel GetEligibleOrderNumberListForReturn(int userId, int portalId);

        /// <summary>
        /// Check Is Order Eligible for Return Items
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="orderNumber">orderNumber</param>
        /// <returns>Returns bool.</returns>
        bool IsOrderEligibleForReturn(int userId, int portalId, string orderNumber);

        /// <summary>
        ///  Get order details by order number for return.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="orderNumber">orderNumber</param>
        /// <param name="isFromAdmin">isFromAdmin</param>
        /// <returns>Returns OrderModel model.</returns>
        OrderModel GetOrderDetailsForReturn(int userId, string orderNumber, bool isFromAdmin = false);

        /// <summary>
        /// Get the list of all returns.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>RMAReturnListModel.</returns>
        RMAReturnListModel GetReturnList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        ///  Get order return details by return number
        /// </summary>
        /// <param name="rmaReturnNumber">Return Number</param>
        /// <param name="expands">Expand Collection</param>
        /// <returns>Returns RMAReturnModel model.</returns>
        RMAReturnModel GetReturnDetails(string rmaReturnNumber, NameValueCollection expands = null);

        /// <summary>
        /// Insert or update order return details.
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        RMAReturnModel SaveOrderReturn(RMAReturnModel returnModel);

        /// <summary>
        ///  //Validate order lineitems for create return
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        RMAReturnModel IsValidReturnItems(RMAReturnModel returnModel);

        /// <summary>
        /// To generate unique return number on basis of current date.
        /// </summary>
        /// <param name="rmaReturnModel">rmaReturnModel</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns return number.</returns>
        string GenerateReturnNumber(RMAReturnModel rmaReturnModel, ParameterModel portalId = null);

        /// <summary>
        /// Delete return order.
        /// </summary>
        /// <param name="returnDetailsId">RmaReturnDetailsId</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteOrderReturn(int returnDetailsId);

        /// <summary>
        /// Delete order return on the basis of return number.
        /// </summary>
        /// <param name="rmaReturnNumber">Return Number</param>
        /// <param name="userId">userId</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteOrderReturnByReturnNumber(string rmaReturnNumber, int userId);

        /// <summary>
        /// Perform calculations for an order return line item.
        /// </summary>
        /// <param name="returnCalculateModel">returnCalculateModel</param>
        /// <returns>Returns RMAReturnCalculateModel.</returns>
        RMAReturnCalculateModel CalculateOrderReturn(RMAReturnCalculateModel returnCalculateModel);

        /// <summary>
        /// Get List of Return States
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="page">int page</param>
        /// <returns>List of Return Status</returns>
        RMAReturnStateListModel GetReturnStatusList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        ///  Get order return details for admin by return number
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <param name="expands">Expand Collection</param>
        /// <returns>Returns RMAReturnModel model.</returns>
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
