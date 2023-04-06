using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IStoreLocatorService
    {
        /// <summary>
        /// Save store data for store location.
        /// </summary>
        /// <param name="model">Model with store data for location.</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataModel Create(StoreLocatorDataModel model);

        /// <summary>
        /// Update an existing store data for store location.
        /// </summary>
        /// <param name="model">Model with store data for location.</param>
        /// <returns>True or false</returns>
        bool UpdateStoreLocator(StoreLocatorDataModel model);


        /// <summary>
        /// Delete an existing store data.
        /// </summary>
        /// <param name="storeIds">Model with ids to delete.</param>
        /// <param name="isDeleteByCode">if true then store locator is delete by code</param>
        /// <returns>True or false</returns>
        bool DeleteStoreLocator(ParameterModel storeIds, bool isDeleteByCode);

        /// <summary>
        /// Get Store List for location.
        /// </summary>
        /// <param name="expands">Expands for store list.</param>
        /// <param name="filters">Filter for store list.</param>
        /// <param name="sorts">sorts for store list.</param>
        /// <param name="page">PageIndex for store list.</param>
        /// <returns>Returns StoreLocator list</returns>
        StoreLocatorListModel GetStoreLocatorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get store data for location.
        /// </summary>
        /// <param name="storeId">id to get store data.</param>
        /// <param name="expands">Expand for store data.</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataModel GetStoreLocator(int storeId, NameValueCollection expands);


        /// <summary>
        /// Get store data for location.
        /// </summary>
        /// <param name="storeLocationCode">code to get store data.</param>
        /// <param name="expands">Expand for store data.</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataModel GetStoreLocator(string storeLocationCode, NameValueCollection expands);

        #region WebStoreLocator
        /// <summary>
        /// Get list of Store Locator.
        /// </summary>
        /// <param name="filters">FilterCollection with the store data for location.</param>
        /// <param name="sorts">Sort for sorting.</param>
        /// <returns>Returns WebStoreLocatorListModel.</returns>
        WebStoreLocatorListModel GetWebStoreLocatorList(FilterCollection filters, NameValueCollection sorts);
        #endregion
    }
}