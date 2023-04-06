using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IAddressClient : IBaseClient
    {
        /// <summary>
        /// Get the complete list of address
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        AddressListModel GetAddressList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }

}