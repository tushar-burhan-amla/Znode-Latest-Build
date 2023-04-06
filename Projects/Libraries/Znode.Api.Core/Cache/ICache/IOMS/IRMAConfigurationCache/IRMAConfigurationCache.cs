namespace Znode.Engine.Api.Cache
{
    public interface IRMAConfigurationCache
    {
        #region RMA Configuration
        /// <summary>
        /// get rma configuration by rmaConfigId.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>rma configuration</returns>
        string GetRMAConfiguration(string routeUri, string routeTemplate);
        #endregion

        #region Request status
        /// <summary>
        /// get status request by requestStatusId.
        /// </summary>
        /// <param name="rmaRequestStatusId">request status id.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>request status</returns>
        string GetRequestStatus(int rmaRequestStatusId, string routeUri, string routeTemplate);

        /// <summary>
        /// get status request list
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>request status list.</returns>
        string GetRequestStatusList(string routeUri, string routeTemplate);
        #endregion

        #region Reason For Return.
        /// <summary>
        /// Get reason For return by reasonForReturnId.
        /// </summary>
        /// <param name="rmaReasonForReturnId">reasonForReturnId to get data.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Reason For Return.</returns>
        string GetReasonForReturn(int rmaReasonForReturnId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get reason For return list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Reason For Return list.</returns>
        string GetReasonForReturnList(string routeUri, string routeTemplate);
        #endregion
    }
}
