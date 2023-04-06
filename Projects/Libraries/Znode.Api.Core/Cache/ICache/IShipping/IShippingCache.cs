namespace Znode.Engine.Api.Cache
{
    public interface IShippingCache
    {
        /// <summary>
        /// Get Shipping list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Shipping list.</returns>
        string GetShippingList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get shipping details.
        /// </summary>
        /// <param name="shippingId">shipping Id to get shipping details</param>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Returns shipping data</returns>
        string GetShipping(int shippingId, string routeUri, string routeTemplate);


        /// <summary>
        /// Get Shipping SKU list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Shipping SKU.</returns>
        string GetShippingSKUList(string routeUri, string routeTemplate);

        #region Shipping service Code
        /// <summary>
        /// Get shipping service code by Id.
        /// </summary>
        /// <param name="shippingServiceCodeId">shipping service code Id</param>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Returns shipping service code data.</returns>
        string GetShippingServiceCode(int shippingServiceCodeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of shipping service code.
        /// </summary>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Returns list of shipping service code.</returns>
        string GetShippingServiceCodes(string routeUri, string routeTemplate);
        #endregion

        #region Shipping Rule

        /// <summary>
        /// Get Shipping Rule from Cache By ShippingRuleId.
        /// </summary>
        /// <param name="shippingRuleId">Id to get Shipping Rule detail.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Shipping Rule on the basis of ShippingRuleId.</returns>
        string GetShippingRule(int shippingRuleId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Shipping Rule list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Shipping Rule.</returns>
        string GetShippingRuleList(string routeUri, string routeTemplate);

        #endregion

        #region Shipping Rule Type

        /// <summary>
        /// Get list of shipping rule type.
        /// </summary>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Returns list of shipping rule type.</returns>
        string GetShippingRuleTypeList(string routeUri, string routeTemplate);

        #endregion

        #region Portal/Profile Shipping
        /// <summary>
        /// Get associated shipping list for Portal/Profile.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns ShippingList.</returns>
        string GetAssociatedShippingList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of unassociated shipping list for Portal/Profile.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated shipping list.</returns>
        string GetUnGetAssociatedShippingList(string routeUri, string routeTemplate);
        #endregion
    }
}
