using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IRMARequestClient : IBaseClient
    {
        /// <summary>
        /// To Get Order RMA Display Flag by Order details Id.
        /// </summary>
        /// <param name="omsOrderDetailsId">int orderId</param>
        ///  <returns>Returns true if Order RMA  is 1 else returns false.</returns>
        bool GetOrderRMAFlag(int omsOrderDetailsId);

        /// <summary>
        /// Gets the list of RMARequest.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with RMARequest list.</param>
        /// <param name="filters">Filters to be applied on RMARequest list.</param>
        /// <param name="sorts">Sorting to be applied on RMARequest list.</param>
        /// <param name="pageIndex">Start page index of RMARequest list.</param>
        /// <param name="pageSize">Page size of RMARequest list.</param>
        /// <returns>Returns RMARequest list.</returns>
        RMARequestListModel GetRMARequest(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets RMA request gift card details.
        /// </summary>
        /// <param name="rmaRequestId">RMA Request ID.</param>
        /// <returns>Issued gift card model.</returns>
        IssuedGiftCardListModel GetRMARequestGiftCardDetails(int rmaRequestId);


        /// <summary>
        /// Gets RMA Request
        /// </summary>
        /// <param name="rmaRequestId">RMA Request ID.</param>
        /// <returns>RMA Request model.</returns>
        RMARequestModel GetRMARequest(int rmaRequestId);

        /// <summary>
        /// Update RMA Request by rmaRequestId
        /// </summary>
        /// <param name="rmaRequestId"></param>
        /// <param name="rmaRequestModel"></param>
        /// <returns></returns>
        RMARequestModel UpdateRMARequest(int rmaRequestId, RMARequestModel rmaRequestModel);

        /// <summary>
        /// Create RMA Request.
        /// </summary>
        /// <param name="model">RMA Request Model</param>
        /// <returns>RMARequestModel</returns>
        RMARequestModel CreateRMARequest(RMARequestModel model);

        /// <summary>
        /// Check gift card mail send
        /// </summary>
        /// <param name="model"></param>
        /// <returns>true/false</returns>
        bool IsGiftCardMailSent(GiftCardModel model);

        /// <summary>
        /// Check mail send status
        /// </summary>
        /// <param name="rmaRequestId"></param>
        /// <returns>true/false</returns>
        bool IsStatusEmailSent(int rmaRequestId);
    }
}
