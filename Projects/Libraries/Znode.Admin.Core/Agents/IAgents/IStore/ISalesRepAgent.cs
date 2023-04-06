using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ISalesRepAgent
    {
        /// <summary>
        /// Get the Sales Rep Users List
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sortCollection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordPerPage"></param>
        /// <returns></returns>
        UsersListViewModel GetSalesRepList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);
    }
}
