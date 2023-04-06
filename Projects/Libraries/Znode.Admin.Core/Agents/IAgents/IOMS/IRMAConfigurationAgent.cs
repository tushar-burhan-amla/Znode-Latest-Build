using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IRMAConfigurationAgent
    {
        #region RMA Configuration
        /// <summary>
        /// Create or Update RMA Configuration.
        /// </summary>
        /// <param name="model">model with rma configuration</param>
        /// <returns>RMAConfigurationModel</returns>
        RMAConfigurationViewModel CreateRMAConfiguration(RMAConfigurationViewModel model);

        /// <summary>
        /// Get RMA Configuration.
        /// </summary>
        /// <returns>RMAConfigurationModel</returns>
        RMAConfigurationViewModel GetRMAConfiguration();
        #endregion

        #region Reason For Return/Request Status
        /// <summary>
        /// Get the list of Reason For Return or Request Status.
        /// </summary>
        /// <param name="expands">Expands to be retrieved.</param>
        /// <param name="filters">Filters to be applied on list.</param>
        /// <param name="sorts">Sorting to be applied on list.</param>
        /// <param name="pageIndex">Start page index of list.</param>
        /// <param name="pageSize">Page size of  list.</param>
        /// <param name="isRequestStatus">On the basis of this flag we get  Reason For Request or Request Status list.</param>
        /// <returns>Returns Reason For Request/Request Status list.</returns>
        RequestStatusListViewModel GetReasonForReturnOrRequestStatusList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isRequestStatus);

        /// <summary>
        /// Create Reason For Return.
        /// </summary>
        /// <param name="model">RequestStatus Model.</param>
        /// <returns>Returns created Reason For Request Model.</returns>
        RequestStatusViewModel CreateReasonForReturn(RequestStatusViewModel model);

        /// <summary>
        /// Get Reason For Return on the basis of reasonForReturnId or get Request Status on the basis of requestStatusId .
        /// </summary>
        /// <param name="rmaReasonForReturnId">id to get data of a particular id.</param>
        /// <param name="rmaRequestStatusId">id to get data of a particular id.</param>
        /// <param name="isRequestStatus">On the basis of this flag we get Reason For Request or Request Status.</param>
        /// <returns>Returns RequestStatus Model.</returns>
        RequestStatusViewModel GetReasonForReturnOrRequestStatus(int rmaReasonForReturnId, int rmaRequestStatusId, bool isRequestStatus);

        /// <summary>
        /// Update Reason For Return data or RequestStatus.
        /// </summary>
        /// <param name="model">RequestStatus model containing reason for request parameters to update.</param>
        /// <param name="isRequestStatus">On the basis of this flag data is updated for Reason For Request or Request Status.</param>
        /// <returns>Returns updated Reason For Request model.</returns>
        RequestStatusViewModel UpdateReasonForReturnOrRequestStatus(RequestStatusViewModel model, bool isRequestStatus);

        /// <summary>
        /// Delete Reason For Return or Request Status.
        /// </summary>
        /// <param name="id">Ids to delete data.</param>
        /// <param name="isRequestStatus">On the basis of this flag data is deleted for Reason For Return or Request Status.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteReasonForReturnOrRequestStatus(string id, bool isRequestStatus, out string errorMessage);
        #endregion
    }
}
