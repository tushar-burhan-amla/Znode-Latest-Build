using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IStoreLocatorClient : IBaseClient
    {   
     
        /// <summary>
        /// Get Store List for location.
        /// </summary>
        /// <param name="expands">Expands for store list</param>
        /// <param name="filters">Filter for store list.</param>
        /// <param name="sorts">sorts for store list.</param>
        /// <param name="pageIndex">PageIndex for store list.</param>
        /// <param name="pageSize">PageSize for store list</param>
        /// <returns>model with store list</returns>
        StoreLocatorListModel GetStoreLocatorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Save store data for store location.
        /// </summary>
        /// <param name="storeLocatorModel">Model with store data for location.</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataModel SaveStore(StoreLocatorDataModel storeLocatorModel);

        /// <summary>
        /// Update an existing store data for store location.
        /// </summary>
        /// <param name="storeLocatorModel">Model with store data for location</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataModel Update(StoreLocatorDataModel storeLocatorModel);

        /// <summary>
        /// Get store data for location.
        /// </summary>
        /// <param name="portalAddresId">id to get store data.</param>
        /// <param name="expands">Expand for store data</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataModel GetStoreLocator(int portalAddresId, ExpandCollection expands);

        /// <summary>
        /// Get store data for location.
        /// </summary>
        /// <param name="storeLocationCode">storeLocationCode to get store locator data</param>
        /// <param name="expands">Expand for store data</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataModel GetStoreLocator(string storeLocationCode, ExpandCollection expands);

        /// <summary>
        /// Delete an existing store data.
        /// </summary>
        /// <param name="parameterModel">Model with ids to delete</param>
        /// <param name="isDeleteByCode">if true then store locator is delete by code</param>
        /// <returns>True or false</returns>
        bool DeleteStoreLocator(ParameterModel parameterModel, bool isDeleteByCode);
    }
}
