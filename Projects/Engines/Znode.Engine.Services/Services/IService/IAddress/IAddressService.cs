using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAddressService
    {
        /// <summary>
        /// Get address list
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <returns>AddressListModel</returns>
        AddressListModel GetAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
