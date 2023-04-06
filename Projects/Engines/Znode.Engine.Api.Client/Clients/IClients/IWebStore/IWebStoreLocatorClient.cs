using System.Collections.Generic;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStoreLocatorClient : IBaseClient
    {
        /// <summary>
        /// Get the list of all Store Locators.
        /// </summary>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sorts">Sorts Collection.</param>      
        /// <returns>StoreLocatorListModel.</returns>
        List<StoreLocatorDataModel> GetPortalList(FilterCollection filters, SortCollection sorts);
    }

}
