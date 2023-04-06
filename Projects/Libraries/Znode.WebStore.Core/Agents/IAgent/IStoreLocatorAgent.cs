using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IStoreLocatorAgent
    {
        /// <summary>
        ///Get distance list.
        /// </summary>    
        /// <returns>List of distances in key value pair.</returns>
        List<SelectListItem> GetDistanceList();

        /// <summary>
        ///Get distance and storeLocator list.
        /// </summary>
        /// <param name="model">Model with store locator data.</param>
        /// <returns>View model with portal list and distance data.</returns>
        StoreLocatorViewModel GetPortalList(StoreLocatorViewModel model);
    }
}
