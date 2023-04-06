namespace Znode.Engine.Api.Cache
{
    public interface ICustomerCache
    {
        #region Profile Association
        /// <summary>
        /// Get list of unassociate profiles. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetUnAssociatedProfileList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of associated profiles based on customers..
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetAssociatedProfileList(string routeUri, string routeTemplate);

        /// <summary>
        ///  Get list of associated profiles based on portal.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetCustomerPortalProfilelist(string routeUri, string routeTemplate);
        #endregion

        #region Affiliate
        /// <summary>
        /// Get list of referral commission type.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of referral commission type.</returns>
        string GetReferralCommissionTypeList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get customer affiliate data.
        /// </summary>
        /// <param name="userId">User id of the customer.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns customer affiliate data.</returns>
        string GetCustomerAffiliate(int userId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of referral commission for user.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of referral commission.</returns>
        string GetReferralCommissionList(string routeUri, string routeTemplate);
        #endregion

        #region Address
        /// <summary>
        /// Gets the customer address list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns the customer address list.</returns>
        string GetAddressList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the customer address.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns the customer address.</returns>
        string GetCustomerAddress(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of search locations.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="searchTerm">Search term.</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns the matched location list.</returns>
        string GetSearchLocation(int portalId, string searchTerm, string routeUri, string routeTemplate);

        #endregion
    }
}
