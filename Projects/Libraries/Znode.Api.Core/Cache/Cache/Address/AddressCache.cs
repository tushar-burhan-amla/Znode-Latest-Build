using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class AddressCache : BaseCache, IAddressCache
    {
        #region Private Variable
        private readonly IAddressService _service;
        #endregion

        #region Constructor
        public AddressCache(IAddressService addressService)
        {
            _service = addressService;
        }
        #endregion

        #region Public Methods
        public virtual string GetAddressList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AddressListModel addresslistModel = _service.GetAddressList(Expands, Filters, Sorts, Page);
                if (HelperUtility.IsNotNull(addresslistModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new AddressListResponse { AddressList = addresslistModel?.AddressList });
            }
            return data;
        }
        #endregion
    }
}
