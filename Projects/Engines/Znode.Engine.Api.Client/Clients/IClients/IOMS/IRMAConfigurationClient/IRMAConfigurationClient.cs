using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IRMAConfigurationClient : IBaseClient
    {
        #region RMA Configuration
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

        #region Reason For Return
        /// <summary>
        /// Gets the list of Reason For Return.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Reason For Return list.</param>
        /// <param name="filters">Filters to be applied on Reason For Return list.</param>
        /// <param name="sorts">Sorting to be applied on Reason For Return list.</param>
        /// <param name="pageIndex">Start page index of Reason For Return list.</param>
        /// <param name="pageSize">Page size of Reason For Return list.</param>
        /// <returns>Returns Reason ForReturnlist.</returns>
        RequestStatusListModel GetReasonForReturnList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create Reason For Return.
        /// </summary>
        /// <param name="model">RequestStatus Model containing reason for return parameters.</param>
        /// <returns>Returns created Reason For Return Model.</returns>
        RequestStatusModel CreateReasonForReturn(RequestStatusModel model);

        /// <summary>
        /// Get Reason For Return on the basis of reasonForReturnId.
        /// </summary>
        /// <param name="rmaReasonForReturnId">reasonForReturnId to get Reason For Return details.</param>
        /// <returns>Returns Reason For Return Model.</returns>
        RequestStatusModel GetReasonForReturn(int rmaReasonForReturnId);

        /// <summary>
        /// Update Reason For Return data.
        /// </summary>
        /// <param name="model">RequestStatus model containing reason for Return parameters to update.</param>
        /// <returns>Returns updated Reason For Return model.</returns>
        RequestStatusModel UpdateReasonForReturn(RequestStatusModel model);

        /// <summary>
        /// Delete Reason For Return.
        /// </summary>
        /// <param name="rmaReasonForReturnId">Reason For Return Ids to delete data.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteReasonForReturn(ParameterModel rmaReasonForReturnId);
        #endregion

        #region Request Status
        /// <summary>
        /// Gets the list of Request Status.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Request Status list.</param>
        /// <param name="filters">Filters to be applied on Request Status list.</param>
        /// <param name="sorts">Sorting to be applied on Request Status list.</param>
        /// <param name="pageIndex">Start page index of Request Status list.</param>
        /// <param name="pageSize">Page size of Request Status list.</param>
        /// <returns>Returns Request Status list.</returns>
        RequestStatusListModel GetRequestStatusList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Request Status on the basis of Request Status id.
        /// </summary>
        /// <param name="rmaRequestStatusId">requestStatusId to get Request Status details.</param>
        /// <returns>Returns RequestStatusModel.</returns>
        RequestStatusModel GetRequestStatus(int rmaRequestStatusId);

        /// <summary>
        /// Update Request Status data.
        /// </summary>
        /// <param name="model">Request Status model to update.</param>
        /// <returns>Returns updated Request Status model.</returns>
        RequestStatusModel UpdateRequestStatus(RequestStatusModel model);

        /// <summary>
        /// Delete RequestStatus.
        /// </summary>
        /// <param name="rmaRequestStatusIds">RmaRequestStatusIds to delete data.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteRequestStatus(ParameterModel rmaRequestStatusIds);
        #endregion
    }
}
