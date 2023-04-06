using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IRMARequestAgent
    {
        /// <summary>
        /// To check RMA is Enable or not
        /// </summary>
        /// <param name="orderId">int OrderId</param>
        /// <param name="orderStatus">string OrderStatus</param>
        /// <param name="orderDate">string OrderDate</param>
        /// <param name="errorMessage">error Message</param>
        /// <returns>returns enable or disable RMA</returns>
        bool IsRMAEnable(int orderId, string orderStatus, string orderDate, out string errorMessage);

        /// <summary>
        /// Gets list of RMA Request list Items.
        /// </summary>
        /// <param name="orderId">Order Id.</param>
        /// <param name="rmaId">RMA Id.</param>
        /// <param name="flag">Current RMA flag</param>
        /// <param name="portalId">Portal id of current RMA</param>
        /// <returns>RMARequestItemList view model.</returns>
        RMARequestItemListViewModel GetRMARequestListItem(int orderId, int rmaId, string flag, int portalId);

        /// <summary>
        /// Generates RMA Request Number.
        /// </summary>
        /// <param name="portalId">Portal id of current RMA</param>
        /// <returns>RMA Request number.</returns>
        string GenerateRequestNumber(int portalId);

        /// <summary>
        /// To Bind Reason For Return list
        /// </summary>
        /// <returns></returns>
        List<SelectListItem> BindReasonForReturn();

        /// <summary>
        /// Gets the list of RMARequest.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with RMARequest list.</param>
        /// <param name="filters">Filters to be applied on RMARequest list.</param>
        /// <param name="sorts">Sorting to be applied on RMARequest list.</param>
        /// <param name="pageIndex">Start page index of RMARequest list.</param>
        /// <param name="pageSize">Page size of RMARequest list.</param>
        /// <returns>Returns RMARequest list.</returns>
        RMARequestListViewModel GetRMARequestList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int? page, int? recordPerPage);

        /// <summary>
        /// Gets an RMA request.
        /// </summary>
        /// <param name="rmaRequestId">RMA request Id.</param>
        /// <returns>RMA request view model.</returns>
        RMARequestViewModel GetRMARequest(int rmaRequestId);

        /// <summary>
        /// Gets RMA request gift card details.
        /// </summary>
        /// <param name="rmaRequestItems">RMA Request item list model.</param>
        /// <param name="rmaRequestId">RMA Request ID.</param>
        void RMARequestGiftCardDetails(RMARequestItemListViewModel rmaRequestItems, int rmaRequestId);

        /// <summary>
        /// Get RMARequestItemList view model.
        /// </summary>
        /// <param name="rmaRequestParamModel"></param>
        /// <param name="flag"></param>
        /// <returns>Return RMARequestItemList ViewMode</returns>
        RMARequestItemListViewModel GetRMARequestItemListViewModel(RMARequestParameterViewModel rmaRequestParamModel, string flag);

        /// <summary>
        /// Make an RMA Request
        /// </summary>
        /// <param name="RMARequestId"></param>
        /// <param name="rmaRequest"></param>
        /// <returns>True or false</returns>
        bool UpdateRMARequest(int RMARequestId, RMARequestViewModel rmaRequest);

        /// <summary>
        /// Create RMA request.
        /// </summary>
        /// <param name="model">RMARequestViewModel</param>
        /// <returns>True or false</returns>
        bool CreateRMARequest(RMARequestViewModel model);

        /// <summary>
        ///Get RMA Request Items For Gift Card
        /// </summary>
        /// <param name="orderLineItems"></param>
        /// <returns></returns>
        RMARequestItemListViewModel GetRMARequestItemsForGiftCard(string orderLineItems);
    }
}