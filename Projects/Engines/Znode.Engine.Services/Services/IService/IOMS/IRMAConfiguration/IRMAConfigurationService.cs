using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IRMAConfigurationService
    {
        #region RMAConfiguration
        /// <summary>
        /// Create or Update RMA Configuration.
        /// </summary>
        /// <param name="model">model with rma configuration</param>
        /// <returns>RMAConfigurationModel</returns>
        RMAConfigurationModel CreateRMAConfiguration(RMAConfigurationModel model);

        /// <summary>
        /// Get RMA Configuration.
        /// </summary>
        /// <returns>RMAConfigurationModel</returns>
        RMAConfigurationModel GetRMAConfiguration();
        #endregion

        #region Request Status
        /// <summary>
        /// Update request status.
        /// </summary>
        /// <param name="model">model with request status details</param>
        /// <returns>RequestStatusModel</returns>
        bool UpdateRequestStatus(RequestStatusModel model);

        /// <summary>
        /// delete request status
        /// </summary>
        /// <param name="requestStatusId">request status id</param>
        /// <returns>true/false</returns>
        bool DeleteRequestStatus(ParameterModel requestStatusId);

        /// <summary>
        /// Get the list of request status list.
        /// </summary>
        /// <param name="expands">Expands for request status list.</param>
        /// <param name="filters">Filters for request status list.</param>
        /// <param name="sorts">Sorts for request status list.</param>
        /// <param name="page">Paging for request status list.</param>
        /// <returns>RequestStatusListModel</returns>
        RequestStatusListModel GetRequestStatusList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get request status by id.
        /// </summary>
        /// <param name="requestStatusId">request status id.</param>
        /// <returns>RequestStatusModel</returns>
        RequestStatusModel GetRequestStatus(int requestStatusId);
        #endregion

        #region Reason For Return
        /// <summary>
        /// Create Reason For Return.
        /// </summary>
        /// <param name="model">RequestStatus Model</param>
        /// <returns>returns RequestStatusModel</returns>
        RequestStatusModel CreateReasonForReturn(RequestStatusModel model);

        /// <summary>
        /// Update Reason For Return.
        /// </summary>
        /// <param name="model">>RequestStatus Model</param>
        /// <returns>return status</returns>
        bool UpdateReasonForReturn(RequestStatusModel model);

        /// <summary>
        /// Get reason for return data by reasonForReturnId.
        /// </summary>
        /// <param name="reasonForReturnId">reasonForReturnId to get data.</param>
        /// <returns>returns RequestStatusModel</returns>
        RequestStatusModel GetReasonForReturn(int reasonForReturnId);

        /// <summary>
        /// Get paged reason for return list.
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filters list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>return RequestStatusListModel.</returns>
        RequestStatusListModel GetReasonForReturnList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete  reason for return by reasonForReturnId.
        /// </summary>
        /// <param name="reasonForReturnId">reasonForReturnId to delete reason for return.</param>
        /// <returns>return status</returns>
        bool DeleteReasonForReturn(ParameterModel reasonForReturnId);
        #endregion
    }
}
