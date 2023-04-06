using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    //Product History Client Interface.
    public interface IProductHistoryClient : IBaseClient
    {
        /// <summary>
        /// Get Product history list.
        /// </summary>
        /// <param name="expands">Expands for Product History.</param>
        /// <param name="filters">Filters for Product History.</param>
        /// <param name="sorts">Sorts for Product History.</param>
        /// <returns>Returns ProductHistoryListModel</returns>
        ProductHistoryListModel GetProductHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get Product history list with paging parameters.
        /// </summary>
        /// <param name="expands">Expands for Product History.</param>
        /// <param name="filters">Filters for Product History.</param>
        /// <param name="sorts">Sorts for Product History.</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Returns ProductHistoryListModel</returns>
        ProductHistoryListModel GetProductHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Product History by Product History Id.
        /// </summary>
        /// <param name="id">Product History Id.</param>
        /// <param name="expands">Expands for Product History.</param>
        /// <returns>Returns ProductHistoryModel.</returns>
        ProductHistoryModel GetProductHistory(int id, ExpandCollection expands);

        /// <summary>
        /// Create Product History.
        /// </summary>
        /// <param name="productHistoryModel">CreateProductHistory</param>
        /// <returns>Returns CreateProductHistory.</returns>
        ProductHistoryModel CreateProductHistoryModel(ProductHistoryModel productHistoryModel);

        /// <summary>
        /// Update Product History on the basis of Product History Id.
        /// </summary>
        /// <param name="productHistoryModel">ProductHistoryModel</param>
        /// <returns>Returns ProductHistoryModel.</returns>
        ProductHistoryModel UpdateProductHistoryModel(ProductHistoryModel productHistoryModel);

        /// <summary>
        /// Delete Product History on the basis of Product History Id.
        /// </summary>
        /// <param name="id">Product History Id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteProductHistoryModel(int id);
    }
}