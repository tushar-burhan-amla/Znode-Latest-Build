using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStoreProductClient : IBaseClient
    {
        /// <summary>
        /// Get list of product
        /// </summary>
        /// <param name="expands">expand to have data from related table</param>
        /// <param name="filters">filters for product list</param>
        /// <param name="sorts">sorts for product list</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns>product list</returns>
        WebStoreProductListModel ProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get product by product id
        /// </summary>
        /// <param name="productId">publish product id</param>
        /// <param name="expands">expand to have data from related table</param>
        /// <returns>model with product data</returns>
        WebStoreProductModel GetProduct(int productId, ExpandCollection expands);

        /// <summary>
        /// Get list of associated products.
        /// </summary>
        /// <param name="productIds">product ids</param>
        /// <returns>list of products</returns>
        WebStoreProductListModel GetAssociatedProducts(ParameterModel productIds);

        /// <summary>
        /// Get Product Highlights
        /// </summary>
        /// <param name="parameterModel">Model with highlights code.</param>
        /// <param name="productId">product id.</param>
        /// <returns>Highlightlistmodel</returns>
        HighlightListModel GetProductHighlights(ParameterProductModel parameterModel, int productId, int localeId);
    }
}
