using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    //Product History Service Interface.
    public interface IProductHistoryService
    {
        /// <summary>
        /// Get Product History list.
        /// </summary>
        /// <param name="expands">Expands for Product History.</param>
        /// <param name="filters">Filters for Product History.</param>
        /// <param name="sorts">Sorts for Product History</param>
        /// <param name="page">Page</param>
        /// <returns>Returns ProductHistoryListModel</returns>
        ProductHistoryListModel GetProductHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Product History by id.
        /// </summary>
        /// <param name="id">Product History Id.</param>
        /// <param name="expands">Expands for Product History.</param>
        /// <returns>Returns ProductHistoryModel.</returns>
        ProductHistoryModel GetProductHistoryById(int id, NameValueCollection expands);

        /// <summary>
        /// Update Product History on the basis of Product History Id.
        /// </summary>
        /// <param name="productHistoryModel">ProductHistoryModel</param>
        /// <returns>Returns ProductHistoryModel.</returns>
        bool UpdateProductHistory(ProductHistoryModel productHistoryModel);

        /// <summary>
        /// Create new Product History.
        /// </summary>
        /// <param name="productHistoryModel">ProductHistoryModel</param>
        /// <returns>Returns ProductHistoryModel.</returns>
        ProductHistoryModel CreateProductHistory(ProductHistoryModel productHistoryModel);

        /// <summary>
        /// Delete Product History on the basis of Product History Id.
        /// </summary>
        /// <param name="id">Product History Id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteProductHistory(int id);
    }
}
