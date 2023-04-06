using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStoreCategoryClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of Categories,SubCategories and Products by category name.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Category list.</param>
        /// <param name="filters">Filters to be applied on Category list.</param>
        /// <param name="sorts">Sorting to be applied on Category list.</param>
        /// <param name="pageIndex">Start page index of Category list.</param>
        /// <param name="pageSize">page size of Category list.</param>
        /// <returns>Category list model.</returns>
        WebStoreCategoryListModel GetWebStoreCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
