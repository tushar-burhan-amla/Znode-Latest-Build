using System;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public class AddressAgent : BaseAgent, IAddressAgent
    {
        #region Private Variables
        private readonly IAddressClient _addressClient;
        #endregion

        public AddressAgent(IAddressClient addressClient)
        {
            _addressClient = GetClient<IAddressClient>(addressClient);
        }

        public AddressListViewModel GetAddressList(int addressId, int otherAddressId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            int[] addressIds = { addressId, otherAddressId };

            filters.Add(new FilterTuple(FilterKeys.AddressId, FilterOperators.In, string.Join("_", addressIds) ));

            AddressListModel listModel = _addressClient.GetAddressList(filters, null, null, null);
            return listModel?.ToViewModel<AddressListViewModel>();
        }
    }
}
