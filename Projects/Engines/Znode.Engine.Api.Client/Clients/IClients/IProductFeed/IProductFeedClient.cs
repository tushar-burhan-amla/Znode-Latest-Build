using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IProductFeedClient : IBaseClient
    {

        /// <summary>
        ///  Creates Google Site Map for generating XML file
        /// </summary>
        /// <param name="model">ProductFeedModel/param>
        /// <returns></returns>
        ProductFeedModel CreateProductFeed(ProductFeedModel productFeedModel);

        /// <summary>
        /// Get product feed list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with product feed list.</param>
        /// <param name="filters">Filters to be applied on product feed list.</param>
        /// <param name="sorts">Sorting to be applied on product feed list.</param>
        /// <param name="pageIndex">Start page index of product feed list.</param>
        /// <param name="pageSize">Page size of product feed list.</param>
        /// <returns>ProductFeedListModel</returns>
        ProductFeedListModel GetProductFeedList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get product feed by id.
        /// </summary>
        /// <param name="productFeedId">Product feed id</param>
        /// <param name="expands">Expands to be retrieved along with product feed list.</param>
        /// <returns>ProductFeedModel</returns>
        ProductFeedModel GetProductFeed(int productFeedId, ExpandCollection expands);

        /// <summary>
        /// Delete Product Feed.
        /// </summary>
        /// <param name="productFeedModel">product feed id</param>
        /// <returns>return true / false</returns>
        bool DeleteProductFeed(ParameterModel productFeedModel);

        /// <summary>
        /// Update product feed.
        /// </summary>
        /// <param name="model">ProductFeedModel</param>
        /// <returns>ProductFeedModel</returns>
        ProductFeedModel UpdateProductFeed(ProductFeedModel model);

        /// <summary>
        /// Get product feed master details.
        /// </summary>
        /// <returns>ProductFeedModel</returns>
        ProductFeedModel GetProductFeedMasterDetails();

        /// <summary>
        /// Check if the file name already exists.
        /// </summary>
        /// <returns>localeId</returns>
        /// <returns>fileName</returns>
        bool FileNameCombinationAlreadyExist(int localeId, string fileName);
    }
}
