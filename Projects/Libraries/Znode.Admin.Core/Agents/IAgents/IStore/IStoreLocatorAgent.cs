using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IStoreLocatorAgent
    {
        /// <summary>
        /// Get Store List for location.
        /// </summary>
        /// <param name="model">Filtercollectiondata model.</param>
        /// <returns>model with list of store for location.</returns>
        StoreLocatorListViewModel GetStoreLocatorList(FilterCollectionDataModel model);

        /// <summary>
        /// Get store data for location.
        /// </summary>
        /// <param name="portalAddressId"></param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataViewModel GetStoreLocator(int portalAddressId);

        /// <summary>
        /// Get store data for location.
        /// </summary>
        /// <param name="storeLocationCode">StoreLocationCode of store</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataViewModel GetStoreLocator(string storeLocationCode);

        /// <summary>
        /// Get data for create view.
        /// </summary>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataViewModel Create();

        /// <summary>
        /// Save store data for store location.
        /// </summary>
        /// <param name="viewModel">Model with store information to locate.</param>
        /// <returns>Model with store information to locate.</returns>
        StoreLocatorDataViewModel SaveStore(StoreLocatorDataViewModel viewModel);

        /// <summary>
        ///Update an existing store data for store location.
        /// </summary>
        /// <param name="viewModel">>Model with store information to locate.</param>
        /// <returns>>Model with store information to locate.</returns>
        StoreLocatorDataViewModel Update(StoreLocatorDataViewModel viewModel);

        /// <summary>
        /// Delete an existing store data.
        /// </summary>
        /// <param name="storeLocatorIds">string of ids.</param>
        /// <param name="isDeleteByCode">if true then delete is perform by storeLocationCode</param>
        /// <returns>True or false</returns>
        bool DeleteStoreLocator(string storeLocatorIds, bool isDeleteByCode = false);
    }
}