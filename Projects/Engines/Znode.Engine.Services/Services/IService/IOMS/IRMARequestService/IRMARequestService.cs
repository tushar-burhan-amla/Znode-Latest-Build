using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IRMARequestService
    {
        /// <summary>
        /// Gets list of RMA request.
        /// </summary>
        /// <param name="expands">Expands for RMA request list.</param>
        /// <param name="filters">Filters for RMA request list.</param>
        /// <param name="sorts">Sorting of RMA request list.</param>
        /// <param name="page">Paging of RMA request.</param>
        /// <returns>List of RMA request.</returns>
        RMARequestListModel GetRMARequestList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Updates an RMA request.
        /// </summary>
        /// <param name="rmaRequestId">RMA request ID.</param>
        /// <param name="rmaRequestModel">RMA Request model.</param>
        /// <returns>Returns RMARequestModel</returns>
        RMARequestModel UpdateRMARequest(int rmaRequestId, RMARequestModel rmaRequestModel);

        /// <summary>
        /// Gets an RMA request.
        /// </summary>
        /// <param name="rmaRequestId">RMA Request Id.</param>
        /// <returns></returns>
        RMARequestModel GetRMARequest(int rmaRequestId);

        /// <summary>
        /// To Get Order RMA Display Flag by Order Id.
        /// </summary>
        /// <param name="omsOrderDetailsId">OMS order details Id</param>
        /// <returns></returns>
        bool GetOrderRMAFlag(int omsOrderDetailsId);

        /// <summary>
        /// Return RMA Gift Card details.
        /// </summary>
        /// <param name="rmaRequestId">RMA Request Id.</param>
        /// <returns>List of IssuedGiftCardModel.</returns>
        IssuedGiftCardListModel GetRMAGiftCardDetails(int rmaRequestId);

        /// <summary>
        /// Creates an RMA request.
        /// </summary>
        /// <param name="rmaRequestModel">RMA Request model.</param>
        /// <returns>RMA Request model.</returns>
        RMARequestModel CreateRMARequest(RMARequestModel rmaRequestModel);

        //To Do: working in progress

        /// <summary>
        /// Sends email to customer.
        /// </summary>
        /// <param name="rmaRequestId">Rma Item Id.</param>
        /// <returns>Returns true or false value showing if the mail is sent or not.</returns>
        bool SendRMAStatusMail(int rmaRequestId);

        /// <summary>
        /// Send Gift card mail.
        /// </summary>
        /// <param name="giftCardModel">Gift card model from where email data will be retrieved.</param>
        /// <returns>Returns true or false value according to email sent status.</returns>
        bool SendGiftCardMail(GiftCardModel giftCardModel);

        /// <summary>
        /// Get list of popular service request according to parameters.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportPopularSearchModel.</returns>
        List<ReportServiceRequestModel> GetServiceRequestReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}

